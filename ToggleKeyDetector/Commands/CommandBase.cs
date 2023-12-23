using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToggleKeyDetector.Commands
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public Func<bool> CanExecuteFunc { get; set; }

        public bool CanExecute(object? parameter)
        {
            return this.CanExecuteFunc?.Invoke() ?? true;
        }

        public abstract void Execute(object? parameter);

        public void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
