using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace tmdwa.ViewModels
{
    internal class TypingScreenViewModel : BaseViewModel
    {
        private bool _isSessionActive = false;
        private bool _isProgrammaticChange = false;
        private string _writtenText = string.Empty;
        private int _sessionTimeLeft;
        private int _timeoutTimeLeft;
        private int _sessionDuration = 300;
        private int _timeoutDuration = 5;
        private double _timeoutDelay = 1.0;
        private bool _colorWarningEnabled = true;
        private double _colorWarningThreshold = 2.5;
        private string _warningColorName = "Red";
        private bool _sessionTimeInMinutes = true;
        private Brush _timeoutTimerColor = Brushes.Black;
        private Brush _textColor = Brushes.Black;
        private double _timeoutCountdownDelay = 0;

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

        public int SessionTimeLeft
        {
            get => _sessionTimeLeft;
            set
            {
                _sessionTimeLeft = value;
                OnPropertyChanged();
            }
        }

        public int TimeoutTimeLeft
        {
            get => _timeoutTimeLeft;
            set
            {
                _timeoutTimeLeft = value;
                OnPropertyChanged();
                TimeoutTimerColor = value == 1 ? Brushes.Red : Brushes.Black;
            }
        }

        public int SessionDuration
        {
            get => _sessionDuration;
            set
            {
                if (value > 0)
                {
                    _sessionDuration = value;
                    if (!_isSessionActive)
                        SessionTimeLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TimeoutDuration
        {
            get => _timeoutDuration;
            set
            {
                if (value > 0)
                {
                    _timeoutDuration = value;
                    if (!_isSessionActive)
                        TimeoutTimeLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public double TimeoutDelay
        {
            get => _timeoutDelay;
            set
            {
                if (value >= 0)
                {
                    _timeoutDelay = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ColorWarningEnabled
        {
            get => _colorWarningEnabled;
            set
            {
                _colorWarningEnabled = value;
                OnPropertyChanged();
            }
        }

        public double ColorWarningThreshold
        {
            get => _colorWarningThreshold;
            set
            {
                if (value >= 0)
                {
                    _colorWarningThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WarningColorName
        {
            get => _warningColorName;
            set
            {
                _warningColorName = value;
                OnPropertyChanged();
            }
        }

        public bool SessionTimeInMinutes
        {
            get => _sessionTimeInMinutes;
            set
            {
                _sessionTimeInMinutes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplaySessionDuration));
            }
        }

        public double DisplaySessionDuration
        {
            get => _sessionTimeInMinutes ? _sessionDuration / 60.0 : _sessionDuration;
            set
            {
                var newDuration = _sessionTimeInMinutes ? (int)(value * 60) : (int)value;
                if (newDuration > 0)
                {
                    SessionDuration = newDuration;
                    OnPropertyChanged();
                }
            }
        }

        public Brush TimeoutTimerColor
        {
            get => _timeoutTimerColor;
            set
            {
                _timeoutTimerColor = value;
                OnPropertyChanged();
            }
        }

        public Brush TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        private readonly DispatcherTimer _displayTimer;
        private readonly DispatcherTimer _timeoutTimer;
        private readonly DispatcherTimer _writingTimer;

        public TypingScreenViewModel()
        {
            _displayTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _displayTimer.Tick += OnDisplayTimerTick;

            _timeoutTimer = new DispatcherTimer();
            _timeoutTimer.Tick += OnTimeout;

            _writingTimer = new DispatcherTimer();
            _writingTimer.Tick += OnWritingTimeOver;

            SessionTimeLeft = _sessionDuration;
            TimeoutTimeLeft = _timeoutDuration;
        }

        private void OnDisplayTimerTick(object sender, EventArgs e)
        {
            if (_isSessionActive)
            {
                SessionTimeLeft--;

                if (_timeoutCountdownDelay > 0)
                {
                    _timeoutCountdownDelay -= 1.0;
                }
                else
                {
                    TimeoutTimeLeft--;

                    if (_colorWarningEnabled && TimeoutTimeLeft <= _colorWarningThreshold)
                    {
                        var warningBrush = GetBrushFromColorName(_warningColorName);
                        TimeoutTimerColor = warningBrush;
                        TextColor = warningBrush;
                    }
                    else
                    {
                        TimeoutTimerColor = Brushes.Black;
                        TextColor = Brushes.Black;
                    }
                }

                if (SessionTimeLeft <= 0)
                {
                    OnWritingTimeOver(sender, e);
                }
                else if (TimeoutTimeLeft <= 0)
                {
                    OnTimeout(sender, e);
                }
            }
        }

        private Brush GetBrushFromColorName(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => Brushes.Red,
                "blue" => Brushes.Blue,
                "green" => Brushes.Green,
                "orange" => Brushes.Orange,
                "purple" => Brushes.Purple,
                "yellow" => Brushes.Yellow,
                "pink" => Brushes.Pink,
                "brown" => Brushes.Brown,
                "gray" => Brushes.Gray,
                "darkred" => Brushes.DarkRed,
                "darkblue" => Brushes.DarkBlue,
                "darkgreen" => Brushes.DarkGreen,
                _ => Brushes.Red
            };
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
            SessionTimeLeft = _sessionDuration;
            TimeoutTimeLeft = _timeoutDuration;
            _timeoutCountdownDelay = _timeoutDelay;
            TimeoutTimerColor = Brushes.Black;
            TextColor = Brushes.Black;

            _timeoutTimer.Interval = TimeSpan.FromSeconds(_timeoutDuration);
            _writingTimer.Interval = TimeSpan.FromSeconds(_sessionDuration);

            _displayTimer.Start();
            _timeoutTimer.Start();
            _writingTimer.Start();
        }

        private void StopSession()
        {
            _isSessionActive = false;
            _displayTimer.Stop();
            _timeoutTimer.Stop();
            _writingTimer.Stop();

            SessionTimeLeft = _sessionDuration;
            TimeoutTimeLeft = _timeoutDuration;
            TimeoutTimerColor = Brushes.Black;
            TextColor = Brushes.Black;
        }

        private void ResetTimeoutTimer()
        {
            if (_isSessionActive)
            {
                TimeoutTimeLeft = _timeoutDuration;
                _timeoutCountdownDelay = _timeoutDelay;
                TimeoutTimerColor = Brushes.Black;
                TextColor = Brushes.Black;
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