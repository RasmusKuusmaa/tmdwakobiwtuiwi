using System;
using System.Windows;
using System.Windows.Threading;

namespace tmdwa.ViewModels
{
    internal class TypingScreenViewModel : BaseViewModel
    {
        private bool _isSessionActive = false;
        private bool _isProgrammaticChange = false;
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

        private readonly DispatcherTimer _timeoutTimer;
        private readonly DispatcherTimer _writingTimer;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _writingTime = TimeSpan.FromSeconds(5);

        public TypingScreenViewModel()
        {
            _timeoutTimer = new DispatcherTimer { Interval = _timeout };
            _timeoutTimer.Tick += OnTimeout;

            _writingTimer = new DispatcherTimer { Interval = _writingTime };
            _writingTimer.Tick += OnWritingTimeOver;
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            StopSession();
            _isProgrammaticChange = true;
            WrittenText = string.Empty;
            _isProgrammaticChange = false;
            MessageBox.Show("typing has been stopped, progress has been lost", "Times up", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OnWritingTimeOver(object sender, EventArgs e)
        {
            StopSession();
            MessageBox.Show("time over");
        }

        private void StartSession()
        {
            _isSessionActive = true;
            _timeoutTimer.Start();
            _writingTimer.Start();
        }

        private void StopSession()
        {
            _isSessionActive = false;
            _timeoutTimer.Stop();
            _writingTimer.Stop();
        }

        private void ResetTimeoutTimer()
        {
            if (_isSessionActive)
            {
                _timeoutTimer.Stop();
                _timeoutTimer.Start();
            }
        }

        private void OnUserTyped()
        {
            if (_isProgrammaticChange)
                return;

            if (!_isSessionActive)
            {
                StartSession();
            }
            else
            {
                ResetTimeoutTimer();
            }
        }
    }
}