namespace BallsAndLines.Model
{
    internal static class Settings
    {
        static Settings()
        {
            BoardSize = int.Parse(Properties.Settings.Default.BoardSize);
            DropBallsPerStep = int.Parse(Properties.Settings.Default.DropBallsPerStep);
            NumBallsInLine = int.Parse(Properties.Settings.Default.NumBallsInLine);
        }

        public static int BoardSize { get; internal set; }

        public static int DropBallsPerStep { get; internal set; }

        public static int NumBallsInLine { get; internal set; }
    }
}
