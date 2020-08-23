using System;
using System.Windows.Input;
namespace Smairo.MSFS2020.Model
{
    public class BaseCommand : ICommand
    {
        public Action<object> ExecuteDelegate { get; set; }
        public event EventHandler CanExecuteChanged;

        public BaseCommand()
        {
            ExecuteDelegate = null;
        }

        public BaseCommand(Action<object> executeDelegate)
        {
            ExecuteDelegate = executeDelegate;
        }

        public bool CanExecute(object oParameter)
        {
            return true;
        }

        public void Execute(object oParameter)
        {
            ExecuteDelegate?.Invoke(oParameter);
        }
    }
}