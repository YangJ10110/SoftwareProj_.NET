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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CPE_106_TimeManager.View
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl
    {
        private string connectionString = "Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True";

        public class ScheduleItem
        {
            public DateTime Date { get; set; }
            public string ToDo { get; set; }
            public string Type { get; set; }
            public string Priority { get; set; }

            
        }

        public ScheduleView()
        {
            InitializeComponent();
        }

        private void DeadlineView_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            if (Date == null)
            {
                MessageBox.Show("Select a Date");
            }

            DateTime selectedDate = Date.SelectedDate.Value; // Retrieve selected date from DatePicker

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT o.objectiveTitle, ot.Type, tl.Deadline, pl.priorityLevelDescription
                                        FROM objective o
                                        INNER JOIN ObjectiveTypeTable ot ON o.ObjectiveID = ot.ObjectiveID
                                        INNER JOIN timelinetable tl ON o.ObjectiveID = tl.ObjectiveID
                                        INNER JOIN priorityLevel pl ON o.priorityID = pl.PriorityID
                                        WHERE o.UserID = (SELECT UserID FROM User_Info WHERE username = @Username) 
                                        AND tl.Deadline = @SelectedDate
                                        ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@SelectedDate", selectedDate);

                List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ScheduleItem item = new ScheduleItem();

                    item.ToDo = reader.GetString(0);
                    item.Type = reader.GetString(1);
                    item.Date = reader.GetDateTime(2);
                    item.Priority = reader.GetString(3);
                    scheduleItems.Add(item);
                }
                reader.Close();
                SchedView.ItemsSource = scheduleItems; // Bind the list of ScheduleItems to the ListView
            }
        }

        private void StartDateView_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            if(Date == null)
            {
                MessageBox.Show("Select a Date");
            }
            
                DateTime selectedDate = Date.SelectedDate.Value; // Retrieve selected date from DatePicker
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string query = @"SELECT o.objectiveTitle, ot.Type, tl.Startdate, pl.priorityLevelDescription
                                        FROM objective o
                                        INNER JOIN ObjectiveTypeTable ot ON o.ObjectiveID = ot.ObjectiveID
                                        INNER JOIN timelinetable tl ON o.ObjectiveID = tl.ObjectiveID
                                        INNER JOIN priorityLevel pl ON o.priorityID = pl.PriorityID
                                        WHERE o.UserID = (SELECT UserID FROM User_Info WHERE username = @Username) 
                                        AND tl.Startdate = @SelectedDate
                                        ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@SelectedDate", selectedDate);

                List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
                    
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ScheduleItem item = new ScheduleItem();
                    
                    item.ToDo = reader.GetString(0);
                    item.Type = reader.GetString(1);
                    item.Date = reader.GetDateTime(2);
                    item.Priority = reader.GetString(3);
                    scheduleItems.Add(item);    
                }
                reader.Close();
                SchedView.ItemsSource = scheduleItems; // Bind the list of ScheduleItems to the ListView
            }
        }

    }
}
