using System;

namespace Kogler.Framework
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        /// <summary>
        /// Raises CanExecute(object parameter) method.
        /// </summary>
        void RaiseCanExecuteChanged();
    }

    public interface ICommandNoParameter : ICommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool CanExecute();

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        void Execute();

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        new void Execute(object parameter);

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        new bool CanExecute(object parameter);
    }

    public interface ICommand<in T> : ICommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        bool CanExecute(T parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        void Execute(T parameter);

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        new void Execute(object parameter);

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        new bool CanExecute(object parameter);
    }

    public class Command : ICommandNoParameter
    {
        #region << Members >>

        private readonly Func<bool> m_CanExecuteAction;
        private readonly Action m_ExecuteAction;

        #endregion

        #region << Public >>

        public Command(Action executeAction, Func<bool> canExecuteAction = null)
        {
            if (executeAction == null) throw new ArgumentNullException(nameof(executeAction));
            m_ExecuteAction = executeAction;
            m_CanExecuteAction = canExecuteAction;
        }

        #endregion

        #region << ICommandNoParameter Members >>

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute()
        {
            return m_CanExecuteAction == null || m_CanExecuteAction();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        public void Execute()
        {
            if (CanExecute()) m_ExecuteAction();
        }

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        public void Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        /// Obsolete, use overload with typed parameter! Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        [Obsolete]
        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        protected void OnCanExecuteChanged(object sender, EventArgs args)
        {
            CanExecuteChanged?.Invoke(sender, args);
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
    
    public class Command<T> : ICommand<T>
    {
        #region << Members >>

        private readonly Func<T, bool> m_CanExecuteAction;
        private readonly Action<T> m_ExecuteAction;

        #endregion

        #region << Public >>

        public Command(Action<T> executeAction, Func<T, bool> canExecuteAction = null)
        {
            if (executeAction == null) throw new ArgumentNullException(nameof(executeAction));
            m_ExecuteAction = executeAction;
            m_CanExecuteAction = canExecuteAction;
        }

        #endregion

        #region << ICommand Members >>

        public bool CanExecute(T parameter)
        {
            return SafeCanExecuteResult(parameter);
        }

        public void Execute(T parameter)
        {
            if (CanExecute(parameter)) m_ExecuteAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        #region << System.Windows.Input.ICommand >>

        [Obsolete]
        public bool CanExecute(object parameter = null)
        {
            return SafeCanExecuteResult(SafeTypedParameter(parameter));
        }

        [Obsolete]
        public void Execute(object parameter)
        {
            T par = SafeTypedParameter(parameter);
            if (CanExecute(parameter)) m_ExecuteAction(par);
        }

        #endregion

        #region << Safe >>

        private static T SafeTypedParameter(object parameter)
        {
            T par;
            try
            {
                par = (T)parameter;
            }
            // prevent exception on invalid parameter type
            catch (InvalidCastException)
            {
                par = default(T);
            }
            // prevent exception when parameter is struct and null.
            catch (NullReferenceException)
            {
                par = default(T);
            }
            return par;
        }

        private bool SafeCanExecuteResult(T parameter)
        {
            bool result;
            try
            {
                result = m_CanExecuteAction == null || m_CanExecuteAction(SafeTypedParameter(parameter));
            }
            // prevent exception on poor CanExecuteAction implementation
            catch (NullReferenceException)
            {
                result = true;
            }
            return result;
        }

        #endregion

        #endregion

        #region << Protected >>

        protected void OnCanExecuteChanged(object sender, EventArgs args)
        {
            CanExecuteChanged?.Invoke(sender, args);
        }

        #endregion
    }
}