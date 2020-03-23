using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YodaApp.Utils
{
    abstract class AbstractCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }

    class RelayCommand : AbstractCommand
    {
        protected Func<bool> canExecute;
        protected Action action;

        protected RelayCommand() { }

        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object _parameter) => canExecute == null ? true : canExecute();

        public override void Execute(object _parameter)
        {
            action();
        }

    }

    class RelayCommand<T> : AbstractCommand
    {
        private Action<T> action;
        private Predicate<T> canExecute;

        public RelayCommand(Action<T> action, Predicate<T> canExecute = null)
        {
            this.canExecute = canExecute;
            this.action = action;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is T val)
                return canExecute(val);
            return false;
        }

        public override void Execute(object parameter)
        {
            if (parameter is T val)
                action(val);
        }
    }

    abstract class AbstractAsyncCommand : AbstractCommand
    {
        private bool isBusy = false;

        public bool IsBusy
        {
            get => isBusy;
            protected set
            {
                isBusy = value;
                RaiseCanExecuteChanged();
            }
        }

    }

    class AsyncRelayCommand : AbstractAsyncCommand
    {

        protected Func<Task> action;
        protected Func<bool> canExecute;

        public AsyncRelayCommand(Func<Task> action, Func<bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object _parameter)
        {
            return !IsBusy && (canExecute == null || canExecute());
        }

        public override async void Execute(object _parameter)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            await action();
            IsBusy = false;
        }
    }

    class AsyncRelayCommand<T>: AbstractAsyncCommand
    {

        protected Func<T, Task> action;
        protected Predicate<T> canExecute;

        public AsyncRelayCommand(Func<T, Task> action, Predicate<T> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            if (IsBusy)
                return false;

            if (canExecute == null)
                return true;

            if (parameter is T val)
                return canExecute(val);
            return false;
        }

        public override async void Execute(object parameter)
        {
            if (IsBusy)
                return;
            if (parameter is T val)
            {
                IsBusy = true;
                await action(val);
                IsBusy = false;
            }
        }
    }
}
