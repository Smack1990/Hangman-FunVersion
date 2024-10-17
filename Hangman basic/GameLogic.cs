using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Hangman_basic;
using System.Threading.Tasks.Sources;
using System.Net;
using System.Runtime.CompilerServices;

namespace Hangman_basic;
public class GameLogic
{
    private static Word S_wordList = new Word();
    private static string? S_correctWord;
    private static char[]? S_letters;
    private static Player? S_player;
    public static int S_incorrectGuesses;
    private static GameUX gameUX = new GameUX();
    private static int S_windHeight = Console.WindowHeight / 2;
    private static int S_windwidth = (Console.WindowWidth / 2);
    private static string? S_clue;
    private static bool S_isModerate = false;
    private static bool S_isEasy = false;
    private static bool S_isHard = false;
    private static bool S_countdown = false;
    private static int S_countDownLimit = 0;
    public static int S_wrongGuessesInRow = 0;
    private static int S_left = 10 - S_incorrectGuesses;
    public bool IsGameOver = false;



    public void StartGame()
    {

        S_incorrectGuesses = 0;
        char upperChar;
        do
        {
            Console.Clear();
            gameUX.HangmanLogoTop();
            gameUX.HangmanLogo();
            gameUX.Centered("Press:  1:[Play Game] | 2:[Add words and clues to Moderate mode] | 3:[Add words to Hard mode]  ");
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        } while (upperChar != '1' && upperChar != '2' && upperChar != '3');

        switch (upperChar)
        {
            case '1':
                LevelChoice();
                break;
            case '3':
                Console.Clear();
                gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHeight - 1);

                gameUX.Centered("Enter a word to the dictionary");

                Console.SetCursorPosition(S_windwidth - 10, Console.CursorTop);
                S_wordList.WriteJson(Console.ReadLine());
                Console.Clear();

                StartGame();
                break;
            case '2':
                Console.Clear();
                gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHeight - 1);

                gameUX.Centered("Enter a word to the dictionary");
                Console.SetCursorPosition(S_windwidth - 10, Console.CursorTop);

                string word = Console.ReadLine();
                gameUX.Centered("Enter a clue to the word");
                Console.SetCursorPosition(S_windwidth - 10, Console.CursorTop);

