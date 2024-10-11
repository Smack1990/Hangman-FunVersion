using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman_basic;
internal class Player
{
    public string PlayerName { get; private set; }
    public int Score { get; set; }
    public List<char> guessedLetters = new List<char>();
    public Player(string name)
    {
        PlayerName = name;
        guessedLetters = new List<char>();  
        Score = 0;

    }


}
