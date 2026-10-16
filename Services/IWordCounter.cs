
using WordCounter.Models;

namespace WordCounter.Services;

public interface IWordCounter
{
    IEnumerable<string> ExtractWords(string? text);

    IReadOnlyList<WordCountResult> CountWords(string? text);
}
