using System;

namespace TwentyFortyEight
{
    /// <summary>
    /// Play 2048
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Specifies possible moves in the game
        /// </summary>
        public enum Move { Up, Left, Down, Right, Restart, Quit };

        /// <summary>
        /// Generates random numbers
        /// </summary>
        static Random numberGenerator = new Random();

        /// <summary>
        /// Number of initial digits on a new 2048 board
        /// </summary>
        const int NUM_STARTING_DIGITS = 2;

        /// <summary>
        /// The width of a cell when drawn to the screen
        /// </summary>
        const int CELL_WIDTH = 4;

        /// <summary>
        /// The chance of a two spawning
        /// </summary>
        const double CHANCE_OF_TWO = 0.9; // 90% chance of a two; 10% chance of a four

        /// <summary>
        /// The size of the 2048 board
        /// </summary>
        const int BOARD_SIZE = 4; // 4x4


        /// <summary>
        /// Runs the game of 2048
        /// </summary>
        static void Main()
        {   
            // A integer varible to count the number of moves
            int move = 0;
            // Create the initial board
            int[,] board = MakeBoard();

            // Run the loop as long as the game can continue
            while (!GameOver(board))
            {
                // Print title
                Console.WriteLine("\n   === 2048 ===   \n");
                // Display the current board, score and the number of moves
                DisplayBoard(board);
                DisplayScore(board); 
                Console.WriteLine($"Move: {move,4}  |");
                // Let user inputs his/her choice
                Move option = ChooseMove(GameOver(board));

                // Refresh the board if user chooses Restart
                if (option == Move.Restart) 
                { 
                    board = MakeBoard();
                    move = 0;
                }
                // End loop if user chooses Quit
                else if (option == Move.Quit)  break;
                // Else, Make move
                else 
                {
                    // And create a new cell as well
                    if (MakeMove(option, board))  PopulateAnEmptyCell(board); 
                    // After MakeMove, the total number of moves increases by 1
                    move++;
                }
                // If gameover,
                if (GameOver(board)) {
                    // First of all, display the final board 
                    Console.Clear();
                    Console.WriteLine("\n  === 2048 ===  \n");
                    DisplayBoard(board);
                    DisplayScore(board); 
                    Console.WriteLine($"Move: {move,4}  |");

                    // User can now only choose Restart or Quit
                    option = ChooseMove(GameOver(board));
                    if (option == Move.Restart) 
                    {
                        // Refresh board
                        board = MakeBoard(); 
                        move = 0;
                    }
                    else if (option == Move.Quit) 
                        break; 
                }
                Console.Clear();
            }
            // A warm goodbye
            Console.WriteLine("\n\nSEE YA...");
            
            // TestRunner.RunTests(); 
            
            // Enter to close the terminal
            Console.ReadLine();
        }

