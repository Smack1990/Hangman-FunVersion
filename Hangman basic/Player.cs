using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman_basic;
public class Player
{
    public string PlayerName { get; private set; }
    public int Score { get; set; }
    public List<char> GuessedLetters = new List<char>();
    public Player(string name)
    {
        PlayerName = name;
        GuessedLetters = new List<char>();
        Score = 0;
    }
    public void AskForUsersName() //Handels username input
    {
        int S_windHeight = Console.WindowHeight / 2;
        int S_windwidth = (Console.WindowWidth / 2);
        int width = Console.WindowWidth / 2 - 10;
        int hight = Console.WindowHeight / 2;

        GameUX gameUX = new GameUX();
        Console.SetCursorPosition(width, hight + 10);
        gameUX.HangmanLogo();
        
        Console.SetCursorPosition(width, hight); Console.Write("Enter your name:");
        Console.SetCursorPosition(width + 16, hight); string input = Console.ReadLine()!;
        Console.SetCursorPosition(width, hight);
        string inputToUpper = char.ToUpper(input[0]) + input.Substring(1);
        Console.Clear();

        if (inputToUpper.Length >= 2)
            PlayerName = inputToUpper;
        else
        {
            Console.Clear();
            Console.SetCursorPosition(width, hight + 10);
            gameUX.HangmanLogo();
            Console.SetCursorPosition(width, hight +1);
            gameUX.Centered("Your name was too short. Please enter a valid name.\n");

            AskForUsersName();
        }
        Console.Clear();
    }


}
