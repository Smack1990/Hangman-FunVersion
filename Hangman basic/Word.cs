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
    private static int S_windHeight = Console.WindowHeight / 2;
    private static int S_windwidth = (Console.WindowWidth / 2);
    public List<string> WordList { get; set; }
    public Dictionary<string, string> WordClue = new Dictionary<string, string> //Dictionary with [keyword, clue]for moderate level. 
        {
            {"Ronaldo", "Fotbollsspelare"},
            {"Lewandowski", "Fotbollsspelare"},
            {"Leo Messi", "Fotbollsspelare"},
            {"Zlatan Ibrahimovich", "Fotbollsspelare"},
            {"Göykeres", "Fotbollsspelare"},
            {"Krukväxt", "Finns i fönstren hemma"},
            {"Plommon", "Växer på träd "},
            {"Apelsin", "Orangutang nästan på ett annat språk"},
            {"Göteborg", "Sveriges framsida"},
            {"Landvetter", "Flygplan drar på tur"},
            {"Medicin", "När man är sjuk"},
            {"Barndomsminnen", "När vi va små"},
            {"Stockholm", "Sveriges baksida"},
            {"Astronaut", "I rymden"},
            {"Motorbåt", "Ett Färdmedel"},
            {"Fika", "En väldigt svensk sak"},
            {"Havremjölk", "Sädesslagsprodukt"},
            {"Bokhylla", "Möbel"},
            {"Svalbard", "Långt i norr med isbjörnar"},
            {"Volvo", "Bilmärke"},
            {"Mercedes", "Bilmärke"},
            {"Toyota", "Bilmärke"},
            {"Ikea", "Möbler"},
            {"Vinter", "Snö och kyla"},
            {"Strand", "Vid havet"},
            {"Sommar", "Sol, bad och semester"},
            {"Mobiltelefon", "Är med hela tiden"},
            {"Skola", "Där barn lär sig"},
            {"Kanelbulle", "Svensk fika"},
            {"Fotboll", "En sport med en boll och mål"},
            {"Midsommar", "Svensk högtid med blommor och dans"},
            {"Abba", "Svenskt musikfenomen"},
            {"Benjamin Ingrosso", "Svensk artist"},
            {"Albin lee Meldau", "Svensk artist"},
            {"Håkan Hellström", "Svensk artist"},
            {"Mötley Crue", "Ett band"},
            {"Gyllene tider", "Ett band"},
            {"Metallica", "Ett band"},
            {"Smack into pieces", "Ett band"},
            {"Sabaton", "Ett band"},
            {"Hammerfall", "Ett band"},
            {"Rammstein", "Ett band"},
            {"Eld", "Värmer när det är kallt"},
            {"Spindel", "Många ben"},
            {"Katt", "Inne eller utomhusdjur"},
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
        };
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
        ReadJson();
    }

    public string GetFilePath(string filename) //fetches the json
    {
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                Console.SetCursorPosition(S_windwidth - text.Length /2, Console.CursorTop);
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
            WordList = WordClue.Keys.ToList();
        }
    } // Reads Jsonfile
}