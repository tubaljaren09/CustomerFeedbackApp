using CustomerFeedbackApp.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomerFeedbackApp.ViewModels.MainViewModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CustomerFeedbackApp.Views;

namespace CustomerFeedbackApp.ViewModels
{
    public class AdminLoginViewModel : ViewModelBase
    {
        private string _username;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public AdminLoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object parameter)
        {
            var currentWindow = parameter as Window; // The login window reference

            try
            {
                using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT PasswordHash FROM Admins WHERE Username = @Username";
                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Username", Username);

                    var dbPasswordHash = cmd.ExecuteScalar()?.ToString();

                    if (dbPasswordHash == null)
                    {
                        MessageBox.Show("No user found in the database.");
                        return;
                    }

                    if (dbPasswordHash == ComputeHash(Password))
                    {
                        NavigateToAdminDashboard();
                        currentWindow?.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}");
            }
        }

        private void NavigateToAdminDashboard()
        {
            // Create an instance of the AdminDashboard window
            var adminDashboard = new AdminDashboard();
            adminDashboard.Show(); // Show the window

            // Close the current login window (optional, depending on your flow)
            Application.Current.MainWindow.Close();

            //Application.Current.MainWindow = adminDashboard;
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
