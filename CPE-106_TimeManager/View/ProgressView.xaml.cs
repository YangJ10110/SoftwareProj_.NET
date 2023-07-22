using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CPE_106_TimeManager.View
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView : UserControl
    {
          private string connectionString = "Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True";
        public class ObjectiveViewModel
        {
            public int completion { get; set; }

            public string Progress { get; set; }
            public string ToDo { get; set; }
            public string Type { get; set; }
            public DateTime StartDate { get; set; }
            public string Priority { get; set; }
            public DateTime Deadline { get; set; }
            
        }
        public ProgressView()
        {
            
            InitializeComponent();
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            List<ObjectiveViewModel> objectives = new List<ObjectiveViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT objectiveTitle, Type, Startdate, priorityLevelDescription, Deadline, isCompleteID
                         FROM objective
                         INNER JOIN User_Info ON objective.UserID = User_Info.UserID
                         INNER JOIN ObjectiveTypeTable ON objective.ObjectiveID = ObjectiveTypeTable.ObjectiveID
                         INNER JOIN timelinetable ON objective.ObjectiveID = timelinetable.ObjectiveID
                         INNER JOIN priorityLevel ON objective.priorityID = priorityLevel.PriorityID
                         WHERE User_Info.username = @Username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ObjectiveViewModel objective = new ObjectiveViewModel();
                        objective.ToDo = reader.GetString(0);
                        objective.Type = reader.GetString(1);
                        objective.StartDate = reader.GetDateTime(2);
                        objective.Priority = reader.GetString(3);
                        objective.Deadline = reader.GetDateTime(4);
                        objective.completion = reader.GetByte(5);

                        if (reader.GetByte(5) == 1)
                        {
                            objective.Progress = "Accomplished";
                        }
                        else if (reader.GetByte(5) == 2)
                        {
                            objective.Progress = "Not Done";
                        }

                        objectives.Add(objective);
                    }
                }
            }

            ProgressListView.ItemsSource = objectives;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            List<ObjectiveViewModel> objectives = new List<ObjectiveViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT objectiveTitle, Type, Startdate, priorityLevelDescription, Deadline, isCompleteID
                         FROM objective
                         INNER JOIN User_Info ON objective.UserID = User_Info.UserID
                         INNER JOIN ObjectiveTypeTable ON objective.ObjectiveID = ObjectiveTypeTable.ObjectiveID
                         INNER JOIN timelinetable ON objective.ObjectiveID = timelinetable.ObjectiveID
                         INNER JOIN priorityLevel ON objective.priorityID = priorityLevel.PriorityID
                         WHERE User_Info.username = @Username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ObjectiveViewModel objective = new ObjectiveViewModel();
                        objective.ToDo = reader.GetString(0);
                        objective.Type = reader.GetString(1);
                        objective.StartDate = reader.GetDateTime(2);
                        objective.Priority = reader.GetString(3);
                        objective.Deadline = reader.GetDateTime(4);
                        objective.completion = reader.GetByte(5);

                        if (reader.GetByte(5) == 1)
                        {
                            objective.Progress = "Accomplished";
                        }
                        else if (reader.GetByte(5) == 2)
                        {
                            objective.Progress = "Not Done";
                        }

                        objectives.Add(objective);
                    }
                    ProgressListView.ItemsSource = null;
                    ProgressListView.ItemsSource = objectives;
                }
            }

        }
    }
}
