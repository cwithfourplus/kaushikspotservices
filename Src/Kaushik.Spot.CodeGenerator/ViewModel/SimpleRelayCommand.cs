using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kaushik.Spot.CodeGenerator.ViewModel
{
    public class SimpleRelayCommand : ICommand
    {
        #region Fields

        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        #endregion

        #region Constructors

        public SimpleRelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public SimpleRelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

        #region Events
        public void OnCanExecuteChanged(object sender = null, EventArgs e = null)
        {
            EventHandler eventHandler = CanExecuteChanged;

            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }
        #endregion Events
    }
}
