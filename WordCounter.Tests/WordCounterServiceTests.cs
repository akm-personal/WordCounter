using WordCounter.Services;

namespace WordCounter.Tests;

public class WordCounterServiceTests
{
    [Fact]
    public void CountWords_ShouldIgnoreCase_AndCountCorrectly()
    {
        // Arrange
        var service = new WordCounterService();
        string input = "Hello hello WORLD";
        // Act
        var results = service.CountWords(input);
        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("hello", results[0].Word);
        Assert.Equal(2, results[0].Count);
        Assert.Equal("world", results[1].Word);
        Assert.Equal(1, results[1].Count);

    }

    [Fact]
    public void CountWords_ShouldReturnEmptyResult_WhenInputIsEmpty()
    {
        // Arrange
        var service = new WordCounterService();
        // Act
        var results = service.CountWords("");
        // Assert
        Assert.Empty(results);
    }

    [Fact]
public void CountWords_ShouldSplitOnPunctuationAndNumbers()
{
    // Arrange
    var service = new WordCounterService();
    string input = "hello, hello! world123 world";
    // Act
    var results = service.CountWords(input);


    // Assert
    Assert.Equal(2, results.Count);
    Assert.Equal("hello", results[0].Word);
    Assert.Equal(2, results[0].Count);
    Assert.Equal("world", results[1].Word);
    Assert.Equal(2, results[1].Count);
}
}
