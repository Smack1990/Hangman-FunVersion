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


/* Broke the program more times than i fixed it. 
 * Learned a lot about how to structure a program and how to use classes and methods.
 * Learned filehandeling and dictionarys.
 * Learned how to work with ASCII art.
 * Used a lot of time on the game logic and the UX.
 * Been working with a lot of new things like the PeriodicTimer and the CancellationTokenSource.
 * Learn that adding specific functionalty while the program is functional can break the program.
 * Take aways: 
 * 1.Do a early map of what functions and classes you need and how they should interact.
 * 2.Do not add new functionalities before the program is fully functional.
 * 3.Dont work on separate functions at the same time, finish one before starting the next.
 * 4.Start building the classes from start. Don't move evertything to classes after the program is functional.
 * 5.Use the debugger more often.
 * 6.Ensure your code compiles at all time.  
 * Do better:
 * Exepction handling.(None in the program)*/