                string clue = Console.ReadLine();
                S_wordList.AddWordToDictionary(word, clue);
                StartGame();
                break;
        }
    } //Entrencepoint to game
    private void PlayGame()
    {
        

        IsGameOver = false;
        bool level = true;
       


        string alreadyGuessed = string.Empty;
        char guessedLetter;
        int width = Console.WindowWidth / 2 - alreadyGuessed.Length / 2;
        int height = Console.WindowHeight / 2;
        var cts = new CancellationTokenSource();



        int count = 10;
        if (S_isModerate)
        {
            gameUX.Secs = 20;
            _ = gameUX.StartTimer(cts.Token);
        }
        else if (S_isHard)
        {
            gameUX.Secs = 10;
            _ = gameUX.StartTimer(cts.Token);
        }



        do
        {
            Console.Clear();


            Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
            gameUX.HangmanLogo();

            Console.SetCursorPosition(S_windwidth, 0);

            gameUX.DisplayScore(S_player.GuessedLetters.Count, S_incorrectGuesses, S_wrongGuessesInRow, S_left, S_player);

            if (!string.IsNullOrEmpty(alreadyGuessed))//Felmedelande vid samma knapptryck
                Console.SetCursorPosition(49, 8); Console.WriteLine($"{alreadyGuessed}");

            if (S_isEasy)
            {
                Console.SetCursorPosition(0, S_windHeight - 2);
                gameUX.Centered($"Clue: {S_clue}");
            }
            // Skriver ut ledtråd om Moderate är valt            
            if (S_isModerate && S_incorrectGuesses >= 4)
            {

                Console.SetCursorPosition(0, S_windHeight - 2);
                gameUX.Centered($"Clue: {S_clue}");
            }




            Console.SetCursorPosition(0, S_windHeight); DisplayMaskedWord(); // Skriver ut ordet med maskade bokstäver
            gameUX.DisplayKeyboard();// skriver ut tagentbordslayouten enligt QWERTY
            PrintingHangman(S_incorrectGuesses);// hangmangubben skrivs ut vid fel bokstav
            guessedLetter = AskForLetter();

            if (S_player.GuessedLetters.Contains(guessedLetter)) //promtar om bokstaven redan är gissad
                alreadyGuessed = $"You've already guessed:{guessedLetter}";
            else
            {
                alreadyGuessed = string.Empty;
                bool correct = CheckLetter(guessedLetter); //Checkar om bokstaven finns i ordet
                if (correct)
                {
                    S_player.Score++;
                    S_wrongGuessesInRow = 0;
                }
                else
                {
                    S_player.Score--;
                    S_incorrectGuesses++;
                    PrintingHangman(S_incorrectGuesses);
                    S_wrongGuessesInRow++;
                    if (S_wrongGuessesInRow == 5)
                    {
                        cts.Cancel();
                        IsGameOver = true;
                        Hanged();
                        break;
                    }
                    else if (S_wrongGuessesInRow == 4)
                    {
                        //gameUX.StopCountdown();
                        ShowWarning();
                        //gameUX.StartCountdown(10);
                    }
                }


                //gameUX.StopCountdown();
                //gameUX.StartCountdown(10);
            }

            if (S_incorrectGuesses > 9) // Spelet förlorat
            {


                IsGameOver = true;
                cts.Cancel(); // Stop the timer when the game ends

                Hanged();
                break;
            }
            if (S_correctWord == new string(S_letters))
            {
                //gameUX.StopCountdown(); // Stop countdown if the game is won
                cts.Cancel(); // Stop the timer when the game ends

                GameWon();
                break;
            }


            // Handle timeout if countdown reaches zero
            //if (gameUX.CountdownTime == 0)
            //{
            //    S_countDownLimit++;
            //    S_incorrectGuesses++;
            //gameUX.StartCountdown(10); // Restart countdown after timeout
            //}



        } while (!IsGameOver); // loop körs så länge det finns _ kvar i ordet

        cts.Cancel(); // Stop the timer when the game ends

        GameWon();
    }// Game logics

    private static void ShowWarning()
    {
        Console.Clear();
        string warning = "Be careful! You are about to get hanged.";
        string warningLine2 = "You guessed wrong 4/5 times in a row!";
        gameUX.TheRope();

        Console.SetCursorPosition(S_windwidth - warning.Length / 2, Console.CursorTop);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(warning);
        Console.SetCursorPosition(S_windwidth - warningLine2.Length / 2, Console.CursorTop);
        Console.WriteLine(warningLine2);
        Console.ResetColor();
        Console.Out.Flush();
        Thread.Sleep(3000);
    }

    private void GameWon()
    {
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        Console.Clear(); // Spelet vunnet
        //Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 3);
        Console.ForegroundColor = ConsoleColor.Green;
        gameUX.Centered($"Congratz {S_player.PlayerName}");
        gameUX.Centered($"Correct word: [{S_correctWord}] ");
        Console.ResetColor();
        EndGame();
        RestartGame();
    } // Spelet vunnet

    public void Hanged()
    {
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        Console.Clear();
        gameUX.HangmanLogoTop();
        Console.ForegroundColor = ConsoleColor.Red; gameUX.Centered($"You've been hanged."); Console.ResetColor();
        gameUX.Centered($"The correct word was [{S_correctWord}]"); Task.Delay(4000).Wait();
        Console.Clear();
        Console.WriteLine("Press a kay to continue");

        gameUX.HangmanLogoTop();
        //Console.SetCursorPosition(S_windwidth, S_windHeight + 8);

        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHeight + 7);

        //RestartGame();

    } // Spelet förlorat

    private static void GuessedLettersDisplay()
    {
        //int startX = 45;
        //int startY = 6;
        //int currentX = startX;

        //Console.SetCursorPosition(startX, startY);
        //Console.Write(new string(' ', Console.WindowWidth - startX));
        //foreach (var letter in S_player.GuessedLetters)
        //{
        //    if (S_correctWord.ToUpper().Contains(letter))
        //        Console.ForegroundColor = ConsoleColor.Green;
        //    else
        //        Console.ForegroundColor = ConsoleColor.Red;
        //    Console.SetCursorPosition(currentX, startY);
        //    Console.Write($"{letter.ToString()}, ");
        //    currentX += 2;
        //}
        //Console.ResetColor();
    }
    private void DisplayMaskedWord() //Positioning and toString of the masked word
    {
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        int widht = Console.WindowWidth / 2 - S_correctWord!.Length / 2;
        Console.SetCursorPosition(widht, Console.CursorTop);
        string maskedWord = new string(S_letters!);
        Console.Write(maskedWord);
        Console.WriteLine();
    }
    private char AskForLetter()
    {

        string text = "Guess a Letter by pressing a key";
        string textInvalid = "Invalid input. You can only use inputs from Keayboard layout!";
        int S_windwidth = Console.WindowWidth / 2 - text.Length / 2;
        int windwidth2 = Console.WindowWidth / 2 - textInvalid.Length / 2;

        while (true) // Loop until a valid letter is entered
        {
            Console.SetCursorPosition(S_windwidth, S_windHeight + 6);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
            char letter = char.ToUpper(Console.ReadKey(true).KeyChar);
             if (GameUX.Keyboard.Contains(letter))
            {
                if (letter == ' ')
                {
                    return '_';
                }
                return letter;
            }
            if (S_player != null && S_player.GuessedLetters.Contains(letter))
            {
                Console.SetCursorPosition(49, 8); Console.WriteLine($"You already guessed: {letter}");
            }
            else if (!GameUX.Keyboard.Contains(letter))
            {
                Console.SetCursorPosition(windwidth2, S_windHeight - 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textInvalid);
                Console.ResetColor();

            }

        }

    } // ask for input letter
    public static bool CheckLetter(char guessedLetter)
    {

        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        guessedLetter = char.ToUpper(guessedLetter);

        if (S_player!.GuessedLetters.Contains(guessedLetter))
            return false;
        S_player.GuessedLetters.Add(guessedLetter);
        bool found = false;
        for (int i = 0;  i < S_correctWord!.Length; i++)
        {
            if (S_correctWord[i] == ' ')
            {
                {
                    S_letters![i] = ' '; // Automatically fill in spaces
                    continue; // Skip to the next letter
                }
            }
            if (guessedLetter == char.ToUpper(S_correctWord[i]))
            {

                S_letters![i] = S_correctWord[i];
                found = true;
            }
        }

        //Scorehandler. Increases the amount of points with 2 if the letter is correct and decreases with 1 if it's wrong.
        if (found)
        {
            int pointsUp = 2;
            S_player.Score += pointsUp;
        }
        else
        {
            int pointsDown = 1;
            S_player.Score = Math.Max(0, S_player.Score - pointsDown);
        }
        gameUX.UpdateKeyboard(guessedLetter);

        return found;
    }  // Check if letter is correct
    private void EndGame()
    {
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        Console.SetCursorPosition(S_windwidth, S_windHeight); Console.WriteLine();
        //gameUX.Centered($"Thanks for playing {S_player!.PlayerName}");
        gameUX.Centered($"Total amount of guesses: {S_player.GuessedLetters.Count}");
        gameUX.Centered($"You made: {S_incorrectGuesses} incorrect guesses");
        gameUX.Centered($"Your final score: {S_player.Score}");
    } // Endgame
    private void RestartGame()
    {
        bool restart = false;
        char upperChar; 
        do
        {
            Console.Clear();
            string restartMessage = "Press: 1:[New Game] | 2:[Change Name] | 3: [Main Meny] | 4:[Quit Game] ";
            restart = false;
            S_windHeight = Console.WindowHeight / 2;
            S_windwidth = (Console.WindowWidth / 2 - restartMessage.Length / 2);
            gameUX.HangmanLogo();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(S_windwidth, S_windHeight + 6);
            Console.WriteLine(restartMessage);
            Console.ResetColor();
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        }
        while (upperChar != '1' && upperChar != '2' && upperChar != '3' && upperChar != '4');
        switch (upperChar)
        {
            case '1':
                Console.Clear();
                gameUX.KeyboardCleanUp();
                restart = true;
                if (S_isEasy)
                {
                    S_incorrectGuesses = 0;
                    S_countDownLimit = 0; //Lägg till alla räknare som skall nollställas.
                    S_wrongGuessesInRow = 0;
                    S_left = 10 - S_incorrectGuesses;
                    S_isEasy = true;
                    S_player.GuessedLetters.Clear();
                    var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                    S_correctWord = randomEntry.Key;
                    S_clue = randomEntry.Value;
                    S_letters = new char[S_correctWord.Length];
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';

                    PlayGame();
                }



                if (S_isModerate)
                {
                    restart = true;
                    S_isModerate = true;
                    var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                    S_correctWord = randomEntry.Key;
                    S_clue = randomEntry.Value;
                    S_letters = new char[S_correctWord.Length];
                    S_incorrectGuesses = 0;
                    S_player.GuessedLetters.Clear();
                    S_countDownLimit = 0; //Lägg till alla räknare som skall nollställas.
                    S_wrongGuessesInRow = 0;
                    S_left = 10 - S_incorrectGuesses;
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';
                    PlayGame();
                }
                //När man väljer att börja om. så går det inte skriva in. den går direkt till restartgame.

                if (!S_isModerate)
                {
                    restart = true;
                    S_countdown = true;
                    S_isModerate = false;  //Lägg till alla räknare som skall nollställas.
                    S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
                    S_letters = new char[S_correctWord.Length];
                    S_incorrectGuesses = 0;
                    S_player.GuessedLetters.Clear();
                    S_countDownLimit = 0; //Lägg till alla räknare som skall nollställas.
                    S_wrongGuessesInRow = 0;
                    S_left = 10 - S_incorrectGuesses;
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';
                    PlayGame();
                }
                break;
            case '2':
                Console.Clear();
                gameUX.KeyboardCleanUp();
                restart = false;
                LevelChoice();

                break;
            case '3':
                StartGame();
                break;
            case '4':
                Environment.Exit(0);
                break;
            default:

                Console.Clear();
                gameUX.KeyboardCleanUp();
                restart = false;
                RestartGame();
                break;
        }
    } //Restart game
    private void LevelChoice()
    {
        S_player = new Player("");
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        Console.Clear();

        S_player.AskForUsersName();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 10);
        gameUX.Centered($"Welcome {S_player.PlayerName}\n");
        gameUX.Centered("In this game you will try to guess the correct word,");
        gameUX.Centered("Depending on which mode you select you will be hanged if:");
        gameUX.Centered("You guess wrong 10 times.");
        gameUX.Centered("You guess wrong letter 5 consecutive times");
        gameUX.Centered("The runs out");
        gameUX.Centered("Clues will display either directly or after 4 wrong guesses depending on mode.");
        Console.WriteLine();
        gameUX.Centered(@" ------------------------------------------------------------------ ");
        gameUX.Centered(@"|  Press key:  | 1:[Easy] | 2:[Moderate] | 3:[Hard] | 4:[Quit Game] |");
        gameUX.Centered(@"|-------------------------|--------------|----------|---------------|");
        gameUX.Centered(@"| Random words |     X    |       X      |     X    |       Why     |");
        gameUX.Centered(@"| Clues        |     X    |       X      |          |      would    |");
        gameUX.Centered(@"| Timer 25s    |          |       X      |          |       you     |");
        gameUX.Centered(@"| Timer 50s    |          |              |     X    |      Quit?    |");
        gameUX.Centered(@" ------------------------------------------------------------------- ");
        //foreach (string line in linesHangman)
        //{
        //    Console.SetCursorPosition(45, Console.CursorTop);
        //    Console.WriteLine(line);
        //}


        Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
        gameUX.HangmanLogo();

        var keyPressed = Console.ReadKey(true).KeyChar;

        switch (char.ToUpper(keyPressed))
        {
            case '1':
                IsGameOver = false;
                S_isEasy = true;
                var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                S_correctWord = randomEntry.Key;
                S_clue = randomEntry.Value;
                S_letters = new char[S_correctWord.Length];
                for (int i = 0; i < S_correctWord.Length; i++)
                    S_letters[i] = '_';

                PlayGame();
                break;

            case '2':
                IsGameOver = false;
                S_isModerate = true;
                randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                S_correctWord = randomEntry.Key;
                S_clue = randomEntry.Value;
                S_letters = new char[S_correctWord.Length];
                for (int i = 0; i < S_correctWord.Length; i++)
                    S_letters[i] = '_';

                PlayGame();
                break;
            case '3':
                IsGameOver = false;
                S_isHard = true;
                S_countdown = true;
                S_isModerate = false;
                S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
                S_letters = new char[S_correctWord.Length];
                for (int i = 0; i < S_correctWord.Length; i++)
                    S_letters[i] = '_';

                PlayGame();
                break;
            case '4':
                Environment.Exit(0);
                break;

            default:
                Console.Clear();
                Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
                gameUX.HangmanLogo();
                gameUX.Centered("Invalid input");
                LevelChoice();
                break;
        }
    } //Which level to play
    private void PrintingHangman(int incorrectGuesses)
    {
        int x = 0;
        int y = 4;
        string[][] hangmanStages = new string[][]
{
    new string[]
    {
        @"     ",
        @"    ",
        @"     / /      /       / ",
        @"        ",
        @"           ",
        @"           ",
        @"         /         /",
        @"              ",
        @"  /          ",
        @"            ",
        @"        /     /   ",
        @"         ",
        @"/        ",
        @"             ",
        @"                     /   ",
        @"          /   ",
        @" /    /       ",
        @"    | |       ",
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     __",
        @"    | .",
        @"    | | ",
        @"    | | ",
        @"    | |",
        @"    | |",
        @"    | |        /",
        @"    | |  ",
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
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ",
        @"    | |/ /        ",
        @"    | | /        ",
        @"    | |/                /",
        @"    | |              ",
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
        //@"    . .          `'       . .",
    },
        new string[]
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
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
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
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y   ",
        @"    | |    /  // |         ",
        @"    | |      //  |    ",
        @"/   | |     ')   |    ",
        @"    | |                 /   ",
        @"    | |      /   ",
        @" /  | |  /      ",
        @"    | |       ",
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  |   |  \\",
        @"/   | |     ')   |  /|   (`",
        @"    | |          ",
        @"    | |               /   ",
        @"    | |      / ",
        @" /  | |  /       ",
        @"    | |         ",
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        //@"    . .          `'       . .",
    },
    new string[]
    {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  | . |  \\",
        @"/   | |     ')   | / |   (`",
        @"    | |          ||'",
        @"    | |          ||        /   ",
        @"    | |      /   || ",
        @" /  | |  /       || ",
        @"    | |        / | | ",
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        //@"    . .          `'       . .",
    },
    new string[]
      {
        @"     ___________.._______",
        @"    | .__________))______|",
        @"    | | / /      ||",
        @"    | |/ /       || ",
        @"    | | /        ||.-''.",
        @"    | |/         |/  _  \",
        @"    | |      /   ||  `/,|       /",
        @"    | |          (\\`_.' ",
        @"  / | |         .-`--'.",
        @"    | |        /Y . . Y\",
        @"    | |    /  // |   | \\   /   ",
        @"    | |      //  | . |  \\",
        @"/   | |     ')   | / |   (`",
        @"    | |          ||'||",
        @"    | |          || ||       /   ",
        @"    | |      /   || ||",
        @" /  | |  /       || ||",
        @"    | |        / |  | \",
        @"    ------------------------| ",
        @"    |-|-------------------|-| ",
        @"    | |  /            /   | |",
        @"    : :                   : :",
        //@"    . .          `'       . .",
    },


};

        switch (incorrectGuesses)
        {
            case 0:

                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                Console.SetCursorPosition(x, y);
                foreach (var line in hangmanStages[incorrectGuesses - 1])
                {
                    Console.WriteLine(line);
                    y++;
                    Console.SetCursorPosition(x, y);
                }
                break;
            default:
                break;
        }
    } //Printing the hangman





}