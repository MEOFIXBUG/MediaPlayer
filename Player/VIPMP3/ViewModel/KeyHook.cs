using Gma.System.MouseKeyHook;
using System.Windows.Forms;
namespace VIPMP3.ViewModel
{
    public partial class MainViewModel
    {

        private IKeyboardMouseEvents _hook;
        private void registerKeyHook()
        {
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += keyUp_hook;
        }
        private void keyUp_hook(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) //play, pause
            {
                if (_isPlaying)
                {
                    ExecutePlayPauseCommand();
                }
                else
                {
                    ExecutePlayPauseCommand();
                }
            }

            if (e.Control && (e.KeyCode == Keys.Right)) //next
            {
                ExecuteNextMusicCommand();
            }

            if (e.Control && (e.KeyCode == Keys.Left)) //previous
            {
                ExecutePreviousMusicCommand();
            }


        }

        ~MainViewModel()
        {
            _hook.KeyUp -= keyUp_hook;
            _hook.Dispose();
        }


    }
}