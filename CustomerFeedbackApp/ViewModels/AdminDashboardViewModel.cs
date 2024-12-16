using CustomerFeedbackApp.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using CustomerFeedbackApp.Models;
using CustomerFeedbackApp.Views;

namespace CustomerFeedbackApp.ViewModels
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private ObservableCollection<Feedback> _feedbackList;
        public ObservableCollection<Feedback> FeedbackList
        {
            get => _feedbackList;
            set => SetProperty(ref _feedbackList, value);
        }

        private int _totalFeedback;
        public int TotalFeedback
        {
            get => _totalFeedback;
            set => SetProperty(ref _totalFeedback, value);
        }

        private double _averageRating; // Optional if rating is added later
        public double AverageRating
        {
            get => _averageRating;
            set => SetProperty(ref _averageRating, value);
        }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public AdminDashboardViewModel()
        {
            FeedbackList = new ObservableCollection<Feedback>();
            EditCommand = new RelayCommand<Feedback>(ExecuteEdit);
            DeleteCommand = new RelayCommand<Feedback>(ExecuteDelete);

            LoadFeedbackData();
        }

        private void LoadFeedbackData()
        {
            try
            {
                using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT FeedbackId, CustomerName, CustomerEmail, Product, Comment, SubmittedAt FROM feedbacks";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            FeedbackList.Clear();

                            while (reader.Read())
                            {
                                Feedback feedback = new Feedback
                                {
                                    FeedbackId = reader.GetInt32("FeedbackId"),
                                    CustomerName = reader.GetString("CustomerName"),
                                    CustomerEmail = reader.GetString("CustomerEmail"),
                                    Product = reader.GetString("Product"),
                                    Comment = reader.GetString("Comment"),
                                    SubmittedAt = reader.GetDateTime("SubmittedAt")
                                };
                                FeedbackList.Add(feedback);
                            }

                            TotalFeedback = FeedbackList.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading feedback data: {ex.Message}");
            }
        }

        private void ExecuteEdit(Feedback feedback)
        {
            if (feedback == null)
            {
                MessageBox.Show("No feedback selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create an instance of the EditFeedbackViewModel with the selected feedback
            var editFeedbackViewModel = new EditFeedbackViewModel(feedback);

            // Create and show the Edit Feedback window
            var editWindow = new EditFeedback
            {
                DataContext = editFeedbackViewModel // Bind the ViewModel to the View
            };

            // Set the CloseAction to close the window
            editFeedbackViewModel.CloseAction = () => editWindow.Close();

            editWindow.ShowDialog();

            // Refresh feedback data after editing
            LoadFeedbackData();
        }

        private void ExecuteDelete(Feedback feedback)
        {
            if (MessageBox.Show("Are you sure you want to delete this feedback?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM feedbacks WHERE FeedbackId = @FeedbackId";
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FeedbackId", feedback.FeedbackId);
                            command.ExecuteNonQuery();
                        }
                    }
                    FeedbackList.Remove(feedback);
                    TotalFeedback--;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting feedback: {ex.Message}");
                }
            }
        }
        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute((T)parameter);
            }

            public void Execute(object parameter)
            {
                _execute((T)parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}
