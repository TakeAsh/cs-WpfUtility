using System;
using System.Windows.Input;

namespace WpfUtility {

    /// <summary>
    /// [InputBinding Class](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.inputbinding)
    /// </summary>
    public class SimpleDelegateCommand : ICommand {

        // Specify the keys and mouse actions that invoke the command. 
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }

        private Action<object> _executeDelegate;

        public SimpleDelegateCommand(Action<object> executeDelegate) {
            _executeDelegate = executeDelegate;
        }

        public void Execute(object parameter) {
            _executeDelegate(parameter);
        }

        public bool CanExecute(object parameter) { return true; }

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
    }
}
