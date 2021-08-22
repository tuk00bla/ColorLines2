using System;
using System.Collections.Generic;

namespace BallsAndLines.Model
{
    public class ActionCellEventArgs : EventArgs
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Ball ActionBall { get; private set; }

        public IList<Cell> ActionCells { get; private set; }

        public ActionCellEventArgs()
        {
            ActionBall = null;
        }

        public ActionCellEventArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public ActionCellEventArgs(Ball ball)
        {
            ActionBall = ball;
        }

        public ActionCellEventArgs(int row, int col, Ball ball)
        {
            Row = row;
            Col = col;
            ActionBall = ball;
        }

        public ActionCellEventArgs(IList<Cell> cells)
        {
            ActionCells = cells;
        }
    }
}
