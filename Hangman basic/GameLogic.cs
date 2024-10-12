using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Hangman_basic;
using System.Threading.Tasks.Sources;

namespace Hangman_basic;
public class GameLogic
{
    private static Word S_wordList = new Word();
    private static string? S_correctWord;
    private static char[]? S_letters;
    private static Player? S_player;
    public GameUX gameUX = new GameUX();
    static int S_windHight = Console.WindowHeight / 2;
    static int S_windwidth = (Console.WindowWidth / 2);


    public void StartGame()
    {
        S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
        S_letters = new char[S_correctWord.Length];

        for (int i = 0; i < S_correctWord.Length; i++)
            S_letters[i] = '_';

        Console.WriteLine("Welcome to hangman\n");
        Console.WriteLine("[P]lay");
        Console.WriteLine("[A]dd word");

        char upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
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


    private void PlayGame()
    {
        S_player = new Player("");
        S_player.AskForUsersName();
        int incorrectGuesses = 0;
        string alreadyGuessed = string.Empty;
        char guessedLetter;
        int width = (Console.WindowWidth / 2 - alreadyGuessed.Length / 2);

        do
        {
            Console.Clear();
            gameUX.Centered($"Number of guesses: {S_player.GuessedLetters.Count}");
            gameUX.Centered($"Number of Incorrect guesses: {incorrectGuesses}");
            gameUX.Centered($"Your guessed letters:");
            GuessedLettersDisplay();

            if (!string.IsNullOrEmpty(alreadyGuessed))//Felmedelande vid samma knapptryck
                Console.ForegroundColor = ConsoleColor.Green; Console.SetCursorPosition(48, 5); Console.WriteLine(alreadyGuessed); Console.ResetColor();

            Console.SetCursorPosition(0, S_windHight); DisplayMaskedWord();
            gameUX.DisplayKeyboard();// skriver ut tagentbordslayouten enligt QWERTY
            PrintingHangman(incorrectGuesses);
            guessedLetter = AskForLetter();
            if (S_player.GuessedLetters.Contains(guessedLetter))
            {

                alreadyGuessed = $"You've already guessed: {guessedLetter}!";
            }
            else
            {
                alreadyGuessed = string.Empty;
                bool correct = CheckLetter(guessedLetter);
                if (!correct)
                {
                    S_player.Score--;
                    incorrectGuesses++;
                    PrintingHangman(incorrectGuesses);
                }
            }

            if (incorrectGuesses >= 9)
            {
                Console.Clear();
                gameUX.Centered($"You lost! The correct word was [{S_correctWord}]");
                EndGame();
                RestartGame();
            }

        } while (S_correctWord != new string(S_letters));


        Console.Clear();
        gameUX.Centered($"Congratz {S_player.PlayerName} You Won!. Correct word: [{S_correctWord}] ");
        RestartGame();




    }

    private static void GuessedLettersDisplay()
    {
        int startX = 45;
        int startY = 4;
        int currentX = startX;

        Console.SetCursorPosition(startX, startY);
        Console.Write(new string(' ', Console.WindowWidth - startX));
        foreach (var letter in S_player.GuessedLetters)
        {
            if (S_correctWord.ToUpper().Contains(letter))
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(currentX, startY);
            Console.Write($"{letter.ToString()}, ");
            currentX += 2;
        }
        Console.ResetColor();
    }

    private void DisplayMaskedWord() //Positioning and toString of the masked word
    {
        int widht = Console.WindowWidth / 2 - S_correctWord!.Length / 2;
        Console.SetCursorPosition(widht, Console.CursorTop);
        string maskedWord = new string(S_letters!);
        Console.Write(maskedWord);
        Console.WriteLine();
    }
    private char AskForLetter()
    {
        string text = "Guess a Letter by pressing a key";
        int S_windwidth = Console.WindowWidth / 2 - text.Length / 2;
        Console.SetCursorPosition(S_windwidth, S_windHight + 6);
        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(text); Console.ResetColor();
        return char.ToUpper(Console.ReadKey(true).KeyChar);
    }

    public static bool CheckLetter(char guessedLetter)
    {
        guessedLetter = char.ToUpper(guessedLetter);

        if (S_player!.GuessedLetters.Contains(guessedLetter))
            return false;
        S_player.GuessedLetters.Add(guessedLetter);
        bool found = false;

        for (int i = 0; i < S_correctWord!.Length; i++)
        {
            if (guessedLetter == char.ToUpper(S_correctWord[i]))
            {
                S_letters![i] = S_correctWord[i];
                found = true;
                S_player.Score++;
            }
            else
                S_player.Score--;
        }
        GameUX gameUX = new GameUX();
        gameUX.UpdateKeyboard(guessedLetter);
        return found;
    }
    private void EndGame()
    {
        Console.Clear();
        gameUX.Centered($"Thanks for playing {S_player!.PlayerName}");
        gameUX.Centered($"Guesses: {S_player.GuessedLetters.Count} Score: {S_player.Score}");
    }

    private void RestartGame()
    {
        gameUX.Centered("Do you want to play again? [Y]es or [N]o");
        char upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        switch (upperChar)
        {
            case 'Y':
                Console.Clear();
                gameUX.KeyboardCleanUp();
                StartGame();
                break;
            case 'N':
                Environment.Exit(0);
                break;
            default:
                gameUX.Centered("Invalid input");
                RestartGame();
                break;
        }
    }

    private void PrintingHangman(int incorrectGuesses)
    {
        //Where it will print
        int x = 57; 
        int y = 8;


        Console.SetCursorPosition(x, y);
        Console.Write(new string(' ', 20));
        switch (incorrectGuesses) 
        {
            case 1:
                Console.SetCursorPosition(x, y);
                Console.WriteLine("      ");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine("      ");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine("      ");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" ");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" ");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 2:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" ");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |     ");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     ");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |     ");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 3:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    ");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     ");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    ");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 4:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     ");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    ");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 5:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     O");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    ");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 6:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     O");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |     |");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 7:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     O");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    /|");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 8:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     O");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    /|\\");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |    /");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            case 9:
                Console.SetCursorPosition(x, y);
                Console.WriteLine(" _______");
                Console.SetCursorPosition(x, y + 1);
                Console.WriteLine(" |/    |");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine(" |     O");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine(" |    /|\\");
                Console.SetCursorPosition(x, y + 4);
                Console.WriteLine(" |    / \\");
                Console.SetCursorPosition(x, y + 5);
                Console.WriteLine("_|_");
                break;
            default:
                break;
        }
    }
}






