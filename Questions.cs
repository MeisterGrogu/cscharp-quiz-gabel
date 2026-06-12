using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
class Questions
{
    public string Path = "questions.json";
    
    public string LoadFile(string Path)
    {
        StreamReader sr = new StreamReader(Path);
        string File = sr.ReadToEnd();
        return File;
    }

    public void ParseQuestions(string Path)
    {
        var ToParse = LoadFile(Path);
        Console.WriteLine(ToParse);
    }
}