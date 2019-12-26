using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VIPMP3.ViewModel;


namespace VIPMP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
        }

        private void Proxima_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Anterior_Click(object sender, RoutedEventArgs e)
        {

        }



        private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            var viewModel = (MainViewModel)this.DataContext;
            var time = TimeSpan.FromMilliseconds( slider.Value / 1000 * viewModel.MediaPlayerNaturalDuration);
            string curTime = "";
            if (time.Minutes < 10)
            {
                curTime += $"0{time.Minutes}";
            }
            else
            {
                curTime += $"{time.Minutes}";
            }
            curTime += ":";
            if (time.Seconds < 10)
            {
                curTime += $"0{time.Seconds}";
            }
            else
            {
                curTime += $"{time.Seconds}";
            }
            viewModel.CurPosition = curTime;
        }





        private void DurationSlider_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Dragging");
            Global.isDragging = true;
            var viewModel = (MainViewModel)this.DataContext;
            viewModel.ticks.Stop();

        }

        private void DurationSlider_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("StopDrag");

            Slider slider = sender as Slider;
            var viewModel = (MainViewModel)this.DataContext;
            viewModel.DurationChange.Execute(slider.Value);
            viewModel.ticks.Start();
            Global.isDragging = false;

        }
    }
}
