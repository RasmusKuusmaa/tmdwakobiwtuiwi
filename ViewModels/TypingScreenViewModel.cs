using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace tmdwa.ViewModels
{
    internal class TypingScreenViewModel : BaseViewModel
    {
        private string _writtenText;

        public string WrittenText
        {
            get { return _writtenText; }
            set { _writtenText = value;
                OnPropertyChanged();
                ResetTimer();
            }
        }
        private readonly DispatcherTimer _timer;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
        private void TimerElapsed(object sender, EventArgs  e)
        {
            _timer.Stop();
            WrittenText = string.Empty;
            MessageBox.Show("typing has been stopped, progress has been lost", "Times up", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public TypingScreenViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = _timeout,
            };
            _timer.Tick += TimerElapsed;
            _timer.Start();
        }
    }
}
