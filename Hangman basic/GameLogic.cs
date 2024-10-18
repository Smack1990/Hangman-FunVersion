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
    public static string? S_correctWord;
    private static char[]? S_letters;
    private static Player? S_player;
    public static int S_incorrectGuesses = 0;
    private static GameUX S_gameUX = new GameUX();
    private static int S_windHeight = Console.WindowHeight / 2;
    private static int S_windwidth = Console.WindowWidth / 2;
    private string? S_clue;
    public bool IsModerate = false;
    public bool IsEasy = false;
    public bool IsHard = false;
    private static bool S_countdown = false;
    private static int S_countDownLimit = 0;
    public int WrongGuessesInRow = 0;
    private int _left = 10 - S_incorrectGuesses;
    public bool IsGameOver = false;



    public void StartGame()
    {


        char upperChar;
        do
        {
            Console.Clear();
            S_gameUX.HangmanLogoTop();
            S_gameUX.HangmanLogo();
            S_gameUX.Centered("Press:  1:[Play Game] | 2:[Add words and clues to Moderate mode] | 3:[Add words to Hard mode]  ");
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        } while (upperChar != '1' && upperChar != '2' && upperChar != '3');

        switch (upperChar)
        {
            case '1':
                LevelChoice();
                break;
            case '3':
                Console.Clear();
                S_gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHeight - 1);

                S_gameUX.Centered("Enter a word to the dictionary");

                Console.SetCursorPosition(S_windwidth - 10, Console.CursorTop);
                S_wordList.WriteJson(Console.ReadLine());
                Console.Clear();

                StartGame();
                break;
            case '2':
                Console.Clear();
                S_gameUX.HangmanLogo();
                Console.SetCursorPosition(S_windwidth, S_windHeight - 1);

                S_gameUX.Centered("Enter a word to the dictionary");
                Console.SetCursorPosition(S_windwidth - 10, Console.CursorTop);

                string word = Console.ReadLine();
                S_gameUX.Centered("Enter a clue to the word");
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
        S_incorrectGuesses = 0; // Reset incorrect guesses
         // Initialize player
        S_player.GuessedLetters.Clear(); // Clear guessed letters
        var cts = new CancellationTokenSource();
        S_gameUX.Secs = 10;
        _left = 10 - S_incorrectGuesses;

        // Start timer
        if (IsModerate)
        {
            S_gameUX.Secs = 60;
            _ = S_gameUX.StartTimer(cts.Token);
        }
        else if (IsHard)
        {
            S_gameUX.Secs = 30;
            _ = S_gameUX.StartTimer(cts.Token);
        }

        do
        {
            Console.Clear();
            S_gameUX.DisplayScore(S_player.GuessedLetters.Count, S_incorrectGuesses, WrongGuessesInRow, _left, S_player);
            Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
            S_gameUX.HangmanLogo();
            Console.SetCursorPosition(0, S_windHeight);


            if (IsEasy || (IsModerate && S_incorrectGuesses >= 4))
            {
                Console.SetCursorPosition(0, S_windHeight - 2);
                S_gameUX.Centered($"Clue: {S_clue}");
            }

            //Check if time has run out
            Console.SetCursorPosition(0, S_windHeight); DisplayMaskedWord();
            S_gameUX.DisplayKeyboard();
            PrintingHangman(S_incorrectGuesses); // Method to display game status

            if (S_gameUX.Secs <= 0)
            {
                cts.Cancel();
                IsGameOver = true; // Set game as over
                break; // Exit the loop
            }

            // Only ask for letter if game is still ongoing
            if (!IsGameOver)
            {
                char guessedLetter = AskForLetter(); // Ask user to guess a letter
                if (S_player.GuessedLetters.Contains(guessedLetter))
                {
                    Console.WriteLine($"You've already guessed: {guessedLetter}");
                }
                else
                {
                    //S_player.GuessedLetters.Add(guessedLetter);
                    S_gameUX.UpdateKeyboard(guessedLetter);// Add guessed letter
                    if (CheckLetter(guessedLetter))
                    {
                        WrongGuessesInRow = 0;
                        Console.SetCursorPosition(0, S_windHeight); DisplayMaskedWord();
                        // Logic for correct guess
                    }
                    else
                    {
                        S_incorrectGuesses++;
                        WrongGuessesInRow++;

                        if (S_incorrectGuesses >= 10)
                        {
                            cts.Cancel();
                            IsGameOver = true; 
                            Hanged();
                            break;
                        }
                        else if (WrongGuessesInRow == 4)
                        {
                            ShowWarning();
                        }
                    }
                }
                if ((S_correctWord == new string(S_letters)))
                {
                    cts.Cancel();
                    IsGameOver = true;
                    GameWon();
                    break;
                }
                if (S_incorrectGuesses > 9)
                {
                    cts.Cancel();
                    IsGameOver = true; 
                    Hanged(); 
                    break; 
                }
            }

        } while (!IsGameOver); 

        RestartGame();
    }
    //private void DisplayGameStatus()
    //{
    //    // Display game status: score, clues, masked word, etc.
    //    gameUX.DisplayScore(S_player.GuessedLetters.Count, S_incorrectGuesses, S_left, S_wrongGuessesInRow, S_player);
    //    Console.SetCursorPosition(0, S_windHeight);
    //    DisplayMaskedWord();
    //    gameUX.DisplayKeyboard();
    //    PrintingHangman(S_incorrectGuesses);
    //}

    private static void ShowWarning()
    {
        Console.Clear();
        string warning = "Be careful! You are about to get hanged.";
        string warningLine2 = "You guessed wrong 4/5 times in a row!";
        S_gameUX.TheRope();

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
        Console.Clear();
        Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
        S_gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 3);
        Console.ForegroundColor = ConsoleColor.Green;
        S_gameUX.Centered($"Congratz {S_player.PlayerName}");
        S_gameUX.Centered($"Correct word: [{S_correctWord}] ");
        Console.ResetColor();
        EndGame();
        S_gameUX.Centered("Press any key to continue");
        Console.ReadKey(true);
    }

    public void Hanged()
    {
        Console.Clear();
        S_gameUX.HangmanLogoTop();
        Console.ForegroundColor = ConsoleColor.Red;
        S_gameUX.Centered("You've been hanged.");
        Console.ResetColor();
        S_gameUX.Centered($"The correct word was [{S_correctWord}]");
        S_gameUX.Centered("Press any key to continue...");
        Console.ReadKey(true); 
        IsGameOver = true;
        RestartGame();
    }


    private void DisplayMaskedWord() //Positioning and toString of the masked word
    {

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
            if (S_gameUX.Secs <= 0)
                break;
            Console.SetCursorPosition(S_windwidth, S_windHeight + 6);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
            char letter = char.ToUpper(Console.ReadKey(true).KeyChar);
            if (GameUX.Keyboard.Contains(letter))
            {
                if (letter == ' ')
                    return '_';
                return letter;
            }
            if (S_player != null && S_player.GuessedLetters.Contains(letter))
            {

            Console.SetCursorPosition(S_windwidth + 8, S_windHeight - 4);
                Console.WriteLine($"Already guessed: {letter}");

            }
            else if (!GameUX.Keyboard.Contains(letter))
            {
                Console.SetCursorPosition(windwidth2, S_windHeight - 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(textInvalid);
                Console.ResetColor();
            }
        }
        return '\n';

    } // ask for input letter
    public static bool CheckLetter(char guessedLetter)
    {
        guessedLetter = char.ToUpper(guessedLetter);
        if (S_player!.GuessedLetters.Contains(guessedLetter))
            return false;
        S_player.GuessedLetters.Add(guessedLetter);
        bool found = false;
        for (int i = 0; i < S_correctWord!.Length; i++)
        {
            if (S_correctWord[i] == ' ')
            {
                {
                    S_letters![i] = ' '; 
                    continue; 
                }
            }
            if (guessedLetter == char.ToUpper(S_correctWord[i]))
            {
                S_letters![i] = S_correctWord[i];
                found = true;
            }
        }
        S_gameUX.UpdateKeyboard(guessedLetter);
        return found;
    }  // Check if letter is correct

    private void EndGame()
    {
        S_windHeight = Console.WindowHeight / 2;
        S_windwidth = (Console.WindowWidth / 2);
        Console.SetCursorPosition(S_windwidth, S_windHeight);
        Console.WriteLine();

        // Determine elapsed time
        int elapsedTime = 0;
        if (IsModerate)
            elapsedTime = 60 - S_gameUX.Secs;
        else if (IsHard)
            elapsedTime = 30 - S_gameUX.Secs;
        S_gameUX.Centered($"Total amount of guesses: {S_player.GuessedLetters.Count}");
        S_gameUX.Centered($"You made: {S_incorrectGuesses} incorrect guesses");
        if (IsModerate || IsHard)
            S_gameUX.Centered($"Elapsed time: {elapsedTime} seconds");
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

            S_windwidth = (Console.WindowWidth / 2 - restartMessage.Length / 2);
            S_gameUX.HangmanLogo();
            Console.SetCursorPosition(S_windwidth, S_windHeight);
            Console.WriteLine(restartMessage);
            upperChar = char.ToUpper(Console.ReadKey(true).KeyChar);
        }
        while (upperChar != '1' && upperChar != '2' && upperChar != '3' && upperChar != '4');
        switch (upperChar)
        {
            case '1':
                Console.Clear();
                S_gameUX.KeyboardCleanUp();
                restart = true;
                if (IsEasy)
                {
                    S_incorrectGuesses = 0;
                    WrongGuessesInRow = 0;
                    _left = 10 - S_incorrectGuesses;
                    IsEasy = true;
                    S_player.GuessedLetters.Clear();
                    var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                    S_correctWord = randomEntry.Key;
                    S_clue = randomEntry.Value;
                    S_letters = new char[S_correctWord.Length];
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';
                    PlayGame();
                }



                if (IsModerate)
                {
                    S_gameUX.KeyboardCleanUp();
                    restart = true;
                    IsModerate = true;
                    var randomEntry = S_wordList.WordClue.ElementAt(new Random().Next(0, S_wordList.WordClue.Count));
                    S_correctWord = randomEntry.Key;
                    S_clue = randomEntry.Value;
                    S_letters = new char[S_correctWord.Length];
                    S_incorrectGuesses = 0;
                    S_player.GuessedLetters.Clear();
                    WrongGuessesInRow = 0;
                    _left = 10 - S_incorrectGuesses;
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';
                    PlayGame();
                }
                if (IsHard)
                {
                    S_gameUX.KeyboardCleanUp();
                    restart = true;
                    S_countdown = true;
                    S_correctWord = S_wordList.WordList[new Random().Next(0, S_wordList.WordList.Count)];
                    S_letters = new char[S_correctWord.Length];
                    S_incorrectGuesses = 0;
                    S_player.GuessedLetters.Clear();
                    WrongGuessesInRow = 0;
                    _left = 10 - S_incorrectGuesses;
                    for (int i = 0; i < S_correctWord.Length; i++)
                        S_letters[i] = '_';
                    PlayGame();
                }
                break;
            case '2':
                Console.Clear();
                restart = true;
                S_incorrectGuesses = 0;
                S_gameUX.KeyboardCleanUp();
                WrongGuessesInRow = 0;
                _left = 10 - S_incorrectGuesses;
                restart = false;
                LevelChoice();
                break;
            case '3':
                restart = true;
                S_incorrectGuesses = 0;
                S_gameUX.KeyboardCleanUp();
                WrongGuessesInRow = 0;
                _left = 10 - S_incorrectGuesses;
                S_windwidth = Console.WindowWidth / 2;
                StartGame();
                break;
            case '4':
                Environment.Exit(0);
                break;
            default:

                Console.Clear();
                S_gameUX.KeyboardCleanUp();
                restart = false;
                RestartGame();
                break;
        }
    } //Restart game
    private void LevelChoice()
    {
        S_player = new Player("");

        Console.Clear();

        S_player.AskForUsersName();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 10);
        S_gameUX.Centered($"Welcome {S_player.PlayerName}\n");
        S_gameUX.Centered("In this game you will try to guess the correct word,");
        S_gameUX.Centered("Depending on which mode you select you will be hanged if:");
        S_gameUX.Centered("You guess wrong 10 times.");
        S_gameUX.Centered("You guess wrong letter 5 consecutive times");
        S_gameUX.Centered("The time runs out");
        S_gameUX.Centered("Clues will display either directly or after 4 wrong guesses depending on mode.");
        Console.WriteLine();
        S_gameUX.Centered(@" ------------------------------------------------------------------ ");
        S_gameUX.Centered(@"|  Press key:  | 1:[Easy] | 2:[Moderate] | 3:[Hard] | 4:[Quit Game] |");
        S_gameUX.Centered(@"|-------------------------|--------------|----------|---------------|");
        S_gameUX.Centered(@"| Random words |     X    |       X      |     X    |       Why     |");
        S_gameUX.Centered(@"| Clues        |     X    |       X      |          |      would    |");
        S_gameUX.Centered(@"| Timer 25s    |          |       X      |          |       you     |");
        S_gameUX.Centered(@"| Timer 50s    |          |              |     X    |      Quit?    |");
        S_gameUX.Centered(@" ------------------------------------------------------------------- ");
 


        Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
        S_gameUX.HangmanLogo();

        var keyPressed = Console.ReadKey(true).KeyChar;

        switch (char.ToUpper(keyPressed))
        {
            case '1':
                IsGameOver = false;
                IsEasy = true;
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
                IsModerate = true;
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
                IsHard = true;
                S_countdown = true;
                IsModerate = false;
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
                S_gameUX.HangmanLogo();
                S_gameUX.Centered("Invalid input");
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