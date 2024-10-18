using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Hangman_basic;
public class GameUX
{
    
    private CancellationTokenSource _cancellationTokenSource;
    static PeriodicTimer secondTimer;
    public int Secs = 26;
    public int Millis = 1000; // Start from 1000 ms for 1 second
    public int CountdownLimit = 0;
    public int countdownTime = 10;


    public static char[] Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\n  ZXCVBNM\n   _  ".ToCharArray(); // Keyboard for the game
    public void KeyboardCleanUp()
    {
        Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\n  ZXCVBNM\n   _  ".ToCharArray();

    } //Method to clean up the keyboard after each game

    public void HangmanLogo()
    {
        string[] logoLines = new string[]
        {
        @"         __  __                                       _______",
        @"         / / / /___ _____  ____ _____ ___  ____ _____  | |// |  ",
        @"       / /_/ / __ `/ __ \/ __ `/ __ `__ \/ __ `/ __ \ | //  O ",
        @"      / __  / /_/ / / / / /_/ / / / / / / /_/ / / / / | |  /|\",
        @"      /_/ /_/\__,_/_/ /_/\__, /_/ /_/ /_/\__,_/_/ /_/  | |  / \ ",
        @"                        /____/                        |_|....."
        };



        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int startRow = (windowHeight / 2) + 7;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(0, startRow);
        foreach (string line in logoLines)
        {
            int centeredPosition = (windowWidth - line.Length) / 2;
            Console.SetCursorPosition(centeredPosition, Console.CursorTop);
            Console.WriteLine(line);
        }
        Console.ResetColor();
    } // Logo for the game
    public void HangmanLogoTop() // logo hangman for the game
    {
        string[] linesHangman = new string[]
{
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||  /       / ",
        @"    | |/ /       ||    ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |  | \\   /   ",
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
};
        int windowWidth = Console.WindowWidth;
        foreach (string line in linesHangman)
        {
            Console.SetCursorPosition(45, Console.CursorTop);
            Console.WriteLine(line);
        }
    }

    public void TheRope()
    {
        string[] lines = new string[]
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
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        };
        int windowWidth = Console.WindowWidth;
        foreach (string line in lines)
        {
            Console.SetCursorPosition(45, Console.CursorTop);
            Console.WriteLine(line);
        }
    }

    public void DisplayScore(int correct, int incorrect, int wrongedGuessedInRow, int left, Player player)
    {
        
        string[] lines = new string[]
        {
        $"              Player: {player.PlayerName}",
        "             __________________________",
        $"                           {correct}      Guesses",
        $"                          {incorrect}    InCorrect",
        $"                            {left} Guesses left",
        $"                            {wrongedGuessedInRow}/5 wrong guesses in a row",
        $"             --------------------------"
        };

        // For each line, calculate the rightmost position and print it
        foreach (var line in lines)
        {
            int x = 7;
            Console.ForegroundColor = ConsoleColor.Red;
            int rightmostPosition = Console.WindowWidth - line.Length;
            Console.SetCursorPosition(rightmostPosition, Console.CursorTop);
            x++;
            Console.WriteLine(line);
        }
        Console.ResetColor();
    } // Method to display the score in the console
    public void DisplayKeyboard()
    {
        foreach (char key in Keyboard)
        {
            if (key == '\n')
            {
                Console.WriteLine();
                CenteredCursor();
            }
            else if (key == '_')
                Console.Write("[Space]");
            else
                Console.Write(key + " ");
        }
        Console.WriteLine();
        Console.ResetColor();
    }  // Method to display the keyboard in the console

    private void CenteredCursor()
    {
        int centerPosition = (Console.WindowWidth / 2) - (Keyboard.Length / 4);
        Console.SetCursorPosition(centerPosition, Console.CursorTop);
    } // Method to center the cursor in the console

    public void UpdateKeyboard(char guessedLetter)
    {
        guessedLetter = char.ToUpper(guessedLetter);
        for (int i = 0; i < Keyboard.Length; i++)
        {
            if (Keyboard[i] == guessedLetter)
            {
                Keyboard[i] = ' ';  // Replace guessed letter with a space
            }
        }
    } // Method to update the keyboard after a letter has been guessed
    public void Centered(string text) //Method for centuring the text in the console. Not using padLeft or padRigt to be able tu use the whole console window if needed.
    {
        int width = Console.WindowWidth / 2 - text.Length / 2;
        Console.SetCursorPosition(width, Console.CursorTop);
        Console.WriteLine(text);
    }

    public async Task StartTimer(CancellationToken token)
    {
        secondTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100)); // Timer ticks every 100ms
        GameLogic gameLogic = new GameLogic();
        while (await secondTimer.WaitForNextTickAsync(token))
        {
            Millis -= 100; // Minska millisekunderna med 100ms vid varje tick

            if (Millis <= 0)
            {
                Secs--; // När 1000ms har gått, minska sekunderna
                Millis = 1000; // Återställ millisekunder
            }

            // Display the time in tenths of a second (e.g., "10:5" instead of "10:500")
            int tenths = Millis / 100; // Convert milliseconds to tenths of a second

            // Update countdown display
            Console.SetCursorPosition(105, 8); // Adjust the position as needed
            Console.Write($"Timer: {Secs.ToString("00")}:{tenths}");


            // Stoppa nedräkningen och kalla Hanged() om tiden är slut
            if (Secs <= 0)
            {
                // Säkerställ att vi inte går under 0 sekunder
                Secs = 0;
                Millis = 0;
                gameLogic.IsGameOver = true;
                Console.Clear();
                // Visa "Time's up!" meddelande och stoppa nedräkningen
                Console.SetCursorPosition(2, 3);
                Console.WriteLine("Sorry but times up! Better luck next time.");
                Console.WriteLine();
                gameLogic.Hanged(); // Anropa Hanged() när tiden tar slut
                break; // Bryt ut från while-loopen
            }
            if (GameLogic.S_wrongGuessesInRow == 5 && Secs < 0)
            {
                gameLogic.IsGameOver = true;
                Console.Clear();
                token.ThrowIfCancellationRequested(); // Om timern har blivit avbruten
                secondTimer?.Dispose(); // Avsluta timern
                Console.SetCursorPosition(50, 3);
                Console.WriteLine("You have been hanged due to too many wrong guesses in row!");
                gameLogic.Hanged(); // Anropa Hanged() vid hängning
                break;

            }
            if (GameLogic.S_incorrectGuesses > 9)
            {
                token.ThrowIfCancellationRequested(); // Check for cancellation
                secondTimer.Dispose(); // Dispose of the timer
                gameLogic.IsGameOver = true; // Set game over flag
                gameLogic.Hanged(); // Call Hanged
                break;
            }



            if (token.IsCancellationRequested)
            {
                break; // Avsluta timern om den har blivit avbruten
            }
        }
    }
    public void ResetTimer()
    {
        _cancellationTokenSource?.Cancel(); // Cancel the previous countdown
        Secs = CountdownLimit; // Reset seconds to the original countdown time
        Millis = 1000; // Reset milliseconds to start from 1000ms
        _cancellationTokenSource = new CancellationTokenSource(); // Reset cancellation token source
        _ = StartTimer(_cancellationTokenSource.Token); // Restart the timer
    }


}





