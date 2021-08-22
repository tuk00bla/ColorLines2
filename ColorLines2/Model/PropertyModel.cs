using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BallsAndLines.Model
{
    internal class PropertyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
