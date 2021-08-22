using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BallsAndLines.Helpers;
using BallsAndLines.Model;

namespace BallsAndLines.Controllers
{
    internal class CellController : PropertyModel
    {
        private readonly Cell _modelCell;

        private RelayCommand _cellAction;

        private bool _isBallSelected;

        private bool _isCellInPath;

        private bool _isCellInCollectedLine;

        public CellController() { }

        public CellController(Cell modelCell)
        {
            _modelCell = modelCell;
        }

        public int Row
        {
            get => _modelCell.Row;
            internal set
            {
                _modelCell.Row = value;
                RaisePropertyChanged();
            }
        }

        public int Col
        {
            get => _modelCell.Col;
            internal set
            {
                _modelCell.Col = value;
                RaisePropertyChanged();
            }
        }

        public Ball Ball
        {
            get => _modelCell.Ball;
            internal set
            {
                _modelCell.Ball = value;

                RaisePropertyChanged();
            }
        }

        public bool IsBallSelected
        {
            get => _isBallSelected;
            set
            {
                _isBallSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCellExistInPath
        {
            get => _isCellInPath;
            set
            {
                _isCellInPath = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCellExistInCollectedLine
        {
            get => _isCellInCollectedLine;
            set
            {
                _isCellInCollectedLine = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler<ActionCellEventArgs> CellMakePath;
        public event Action ClearExistingPath;

        public event EventHandler<ActionCellEventArgs> CellExistInLine;
        public event Action ClearExistingLine;

        public ICommand CellAction => _cellAction ?? (_cellAction = new RelayCommand(CellClick));

        private async void CellClick(object o)
        {
            await GameController.semaphoreSlim.WaitAsync();
            try
            {
                await ActionsWithCellAsync();
            }
            finally
            {
                _ = GameController.semaphoreSlim.Release();
            }
        }

        private async Task ActionsWithCellAsync()
        {
            if (GameController.IsBallExist(this.Row, this.Col))
            {
                GameController.UpdateSelectedBall(this.Row, this.Col);
            }
            else if (GameController.SelectedBall != null)
            {
                if (CanReplaceBall())
                {
                    await ReplaceSelectedBallAsync();

                    if (IsBallMakeNewLines())
                    {
                        await RemoveCollectedLinesAsync();
                    }
                    else if (!GameController.IsGameOver())
                    {
                        await Task.Run(() => GenNextBallsAndCheckLinesAsync());
                    }
                }
                else
                {
                    MessageBox.Show("For " + GameController.SelectedBall.Ball.Color.ToString() + " ball path doesn't exist.", "");
                }
            }
        }

        private bool CanReplaceBall()
        {
            return GameController.CanReplaceSelectedBallToCell(this.Row, this.Col);
        }

        private async Task ReplaceSelectedBallAsync()
        {
            IList<Cell> ballPath = GameController.GetExistingBallPath(Row, Col);

            OnPathAnimation(ballPath);

            await Task.Run(() => StopAnimAndRelocateBall());
        }

        private bool IsBallMakeNewLines()
        {
            return GameController.IsCollectedLineExist(Row, this.Col);
        }

        private async Task RemoveCollectedLinesAsync()
        {
            OnLinesAnimation();

            await Task.Run(() => StopAnimAndRemoveLines());
        }

        private async Task GenNextBallsAndCheckLinesAsync()
        {
            await Task.Delay(GameController.CreatingRandomBallsDelay);

            List<Cell> addBallsList = GameController.CreateOnBoardNewRandomBalls();

            for (int i = 0; i < addBallsList.Count; i++)
            {
                if (GameController.IsCollectedLineExist(addBallsList[i].Row, addBallsList[i].Col))
                {
                    OnLinesAnimation();

                    await Task.Run(StopAnimAndRemoveLines);
                }
            }
        }

        private void OnPathAnimation(IList<Cell> path)
        {
            CellMakePath(this, new ActionCellEventArgs(path));
        }

        private async Task StopAnimAndRelocateBall()
        {
            await Task.Delay(GameController.LinePathAnimDelay);

            ClearExistingPath();

            GameController.RelocateSelectedBall(this.Row, this.Col);
        }

        private void OnLinesAnimation()
        {
            CellExistInLine(this, new ActionCellEventArgs(GameController.CollectedBallsLines));
        }

        private async Task StopAnimAndRemoveLines()
        {
            await Task.Delay(GameController.CollectedLineAnimDelay);

            ClearExistingLine();

            GameController.RemoveCollectedLines();
        }
    }
}
