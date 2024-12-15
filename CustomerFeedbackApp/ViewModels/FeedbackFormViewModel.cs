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
            string feedbackMessage = $"Name: {CustomerName}\n" +
                                      $"Email: {CustomerEmail}\n" +
                                      $"Product/Service: {SelectedProduct}\n" +
                                      $"Comment: {CustomerComment}";

            // Here you would save to a database or call a service
            MessageBox.Show($"Feedback Submitted:\n{feedbackMessage}", "Feedback Confirmation");

            // Close the window after submission
            _view.CloseWindow();
        }
    }
}
