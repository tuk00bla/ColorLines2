using BallsAndLines.Model;

namespace BallsAndLines.Controllers
{
    internal class ScoreController : PropertyModel
    {
        public ScoreController()
        {
            Score = new Score();
        }

        public ScoreController(Score score)
        {
            Score = score;
        }

        public Score Score { get; }

        public int CurrentScore
        {
            get => Score.CurrentScore;
            set
            {
                Score.CurrentScore = value;
                RaisePropertyChanged();

                MaxScore = value;
            }
        }

        public int MaxScore
        {
            get => Score.GetMaxScore();

            private set
            {
                bool IsNewRecordReached = Score.CheckOnMaxScore(value);

                if (IsNewRecordReached)
                {
                    Score.SaveOnlyNewRecord(GameController.GameStorePath + "score.xml");

                    RaisePropertyChanged();
                }
            }
        }

        public void OnUpdateGameScore(int addPoints)
        {
            CurrentScore += addPoints;
        }

        public bool SerializeScore(string fileName)
        {
            return Score.Save(GameController.GameStorePath + fileName);
        }
    }
}
