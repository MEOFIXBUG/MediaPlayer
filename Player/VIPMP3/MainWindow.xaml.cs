﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            Slider slider = sender as Slider;
            var viewModel = (MainViewModel)this.DataContext;
            viewModel.DurationChange.Execute(slider.Value);
        }





        private void DurationSlider_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Dragging");
            Global.isDragging = true;
        }

        private void DurationSlider_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("StopDrag");
            Global.isDragging = false;
        }
    }
}
