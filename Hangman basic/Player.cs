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
    public void AskForUsersName()
    {
        int width = Console.WindowWidth / 2 - 10;
        GameUX gameUX = new GameUX();
        Console.SetCursorPosition(width, Console.CursorTop); Console.Write("Enter your name:");

        Console.SetCursorPosition(width + 16, Console.CursorTop); string input = Console.ReadLine()!;
        if (input.Length >= 2)
          PlayerName = input;
        else
        {
            Console.Clear();
            gameUX.Centered("Your name was too short. Please enter a valid name.\n");
            AskForUsersName();
        }
    }


}
