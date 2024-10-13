using Hangman_basic;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.Json;
using System.Xml.Linq;

namespace Hangman
{
    class Program
    {

 
        static void Main(string[] args)
        {
            
            //HangmanBoring. gör tre olika listor att ladda beroende på nivå. 
            //svåra långa ord för hard. Lätta ord för easy.Skriv ut en text i hangman beroende på vilken nivå man valt. "Easey mode activated, i denna klass hanterar vi enkla adjektiv typ" eller "Hard mode activated"
            
            GameLogic gameLogic = new GameLogic();
            GameUX gameUX = new GameUX();   
            bool logo = false;
        
            gameLogic.StartGame();
        }
        //private static void StartGame()
        //{
        //    S_player = new Player("Unknown");
        //    S_player.AskForUsersName();

        //    GameLogic gameLogic = new GameLogic(S_wordList, S_player);
        //    gameLogic.StartGame();
        //}

    }

}


