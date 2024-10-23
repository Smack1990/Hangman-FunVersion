using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Xsl;
using static System.Net.Mime.MediaTypeNames;

namespace Hangman_basic;
public class Word
{
    const string JsonWords = @"MyWords.json";
    const string JsonClues = @"WordClues.json";
    private static int S_windHeight = Console.WindowHeight / 2;
    private static int S_windwidth = (Console.WindowWidth / 2);
    public List<string> WordList { get; set; }
    public Dictionary<string, string> WordClue = new Dictionary<string, string>(); //Dictionary with [keyword, clue]for moderate level. 
       
    public void AddWordToDictionary(string input, string clue)
    {
        string revisedInput = input.Trim();
        string revisedClue = clue.Trim();

        revisedInput = char.ToUpper(revisedInput[0]) + revisedInput.Substring(1).ToLower();
        revisedClue = char.ToUpper(revisedClue[0]) + revisedClue.Substring(1).ToLower();
        if (!WordClue.ContainsKey(revisedInput))
        {
            WordClue.Add(revisedInput, revisedClue);
            WriteDictionary();
            string text = "Word and clue was added succesfully"; 
            Console.SetCursorPosition(S_windwidth - text.Length / 2, Console.CursorTop);

            Console.WriteLine(text); Thread.Sleep(2000);
        }
        else
        {
            string text = "The word already exist in this dictionary"; 
            Console.SetCursorPosition(S_windwidth - text.Length / 2, Console.CursorTop);
            Console.WriteLine(text); Thread.Sleep(2000);
        }


    }
    public string GetClue(string word) //returns keyword for level moderate
    {
        if (WordClue.ContainsKey(word))
            return WordClue[word];
        return string.Empty;
    }


    public void PrintClue(string word) // Prints clue to screen in moderate level
    {
        if (WordClue.ContainsKey(word))
        {
            Console.WriteLine("Clue: " + WordClue[word]);
        }
        else
        {
            Console.WriteLine("No clue available for this word.");
        }
    }

    public Word() //Constructor that reads creates WordsList list and reads from jsonfile at the beginning of the game. 
    {
        WordList = new List<string>();
        ReadDictionary();
        ReadJson();
    }
    private void WriteDictionary() // Writes to dictionary
    {
        var filePath = GetFilePath(JsonClues);
        string updatedJson = JsonSerializer.Serialize(WordClue);
        File.WriteAllText(filePath, updatedJson);

    }
    private void ReadDictionary() //reads fron dictionary or fall back on default values if file doesnt exist.
    {
        var filePath = GetFilePath(JsonClues);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordClue = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        else
        {
            WordClue = new Dictionary<string, string>
            {
                { "Ronaldo", "Fotbollsspelare" },
                { "Lewandowski", "Fotbollsspelare" },
                { "Leo Messi", "Fotbollsspelare" },
                { "Zlatan Ibrahimovich", "Fotbollsspelare" },
                { "Göykeres", "Fotbollsspelare" },
                { "Krukväxt", "Finns i fönstren hemma" },
                { "Plommon", "Växer på träd " },
                { "Apelsin", "Orangutang nästan på ett annat språk" },
                { "Göteborg", "Sveriges framsida" },
                { "Landvetter", "Flygplan drar på tur" },
                {"Saxofon", "Instrument"},
                {"Piano", "Instrument"},
                {"Gitarr", "Instrument"},
                {"Trummor", "Instrument"},
                {"Klarinett", "Instrument"},
                {"Kontrabas", "Instrument"},
                {"Mungiga", "Instrument"},
                {"Dator", "Jobbar och surfar på nätet"},
                {"Hus", "Bor man i"},
                {"Regn", "Vatten som faller från himlen"},
                {"Sjö", "Stor vattenmassa inuti landet"},
                {"Björn", "Stort djur i skogen"},
                {"Vildsvin", "Djur i skogen"},
                {"Träd", "Har grenar och löv"},
                {"Bil", "Fyrhjuligt transportmedel"},
                {"Flygplan", "Färdas i luften"},
                {"Bok", "Läser du för att få kunskap eller nöje"},
                {"Hund", "Människans bästa vän"},
                {"Glass", "Oftast på sommaren"},
                {"Cykel", "Två hjul"},
                {"Skridskor", "På is"},
                {"Fisk", "Simma i vattnet"},
                {"Ficklampa", "Bra att ha i mörkret"},
                {"Orkerster", "Musikgrupp"},
                {"Skulpur", "Tredimensionell konst"},
                {"Parallellogram", "Fyrhörning med lika sidor"},
                {"Tetragrammaton", "Guds namn"},
                {"Fotografi", "Ljudbild"},
                {"Symfoni", "Stort musikverk"},
                {"Kryptografi", "Hemlig skrivkonst"},
                {"Astrologi", "Sjärnor och liv"},
                {"Mikroskop", "Förstorar små objekt"},
                {"Paradox", "Motsägelsefull utsaga"},
                { "Manchester United", "Fotbollslag" },
                { "Real Madrid", "Fotbollslag" },
                { "FC Barcelona ", "Fotbollslag" },
                { "Juventus", "Fotbollslag" },
                { "Liverpool", "Fotbollslag" },
                { "Manchester City", "Fotbollslag" },
  
            };


        }
    }

    public string GetFilePath(string filename) //fetches the json
    {
        string filePath = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(filePath, filename);
    }

    public void WriteJson(string input) // Prints new words to json-file
    {
        GameUX gameUX = new GameUX();
        if (!string.IsNullOrEmpty(input))
        {
            string inputToUpper = char.ToUpper(input[0]) + input.Substring(1);

            var filePath = GetFilePath(JsonWords);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                WordList = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }


            if (WordList.Contains(inputToUpper, StringComparer.OrdinalIgnoreCase))
            {
                string text = "The word already exists in the list.";
                Console.SetCursorPosition(S_windwidth - text.Length / 2, Console.CursorTop);
                Console.WriteLine(text);
                Thread.Sleep(2000);
                return;
            }
            else
            {
                string text = "The word was added successfully";
                Console.SetCursorPosition(S_windwidth - text.Length / 2, Console.CursorTop);
                Console.WriteLine(text);
                Thread.Sleep(2000);
            }


            WordList.Add(inputToUpper);


            string updatedJson = JsonSerializer.Serialize(WordList);
            File.WriteAllText(filePath, updatedJson);
        }
    }

    public void ReadJson()// Reads Jsonfile
    {
        string filePath = GetFilePath(JsonWords);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordList = JsonSerializer.Deserialize<List<string>>(json)!;
         
        }
        else
        {
            WordList = WordClue.Keys.ToList();
        }
    } 
}