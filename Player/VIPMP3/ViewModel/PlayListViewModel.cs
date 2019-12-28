using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VIPMP3.ViewModel
{
    public class PlayListViewModel : BaseViewModel
    {
        //#region CloseCommand
        //private ICommand _closeCommand;
        //public ICommand CloseCommand
        //{
        //    get
        //    {
        //        return _closeCommand ??
        //             (_closeCommand = new RelayCommand<object>(
        //                 (p) => CanExecuteCloseCommand(),
        //                    (p) => ExecuteCloseCommand()));
        //    }
        //}
        //private bool CanExecuteCloseCommand()
        //{
        //    return true;
        //}

        //private void ExecuteCloseCommand()
        //{
        //    Application.Current.Shutdown();
        //}
        //#endregion
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
                

            }

        }
        #endregion

    }
}
