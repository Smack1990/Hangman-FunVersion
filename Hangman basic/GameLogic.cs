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
    public static int S_incorrectGuesses = 0;
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




        string alreadyGuessed = string.Empty;
        char guessedLetter;
        int width = Console.WindowWidth / 2 - alreadyGuessed.Length / 2;
        int height = Console.WindowHeight / 2;
        var cts = new CancellationTokenSource();




        if (S_isModerate)
        {
            gameUX.Secs = 60;
            _ = gameUX.StartTimer(cts.Token);
        }
        else if (S_isHard)
        {
            gameUX.Secs = 3;
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

            //Updatera askforletter pausa inmatningen om tiden tar slut. 


            Console.SetCursorPosition(0, S_windHeight); DisplayMaskedWord(); // Skriver ut ordet med maskade bokstäver
            gameUX.DisplayKeyboard();// skriver ut tagentbordslayouten enligt QWERTY
            PrintingHangman(S_incorrectGuesses);// hangmangubben skrivs ut vid fel bokstav
            if (!IsGameOver) // Check if the game is still ongoing
            {
                guessedLetter = AskForLetter(); // Ask the user to guess a letter

                if (S_player.GuessedLetters.Contains(guessedLetter)) // Check if the letter has already been guessed
                {
                    alreadyGuessed = $"You've already guessed: {guessedLetter}";
                }
                else
                {
                    alreadyGuessed = string.Empty; // Reset message
                    bool correct = CheckLetter(guessedLetter); // Check if the letter is in the word

                    if (correct)
                    {
                        S_wrongGuessesInRow = 0;
                    }
                    else
                    {
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
                            ShowWarning();
                        }
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
            //if (S_correctWord == new string(S_letters))
            //{

            //    cts.Cancel(); // Stop the timer when the game ends

            //    GameWon();

            //    break;
            //}





        } while (S_correctWord == new string(S_letters)); // loop körs så länge det finns _ kvar i ordet

        cts.Cancel(); // Stop the timer when the game ends
        if (S_correctWord == new string(S_letters))
        {

            GameWon();
        }
        // else
        // {
        //    RestartGame();
        // }


    }// Game logics


    private void ThanksForPlaying()
    {
        gameUX.Centered("Loading...");
        gameUX.Centered($"Thanks for playing {S_player.PlayerName}");
        GameWon();
    }
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


        Console.Clear();
        Console.SetCursorPosition(S_windwidth, S_windHeight + 10);
        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 3);
        Console.ForegroundColor = ConsoleColor.Green;
        gameUX.Centered($"Congratz {S_player.PlayerName}");
        gameUX.Centered($"Correct word: [{S_correctWord}] ");
        Console.ResetColor();
        EndGame();
        gameUX.Centered("Press any key to continue");
        //Console.ReadKey(true);
        RestartGame();
    } // Spelet vunnet

    public void Hanged()
    {

        //Console.Clear();
        gameUX.HangmanLogoTop();
        Console.ForegroundColor = ConsoleColor.Red; gameUX.Centered($"You've been hanged."); Console.ResetColor();
        gameUX.Centered($"The correct word was [{S_correctWord}]"); Task.Delay(4000).Wait();
        Console.Clear();
        Console.WriteLine("Press a kay to continue");

        gameUX.HangmanLogoTop();
        //Console.SetCursorPosition(S_windwidth, S_windHeight + 8);

        gameUX.HangmanLogo();
        Console.SetCursorPosition(S_windwidth, S_windHeight + 7);

        RestartGame();

    } // Spelet förlorat


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
        if (gameUX.Secs <= 0)
            {
                break;

            }
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
        return '\0';

    } // ask for input letter
    public static bool CheckLetter(char guessedLetter)
    {

        ;
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

        gameUX.UpdateKeyboard(guessedLetter);

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
        if (S_isModerate)
        {
            // For Moderate mode, starting time is 60 seconds
            elapsedTime = 60 - gameUX.Secs;
        }
        else if (S_isHard)
        {
            // For Hard mode, starting time is 30 seconds
            elapsedTime = 30 - gameUX.Secs;
        }

        // Display results
        gameUX.Centered($"Total amount of guesses: {S_player.GuessedLetters.Count}");
        gameUX.Centered($"You made: {S_incorrectGuesses} incorrect guesses");
       

        // Display elapsed time
        if (S_isModerate || S_isHard)
        {
            gameUX.Centered($"Elapsed time: {elapsedTime} seconds");
        }
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
            gameUX.HangmanLogo();
            Console.SetCursorPosition(S_windwidth, S_windHeight);
            Console.WriteLine(restartMessage);
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
                    gameUX.KeyboardCleanUp();
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

                if (S_isHard)
                {
                    gameUX.KeyboardCleanUp();
                    restart = true;
                    S_countdown = true;
                    S_isHard = true;  //Lägg till alla räknare som skall nollställas.
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
                restart = true;
                S_incorrectGuesses = 0;
                gameUX.KeyboardCleanUp();
                S_countDownLimit = 0; //Lägg till alla räknare som skall nollställas.
                S_wrongGuessesInRow = 0;
                S_left = 10 - S_incorrectGuesses;
                restart = false;
                LevelChoice();

                break;
            case '3':
                restart = true;
                S_incorrectGuesses = 0;
                gameUX.KeyboardCleanUp();
                S_countDownLimit = 0; //Lägg till alla räknare som skall nollställas.
                S_wrongGuessesInRow = 0;
                S_left = 10 - S_incorrectGuesses;
                S_windwidth = Console.WindowWidth / 2;
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

        Console.Clear();

        S_player.AskForUsersName();
        Console.SetCursorPosition(S_windwidth, S_windHeight - 10);
        gameUX.Centered($"Welcome {S_player.PlayerName}\n");
        gameUX.Centered("In this game you will try to guess the correct word,");
        gameUX.Centered("Depending on which mode you select you will be hanged if:");
        gameUX.Centered("You guess wrong 10 times.");
        gameUX.Centered("You guess wrong letter 5 consecutive times");
        gameUX.Centered("The time runs out");
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