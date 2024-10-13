using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hangman_basic;
public class GameUX
{

    public static char[] Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\nZXCVBNM".ToCharArray();
    public void KeyboardCleanUp()
    {
        Keyboard = "\nQWERTYUIOPÅ\nASDFGHJKLÖÄ\nZXCVBNM".ToCharArray();

    }

    public void HangmanLogo()
    {
        string[] logoLines = new string[]
        {
        @"         __  __                                      ",
        @"        / / / /___ _____  ____ _____ ___  ____ _____ ",
        @"       / /_/ / __ `/ __ \/ __ `/ __ `__ \/ __ `/ __ \",
        @"      / __  / /_/ / / / / /_/ / / / / / / /_/ / / / /",
        @"     /_/ /_/\__,_/_/ /_/\__, /_/ /_/ /_/\__,_/_/ /_/ ",
        @"                       /____/                        "
        };




        int windowWidth = Console.WindowWidth;
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (string line in logoLines)
        {
            int centeredPosition = (windowWidth - line.Length) / 2;
            Console.SetCursorPosition(centeredPosition, Console.CursorTop);
            Console.WriteLine(line);
        }
        Console.ResetColor();

    }
    public void HangmanLogoTop()
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
    //@"    . .          `'       . .",
};
        int windowWidth = Console.WindowWidth;
        foreach (string line in linesHangman)
        {
            Console.SetCursorPosition(45, Console.CursorTop);
            Console.WriteLine(line);
        }
    }

    public void DisplayScore(int correct, int incorrect, int score)
    {
        string[] lines = new string[]
        {
        "                        ___________ ",
        $"                        |Correct   {correct}|",   // Left-align the numbers with a width of 3
        $"                        |InCorrect {incorrect}|", // Left-align with width 3
        $"                        |Score     {score}|",  // Left-align with width 3
        "                        ----------- "
        };

        // For each line, calculate the rightmost position and print it
        foreach (var line in lines)
        {
            int rightmostPosition = Console.WindowWidth - line.Length; // Calculate X for each line
            Console.SetCursorPosition(rightmostPosition, Console.CursorTop); // Set cursor at the rightmost position
            Console.WriteLine(line); // Print the line
        }
    }
        public void DisplayKeyboard()
    {
        int bottomrow = Console.WindowHeight - 3;



        foreach (char key in Keyboard)
        {
            if (key == '\n')
            {
                Console.WriteLine();
                CenteredCursor();
            }
            else
            {
                Console.Write(key + " ");
            }
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    private void CenteredCursor()
    {
        int centerPosition = (Console.WindowWidth / 2) - (Keyboard.Length / 3);
        Console.SetCursorPosition(centerPosition, Console.CursorTop);
    }

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
    }
    public void Centered(string text) //Method for centuring the text in the console. Not using padLeft or padRigt to be able tu use the whole console window if needed.
    {
        int width = Console.WindowWidth / 2 - text.Length / 2;
        Console.SetCursorPosition(width, Console.CursorTop);
        Console.WriteLine(text);
    }

}

