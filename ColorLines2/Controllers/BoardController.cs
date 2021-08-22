using System.Collections.Generic;
using System.Collections.ObjectModel;
using BallsAndLines.Model;

namespace BallsAndLines.Controllers
{
    internal class BoardController : PropertyModel
    {
        private Board _gameBoard;

        private ObservableCollection<ObservableCollection<CellController>> _cells;

        private IList<Cell> _cellsInPath;
        private IList<Cell> _cellsInLine;

        public BoardController(Board board)
        {
            _gameBoard = board;

            InitBoardCells();

            InitBoardAndCellsEvents();
        }

        public Board Board => _gameBoard;

        public ObservableCollection<ObservableCollection<CellController>> Cells => _cells;

        private void InitBoardCells()
        {
            if (_cells == null)
            {
                _cells = new ObservableCollection<ObservableCollection<CellController>>();
            }

            for (int row = 0; row < Settings.BoardSize; row++)
            {
                _cells.Add(new ObservableCollection<CellController>());

                for (int col = 0; col < Settings.BoardSize; col++)
                {
                    _cells[row].Add(new CellController(_gameBoard[row][col]));
                }
            }
        }

        private void InitBoardAndCellsEvents()
        {
            _gameBoard.CellChanged += OnChangeCell;

            for (int row = 0; row < Settings.BoardSize; row++)
            {
                for (int col = 0; col < Settings.BoardSize; col++)
                {
                    _cells[row][col].CellMakePath += OnCellMakePath;
                    _cells[row][col].ClearExistingPath += OnClearCellPath;

                    _cells[row][col].CellExistInLine += OnCellExistInLine;
                    _cells[row][col].ClearExistingLine += OnClearCellLine;
                }
            }
        }

        public void DropRandomBalls()
        {
            GameController.CreateOnBoardNewRandomBalls();
        }

        private void OnCellExistInLine(object sender, ActionCellEventArgs cells)
        {
            _cellsInLine = cells.ActionCells;

            foreach (Cell cell in _cellsInLine)
            {
                Cells[cell.Row][cell.Col].IsCellExistInCollectedLine = true;
            }
        }
        private void OnClearCellLine()
        {
            foreach (Cell cell in _cellsInLine)
            {
                Cells[cell.Row][cell.Col].IsCellExistInCollectedLine = false;
            }

            _cellsInLine = null;
        }

        private void OnCellMakePath(object sender, ActionCellEventArgs cells)
        {
            _cellsInPath = cells.ActionCells;

            foreach (Cell cell in _cellsInPath)
            {
                Cells[cell.Row][cell.Col].IsCellExistInPath = true;
            }
        }

        private void OnClearCellPath()
        {
            foreach (Cell cell in _cellsInPath)
            {
                Cells[cell.Row][cell.Col].IsCellExistInPath = false;
            }

            _cellsInPath = null;
        }

        private void OnChangeCell(object sender, ActionCellEventArgs cell)
        {
            int row = cell.Row;
            int col = cell.Col;
            Ball changedBallState = cell.ActionBall;

            Cells[row][col].Ball = changedBallState;
        }

        public void OnCellSelected(object sender, ActionCellEventArgs selectedCell)
        {
            Cells[selectedCell.Row][selectedCell.Col].IsBallSelected = true;
        }

        public void OnCellUnselected(object sender, ActionCellEventArgs selectedCell)
        {
            Cells[selectedCell.Row][selectedCell.Col].IsBallSelected = false;
        }

        public bool SerializeGameBoard(string fileName)
        {
            return _gameBoard.Save(GameController.GameStorePath + fileName);
        }
    }
}
