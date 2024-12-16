using CustomerFeedbackApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using static CustomerFeedbackApp.ViewModels.AdminDashboardViewModel;
using CustomerFeedbackApp.Helpers;
using MySql.Data.MySqlClient;

namespace CustomerFeedbackApp.ViewModels
{
    public class EditFeedbackViewModel : ViewModelBase
    {
        private Feedback _feedback;

        public Feedback Feedback
        {
            get => _feedback;
            set => SetProperty(ref _feedback, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        // Constructor that takes a Feedback object
        public EditFeedbackViewModel(Feedback feedback)
        {
            // Use the provided Feedback object or initialize a new one
            Feedback = feedback ?? new Feedback();

            SaveCommand = new RelayCommand<Feedback>(ExecuteSave);
            CancelCommand = new RelayCommand<object>(ExecuteCancel);
        }

        private void ExecuteSave(Feedback feedback)
        {
            try
            {
                using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string query = @"
                UPDATE feedbacks 
                SET CustomerName = @CustomerName, 
                    CustomerEmail = @CustomerEmail, 
                    Product = @Product, 
                    Comment = @Comment 
                WHERE FeedbackId = @FeedbackId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FeedbackId", Feedback.FeedbackId);
                        command.Parameters.AddWithValue("@CustomerName", Feedback.CustomerName);
                        command.Parameters.AddWithValue("@CustomerEmail", Feedback.CustomerEmail);
                        command.Parameters.AddWithValue("@Product", Feedback.Product);
                        command.Parameters.AddWithValue("@Comment", Feedback.Comment);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Feedback updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the edit window
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving feedback: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object parameter)
        {
            CloseAction?.Invoke();
        }
    }
}
