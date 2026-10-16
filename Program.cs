using System.Text;
using WordCounter.Services;

if (args.Length == 0)
{
    Console.Error.WriteLine("Please provide the path of a text file to analyze.");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Example:");
    Console.Error.WriteLine("  dotnet run -- InputFile/mobydick.txt");   
    return;
}

string filePath = args[0];

try
{
    var wordCounter = new WordCounterService();
    var fileCounter = new FileWordCounter(wordCounter);

    var results = fileCounter.CountFileWords(filePath);

    foreach (var result in results.Take(20))
    {
        Console.WriteLine($"{result.Count,7} {result.Word}");
    }
}
catch (Exception ex) when (
    ex is IOException ||
    ex is UnauthorizedAccessException ||
    ex is ArgumentException ||
    ex is NotSupportedException ||
    ex is InvalidDataException)
{
    string message = ex switch
    {
        FileNotFoundException =>
            "The specified file could not be found. Please check the file path.",

        DirectoryNotFoundException =>
            "The specified directory could not be found. Please check the file path.",

        UnauthorizedAccessException =>
            "Access to the file was denied. Please check your file permissions.",

        InvalidDataException =>
            "The file appears to contain binary data. Please provide a text file.",

        DecoderFallbackException =>
            "The file could not be decoded as valid text. Please check its encoding.",

        ArgumentException =>
            "The file path is invalid. Please provide a valid file path.",

        NotSupportedException =>
            "The specified file path format is not supported.",

        IOException =>
            "An error occurred while reading the file. Please check the file and try again.",

        _ =>
            "An unexpected input error occurred."
    };

    Console.Error.WriteLine($"Error: {message}");
}
