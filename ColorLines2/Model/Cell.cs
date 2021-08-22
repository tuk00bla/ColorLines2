using System.Runtime.Serialization;

namespace BallsAndLines.Model
{
    [DataContract]
    public class Cell
    {
        public Cell()
        {
            Mark = -1;
        }

        public Cell(int row, int col) : this()
        {
            Row = row;
            Col = col;
        }

        public Cell(int row, int col, Ball ball) : this(row, col)
        {
            Ball = ball;
        }

        [DataMember]
        public int Row { get; internal set; } 
        [DataMember]
        public int Col { get; internal set; }
        [DataMember]
        public Ball Ball { get; internal set; }

        public int Mark { get; internal set; } 

        internal bool IsFreeForMarking()
        {
            return Ball == null && Mark == -1;
        }

        public Cell Clone()
        {
            return new Cell(Row, Col, Ball?.Clone());
        }
    }
}