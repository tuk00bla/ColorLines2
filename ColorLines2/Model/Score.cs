using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using BallsAndLines.Controllers;

namespace BallsAndLines.Model
{
    [DataContract]
    internal class Score
    {
        private int _currentScore;

        private int _tempSavedCurScore;
        private bool _tempNewRecordReached;

        private Dictionary<int, int> _bestScoreList;

        public Score()
        {
            InitBestScoreList();
        }

        public Score(int currentScore, Dictionary<int, int> bestScoreList)
        {
            _currentScore = currentScore;
            _bestScoreList = bestScoreList;
        }

        private void InitBestScoreList()
        {
            _bestScoreList = new Dictionary<int, int>();

            for (int i = Board.MinSize; i < Board.MaxSize; i++)
            {
                _bestScoreList[i] = 0;
            }
        }

        public int GetMaxScore()
        {
            return _bestScoreList[Settings.BoardSize];
        }

        public bool CheckOnMaxScore(int newScore)
        {
            if (_bestScoreList[Settings.BoardSize] < newScore)
            {
                _bestScoreList[Settings.BoardSize] = newScore;

                return true;
            }
            else
            {
                return false;
            }
        }

        public int this[int sizeBoard]
        {
            get => _bestScoreList[sizeBoard];
            set => _bestScoreList[sizeBoard] = value;
        }


        public static Score Open(string filePath)
        {
            Score score = null;

            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                return null;
            }

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(file, new XmlDictionaryReaderQuotas()))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Score));
                        var objScore = serializer.ReadObject(reader, true);
                        score = (Score)objScore;
                    }
                }
            }
            catch
            {
                throw new BallsAndLinesException("Error reading file");
            }

            return score;
        }


        public bool Save(string filePath)
        {
            try
            {
                using (FileStream writer = new FileStream(filePath, FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Score));
                    serializer.WriteObject(writer, this);
                }
            }
            catch
            {
                throw new BallsAndLinesException("Error writing file");
            }

            return true;
        }

        public bool SaveOnlyNewRecord(string filePath)
        {
            _tempNewRecordReached = true;

            _tempSavedCurScore = _currentScore;

            Score savedScore = Open(GameController.GameStorePath + "score.xml");

            if (savedScore != null)
            {
                _currentScore = savedScore.CurrentScore;
            }

            Save(filePath);

            return true;
        }

        [DataMember]
        public int CurrentScore
        {
            get => _currentScore;
            set
            {
                if (value >= 0)
                {
                    _currentScore = value;
                }
            }
        }

        [DataMember]
        public Dictionary<int, int> BestScoreList
        {
            get => _bestScoreList;
            set => _bestScoreList = value;
        }

        [OnSerialized]
        private void SetScoreOnSerialized(StreamingContext context)
        {
            if (_tempNewRecordReached)
            {
                _tempNewRecordReached = false;

                _currentScore = _tempSavedCurScore;
            }
        }
    }
}
