using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VIPMP3.Model;
using VIPMP3.Ultils;

namespace VIPMP3.ViewModel
{
    public class PlayListViewModel : BaseViewModel
    {

        #region Contructor

        public PlayListViewModel()
        {
            //initial
            //init media player
            _mediaPlayer = new MediaPlayer();
           
        }
        #endregion
        #region Propertise
        #region SelectedMusic

        private Music _SelectedItem;
        public Music SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region DeleteFileCommand
        private ICommand _DelFileMP3Command;

        public ICommand DelFileMP3Command
        {
            get
            {
                return _DelFileMP3Command ??
                     (_DelFileMP3Command = new RelayCommand<object>(
                         (p) => CanExecuteDelFileCommand(),
                            (p) => ExecuteDelFileCommand()));
            }
        }

        private bool CanExecuteDelFileCommand()
        {
            return (SelectedItem != null);
        }

        private void ExecuteDelFileCommand()
        {
            Musics.Remove(SelectedItem);
        }
        #endregion
        public event EventHandler RefreshPlayListList;

        #region NamePlaylist
        private string _namePlayList;
        public string NamePlayList
        {
            get
            {
                return _namePlayList;
            }
            set
            {
                if (_namePlayList != value)
                {
                    _namePlayList = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region ListPlaying
        private ObservableCollection<Music> _musics = new ObservableCollection<Music>();
        public ObservableCollection<Music> Musics
        {
            get
            {
                return _musics;
            }
            set
            {
                if (_musics != value)
                {
                    _musics = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        MediaPlayer _mediaPlayer;
        /// <summary>
        /// return duration as milisecond
        /// </summary>
        public double MediaPlayerNaturalDuration
        {
            get { return _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds : 0; }
        }
        #region Data
        private ObservableCollection<Music> _listPlayingMusics { get; set; }
       
        #endregion
        #endregion
       
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
            dialog.Multiselect = true;
            dialog.Filter = "MP3 files (*.mp3)|*.mp3|M4A files (*.m4a)|*.m4a|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                ReadMusicData(_mediaPlayer, ref dialog);
            }

        }
        #endregion
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
        private void ReadMusicData(MediaPlayer mediaPlayer2, ref OpenFileDialog dialog)
        {
            if (mediaPlayer2 == null)
            {
                mediaPlayer2 = new MediaPlayer();
            }

            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                mediaPlayer2.Open(new Uri(dialog.FileNames[i]));
                Music music = new Music();
                music.Name = dialog.SafeFileNames[i];
                music.Path = dialog.FileNames[i];
                music.Cover = null;
                while (!mediaPlayer2.NaturalDuration.HasTimeSpan)
                {

                }
                music.Duration = mediaPlayer2.NaturalDuration.TimeSpan;
                music.DurationInString = convertLengthToString(music.Duration.Minutes, music.Duration.Seconds);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Musics.Add(music);
                    OnPropertyChanged();
                });
            }

        }
        #region SavePlayListCommand
        private ICommand _savePlayListCommand;
        public ICommand SavePlayListCommand
        {
            get
            {
                return _savePlayListCommand ??
                     (_savePlayListCommand = new RelayCommand<object>(
                         (p) => CanExecuteSavePlayListCommand(),
                            (p) => ExecuteSavePlayListCommand()));
            }
        }
        private bool CanExecuteSavePlayListCommand()
        {
            if (NamePlayList==null || NamePlayList =="")
            {
                return false;
            }
            return true;
        }

        private void ExecuteSavePlayListCommand()
        {
            Common.WriteToXmlFile<ObservableCollection<Music>>(NamePlayList+".txt", Musics);
            MessageBox.Show("Lưu Thành Công");

            RefreshPlayListList?.Invoke(this, null);
            //update mainWindow.
        }
        #endregion
    }
}
