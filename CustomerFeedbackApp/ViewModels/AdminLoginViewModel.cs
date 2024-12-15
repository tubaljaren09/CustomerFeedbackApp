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
            MessageBox.Show($"Username: {Username}, Password: {Password}");

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

                    MessageBox.Show($"Retrieved hash from database: {dbPasswordHash}");

                    if (dbPasswordHash == ComputeHash(Password))
                    {
                        MessageBox.Show("Login successful!");
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
            // Navigate to Admin Dashboard View
            MessageBox.Show("Login successful!", "Success");
            // Implement navigation logic here
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
