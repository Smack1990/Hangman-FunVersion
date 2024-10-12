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
        Console.WriteLine("Enter your name:");
        string input = Console.ReadLine()!;
        if (input.Length >= 2)
          PlayerName = input;
        else
        {
            Console.WriteLine("Your name was too short. Please enter a valid name.");
            AskForUsersName();
        }
    }


}
