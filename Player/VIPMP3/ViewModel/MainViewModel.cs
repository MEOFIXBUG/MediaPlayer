using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
                _mediaPlayer.Open(new Uri(dialog.FileName));
                StartMusic();

                Music music = new Music();
                music.Name = dialog.SafeFileName;
                music.Path = dialog.FileName;
                music.Cover = null;
                while (!_mediaPlayer.NaturalDuration.HasTimeSpan)
                {

                }
                music.Duration = _mediaPlayer.NaturalDuration.TimeSpan;
                string len = "";
                if (music.Duration.Minutes < 10)
                {
                    len += $"0{music.Duration.Minutes}";
                }
                else
                {
                    len += $"{music.Duration.Minutes}";
                }
                len += ":";
                if (music.Duration.Seconds < 10)
                {
                    len += $"0{music.Duration.Seconds}";
                }
                else
                {
                    len += $"{music.Duration.Seconds}";
                }
                LengthMusic = len;
                _listPlayingMusics.Add(music);

            }
        }
        private void StartMusic()
        {
            _mediaPlayer.Play();
            _isPlaying = true;
            IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += ticks_Tick;
            ticks.Start();
            OnPropertyChanged();
        }
        void ticks_Tick(object sender, object e)
        {
            DurationValue = (int)(_mediaPlayer.Position.TotalMilliseconds / _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds * 1000);
            string curTime = "";
            if (_mediaPlayer.Position.Minutes < 10)
            {
                curTime += $"0{_mediaPlayer.Position.Minutes}";
            } else
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
            Debug.WriteLine($"Time: {DurationValue}");
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
        private string lengthMusic;
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
        #endregion
    }
}
