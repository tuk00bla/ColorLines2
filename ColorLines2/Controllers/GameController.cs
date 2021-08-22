using System;
using System.Collections.Generic;
using System.Threading;
using BallsAndLines.Model;

namespace BallsAndLines.Controllers
{
    internal static class GameController
    {
        public static readonly string GameStorePath = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly int LinePathAnimDelay;
        public static readonly int CollectedLineAnimDelay;
        public static readonly int CreatingRandomBallsDelay;

        private static Cell _selectedBall;

        private static Board _board;

        private static IList<Cell> _collectedBallsLines;
        private static List<Cell> _randBallsList;

        static GameController()
        {
            LinePathAnimDelay = 420;
            CollectedLineAnimDelay = 800;
            CreatingRandomBallsDelay = 450;
        }

        public static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static IList<Cell> CollectedBallsLines => _collectedBallsLines;

        public static event EventHandler<ActionCellEventArgs> CellSelected;
        public static event EventHandler<ActionCellEventArgs> CellUnselected;

        public static event Action<int> ScoreUpdated;
        public static event Action GameOver;

        public static Cell SelectedBall
        {
            get { return _selectedBall; }
            set
            {
                if (_selectedBall != null)
                    CellUnselected?.Invoke(typeof(GameController), new ActionCellEventArgs(_selectedBall.Row, _selectedBall.Col));

                _selectedBall = value;

                if (_selectedBall != null)
                    CellSelected?.Invoke(typeof(GameController), new ActionCellEventArgs(_selectedBall.Row, _selectedBall.Col));

            }
        }
        public static List<Cell> CreateOnBoardNewRandomBalls()
        {
            GenerateRandomBalls(Settings.DropBallsPerStep);

            AddRandomBallsToBoard();

            return _randBallsList;
        }

        internal static List<Cell> GenerateRandomBalls(int countBalls)
        {
            _randBallsList = new List<Cell>(countBalls);

            Cell randEmptyCell;
            Ball randBall;

            while (_randBallsList.Count < countBalls)
            {
                randEmptyCell = RandomCell.GetRand();
                randBall = RandomBall.GetRand();

                if (CanAddBall(randEmptyCell.Row, randEmptyCell.Col, _randBallsList))
                {
                    randEmptyCell.Ball = randBall;

                    _randBallsList.Add(randEmptyCell);
                }
            }

            return _randBallsList;
        }
        public static IList<Cell> GetExistingBallPath(int row, int col)
        {
            return _board.GetBallPath();
        }

        public static Board CreateNewGameBoard(int size, Board serializedBoard = null)
        {
            ChangeBoardSize(size);

            ClearEventHandlers();

            _board = serializedBoard ?? new Board();

            return _board;
        }

        private static void ClearEventHandlers()
        {
            if (CellSelected != null && CellUnselected != null && ScoreUpdated != null)
            {
                foreach (EventHandler<ActionCellEventArgs> handler in CellSelected.GetInvocationList())
                {
                    CellSelected -= handler;
                }

                foreach (EventHandler<ActionCellEventArgs> handler in CellUnselected.GetInvocationList())
                {
                    CellUnselected -= handler;
                }

                foreach (Action<int> handler in ScoreUpdated?.GetInvocationList())
                {
                    ScoreUpdated -= handler;
                }

                foreach (Action handler in GameOver.GetInvocationList())
                {
                    GameOver -= handler;
                }
            }
        }

       

        public static void AddRandomBallsToBoard()
        {
            _board.AddBallToCell(_randBallsList.ToArray());
        }

        private static bool CanAddBall(int row, int col, List<Cell> cellsList)
        {
            return _board[row, col].Ball == null &&                       // check cell is free
                   !(cellsList.Exists(c => c.Row == row && c.Col == col));// avoid ball duplication in cellsList                   
        }

        public static bool IsBallExist(int row, int col)
        {
            return _board[row][col].Ball != null;
        }

        public static void UpdateSelectedBall(int row, int col)
        {
            SelectedBall = _board[row, col];
        }

        public static bool CanReplaceSelectedBallToCell(int toRow, int toCol)
        {
            return _board.IsPathExist(_selectedBall, _board[toRow, toCol]);
        }

        public static void RelocateSelectedBall(int row, int col)
        {
            Cell startCell = _selectedBall;
            Cell targetCell = _board[row][col];

            _board.AddBallToCell(new Cell(targetCell.Row, targetCell.Col, startCell.Ball));

            _board.RemoveBallFromCell(startCell);

            SelectedBall = null;

            _board.ClearObsoletePathData();
        }

        public static bool IsCollectedLineExist(int lastAddBallRow, int lastAddBallCol)
        {
            _collectedBallsLines = _board.GetCollectedBallsInLines(lastAddBallRow, lastAddBallCol);

            if (_collectedBallsLines.Count < Settings.NumBallsInLine)
            {
                return false;
            }

            CalcAndUpdateScore();

            return true;
        }

        private static void CalcAndUpdateScore()
        {
            int addPoints = _collectedBallsLines.Count > Settings.NumBallsInLine
                                                        ? _collectedBallsLines.Count * 2
                                                        : _collectedBallsLines.Count;

            ScoreUpdated?.Invoke(addPoints);
        }

        public static void RemoveCollectedLines()
        {
            _board.RemoveCollectedLines(_collectedBallsLines);
        }

        public static bool IsGameOver()
        {
            if (_board.IsGameOver())
            {
                GameOver();

                return true;
            }
            return false;
        }

        public static bool ChangeBoardSize(int newSize)
        {
            Settings.BoardSize = newSize < Board.MinSize ? Board.MinSize :
                                 newSize > Board.MaxSize ? Board.MaxSize :
                                 newSize;

            Properties.Settings.Default.BoardSize = newSize.ToString();
            Properties.Settings.Default.Save();

            return true;
        }

        public static bool ChangeDroppingBallsPerStep(int newCount)
        {
            if (newCount > 0 && newCount <= Settings.BoardSize)
            {
                Settings.DropBallsPerStep = newCount;

                Properties.Settings.Default.DropBallsPerStep = newCount.ToString();
                Properties.Settings.Default.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ChangeCountBallsForLineCollected(int newCount)
        {
            if (newCount > 1 && newCount <= Settings.BoardSize)
            {
                Settings.NumBallsInLine = newCount;

                Properties.Settings.Default.NumBallsInLine = newCount.ToString();
                Properties.Settings.Default.Save();

                return true;
            }
            else
                return false;
        }
    }
}