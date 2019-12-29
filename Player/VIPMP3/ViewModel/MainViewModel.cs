using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VIPMP3.Model;
using VIPMP3.Ultils;

namespace VIPMP3.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        #region Properties
        //code
        #region PlayList
        private ObservableCollection<PlayList> _playListCollection;
        public ObservableCollection<PlayList> PlayListCollection
        {
            get => _playListCollection;
            set
            {
                _playListCollection = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region S_operation
        private PlayList _selectedPlayList;
        public PlayList SelectedPlayList
        {
            get => _selectedPlayList;
            set
            {
                _selectedPlayList = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region MediaPlayer
        MediaPlayer _mediaPlayer;
        /// <summary>
        /// return duration as milisecond
        /// </summary>
        public double MediaPlayerNaturalDuration
        {
            get { return _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds : 0; }
        }
        bool _isDragging = false;
        bool _isPlaying = false;
        bool _isStopped = true;
        public delegate void timerTick();
        public DispatcherTimer ticks = new DispatcherTimer();
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
        private ObservableCollection<Music> _listPlayingMusics { get; set; }
        private List<int> RecentPlayed;
        private int _curPlayingIndex = -1;
        public int PlayingIndex
        {
            get => _curPlayingIndex;
            set
            {
                _curPlayingIndex = value;
                OnPropertyChanged();
            }
        }
        private Music _playingMusic;
        public Music PlayingMusic
        {
            get => _playingMusic;
            set
            {
                _playingMusic = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region State
        private const int REPEAT_OFF = 0;
        private const int REPEAT = 1;
        private const int REPEAT_ONE = 2;
        private int mode = 0;
        private bool isShuffle = false;
        #endregion
        #endregion

        #region Contructor
        public MainViewModel()
        {
            //initial
            //init media player
            string path =
                  AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            PlayListCollection = new ObservableCollection<PlayList>();
            PlayListCollection.Clear();
            foreach (FileInfo file in Files)
            {
                var temp = new PlayList();
                temp.Name = Path.GetFileNameWithoutExtension(file.Name);
                temp.musicList=Common.ReadFromXmlFile<List<Music>>(file.FullName);
                PlayListCollection.Add(temp);
            }
            //SelectedPlayList = PlayListCollection[0];
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
            IconKind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            _listPlayingMusics = new ObservableCollection<Music>();
            RecentPlayed = new List<int>();

            //register key hook
            registerKeyHook();
        }
        //Kiem tra da choi ngau nhien tat ca bai hat
        private bool CheckAllPlayed()
        {
            if (RecentPlayed.Count == 0) return true;
            return false;
        }
        private void SummonAgain()
        {
            for (int i = 0; i < _listPlayingMusics.Count; i++)
            {
                RecentPlayed.Add(i);
            }
        }
        private void CheckMusicPlayed(int index)
        {
            RecentPlayed.RemoveAt(index);
        }
        private void PlayAShuffle()
        {

            var r = new Random();
            int i;
            if (CheckAllPlayed() == true)
            {
                SummonAgain();
            }
            do
            {
                i = r.Next(0, RecentPlayed.Count - 1);
            } while (RecentPlayed[i] == _curPlayingIndex);
            StartMusic(_listPlayingMusics[RecentPlayed[i]], RecentPlayed[i]);
            CheckMusicPlayed(i);

        }
        private void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            switch (mode)
            {
                case REPEAT:
                    if (isShuffle && _listPlayingMusics.Count > 2)
                    {
                        PlayAShuffle();
                        return;
                    }
                    if (_curPlayingIndex == _listPlayingMusics.Count - 1)
                    {
                        StartMusic(_listPlayingMusics[0], 0);
                    }
                    else
                    {
                        StartMusic(_listPlayingMusics[_curPlayingIndex + 1], _curPlayingIndex + 1);
                    }
                    break;
                case REPEAT_OFF:
                    if (isShuffle && _listPlayingMusics.Count > 2)
                    {
                        if (CheckAllPlayed() == true)
                        {
                            _isStopped = true;
                            _isPlaying = false;
                            _curPlayingIndex = 0;
                            IconKind = PackIconKind.Play;
                            return;
                        }
                        else
                            PlayAShuffle();
                        return;
                    }
                    if (_curPlayingIndex == _listPlayingMusics.Count - 1)
                    {
                        _curPlayingIndex = 0;
                        _isPlaying = false;
                        _isStopped = true;
                        IconKind = PackIconKind.Play;
                    }
                    else
                    {
                        StartMusic(_listPlayingMusics[_curPlayingIndex + 1], _curPlayingIndex + 1);
                    }
                    break;
                case REPEAT_ONE:

                    StartMusic(_listPlayingMusics[_curPlayingIndex], _curPlayingIndex);
                    break;
            }
        }
        #endregion

        #region Command
        #region

        private ICommand _selectedMusicChangedCommand;
        public ICommand SelectedMusicChangedCommand
        {
            get
            {
                return _selectedMusicChangedCommand ??
                     (_selectedMusicChangedCommand = new RelayCommand<object>(
                         (p) =>
                         {
                             return true;
                         },
                         (p) =>
                         {
                             StartMusic(_listPlayingMusics[(int)p], (int)p);
                         }));

            }
        }
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
                if (_listPlayingMusics.Count == 0)
                {
                    ReadMusicData(_mediaPlayer, ref dialog);
                    StartMusic(_listPlayingMusics[0], 0);
                }
                else
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
                    _listPlayingMusics.Add(music);
                    RecentPlayed.Add(RecentPlayed.Count);
                    Musics = _listPlayingMusics;
                    Debug.WriteLine(_listPlayingMusics.Count);
                    OnPropertyChanged();
                });
            }

        }
        private void StartMusic(Music music, int index)
        {
            _mediaPlayer.Open(new Uri(music.Path));
            _curPlayingIndex = index;
            PlayingMusic = _listPlayingMusics[index];
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
            while (!_mediaPlayer.NaturalDuration.HasTimeSpan) { }
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
            return _listPlayingMusics.Count != 0;
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
                IconKind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                if (_isStopped)
                {
                    StartMusic(_listPlayingMusics[0], 0);
                    _isStopped = false;
                }
                else
                    _mediaPlayer.Play();
            }
            _isPlaying = !_isPlaying;
            OnPropertyChanged();
        }
        #endregion
        #region StopCommand
        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ??
                    (_stopCommand = new RelayCommand<object>(
                        (p) => CanExecuteStopCommand(),
                        (p) => ExecuteStopCommand()));
            }
        }

        private bool CanExecuteStopCommand()
        {
            return _isPlaying != false;
        }
        private void ExecuteStopCommand()
        {
            _isStopped = true;
            _isPlaying = false;
            IconKind = PackIconKind.Play;
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Stop();
            DurationValue = 0;
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
            return true;
        }
        private void ExecuteNextMusicCommand()
        {
            if (isShuffle && _listPlayingMusics.Count > 2)
            {
                PlayAShuffle();
                return;
            }
            if (_curPlayingIndex == _listPlayingMusics.Count - 1)
            {
                StartMusic(_listPlayingMusics[0], 0);
            }
            else
            {
                StartMusic(_listPlayingMusics[_curPlayingIndex + 1], _curPlayingIndex + 1);
            }
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
            StartMusic(_listPlayingMusics[_curPlayingIndex - 1], _curPlayingIndex - 1);
        }
        #endregion
        #region ChangeModePlay
        private ICommand _changeModePlay;
        public ICommand ChangeModePlay
        {
            get
            {
                return _changeModePlay ??
                    (_changeModePlay = new RelayCommand<object>(
                        (p) => CanExecuteChangeModePlay(),
                        (p) => ExecuteChangeModePlay()));
            }
        }
        private bool CanExecuteChangeModePlay()
        {
            return true;
        }
        public void ExecuteChangeModePlay()
        {
            mode = (mode + 1) % 3;
            if (mode == REPEAT_OFF) //Khong repeat
            {
                ModeKind = PackIconKind.RepeatOff;
            }
            else
            if (mode == REPEAT) //Khong repeat
            {
                ModeKind = PackIconKind.Repeat;
            }
            else
            if (mode == REPEAT_ONE) //Khong repeat
            {
                ModeKind = PackIconKind.RepeatOne;
            }
            OnPropertyChanged();
        }
        #endregion
        #region ModeKind
        private PackIconKind _modeKind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOff;
        public PackIconKind ModeKind
        {
            get { return _modeKind; }
            set
            {
                _modeKind = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region ShuffleKind
        private PackIconKind _shuffleKind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleDisabled;
        public PackIconKind ShuffleKind
        {
            get { return _shuffleKind; }
            set
            {
                _shuffleKind = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region ShuffleMode
        private ICommand _shuffleMode;
        public ICommand ShuffleMode
        {
            get
            {
                return _shuffleMode ??
                    (_shuffleMode = new RelayCommand<object>(
                        (p) => CanExecuteShuffleMode(),
                        (p) => ExecuteShuffleMode()));
            }
        }
        private bool CanExecuteShuffleMode()
        {
            return true;
        }
        public void ExecuteShuffleMode()
        {
            isShuffle = !isShuffle;
            ShuffleKind = isShuffle ? PackIconKind.Shuffle : PackIconKind.ShuffleDisabled;
            OnPropertyChanged();
        }
        #endregion
        #region DurationChangeBySlider
        private ICommand _durationChange;
        public ICommand DurationChange
        {
            get
            {
                return _durationChange ??
                    (_durationChange = new RelayCommand<object>(
                        (p) => CanExecuteDurationChange(),
                        (p) => ExecuteDurationChange(p)));
            }
        }
        private bool CanExecuteDurationChange() { return true; }
        private void ExecuteDurationChange(object p)
        {
            //ticks.Stop();
            if (_mediaPlayer.NaturalDuration.HasTimeSpan && Global.isDragging == true)
            {
                int seekPos = int.Parse(string.Format("{0}", p));
                _mediaPlayer.Position = TimeSpan.FromMilliseconds((double)seekPos *
                    _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds / 1000);
                Debug.WriteLine(seekPos);
            }
            //ticks.Start();
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
        #region CreatePlayListCommand
        private ICommand _createPlayListCommand;
        public ICommand CreatePlayListCommand
        {
            get
            {
                return _createPlayListCommand ??
                     (_createPlayListCommand = new RelayCommand<object>(
                         (p) => CanExecuteCreatePlayListCommand(),
                            (p) => ExecuteCreatePlayListCommand()));
            }
        }
        private bool CanExecuteCreatePlayListCommand()
        {
            return true;
        }

        private void ExecuteCreatePlayListCommand()
        {
            CreatePlayList createPlayList = new CreatePlayList(null);
            var playListVM = createPlayList.DataContext as PlayListViewModel;
            playListVM.RefreshPlayListList += PlayListVM_RefreshPlayListList;
            createPlayList.ShowDialog();
        }

        private void PlayListVM_RefreshPlayListList(object sender, EventArgs e)
        {
            string path =
                    AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            PlayListCollection = new ObservableCollection<PlayList>();
            PlayListCollection.Clear();
            foreach (FileInfo file in Files)
            {
                var temp = new PlayList();
                temp.Name = Path.GetFileNameWithoutExtension(file.Name);
                temp.musicList = Common.ReadFromXmlFile<List<Music>>(file.FullName);
                PlayListCollection.Add(temp);
            }
        }

      
        #endregion
        #region PlaylistSelectionChangedCommand
        private ICommand _playlistSelectionChangedCommand;
        public ICommand PlaylistSelectionChangedCommand
        {
            get
            {
                return _playlistSelectionChangedCommand ??
                     (_playlistSelectionChangedCommand = new RelayCommand<object>(
                         (p) => { return (int)p >= 0; },
                            (p) =>
                            {
                                //read file
                                ObservableCollection<Music> musics = new ObservableCollection<Music>();
                                string path = AppDomain.CurrentDomain.BaseDirectory + "/" + PlayListCollection[(int)p].Name + ".txt";

                                Musics = Common.ReadFromXmlFile<ObservableCollection<Music>>(path);
                                _listPlayingMusics = Musics;

                            }));
            }
        }
        #endregion
        #region UpdatePlayList
        private ICommand _updatePlayListCommand;
        public ICommand UpdatePlayListCommand
        {
            get
            {
                return _updatePlayListCommand ??
                     (_updatePlayListCommand = new RelayCommand<object>(
                         (p) => CanExecuteUpdatePlayListCommand(),
                            (p) => ExecuteUpdatePlayListCommand()));
            }
        }
        private bool CanExecuteUpdatePlayListCommand()
        {
            if (SelectedPlayList != null)
            {
                return true;
            }
            return false;
        }

        private void ExecuteUpdatePlayListCommand()
        {
            CreatePlayList createPlayList = new CreatePlayList(SelectedPlayList);
            createPlayList.ShowDialog();
        }
        #endregion
        #endregion
    }
}
