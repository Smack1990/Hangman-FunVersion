using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Hangman_basic;
using System.Threading.Tasks.Sources;
using System.Net;
using System.Runtime.CompilerServices;

namespace Hangman_basic;
public class GameLogic
{
    private static Word S_wordList = new Word();
    private static string? S_correctWord;
    private static char[]? S_letters;
    private static Player? S_player;
    private static int S_incorrectGuesses;
    private static GameUX gameUX = new GameUX();
    static int S_windHight = Console.WindowHeight / 2;
    private static string? S_clue;
    private static int S_windwidth = (Console.WindowWidth / 2);
    private static bool S_isModerate = false;



    public void StartGame()
    {

        S_incorrectGuesses = 0;
        char upperChar;
        do
        {
            Console.Clear();
            gameUX.HangmanLogoTop();
            gameUX.HangmanLogo();
            gameUX.Centered("[P]lay Game | [A]dd word to Hard mode list");
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        } while (upperChar != 'P' && upperChar != 'A');

        switch (upperChar)
        {
            case 'P':
                LevelChoice();
                break;
            case 'A':
                Console.Clear();
                //Console.SetCursorPosition(S_windwidth, S_windHight + 10);
                gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHight - 1);

                gameUX.Centered("Enter a word to the dictionary");
                Console.SetCursorPosition(S_windwidth, Console.CursorTop);
                S_wordList.WriteJson(Console.ReadLine()!);
                Console.Clear();
                gameUX.Centered("The word was added!"); Task.Delay(2000).Wait();
                StartGame();
                break;
        }
    } //Entrencepoint to game
    private void PlayGame()
    {
        bool level = true;
        S_player = new Player("");
        S_player.AskForUsersName();
        string alreadyGuessed = string.Empty;
        char guessedLetter;
        int width = (Console.WindowWidth / 2 - alreadyGuessed.Length / 2);
        string text = "                      ";
        int rightmostPosition = Console.WindowWidth - text.Length;
        do
        {
            Console.Clear();
            Console.SetCursorPosition(S_windwidth, S_windHight + 10);
            gameUX.HangmanLogo();
            Console.SetCursorPosition(S_windwidth, 0);
            int left = 10 - S_incorrectGuesses;
            gameUX.DisplayScore(S_player.GuessedLetters.Count, S_incorrectGuesses, S_player.Score, left);
            if (!string.IsNullOrEmpty(alreadyGuessed))//Felmedelande vid samma knapptryck
                Console.ForegroundColor = ConsoleColor.Green; Console.SetCursorPosition(48, 10); Console.WriteLine(alreadyGuessed); Console.ResetColor();
            // Skriver ut ledtråd om Moderate är valt            
            if (S_isModerate)
            {
                Console.SetCursorPosition(0, S_windHight - 2);
                gameUX.Centered($"Clue: {S_clue}");
            }

            Console.SetCursorPosition(0, S_windHight); DisplayMaskedWord(); // Skriver ut ordet med maskade bokstäver
            gameUX.DisplayKeyboard();// skriver ut tagentbordslayouten enligt QWERTY
            PrintingHangman(S_incorrectGuesses);// hangmangubben skrivs ut vid fel bokstav
            guessedLetter = AskForLetter();
            if (guessedLetter == '\0')
            {
                alreadyGuessed = "Invalid input! Please choose a letter";
                continue;
            }
            if (S_player.GuessedLetters.Contains(guessedLetter)) //promtar om bokstaven redan är gissad
                alreadyGuessed = $"You've already guessed: {guessedLetter}!";
            else
            {
                alreadyGuessed = string.Empty;
                bool correct = CheckLetter(guessedLetter); //Checkar om bokstaven finns i ordet
                if (!correct)
                {
                    S_incorrectGuesses++;
                    PrintingHangman(S_incorrectGuesses);
                }
            }
            if (S_incorrectGuesses > 9) // Spelet förlorat
            {
                Hanged();
            }

        } while (S_correctWord != new string(S_letters)); // loop körs så länge det finns _ kvar i ordet
        GameWon();
    }// Game logics

    private void GameWon()
    {
        Console.Clear(); // Spelet vunnet
        Console.SetCursorPosition(S_windwidth, S_windHight - 10);
        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHight - 3);
        Console.ForegroundColor = ConsoleColor.Green;
        gameUX.Centered($"Congratz {S_player.PlayerName}");
        gameUX.Centered($"Correct word: [{S_correctWord}] ");
        Console.ResetColor();
        EndGame();
        RestartGame();
    } // Spelet vunnet

    private void Hanged()
    {
        Console.Clear();
        gameUX.HangmanLogoTop();
        Console.ForegroundColor = ConsoleColor.Red; gameUX.Centered($"You've been hanged."); Console.ResetColor();
        gameUX.Centered($"The correct word was [{S_correctWord}]"); Task.Delay(4000).Wait();
        Console.Clear();

        gameUX.HangmanLogoTop();
        Console.SetCursorPosition(S_windwidth, S_windHight + 8);

        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHight + 7);
        RestartGame();

    } // Spelet förlorat

    //private static void GuessedLettersDisplay()  
    //{
    //    int startX = 45;
    //    int startY = 6;
    //    int currentX = startX;

    //    Console.SetCursorPosition(startX, startY);
    //    Console.Write(new string(' ', Console.WindowWidth - startX));
    //    foreach (var letter in S_player.GuessedLetters)
    //    {
    //        if (S_correctWord.ToUpper().Contains(letter))
    //            Console.ForegroundColor = ConsoleColor.Green;
    //        else
    //            Console.ForegroundColor = ConsoleColor.Red;
    //        Console.SetCursorPosition(currentX, startY);
    //        Console.Write($"{letter.ToString()}, ");
    //        currentX += 2;
    //    }
    //    Console.ResetColor();
    //}
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
        string textInvalid = "Invalid input! Please choose a letter from the keyboard.";
        int S_windwidth = Console.WindowWidth / 2 - text.Length / 2;
        int windwidth2 = Console.WindowWidth / 2 - textInvalid.Length / 2;
        while (true) // Loop until a valid letter is entered
        {
            Console.SetCursorPosition(S_windwidth, S_windHight + 6);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
            char letter = char.ToUpper(Console.ReadKey(true).KeyChar);
            if (GameUX.Keyboard.Contains(letter))
            {
                if (letter == ' ')
                {
                    return '_';
                }
                return letter;
            }
            else
            {
                Console.SetCursorPosition(windwidth2, S_windHight + -3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textInvalid);
                Console.ResetColor();
            }
        }
    } // ask for input letter
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
            }
        }
        //Scorehandler. Increases the amount of points with 2 if the letter is correct and decreases with 1 if it's wrong.
        if (found)
        {
            int pointsUp = 2;
            S_player.Score += pointsUp;
        }
        else
        {
            int pointsDown = 1;
            S_player.Score = Math.Max(0, S_player.Score - pointsDown);
        }

        gameUX.UpdateKeyboard(guessedLetter);
        return found;
    }  // Check if letter is correct
    private void EndGame()
    {
        Console.SetCursorPosition(S_windwidth, S_windHight); Console.WriteLine();
        //gameUX.Centered($"Thanks for playing {S_player!.PlayerName}");
        gameUX.Centered($"Total amount of guesses: {S_player.GuessedLetters.Count}");
        gameUX.Centered($"You made: {S_incorrectGuesses} incorrect guesses");
        gameUX.Centered($"Your final score: {S_player.Score}");
    } // Endgame
    private void RestartGame()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        gameUX.Centered("Do you want to play again? [Y]es or [N]o");
        Console.ResetColor();
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


                RestartGame();
                break;
        }
    } //Restart game
    private void LevelChoice()
    {
        Console.Clear();
        Console.SetCursorPosition(S_windwidth, S_windHight + 10);
        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHight - 6);
        gameUX.Centered("In this game you will try to guess the correct word");
        gameUX.Centered("You will earn two points for every correct guessed letter");
        gameUX.Centered("and you will lose one point for every incorrect one.");
        gameUX.Centered("Try to get as many points as possible by thinking through your decision");
        Console.WriteLine();
        gameUX.Centered("Choose difficulty level");
        gameUX.Centered("Moderate mode has clues, and Hard mode is just what it is.");
        gameUX.Centered("Ah! By the way... We do not do Easy mode...");
        Console.WriteLine();
        gameUX.Centered("[M]oderate | [H]ard");

        var keyPressed = Console.ReadKey(true).KeyChar;

        switch (char.ToUpper(keyPressed))
        {
            case 'M':
                S_isModerate = true;
                var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                S_correctWord = randomEntry.Key;
                S_clue = randomEntry.Value;
                S_letters = new char[S_correctWord.Length];
                for (int i = 0; i < S_correctWord.Length; i++)
                    S_letters[i] = '_';
                PlayGame();
                break;

            case 'H':
                S_isModerate = false;
                S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
                S_letters = new char[S_correctWord.Length];
                for (int i = 0; i < S_correctWord.Length; i++)
                    S_letters[i] = '_';
                PlayGame();
                break;

            default:
                Console.Clear();
                Console.SetCursorPosition(S_windwidth, S_windHight + 10);
                gameUX.HangmanLogo();
                gameUX.Centered("Invalid input");
                LevelChoice();
                break;
        }
    } //Which level to play
    private void PrintingHangman(int incorrectGuesses)
    {
        int x = 0;
        int y = 4;
        string[][] hangmanStages = new string[][]
{
    new string[]
    {
        @"     ",
        @"    ",
        @"     / /      /       / ",
        @"        ",
        @"           ",
        @"           ",
        @"         /         /",
        @"              ",
        @"  /          ",
        @"            ",
        @"        /     /   ",
        @"         ",
        @"/        ",
        @"             ",
        @"                     /   ",
        @"          /   ",
        @" /    /       ",
        @"    | |       ",
        @"    -----------|         |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     __",
        @"    | .",
        @"    | | ",
        @"    | | ",
        @"    | |",
        @"    | |",
        @"    | |        /",
        @"    | |  ",
        @"  / | |  .",
        @"    | |        ",
        @"    | |       /   ",
        @"    | |      ",
        @"/   | |     ",
        @"    | |        ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------          |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ",
        @"    | |/ /        ",
        @"    | | /        ",
        @"    | |/                /",
        @"    | |              ",
        @"  / | |  .",
        @"    | |        ",
        @"    | |       /   ",
        @"    | |      ",
        @"/   | |     ",
        @"    | |        ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------          |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
        new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||",
        @"    | |/         |/    /",
        @"    | |          ||    ",
        @"  / | |  .",
        @"    | |        ",
        @"    | |       /   ",
        @"    | |      ",
        @"/   | |     ",
        @"    | |        ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------          |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |       /   ",
        @"    | |      ",
        @"/   | |     ",
        @"    | |        ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------          |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y   ",
        @"    | |    /  // |         ",
        @"    | |      //  |    ",
        @"/   | |     ')   |    ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------          |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  |   |  \\",
        @"/   | |     ')   |  /|   (`",
        @"    | |          ",
        @"    | |               /   ",
        @"    | |      / ",
        @" /  | |  /       ",
        @"    | |         ",
        @"    -----------|         |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  | . |  \\",
        @"/   | |     ')   | / |   (`",
        @"    | |          ||'",
        @"    | |          ||        /   ",
        @"    | |      /   || ",
        @" /  | |  /       || ",
        @"    | |        / | | ",
        @"    -----------|`-'      |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    new string[]
      {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  | . |  \\",
        @"/   | |     ')   | / |   (`",
        @"    | |          ||'||",
        @"    | |          || ||       /   ",
        @"    | |      /   || ||",
        @" /  | |  /       || ||",
        @"    | |        / |  | \",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },


};

        switch (incorrectGuesses)
        {
            case 0:

                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                Console.SetCursorPosition(x, y);
                foreach (var line in hangmanStages[incorrectGuesses - 1])
                {
                    Console.WriteLine(line);
                    y++;
                    Console.SetCursorPosition(x, y);
                }
                break;
            default:
                break;
        }
    } //Printing the hangman





}