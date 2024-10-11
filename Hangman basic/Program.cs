using Hangman_basic;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.Json;
using System.Xml.Linq;

namespace Hangman
{
    class Program
    {

        static Words S_wordList = new Words();
        static string[]? S_secretWords;
        static string? S_correctWord;
        static char[]? S_letters;
        private static Player? S_player;
        static char[] Keyboard = "QWERTYUIOPÅASDFGHJKLÖÄZXCVBNM".ToCharArray();
        //Skapa svårighetsgrad baserat på ordlängd. 
        //kontrollera alla slutprodukter.  
        //lägg till "Du har redan gissat på detta"



        static void Main(string[] args)
        {
            //Listan implementerad och fungerar.
            //HangmanBoring. gör tre olika listor att ladda beroende på nivå. 
            //svåra långa ord för hard. Lätta ord för easy.Skriv ut en text i hangman beroende på vilken nivå man valt. "Easey mode activated, i denna klass hanterar vi enkla adjektiv typ" eller "Hard mode activated"






            StartGame();



        }

        private static void PrintingHangman(int incorrectGuesses)
        {
            

            switch (incorrectGuesses)
            {
                case 1:
                    Console.WriteLine(" ");
                    Console.WriteLine("      ");
                    Console.WriteLine("      ");
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                    Console.WriteLine("_|_");
                    break;
                case 2:
                    Console.WriteLine(" ");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |");
                    Console.WriteLine("_|_");
                    break;
                case 3:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |    ");
                    Console.WriteLine(" |");
                    Console.WriteLine("_|_");
                    break;
                case 4:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     ");
                    Console.WriteLine(" |    ");
                    Console.WriteLine(" |");
                    Console.WriteLine("_|_");
                    break;
                case 5:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     O");
                    Console.WriteLine(" |    ");
                    Console.WriteLine(" |    ");
                    Console.WriteLine("_|_");
                    break;
                case 6:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     O");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |    ");
                    Console.WriteLine("_|_");
                    break;
                case 7:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     O");
                    Console.WriteLine(" |    /|");
                    Console.WriteLine(" |    ");
                    Console.WriteLine("_|_");
                    break;
                case 8:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     O");
                    Console.WriteLine(" |    /|\\");
                    Console.WriteLine(" |    /");
                    Console.WriteLine("_|_");
                    break;
                case 9:
                    Console.WriteLine(" _______");
                    Console.WriteLine(" |     |");
                    Console.WriteLine(" |     O");
                    Console.WriteLine(" |    /|\\");
                    Console.WriteLine(" |    / \\");
                    Console.WriteLine("_|_");
                    break;

                default:
                    break;
            }
        }

        private static void StartGame()
        {
            S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
            //S_correctWord = S_secretWords[new Random().Next(0, S_secretWords.Length)];

            S_letters = new char[S_correctWord.Length];
            for (int i = 0; i < S_correctWord.Length; i++)
                S_letters[i] = '_';

            Console.WriteLine("Welcome to hangman\n");
            Console.WriteLine("[P]lay");
            Console.WriteLine("[A]dd word");

            char upperChar;
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
            switch (upperChar)
            {
                case 'P':
                    PlayGame();
                    break;
                case 'A':
                    Console.WriteLine("\nEnter a word to the dictionary");
                    S_wordList.WriteJson(Console.ReadLine()!);
                    Console.Clear();
                    StartGame();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    StartGame();
                    break;
            }
        }


        private static void AskForUsersName()
        {
            Console.WriteLine("Enter your name:");
            string input = Console.ReadLine()!;

            if (input.Length >= 2)
                S_player = new Player(input);
            else
            {
                Console.WriteLine("Your name was too short. Please enter a valid name.");
                AskForUsersName();
            }
        }

