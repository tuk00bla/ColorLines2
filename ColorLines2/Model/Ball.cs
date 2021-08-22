 using System.Runtime.Serialization;

namespace BallsAndLines.Model
{
    [DataContract]
    public class Ball
    {
        public Ball() { }
        public Ball(Color color) => this.Color = color;

        [DataMember]
        public Color Color { get; internal set; }

        public Ball Clone() => new Ball() { Color = this.Color };

        public override string ToString() => Color.ToString();

    }
}