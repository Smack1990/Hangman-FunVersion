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
            GameLogic gameLogic = new GameLogic();
            GameUX gameUX = new GameUX();   
            gameLogic.StartGame();
        }


    }

}


