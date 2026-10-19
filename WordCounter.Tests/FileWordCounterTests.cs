
using System;
using System.IO;
using WordCounter.Services;
using Xunit;

namespace WordCounter.Tests;

public class FileWordCounterTests
{
    [Fact]
    public void CountFileWords_ShouldCountWordsFromFile()
    {
        // Arrange
        string filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(
                filePath,
                "Hello hello world\nworld dotnet");
            var wordCounter = new WordCounterService();
            var fileWordCounter = new FileWordCounter(wordCounter);
            // Act
            var results = fileWordCounter.CountFileWords(filePath);
            // Assert
            Assert.Equal(3, results.Count);
            Assert.Equal("hello", results[0].Word);
            Assert.Equal(2, results[0].Count);
            Assert.Equal("world", results[1].Word);
            Assert.Equal(2, results[1].Count);
            Assert.Equal("dotnet", results[2].Word);
            Assert.Equal(1, results[2].Count);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
    
    [Fact]
public void CountFileWords_ShouldThrow_WhenFileDoesNotExist()
{
    // Arrange
    var wordCounter = new WordCounterService();
    var fileWordCounter = new FileWordCounter(wordCounter);

    string filePath = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString("N") + ".txt");

    // Act & Assert
    Assert.Throws<FileNotFoundException>(
        () => fileWordCounter.CountFileWords(filePath));
}
}
