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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CPE_106_TimeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateUserName(string newUserName)
        {
            // C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml
            // Load the XML file
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");

            // Find the "Name" element and update its value
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            nameElement.Value = newUserName;

            // Save the changes to the XML file
            xmlDoc.Save(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
        }

        private void signIN_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM user_info WHERE username=@username AND password=@password", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    MessageBox.Show("User not found.");
                }
                else
                {
                    MainProgram theApp = new MainProgram();
                    
                    
                    theApp.Show();
                    Close();

                    UpdateUserName(txtUsername.Text);
                    
                }
            }
        }


        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow signup = new CreateAccountWindow();
            signup.Show(); 
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgetPasswordWindow changepass = new ForgetPasswordWindow();
            changepass.Show();
        }
    }
}
