using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ToggleKeyDetector.Commands
{
    public class TerminateAppCommand : CommandBase
    {
        public TerminateAppCommand()
        {
        }

        public override void Execute(object? parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
