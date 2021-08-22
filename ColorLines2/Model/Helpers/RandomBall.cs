using System;

namespace BallsAndLines.Model
{
    public static class RandomBall
    {
        private static readonly Random random = new Random();

        public static Ball GetRand()
        {
            return new Ball() { Color = (Color)random.Next(0, Enum.GetValues(typeof(Color)).Length) };
        }
    }
}
