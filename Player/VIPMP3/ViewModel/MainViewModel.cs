using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
                _mediaPlayer.Play();

                Music music = new Music();
                music.Name = dialog.SafeFileName;
                music.Path = dialog.FileName;
                music.Cover = null;
                while(!_mediaPlayer.NaturalDuration.HasTimeSpan)
                {

                }
                music.Duration = _mediaPlayer.NaturalDuration.TimeSpan;
                _listPlayingMusics.Add(music);

                _isPlaying = true;
                IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                OnPropertyChanged();
            }
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
            } else
            {
                _mediaPlayer.Play();
                IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            _isPlaying = !_isPlaying;
            OnPropertyChanged();
        }
        #endregion
        #endregion
    }
}
