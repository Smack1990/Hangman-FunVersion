using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hangman_basic;
public class Word
{
    const string JsonWords = @"MyWords.json"; 
    public List<string> WordList { get; set; }

    public Word()
    {
        WordList = new List<string>();
        ReadJson(); 
    }

    public string GetFilePath(string filename)
    {
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(filePath, filename);
    }

    public void WriteJson(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            WordList.Add(input);
        }
        string json = JsonSerializer.Serialize(WordList);
        var filePath = GetFilePath(JsonWords);
        File.WriteAllText(filePath, json);
        
    }

    public void ReadJson()
    {
        string filePath = GetFilePath(JsonWords);
        if (File.Exists(filePath)) 
        {
            string json = File.ReadAllText(filePath);
            WordList = JsonSerializer.Deserialize<List<string>>(json)!;
            //foreach (var word in WordList) //Kontrollera om listan finns eller ej. 
            //{
            //    Console.WriteLine(word);
            //}
        }
        else
        {
            WordList = new List<string> { "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew", "kiwi", "lemon", "mango", "nectarine", "orange", "pear", "quince", "raspberry", "strawberry", "tangerine", "watermelon" };
            
        }
    }
}