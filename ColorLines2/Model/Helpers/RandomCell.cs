using System;

namespace BallsAndLines.Model
{
    public static class RandomCell
    {
        private static readonly Random _random = new Random();

        public static Cell GetRand()
        {
            return new Cell(_random.Next(0, Settings.BoardSize), _random.Next(0, Settings.BoardSize));
        }
    }
}
