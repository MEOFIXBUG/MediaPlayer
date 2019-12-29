using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VIPMP3.Model;

namespace VIPMP3
{
    /// <summary>
    /// Interaction logic for CreatePlayList.xaml
    /// </summary>
    public partial class CreatePlayList : Window
    {
        public CreatePlayList(PlayList playList)
        {
            InitializeComponent();
            if (playList != null)
            {
                PlayListName.Text = playList.Name;
                PlayListName.IsEnabled = false;
                musicsListView.ItemsSource= new ObservableCollection<Music>(playList.musicList);
            }
           
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
