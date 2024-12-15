using CustomerFeedbackApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomerFeedbackApp.ViewModels.MainViewModel;
using System.Windows.Input;
using System.Windows;
using CustomerFeedbackApp.Helpers;
using CustomerFeedbackApp.Models;

namespace CustomerFeedbackApp.ViewModels
{
    public class FeedbackFormViewModel
    {
        private readonly FeedbackForm _view;

        // Properties bound to the UI
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string SelectedProduct { get; set; }
        public string CustomerComment { get; set; }

        // List of products/services
        public ObservableCollection<string> Products { get; set; }

        public ICommand SubmitFeedbackCommand { get; set; }

        public FeedbackFormViewModel(FeedbackForm view)
        {
            _view = view;

            // Sample list of products
            Products = new ObservableCollection<string>
            {
                "Product 1",
                "Service A",
                "Product 2"
            };

            SubmitFeedbackCommand = new RelayCommand(SubmitFeedback);
        }

        private void SubmitFeedback(object parameter)
        {
            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerComment))
            {
                MessageBox.Show("Name and comment are required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var feedback = new Feedback
            {
                CustomerName = CustomerName,
                CustomerEmail = CustomerEmail,
                Product = SelectedProduct,
                Comment = CustomerComment
            };

            try
            { 

                DatabaseHelper.SaveFeedback(feedback);

                MessageBox.Show("Feedback submitted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear fields after successful submission
                CustomerName = string.Empty;
                CustomerEmail = string.Empty;
                CustomerComment = string.Empty;
                SelectedProduct = null;

                _view.CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
