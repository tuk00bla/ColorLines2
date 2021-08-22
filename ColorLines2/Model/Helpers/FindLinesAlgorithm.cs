using System.Collections.Generic;
using System.Linq;
using Cells2DList = System.Collections.Generic.List<System.Collections.Generic.List<BallsAndLines.Model.Cell>>;

namespace BallsAndLines.Model
{
    internal struct FindLinesAlgorithm
    {
        private readonly int _centralRow;
        private readonly int _centralCol;

        private Cells2DList _boardCells;

        public FindLinesAlgorithm(int findFromRow, int findFromCol, Cells2DList originalBoardCells) : this()
        {
            _centralRow = findFromRow;
            _centralCol = findFromCol;

            _boardCells = originalBoardCells;
        }

        public Cells2DList GetPossibleLines()
        {
            Cells2DList cellsOfPossibleLines = new Cells2DList();

            int countLines = 4;
            for (int i = 0; i < countLines; i++)
            {
                cellsOfPossibleLines.Add(new List<Cell>());
            }

            #region Find lines in four ways (vertical, horizontal, left/rigth diagonals)

            //Set corners for add all vertical and horizontal cells in witch can exists line 
            int topRow = _centralRow + 1 - Settings.NumBallsInLine <= 0 ? 0 : _centralRow + 1 - Settings.NumBallsInLine;
            int bottomRow = _centralRow + Settings.NumBallsInLine >= Settings.BoardSize ? Settings.BoardSize : _centralRow + Settings.NumBallsInLine;

            int leftCol = _centralCol + 1 - Settings.NumBallsInLine <= 0 ? 0 : _centralCol + 1 - Settings.NumBallsInLine;
            int rightCol = _centralCol + Settings.NumBallsInLine >= Settings.BoardSize ? Settings.BoardSize : _centralCol + Settings.NumBallsInLine;

            //---Add-horizontal-cells  
            int indxLine = 0;
            for (int c = leftCol; c < rightCol; c++)
            {
                cellsOfPossibleLines[indxLine].Add(_boardCells[_centralRow][c]);
            }

            //---Add-vertical-cells 
            indxLine++;
            for (int r = topRow; r < bottomRow; r++)
            {
                cellsOfPossibleLines[indxLine].Add(_boardCells[r][_centralCol]);
            }

            //Set corners for add LEFT diagonal cells in target square 
            topRow = bottomRow = _centralRow;
            leftCol = rightCol = _centralCol;

            while (topRow > 0 && leftCol > 0)
            {
                topRow--; leftCol--;
            }
            while (bottomRow < Settings.BoardSize && rightCol < Settings.BoardSize)
            {
                bottomRow++; rightCol++;
            }

            //---Add-left-diagonal-cells 
            indxLine++;
            for (int r = topRow, c = leftCol; r < bottomRow && c < rightCol; r++, c++)
            {
                cellsOfPossibleLines[indxLine].Add(_boardCells[r][c]);
            }

            //Set corners for find RIGHT diagonal cells in target square 
            topRow = _centralRow;
            bottomRow = _centralRow + 1;
            leftCol = rightCol = _centralCol;

            while (topRow > 0 && rightCol < Settings.BoardSize - 1)
            {
                topRow--; rightCol++;
            }
            while (bottomRow < Settings.BoardSize && leftCol > 0)
            {
                bottomRow++; leftCol--;
            }

            //---Add-right-diagonal-cells 
            indxLine++;
            for (int r = topRow, c = rightCol; r < bottomRow && c >= leftCol; r++, c--)
            {
                cellsOfPossibleLines[indxLine].Add(_boardCells[r][c]);
            }
            ///////////////////////////

            #endregion

            return cellsOfPossibleLines;
        }

        public IList<Cell> FindCollectedLines(Cells2DList cellsList)
        {
            Color findColor = GetLastAddBallColor();

            IList <Cell> collectedLines = new List<Cell>();

            List<Cell> tempLine;
            for (int i = 0; i < cellsList.Count; i++)
            {
                do
                {
                    tempLine = cellsList[i].TakeWhile(cell => cell.Ball != null && cell.Ball.Color == findColor).ToList();

                    if (tempLine.Count > 0)
                    {
                        if (tempLine.Count >= Settings.NumBallsInLine)
                        {
                            foreach (Cell cell in tempLine)
                                collectedLines.Add(cell);
                        }

                        cellsList[i] = cellsList[i].SkipWhile(c => c.Ball != null && c.Ball.Color == findColor).ToList();
                    }
                    else cellsList[i] = cellsList[i].SkipWhile(c => c.Ball == null || c.Ball.Color != findColor).ToList();

                } while (cellsList[i].Exists(c => c.Ball != null && c.Ball.Color == findColor));
            }

            return collectedLines;
        }

        public Color GetLastAddBallColor()
        {
            return _boardCells[_centralRow][_centralCol].Ball.Color;
        }
    }
}
