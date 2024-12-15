using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace CustomerFeedbackApp.ViewModels
{
    public class MainViewModel
    {
        public ICommand SubmitFeedbackCommand { get; set; }
        public ICommand LoginAsAdminCommand { get; set; }

        public MainViewModel()
        {
            SubmitFeedbackCommand = new RelayCommand(OpenFeedbackForm);
            LoginAsAdminCommand = new RelayCommand(OpenAdminLogin);
        }

        private void OpenFeedbackForm(object parameter)
        {
            var feedbackForm = new Views.FeedbackForm();
            feedbackForm.Show();
        }

        private void OpenAdminLogin(object parameter)
        {
            MessageBox.Show("Opening Admin Login...");
            // Navigate to Login Page for Admins
        }
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public event EventHandler CanExecuteChanged;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

            public void Execute(object parameter) => _execute(parameter);
        }
    }
}
