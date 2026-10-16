
using System.Text;
using WordCounter.Models;

namespace WordCounter.Services;

public sealed class FileWordCounter
{
    private readonly IWordCounter _wordCounter;

    public FileWordCounter(IWordCounter wordCounter)
    {
        _wordCounter = wordCounter
            ?? throw new ArgumentNullException(nameof(wordCounter));
    }

    public IReadOnlyList<WordCountResult> CountFileWords(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path cannot be empty.", nameof(filePath));
        }

        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.SequentialScan);

        // Inspect a small sample before decoding the file as text.
        byte[] sample = new byte[4096];
        int bytesRead = stream.Read(sample, 0, sample.Length);

        bool hasUnicodeBom =
            (bytesRead >= 2 &&
             sample[0] == 0xFF && sample[1] == 0xFE) ||
            (bytesRead >= 2 &&
             sample[0] == 0xFE && sample[1] == 0xFF) ||
            (bytesRead >= 4 &&
             sample[0] == 0x00 && sample[1] == 0x00 &&
             sample[2] == 0xFE && sample[3] == 0xFF) ||
            (bytesRead >= 4 &&
             sample[0] == 0xFF && sample[1] == 0xFE &&
             sample[2] == 0x00 && sample[3] == 0x00);

        bool hasNullByte =
            Array.IndexOf(sample, (byte)0, 0, bytesRead) >= 0;

        // NUL-byte detection is a heuristic, not a universal
        // binary-file detector. Unicode BOMs need separate handling.
        if (hasNullByte && !hasUnicodeBom)
        {
            throw new InvalidDataException(
                "The file appears to contain binary data.");
        }

        stream.Position = 0;

        using var reader = new StreamReader(
            stream,
            new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true),
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 4096,
            leaveOpen: false);

       
        var counts = new Dictionary<string, int>(
            StringComparer.Ordinal);

        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            foreach (string word in _wordCounter.ExtractWords(line))
            {
                counts.TryGetValue(word, out int count);
                counts[word] = count + 1;
            }
        }

        return counts
            .Select(item => new WordCountResult(item.Key, item.Value))
            .OrderByDescending(result => result.Count)
            .ThenBy(result => result.Word, StringComparer.Ordinal)
            .ToArray();
    }
}
