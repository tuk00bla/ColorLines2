using System.Collections.Generic;
using Cells2DList = System.Collections.Generic.List<System.Collections.Generic.List<BallsAndLines.Model.Cell>>;

namespace BallsAndLines.Model
{
    internal class FindPathAlgorithm
    {
        private Cell _startCell;
        private Cell _targetCell;

        private Cells2DList _boardCells;

        private IList<Cell> _lastFoundPath;

        public FindPathAlgorithm(Cell startCell, Cell targetCell, Cells2DList boardCells)
        {
            _startCell = startCell;
            _targetCell = targetCell;

            _boardCells = boardCells;
        }

        public bool CanFindPath()
        {
            _startCell.Mark = 0;

            MarkCellsAround(_startCell.Row, _startCell.Col, _startCell.Mark + 1);

            int cellsCanBeMarked = CountFreeCells();

            for (int i = 0; i < cellsCanBeMarked && _targetCell.Mark == -1; i++)
            {
                MarkNextWaveOfCells(maxMark: i + 1);
            }

            bool IsTargetCellMarked = _boardCells[_targetCell.Row][_targetCell.Col].Mark != -1;

            return IsTargetCellMarked;
        }

        private void MarkCellsAround(int row, int col, int mark)
        {
            if (col > 0 && _boardCells[row][col - 1].IsFreeForMarking())
            {
                _boardCells[row][col - 1].Mark = mark;
            }
            if (row > 0 && _boardCells[row - 1][col].IsFreeForMarking())
            {
                _boardCells[row - 1][col].Mark = mark;
            }
            if (col < Settings.BoardSize - 1 && _boardCells[row][col + 1].IsFreeForMarking())
            {
                _boardCells[row][col + 1].Mark = mark;
            }
            if (row < Settings.BoardSize - 1 && _boardCells[row + 1][col].IsFreeForMarking())
            {
                _boardCells[row + 1][col].Mark = mark;
            }
        }

        private int CountFreeCells()
        {
            int count = 0;

            for (int i = 0; i < Settings.BoardSize; i++)
            {
                for (int j = 0; j < Settings.BoardSize; j++)
                {
                    if (_boardCells[i][j].Ball == null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void MarkNextWaveOfCells(int maxMark)
        {
            for (int row = 0; row < Settings.BoardSize; row++)
            {
                for (int col = 0; col < Settings.BoardSize; col++)
                {
                    if (_boardCells[row][col].Mark == maxMark)
                    {
                        MarkCellsAround(row, col, maxMark + 1);
                    }
                }
            }
        }

        public IList<Cell> GetLastFoundPath()
        {
            _lastFoundPath = new List<Cell>();

            Cell curCellinPath = _targetCell;

            for (int i = _targetCell.Mark; i > 0; i--)
            {
                _lastFoundPath.Add(curCellinPath);

                curCellinPath = GetNextCell(curCellinPath, i);
            }

            return _lastFoundPath;
        }

        private Cell GetNextCell(Cell cell, int mark)
        {
            if (cell.Col > 0 && _boardCells[cell.Row][cell.Col - 1].Mark == mark - 1)
            {
                return _boardCells[cell.Row][cell.Col - 1];
            }
            else if (cell.Row > 0 && _boardCells[cell.Row - 1][cell.Col].Mark == mark - 1)
            {
                return _boardCells[cell.Row - 1][cell.Col];
            }
            else if (cell.Col < Settings.BoardSize - 1 && _boardCells[cell.Row][cell.Col + 1].Mark == mark - 1)
            {
                return _boardCells[cell.Row][cell.Col + 1];
            }
            else if (cell.Row < Settings.BoardSize - 1 && _boardCells[cell.Row + 1][cell.Col].Mark == mark - 1)
            {
                return _boardCells[cell.Row + 1][cell.Col];
            }
            else
            {
                throw new BallsAndLinesException("Next cell doesn't exist.");
            }
        }

        public void ClearAllMarks()
        {
            for (int row = 0; row < _boardCells.Count; row++)
            {
                for (int col = 0; col < _boardCells.Count; col++)
                {
                    _boardCells[row][col].Mark = -1;
                }
            }
        }
    }
}
