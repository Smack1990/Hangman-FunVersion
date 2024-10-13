using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Hangman_basic;
using System.Threading.Tasks.Sources;
using System.Net;

namespace Hangman_basic;
public class GameLogic
{
    private static Word S_wordList = new Word();
    private static string? S_correctWord;
    private static char[]? S_letters;
    private static Player? S_player;
    private static int S_incorrectGuesses;
    public GameUX gameUX = new GameUX();
    static int S_windHight = Console.WindowHeight / 2;
    static int S_windwidth = (Console.WindowWidth / 2);


    public void StartGame()
    {
        S_incorrectGuesses = 0;
        S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
        S_letters = new char[S_correctWord.Length];

        for (int i = 0; i < S_correctWord.Length; i++)
            S_letters[i] = '_';
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
                PlayGame();
                break;
            case 'A':
                Console.Clear();
                Console.SetCursorPosition(S_windwidth, S_windHight + 10);
                gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHight - 1);

                gameUX.Centered("Enter a word to the dictionary");
                Console.SetCursorPosition(S_windwidth, Console.CursorTop);
                S_wordList.WriteJson(Console.ReadLine()!);
                Console.Clear();
                StartGame();
                break;
        }
    }


    private void PlayGame()
    {
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
            gameUX.DisplayScore(S_player.GuessedLetters.Count, S_incorrectGuesses, S_player.Score);
            //Console.SetCursorPosition''(rightmostPosition, S_windHight -3); Console.Write($"Your guessed letters");
            //GuessedLettersDisplay();

            if (!string.IsNullOrEmpty(alreadyGuessed))//Felmedelande vid samma knapptryck
                Console.ForegroundColor = ConsoleColor.Green; Console.SetCursorPosition(48, 10); Console.WriteLine(alreadyGuessed); Console.ResetColor();

            Console.SetCursorPosition(0, S_windHight); DisplayMaskedWord();
            gameUX.DisplayKeyboard();// skriver ut tagentbordslayouten enligt QWERTY
            PrintingHangman(S_incorrectGuesses);
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
                    S_incorrectGuesses++;
                    //S_player.Score--;
                    PrintingHangman(S_incorrectGuesses);
                }
            }

            if (S_incorrectGuesses > 9)
            {
                Console.Clear();
                gameUX.Centered($"You lost! The correct word was [{S_correctWord}]");
                EndGame();
                RestartGame();
            }

        } while (S_correctWord != new string(S_letters));


        Console.Clear();
        Console.SetCursorPosition(S_windwidth, S_windHight - 3);
        gameUX.Centered($"Congratz {S_player.PlayerName}");
        gameUX.Centered($"Correct word: [{S_correctWord}] ");
        EndGame();
        RestartGame();




    }

    private static void GuessedLettersDisplay()
    {
        int startX = 45;
        int startY = 6;
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
        Console.SetCursorPosition(S_windwidth, S_windHight + 5);
        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(text); Console.ResetColor();
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

        GameUX gameUX = new GameUX();
        gameUX.UpdateKeyboard(guessedLetter);
        return found;
    }
    private void EndGame()
    {
        Console.SetCursorPosition(S_windwidth, S_windHight - 1); Console.WriteLine();
        //gameUX.Centered($"Thanks for playing {S_player!.PlayerName}");
        gameUX.Centered($"Total amount of guesses: {S_player.GuessedLetters.Count}");
        gameUX.Centered($"You made: {S_incorrectGuesses} incorrect guesses");
        gameUX.Centered($"Your final score: {S_player.Score}");
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
        int x = 0;
        int y = 4;
        string[][] hangmanStages = new string[][]
{
    // Stage 0: No incorrect guesses
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
    // Stage 1: One incorrect guess
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
    // Stage 2: Two incorrect guesses
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
    // Stage 3: Three incorrect guesses
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
    // Stage 4: Four incorrect guesses
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
    // Stage 5: Five incorrect guesses
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
        @"    | |        / | | \",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    // Stage 6: Six incorrect guesses
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
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    // Stage 7: Seven incorrect guesses
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
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    // Stage 8: Eight incorrect guesses
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
        @"    | |        / | | \",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
    // Stage 9: Nine incorrect guesses
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
        @"    | |        / | | \",
        @"    -----------|`-' `-'  |--| ",
        @"    |-|-------\ \       --|-| ",
        @"    | |  /     \ \    /   | |",
        @"    : :         \ \       : :",
        //@"    . .          `'       . .",
    },
};

        // Example switch-case structure using the hangmanStages
        switch (incorrectGuesses)
        {
            case 0:
                // No hangman displayed
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
                // Set the position to draw
                Console.SetCursorPosition(x, y);
                foreach (var line in hangmanStages[incorrectGuesses -1])
                {
                    Console.WriteLine(line);
                    y++; // Move to the next line down
                    Console.SetCursorPosition(x, y);
                }
                break;
            default:
                break;
        }
    }
}








