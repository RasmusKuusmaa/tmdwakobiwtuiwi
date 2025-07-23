using System;
using System.Windows;
using System.Windows.Threading;
namespace tmdwa.ViewModels
{
    internal class TypingScreenViewModel : BaseViewModel
    {
        private bool _isAwaitingRestart = false;
        private string _writtenText = string.Empty;
        public string WrittenText
        {
            get => _writtenText;
            set
            {
                if (_writtenText != value)
                {
                    _writtenText = value;
                    OnPropertyChanged();
                    OnUserTyped();
                }
            }
        }
        private readonly DispatcherTimer _timer;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(2);
        private bool _isProgrammaticChange = false;
        public TypingScreenViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = _timeout,
            };
            _timer.Tick += TimerElapsed;
        }
        private void TimerElapsed(object sender, EventArgs e)
        {
            if (_isAwaitingRestart)
                return;
            _timer.Stop();
            _isAwaitingRestart = true;
            _isProgrammaticChange = true;
            WrittenText = string.Empty;
            _isProgrammaticChange = false;
            MessageBox.Show("typing has been stopped, progress has been lost", "Times up", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
        private void OnUserTyped()
        {
            if (_isProgrammaticChange)
                return;
            if (_isAwaitingRestart)
            {
                _isAwaitingRestart = false;
                _timer.Start();
            }
            else
            {
                if (!_timer.IsEnabled)
                    _timer.Start();
                else
                    ResetTimer();
            }
        }
    }
}