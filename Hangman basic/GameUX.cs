using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hangman_basic;
public class GameUX
{

    private CancellationTokenSource _cancellationTokenSource;
    static PeriodicTimer secondTimer;
    public int Secs = 10;
    public int Millis = 1000; 
    public int CountdownLimit = 10;


    public static char[] Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\n  ZXCVBNM\n   _  ".ToCharArray(); // Keyboard for the game
    public static char[] KeyboardEasy = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\n  ZXCVBNM\n     ".ToCharArray(); // Keyboard for the game
    public void KeyboardCleanUp() //Method to clean up the keyboard after each game
    {
        Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\n  ZXCVBNM\n   _  ".ToCharArray();

    }
    #region Logos, Graphics and Score
    public void HangmanLogo()// Logo for the game
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
    } 
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

    public void TheRope() //Warning logo with th rope
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

    public void DisplayScore(int correct, int incorrect, int wrongedGuessedInRow, int left, Player player)// Method to display the score in the console
    {
        left = 10 - GameLogic.S_incorrectGuesses;

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
    
            Console.ForegroundColor = ConsoleColor.Red;
            int rightmostPosition = Console.WindowWidth - line.Length;
            Console.SetCursorPosition(rightmostPosition, Console.CursorTop);
    
            Console.WriteLine(line);
        }
        Console.ResetColor();
    }

    public void DisplayKeyboard() // Method to display the keyboard in the console
    {
        
        // Determine which keyboard to use based on game mode
        char[] currentKeyboard = GameLogic.S_isHard ? Keyboard : KeyboardEasy;

        foreach (char key in currentKeyboard)
        {
            if (key == '\n')
            {
                Console.WriteLine();
                CenteredCursor(); // Center the cursor for new lines
            }
            else if (key == '_')
            {
                Console.Write("[Space] "); // Display for space
            }
            else
            {
                Console.Write(key + " "); // Display for other keys
            }
        }
        Console.WriteLine(); // New line after displaying the keyboard
        Console.ResetColor(); // Reset console color
    }

    public void UpdateKeyboard(char guessedLetter)// Method to update the keyboard after a letter has been guessed
    {
        guessedLetter = char.ToUpper(guessedLetter);
        for (int i = 0; i < Keyboard.Length; i++)
        {
            if (Keyboard[i] == guessedLetter)
            {
                Keyboard[i] = ' ';  // Replace guessed letter with a space
            }
        }
    }
    #endregion 
    private void CenteredCursor() // Method to center the cursor in the console
    {
        int centerPosition = (Console.WindowWidth / 2) - (Keyboard.Length / 4);
        Console.SetCursorPosition(centerPosition, Console.CursorTop);
    }


    public void Centered(string text) //Method for centuring the text in the console. Not using padLeft or padRigt to be able tu use the whole console window if needed.
    {
        int width = Console.WindowWidth / 2 - text.Length / 2;
        Console.SetCursorPosition(width, Console.CursorTop);
        Console.WriteLine(text);
    }

    public async Task StartTimer(CancellationToken token)
    {
        string text = "Sorry but time's up! Better luck next time.";
        string text2 = "Press any key to continue.";
        int windW = Console.WindowWidth / 2 -text.Length / 2;
        int windW2 = Console.WindowWidth / 2 -text2.Length / 2;
        GameLogic gameLogic = new GameLogic();
        secondTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100)); 
        while (await secondTimer.WaitForNextTickAsync(token))
        {
            Millis -= 100;

            if (Millis <= 0)
            {
                Secs--; 
                Millis = 1000; 
            }

            // Update the timer display
            int tenths = Millis / 100; 
            Console.SetCursorPosition(105, 8);
            Console.Write($"Timer: {Secs.ToString("00")}:{tenths}");

            // Check if time has expired
            if (Secs <= 0)
            {
                Secs = 0; 

                Console.Clear();
                Console.SetCursorPosition(windW, 3);
                Console.WriteLine(text);
                gameLogic.IsGameOver = true;
                HangmanLogoTop();
                HangmanLogo();
                Console.SetCursorPosition(windW2, Console.CursorTop); Console.WriteLine(text2);

                
                break; 
            }



            if (token.IsCancellationRequested)
            {
                break; 
            }
        }
    }



}





