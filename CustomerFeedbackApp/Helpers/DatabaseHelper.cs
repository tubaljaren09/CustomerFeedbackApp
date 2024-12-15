using CustomerFeedbackApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomerFeedbackApp.Helpers
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString = "Server=localhost;Database=customerfeedbackapp;User=root;Password=";

        public static void SaveFeedback(Feedback feedback)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Feedbacks (CustomerName, CustomerEmail, Product, Comment)
                                     VALUES (@CustomerName, @CustomerEmail, @Product, @Comment)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerName", feedback.CustomerName);
                        command.Parameters.AddWithValue("@CustomerEmail", string.IsNullOrEmpty(feedback.CustomerEmail) ? DBNull.Value : feedback.CustomerEmail);
                        command.Parameters.AddWithValue("@Product", feedback.Product);
                        command.Parameters.AddWithValue("@Comment", feedback.Comment);

                        command.ExecuteNonQuery();
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("Error saving feedback to the database.", ex);
            //}
            catch (MySqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}\n{ex.StackTrace}", "Database Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected Error: {ex.Message}\n{ex.StackTrace}", "Error");
            }

        }
    }
}
