using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Windows.Shapes;

namespace CPE_106_TimeManager
{
    /// <summary>
    /// Interaction logic for CreateAccountWindow.xaml
    /// </summary>
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        private void SignUpBTN_Click(object sender, RoutedEventArgs e)
        {
            string username = createUsername.Text;
            string password = createPassword.Text;
            string confirmPassword = createConfmPassword.Text;
            string backUpCode1 = backupCode.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(backUpCode1))
            {
                MessageBox.Show("All fields are required to be filled.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords don't match.");
                return;
            }

            if (backUpCode1.Length != 4)
            {
                MessageBox.Show("Backup code must be exactly 4 characters long.");
                return;
            }

            using (SqlConnection con = new SqlConnection("Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True"))
            {
                con.Open();

                // Check if username already exists
                using (SqlCommand checkExistingUser = new SqlCommand("SELECT COUNT(*) FROM user_info WHERE username = @username", con))
                {
                    checkExistingUser.Parameters.AddWithValue("@username", username);
                    int existingUserCount = (int)checkExistingUser.ExecuteScalar();

                    if (existingUserCount > 0)
                    {
                        MessageBox.Show("A user with that username already exists.");
                        return;
                    }
                }

                // Insert new user
                using (SqlCommand addNewUser = new SqlCommand("INSERT INTO user_info (username, password, resetCode) VALUES (@username, @password, @resetCode)", con))
                {
                    addNewUser.Parameters.AddWithValue("@username", username);
                    addNewUser.Parameters.AddWithValue("@password", password);
                    addNewUser.Parameters.AddWithValue("@resetCode", backUpCode1);

                    int result = addNewUser.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Account successfully created!");
                    }
                    else
                    {
                        MessageBox.Show("Error creating account. Please try again.");
                    }
                }
            }
        }



    }
}