        private static void PlayGame()
        {
            AskForUsersName();
            int incorrectGuesses = 0;
            do
            {
                Console.Clear();
                Console.WriteLine($"Hello {S_player.PlayerName}! Let's play hangman!\n");

                Console.SetCursorPosition(0, 2); Console.Write($"Your guessed letters: ");


                for (int i = 0; i < S_player.guessedLetters.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.Write($"{S_player.guessedLetters[i]},"); Console.ResetColor();
                }
                Console.WriteLine();
                Console.SetCursorPosition(0, 3); Console.WriteLine($"Number of guesses: {S_player.guessedLetters.Count}");
                Console.SetCursorPosition(0, 4); Console.WriteLine($"Number of Incorrect guesses: {incorrectGuesses}");
                KeyboardOutput();

                DisplayMaskedWord();

                PrintingHangman(incorrectGuesses);
                char guessedLetter = AskForLetter();
                bool correct = CheckLetter(guessedLetter);

                if (!correct)
                {
                    incorrectGuesses++;
                PrintingHangman(incorrectGuesses);
                }

                if (incorrectGuesses >= 9)
                {
                    Console.Clear();
                    Console.WriteLine($" {S_player.PlayerName} You Los the game. The correct Word was [{S_correctWord}] ");
                    EndGame(); Task.Delay(4000);
                    RestartGame();
                }

            } while (S_correctWord != new string(S_letters));


            Console.Clear();
            Console.WriteLine($"Congratz {S_player.PlayerName} You Won!. Correct word: [{S_correctWord}] ");
            EndGame();
            RestartGame();




        }

        private static bool CheckLetter(char guessedLetter)
        {
            guessedLetter = char.ToUpper(guessedLetter);

            if (!S_player.guessedLetters.Contains(guessedLetter))
            {
                S_player.guessedLetters.Add(guessedLetter);
            }



            bool found = false;

            for (int i = 0; i < S_correctWord!.Length; i++)
            {
                if (char.ToLower(guessedLetter) == char.ToLower(S_correctWord[i]))
                {
                    S_letters![i] = S_correctWord[i];
                    found = true;
                    S_player!.Score++;
                }
            }
            UpdateKeyboard(guessedLetter);


            return found;


        }

        static void DisplayMaskedWord()
        {
            foreach (char c in S_letters!)
                Console.Write(c);

            Console.WriteLine();
        }

        static char AskForLetter()
        {
            string input;
            do
            {
                Console.WriteLine("Press a key from the chosen board");
                Console.WriteLine("Tips... Dont forget that [Space] exist");
                input = Console.ReadKey(true).KeyChar.ToString().ToUpper();
            } while (input.Length != 1);

            var Letter = input[0];



            return Letter;
        }

        private static void EndGame()
        {
           
            Console.Clear();
            Console.WriteLine($"Thanks for playing {S_player!.PlayerName}");
            Console.WriteLine($"Guesses: {S_player.guessedLetters.Count} Score: {S_player.Score}");
        }


        static string GetFilePath(string filename)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string ret = Path.Combine(filePath, filename);
            return ret;
        }
        private static void RestartGame()
        {
            Console.WriteLine("Do you want to play again? [Y]es or [N]o");
            char upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
            switch (upperChar)
            {
                case 'Y':
                    Console.Clear();
                    StartGame();
                    break;
                case 'N':
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    RestartGame();
                    break;
            }
        }
        private static void KeyboardOutput()
        {
            Console.WriteLine("Available letters\n");
            KeyboardRow(0, 10);
            KeyboardRow(12, 21);
            KeyboardRow(22, 28);

            Console.ResetColor();
            Console.WriteLine();
        }
        private static void KeyboardRow(int first, int last)
        {
            for (int i = first; i <= last; i++)
            {
                if (Keyboard[i] == ' ')
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.Write(Keyboard[i]);
            }
            Console.WriteLine();
        }
        static void UpdateKeyboard(char guessedLetter)
        {
            for (int i = 0; i < Keyboard.Length; i++)
            {
                if (Keyboard[i] == guessedLetter)
                {
                    Keyboard[i] = ' ';
                }
            }
        }
    }
}


