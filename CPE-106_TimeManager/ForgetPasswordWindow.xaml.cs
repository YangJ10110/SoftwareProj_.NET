using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPE_106_TimeManager
{
    /// <summary>
    /// Interaction logic for ForgetPasswordWindow.xaml
    /// </summary>
    public partial class ForgetPasswordWindow : Window
    {
        private string connectionString = "Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True";

        public ForgetPasswordWindow()
        {
            InitializeComponent();
        }

        private void findAccBTN_Click(object sender, RoutedEventArgs e)
        {
            string username = findUsername.Text;
            string backupCode1 = findBackupCode.Text;

            if (backupCode1.Length != 4)
            {
                MessageBox.Show("Backup code must be exactly four characters long.");
                return;
            }

            // Check if the user exists in the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM user_info WHERE username = @username AND resetCode = @resetCode", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@resetCode", backupCode1);
                connection.Open();

                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    MessageBox.Show("No user found with the given username and backup code.");
                    return;
                }
            }

            // Show the password change field
            changePasswordField.Visibility = Visibility.Visible;
        }


        private void CHANGE_PASSWORD_clicked(object sender, RoutedEventArgs e)
        {
            string username = findUsername.Text;
        string password = newPass.Text;
        string confirmPassword = confNewPass.Text;

        if (password != confirmPassword)
        {
            MessageBox.Show("Passwords do not match.");
            return;
        }

        // Update the user's password in the database
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand("UPDATE user_info SET password = @password WHERE username = @username", connection);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@username", username);
            connection.Open();

            int result = command.ExecuteNonQuery();

            if (result > 0)
            {
                MessageBox.Show("Password updated successfully.");
            }
            else
            {
                MessageBox.Show("Error updating password. Please try again.");
            }
        }
        }
    }
}