        /// <summary>
        /// Display the given 2048 board in the terminal nicely
        /// </summary>
        /// <param name="board">The 2048 board to display</param>
        public static void DisplayBoard(int[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == 0)
                        Console.Write($"{"-",CELL_WIDTH}");
                    else
                        Console.Write($"{board[i, j],CELL_WIDTH}");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Display controls and instrutions, and gets a Move from the user
        /// such as UP, LEFT, DOWN, RIGHT, RESTART or QUIT
        /// </summary>
        /// <returns>The chosen Move</returns>
        public static Move ChooseMove(bool gameOver = false)
        {
            Move option = Move.Up;

            // If the game can continue, display all possible moves
            if (!gameOver) 
            {
                Console.WriteLine("WASD: Move   |  R: Restart  |  O: Quit \n");
                char inputKey = Console.ReadKey().KeyChar;

                // Ask user to input move until it is valid 
                string validInput = "wasdro";
                while (validInput.IndexOf(inputKey) == -1)
                    inputKey = Console.ReadKey().KeyChar;
                
                // Then assign the value to "option" accordingly
                switch (inputKey)
                {
                    case 'w':
                        option = Move.Up;       break;
                    case 's':
                        option = Move.Down;     break;
                    case 'a':
                        option = Move.Left;     break;
                    case 'd':
                        option = Move.Right;    break;
                    case 'r':
                        option = Move.Restart;  break;
                    case 'o':
                        option = Move.Quit;     break;
                }
            }
            // If gameover, notify user gameover, and user can now only Restart or Quit
            else 
            {                        
                Console.WriteLine("R: Restart   |  O: Quit     |  GAMEOVER !!!  \n");
                // Ask user to input move until it is valid 
                string validInput = "ro";
                char inputKey = Console.ReadKey().KeyChar;
                while (validInput.IndexOf(inputKey) == -1)
                    inputKey = Console.ReadKey().KeyChar;
                // Then assign the value to "option" accordingly
                switch (inputKey)
                {
                    case 'r':
                        option = Move.Restart;    break;
                    case 'o':
                        option = Move.Quit;       break;
                }
            }
            return option;    
        }

        /// <summary>
        /// Returns true if the given board is in a 'game over' state, meaning
        /// that no moves can be made that affect the board.
        /// 
        /// No moves are possible when there are no zeros left on the board,
        /// AND there are no adjacent cells containing the same number.
        /// </summary>
        /// <param name="board">A 2048 board to check</param>
        /// <returns>True if no moves can be made on the board</returns>
        public static bool GameOver(int[,] board)
        {   
            bool gameOver = false;
            bool cannotCombineRow = true;
            bool cannotCombineCol = true;
            int[] temp;
            
            // Check if whether it can move horizontally or not
            for (int i = 0; i < board.GetLength(1); i++) 
            {
                temp = MatrixExtensions.GetRow(board,i);
                if (CombineLeft(temp))   cannotCombineRow = false;
            }
            // Check if whether it can move vertically or not
            for (int i = 0; i < board.GetLength(0); i++) 
            {
                temp = MatrixExtensions.GetCol(board,i);
                if (CombineLeft(temp))   cannotCombineCol = false;
            }
            // Gameover if it is full and not able to neither move vertically nor horizontally
            gameOver = IsFull(board) & cannotCombineRow & cannotCombineCol;
            return gameOver;
        }

        /// <summary>
        /// Returns true if the given 2048 board is full (contains no zeros)
        /// </summary>
        /// <param name="board">A 2048 board to check</param>
        /// <returns>True if the board is full; false otherwise</returns>
        public static bool IsFull(int[,] board)
        {
            bool full = true;
            // The board is considered full when there's no Zero and vice versa
            for (int i = 0; i < board.GetLength(0) && full == true; i++) 
                for (int j = 0; j < board.GetLength(1) && full == true; j++) 
                    if (board[i, j] == 0)   full = false;
            return full;
        }

        /// <summary>
        /// If the board is not full, choose a random empty cell and add a two or a four.
        /// There should be a 90% chance of adding a two, and a 10% chance of adding a four.
        /// </summary>
        /// <param name="board">The board to add a new number to</param>
        /// <returns>False if the board is already full; true otherwise</returns>
        public static bool PopulateAnEmptyCell(int[,] board)
        {
            // The variable is created right below to track if change is made by this method
            bool changed = false;
            int numberToFill;
            int randomPosition;
            
            // Generate the number to fill in the board
            double randomNumber = numberGenerator.NextDouble();
            if (randomNumber <= CHANCE_OF_TWO) { numberToFill = 2; }
            else                               { numberToFill = 4; }
            // Pick a random empty slot to fill
            while (changed == false && !IsFull(board)) 
            {
                for (int i = 0; i < board.GetLength(0) && !changed; i++) 
                {
                    for (int j = 0; j < board.GetLength(1) && !changed; j++) 
                    {
                        if (board[i, j] == 0) 
                        { 
                            // The chance of filling is 1 in 16 (the number of boxes in the board)
                            randomPosition = numberGenerator.Next(1, BOARD_SIZE * BOARD_SIZE + 1);
                            changed = randomPosition == 1;
                            if (changed)  board[i, j] = numberToFill;
                        }
                    }
                }
            }           
            return changed;
        }

        /// <summary>
        /// Creates a new 2048 board as a 2D int array, with a size of 4x4,
        /// populated with two initial cell values (using PopulateAnEmptyCell)
        /// </summary>
        /// <returns>A new 2048 board</returns>
        public static int[,] MakeBoard()
        {
            int[,] board = new int[,] {
                {    0,    0,    0,    0    },
                {    0,    0,    0,    0    },
                {    0,    0,    0,    0    },
                {    0,    0,    0,    0    }
            };
            PopulateAnEmptyCell(board);
            PopulateAnEmptyCell(board);
            return board;
        }

        /// <summary>
        /// Applies the chosen Move on the given 2048 board
        /// </summary>
        /// <param name="move">A move such as UP, LEFT, RIGHT or DOWN</param>
        /// <param name="board">A 2048 board</param>
        /// <returns>True if the move had an effect on the game; false otherwise</returns>
        public static bool MakeMove(Move move, int[,] board)
        {
            // This variable is created right below to track if change is made by this method
            bool changed = false;
            int[] colVector;
            int[] rowVector;

            switch (move)
            {
                // Move everything upwards (and combine as well)
                case Move.Up:
                    for (int col = 0; col < board.GetLength(0); col++) 
                    {
                        colVector = MatrixExtensions.GetCol(board, col);
                        if (ShiftCombineShift(colVector, true))  changed = true; 
                        MatrixExtensions.SetCol(board, col, colVector);
                    }
                    break;
                // Move everything downwards (and combine)    
                case Move.Down:
                    for (int col = 0; col < board.GetLength(0); col++) 
                    {
                        colVector = MatrixExtensions.GetCol(board, col);
                        if (ShiftCombineShift(colVector, false))  changed = true;
                        MatrixExtensions.SetCol(board, col, colVector);
                    }
                    break;
                // Move everything towards the left (and combine)
                case Move.Left:
                    for (int row = 0; row < board.GetLength(1); row++) 
                    {
                        rowVector = MatrixExtensions.GetRow(board, row);
                        if (ShiftCombineShift(rowVector, true))  changed = true;
                        MatrixExtensions.SetRow(board, row, rowVector);
                    }
                    break;
                // Move everything towards the right (and combine)
                case Move.Right:
                    for (int row = 0; row < board.GetLength(1); row++) 
                    {
                        rowVector = MatrixExtensions.GetRow(board, row);
                        if (ShiftCombineShift(rowVector, false))  changed = true; 
                        MatrixExtensions.SetRow(board, row, rowVector);
                    }
                    break;
            }
            return changed;
        }

        /// <summary>
        /// Shifts the non-zero integers in the given 1D array to the left
        /// </summary>
        /// <param name="nums">A 1D array of integers</param>
        /// <returns>True if shifting had an effect; false otherwise</returns>
        /// <example>
        ///   If nums has the values:
        ///       { 0, 2, 2, 4, 4, 0, 0, 8, 8, 5, 3 }
        ///   It will be modified to:
        ///       { 2, 2, 4, 4, 8, 8, 5, 3, 0, 0, 0 }
        /// </example>
        public static bool ShiftLeft(int[] nums)
        {
            // This variable is created right below to track if change is made by this method
            bool changed = false;
            int trackPosition = 0;
            int temp;
            for (int i = 0; i < nums.Length; i++)
            {
                // Swap the very left zero (if have any) by a number (if have any) that is first seen on its right     
                if (nums[i] != 0) 
                {
                    temp = nums[trackPosition];
                    nums[trackPosition] = nums[i];
                    nums[i] = temp;

                    if (temp != nums[trackPosition])  changed = true;

                    trackPosition++;   
                }
            }
            return changed;
        }

        /// <summary>
        /// Combines identical, non-zero integers that are adjacent to one another by summing 
        /// them in the left integer, and replacing the right-most integer with a zero
        /// </summary>
        /// <param name="nums">A 1D array of integers</param>
        /// <returns>True if combining had an effect; false otherwise</returns>
        /// <example>
        ///   If nums has the values:
        ///       { 0, 2, 2, 4, 4, 0, 0, 8,  8, 5, 3 }
        ///   It will be modified to:
        ///       { 0, 4, 0, 8, 0, 0, 0, 16, 0, 5, 3 }
        /// </example>
        public static bool CombineLeft(int[] nums)
        {
            // The variable is below created to track if change is made by this method
            bool changed = false;

            for (int i = 0; i < nums.Length - 1; i++) 
            {
                if (nums[i] == nums[i+1] && nums[i] != 0)
                {
                    nums[i] *= 2;
                    i++;
                    nums[i] = 0;
                    changed = true;
                }
            }
            return changed;
        }

        /// <summary>
        /// Shifts the numbers in the array in the specified direction, then combines them, then 
        /// shifts them again.
        /// </summary>
        /// <param name="nums">A 1D array of integers</param>
        /// <param name="left">True if numbers should be shifted to the left; false otherwise</param>
        /// <returns>True if shifting and combining had an effect; false otherwise</returns>
        /// <example>
        ///   If nums has the values below, and shiftLeft is true:
        ///       { 0, 2,  2, 4, 4, 0, 0, 8, 8, 5, 3 }
        ///   It will be modified to:
        ///       { 4, 8, 16, 5, 3, 0, 0, 0, 0, 0, 0 }
        ///       
        ///   If nums has the values below, and shiftLeft is false:
        ///       { 0, 2, 2, 4, 4, 0, 0, 8,  8, 5, 3 }
        ///   It will be modified to:
        ///       { 0, 0, 0, 0, 0, 0, 4, 8, 16, 5, 3 }
        /// </example>
        public static bool ShiftCombineShift(int[] nums, bool shiftLeft)
        {
            // This variable below is created to track if change is made by this method
            bool changed = false;

            // Reverse the array if the numbers are shifted to the right
            if (shiftLeft == false)  Array.Reverse(nums); 

            changed = ShiftLeft(nums) | CombineLeft(nums) | ShiftLeft(nums);

            // And, reverse the array again 
            if (shiftLeft == false)  Array.Reverse(nums); 
            return changed;
        }

        /// <summary>
        /// Print out the total score (sum of all the numbers on the board)
        /// </summary>
        /// <param name="board">A 2048 board</param>
        public static void DisplayScore(int[,] board)
        {
            int score = 0;

            // iterate through each box to add its value to the total score
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    score += board[i, j];

            Console.Write($"\nScore: {score,-6}|  ");
        }
    }
}
