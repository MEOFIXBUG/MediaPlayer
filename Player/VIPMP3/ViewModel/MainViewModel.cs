using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VIPMP3.Model;

namespace VIPMP3.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        //code
        #region MediaPlayer
        MediaPlayer _mediaPlayer;
        bool _isPlaying = false;
        public delegate void timerTick();
        DispatcherTimer ticks = new DispatcherTimer();
        timerTick tick;
        #endregion
        #region Play_PauseButton
        public PackIconKind IconKind
        {
            get { return _IconKind; }
            set { _IconKind = value; OnPropertyChanged("IconKind"); }

        }


        private PackIconKind _IconKind = PackIconKind.Play;
        #endregion
        #region Data
        private ObservableCollection<Music> _listPlayingMusics;
        private int _curPlayingIndex = -1;
        #endregion
        #endregion

        #region Contructor
        public MainViewModel()
        {
            //initial
            //init media player
            _mediaPlayer = new MediaPlayer();
            IconKind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            _listPlayingMusics = new ObservableCollection<Music>();
        }
        #endregion

        #region Command
        #region AddFileMP3Command
        private ICommand _addFileMP3Command;
        public ICommand AddFileMP3Command
        {
            get
            {
                return _addFileMP3Command ??
                     (_addFileMP3Command = new RelayCommand<object>(
                         (p) => CanExecuteAddFileCommand(),
                            (p) => ExecuteAddFileCommand()));
            }
        }
        private bool CanExecuteAddFileCommand()
        {
            return true;
        }

        private void ExecuteAddFileCommand()
        {
            //CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.IsFolderPicker = true;

            //if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            //{
            //    var listFile = Directory.GetFiles(dialog.FileName);
            //    //var listFile = Directory.GetFiles(dialog.FileName);

            //}
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MP3 files (*.mp3)|*.mp3|M4A files (*.m4a)|*.m4a|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                if (_listPlayingMusics.Count == 0)
                {
                    ReadMusicData(_mediaPlayer,ref dialog);
                    StartMusic(_listPlayingMusics[0]);
                    _curPlayingIndex = 0;
                } else
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        /* run your code here */
                        ReadMusicData(null, ref dialog);
                        Thread.CurrentThread.Abort();
                    }).Start();
                }

            }
            
        }
        private void ReadMusicData(MediaPlayer mediaPlayer2,ref OpenFileDialog dialog)
        {
            if (mediaPlayer2 == null)
            {
                mediaPlayer2 = new MediaPlayer();
            } 
            mediaPlayer2.Open(new Uri(dialog.FileName));
            Music music = new Music();
            music.Name = dialog.SafeFileName;
            music.Path = dialog.FileName;
            music.Cover = null;
            while (!mediaPlayer2.NaturalDuration.HasTimeSpan)
            {

            }
            music.Duration = mediaPlayer2.NaturalDuration.TimeSpan;
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _listPlayingMusics.Add(music);
                Debug.WriteLine(_listPlayingMusics.Count);
                OnPropertyChanged();
            });

        }
        private void StartMusic(Music music)
        {
            _mediaPlayer.Open(new Uri(music.Path));
            _mediaPlayer.Play();
            LengthMusic = convertLengthToString(music.Duration.Minutes, music.Duration.Seconds);
            NameMusic = music.Name;
            _isPlaying = true;
            IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += ticks_Tick;
            ticks.Start();
            OnPropertyChanged();
        }
        private string convertLengthToString(int minutes, int seconds)
        {
            string len = "";
            if (minutes < 10)
            {
                len += $"0{minutes}";
            }
            else
            {
                len += $"{minutes}";
            }
            len += ":";
            if (seconds < 10)
            {
                len += $"0{seconds}";
            }
            else
            {
                len += $"{seconds}";
            }
            return len;
        }
        void ticks_Tick(object sender, object e)
        {
            while(!_mediaPlayer.NaturalDuration.HasTimeSpan) { }
            DurationValue = (int)(_mediaPlayer.Position.TotalMilliseconds / _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds * 1000);
            string curTime = "";
            if (_mediaPlayer.Position.Minutes < 10)
            {
                curTime += $"0{_mediaPlayer.Position.Minutes}";
            }
            else
            {
                curTime += $"{_mediaPlayer.Position.Minutes}";
            }
            curTime += ":";
            if (_mediaPlayer.Position.Seconds < 10)
            {
                curTime += $"0{_mediaPlayer.Position.Seconds}";
            }
            else
            {
                curTime += $"{_mediaPlayer.Position.Seconds}";
            }
            CurPosition = curTime;
        }
        void changeStatus()
        {
            /* If you want the Slider to Update Regularly Just UnComment the Line Below*/
        }
        #endregion
        #region CloseCommand
        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                     (_closeCommand = new RelayCommand<object>(
                         (p) => CanExecuteCloseCommand(),
                            (p) => ExecuteCloseCommand()));
            }
        }
        private bool CanExecuteCloseCommand()
        {
            return true;
        }

        private void ExecuteCloseCommand()
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region MinimizeCommand
        private ICommand _minimizeCommand;
        public ICommand MinimizeCommand
        {
            get
            {
                return _minimizeCommand ??
                     (_minimizeCommand = new RelayCommand<object>(
                         (p) => CanExecuteMinimizeCommand(),
                         (p) => ExecuteMinimizeCommand()));
            }
        }
        private bool CanExecuteMinimizeCommand()
        {
            return true;
        }
        private void ExecuteMinimizeCommand()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        #endregion
        #region Play_PauseCommand
        private ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                return _playPauseCommand ??
                    (_playPauseCommand = new RelayCommand<object>(
                        (p) => CanExecutePlayPauseCommand(),
                        (p) => ExecutePlayPauseCommand()));
            }
        }

        private bool CanExecutePlayPauseCommand()
        {
            return true;
        }
        private void ExecutePlayPauseCommand()
        {
            if (_listPlayingMusics.Count == 0)
            {
                ExecuteAddFileCommand();
                return;
            }
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                IconKind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
            else
            {
                _mediaPlayer.Play();
                IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            _isPlaying = !_isPlaying;
            OnPropertyChanged();
        }
        #endregion

        #region UpdateDuration
        private int _value;
        public int DurationValue
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region CurrentTime
        private string _curTime = "00:00";
        public string CurPosition
        {
            get { return _curTime; }
            set
            {
                _curTime = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region LengthMusic
        private string lengthMusic = "00:00";
        public string LengthMusic
        {
            get { return lengthMusic; }
            set
            {
                lengthMusic = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region NameMusic
        private string nameMusic = "Please Choose Musics";
        public string NameMusic
        {
            get
            {
                return nameMusic;
            }
            set
            {
                nameMusic = value;
                OnPropertyChanged();

            }
        }
        #endregion
        #region NextMusic
        private ICommand _nextMusic;
        public ICommand NextMusic
        {
            get
            {
                return _nextMusic ??
                    (_nextMusic = new RelayCommand<object>(
                        (p) => CanExecuteNextMusicCommand(),
                        (p) => ExecuteNextMusicCommand()));
            }
        }
        private bool CanExecuteNextMusicCommand()
        {
            if (_curPlayingIndex < _listPlayingMusics.Count - 1)
            {
                return true;
            } return false;
        }
        private void ExecuteNextMusicCommand()
        {
            StartMusic(_listPlayingMusics[_curPlayingIndex + 1]);
            _curPlayingIndex += 1;
        }
        #endregion

        #region PreviousMusic
        private ICommand _previousMusic;
        public ICommand PreviousMusic
        {
            get
            {
                return _previousMusic ??
                    (_previousMusic = new RelayCommand<object>(
                        (p) => CanExecutePreviousMusicCommand(),
                        (p) => ExecutePreviousMusicCommand()));
            }
        }
        private bool CanExecutePreviousMusicCommand()
        {
            if (_curPlayingIndex > 0)
            {
                return true;
            }
            return false;
        }
        private void ExecutePreviousMusicCommand()
        {
            StartMusic(_listPlayingMusics[_curPlayingIndex - 1]);
            _curPlayingIndex -= 1;
        }
        #endregion
        #endregion
    }
}
