using pack;
using System.CommandLine;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Xml.Schema;



//========define commands===========
var rootCommand = new RootCommand("root command for file bundle CLI");
var bundleCommand = new Command("bundle", "bundle code files to single file");
var createRspCommand = new Command("create-rsp", "create a response file");
createRspCommand.AddAlias("crsp");

//==========adding commands==========
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);


//========define options===========
var languageOption = new Option<string>(new string[] { "--language", "-l" }, "code language list") { IsRequired = true, AllowMultipleArgumentsPerToken = true };
var outputOption = new Option<FileInfo>(new string[] { "--output", "-o" }, "file path or file name");
var noteOption = new Option<bool>(new string[] { "--note", "-n" }, "write the source as a comment in bundle");
var sortOption = new Option<string>(new string[] { "--sort", "-s" }, "the order of copying the code files");
var removeEmptyLinesOption = new Option<bool>(new string[] { "----remove", "-r" }, "remove empty lines in bundle");
var authorOption = new Option<string>(new string[] { "--author", "-a" }, "adding the author name");


//=========adding option============
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeEmptyLinesOption);
bundleCommand.AddOption(authorOption);


//language
static bool LangugeValidaton(string language)
{
    if (string.IsNullOrEmpty(language))
        return false;

    if (language == "all")
        return true;

    string[] languages = language.Split(' ');


    //check if the input is contained in the list
    foreach (var lang in languages)
        if (!Language.ProgrammingLanguages.ContainsKey(lang))
            return false;

    return true;
}
static List<string> FilterDict(string directoryPath, List<string> files, string language)
{
    if (language == "all")
        language = string.Join(" ", Language.ProgrammingLanguages.Keys);

    //get all the extension of the chosen languages
    var extension = Language.ProgrammingLanguages.Where(lang => language.Contains(lang.Key))
    .Select(lang => lang.Value);

    //filter the file include in the list, exclude bin and debug files
    var filteredFiles = files.Where(file =>
           extension.Contains(Path.GetExtension(file)) ||
           !file.Contains(Path.Combine(directoryPath, "bin")) ||
           !file.Contains(Path.Combine(directoryPath, "debug"))).ToList();

    return filteredFiles;
}

bundleCommand.SetHandler((language, output, note, sort, removeEmptyLines, author) =>
{

    try
    {

        if (!LangugeValidaton(language))
            throw new InvalidOperationException("invalid languages");

        var directoryPath = Directory.GetCurrentDirectory();
        var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).ToList();

        files = FilterDict(directoryPath, files, language);

        var bundleFile = new StreamWriter(output.FullName, append: false)

        if (sort != "alphabet")
            files = files.OrderBy(file => Path.GetExtension(file)).ToList();
        files = files.OrderBy(file => Path.GetFileName(file)).ToList();

        if (!string.IsNullOrEmpty(author))
            bundleFile.WriteLine($"author: {author}");
        foreach (var file in files)
        {
            var currentFile = File.ReadAllLines(file);

            if (removeEmptyLines)
                currentFile = currentFile.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            if (note)
            {
                bundleFile.WriteLine($"// Source: {Path.GetFileName(file)} {Path.GetRelativePath(directoryPath, file)}");
            }
            using (bundleFile = new StreamWriter(output.FullName, append: true)) ;


            //copying single file content
            foreach (string line in currentFile)
                bundleFile.WriteLine(line);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("path file is invalid");
    }
},
languageOption,
outputOption,
noteOption,
sortOption,
removeEmptyLinesOption,
authorOption
);

createRspCommand.SetHandler(() =>
{
    var quesAns = new Dictionary<string, string> {
        {"language (comma-separated, or 'all'","" } ,
        { "include source file paths as comments in the bundle ? (true / false)","false"},
        { "sort files by ('name' or 'type'","name"},
        { "remove empty lines from the source files ? (true / false)","false"},
        {"add Author to the file?(auther name)","" },
        { "output -File path and name",""}
    };
    foreach (var key in quesAns.Keys)
    {
        Console.Write($"Enter {key}: ");
        quesAns[key] = Console.ReadLine() ?? quesAns[key];
    }
    if (string.IsNullOrWhiteSpace(quesAns["language (comma-separated, or 'all'"]) ||
         string.IsNullOrWhiteSpace(quesAns["output -File path and name"]))
        throw new Exception("Error: 'language' and 'output' are required fields.");
    var rspName = "bundle.rsp";
    using (FileStream resFile = new FileStream(rspName, FileMode.CreateNew, FileAccess.Write))
    using (StreamWriter writer = new StreamWriter(resFile))
    {
        foreach (var (key, value) in quesAns)
        {
            var option = key[0];
            writer.WriteLine($"--{option} {value}");
        }
    }
    Console.WriteLine($"Response File created successfully. Run it with pack @{rspName}");
});

rootCommand.InvokeAsync(args);
