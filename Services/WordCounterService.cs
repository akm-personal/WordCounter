
using System.Text.RegularExpressions;
using WordCounter.Models;

namespace WordCounter.Services;

public sealed class WordCounterService : IWordCounter
{
    private static readonly Regex WordPattern = new(
        @"[a-zA-Z]+",
        RegexOptions.Compiled);

    public IEnumerable<string> ExtractWords(string? text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        foreach (Match match in WordPattern.Matches(text))
        {
            yield return match.Value.ToLowerInvariant();
        }
    }

    public IReadOnlyList<WordCountResult> CountWords(string? text)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (string word in ExtractWords(text))
        {
            counts.TryGetValue(word, out int count);
            counts[word] = count + 1;
        }

        return counts
            .Select(item => new WordCountResult(item.Key, item.Value))
            .OrderByDescending(result => result.Count)
            .ThenBy(result => result.Word, StringComparer.Ordinal)
            .ToArray();
    }
}
