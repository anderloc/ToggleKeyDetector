using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToggleKeyDetector.Commands;

namespace ToggleKeyDetector.ViewModels
{
    public class NotifyIconViewModel : ViewModelBase
    {
        public ICommand Shutdown => new TerminateAppCommand();
    }
}
