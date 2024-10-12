using System;
using System.Collections.Generic;
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

