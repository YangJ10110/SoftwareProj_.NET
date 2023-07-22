using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Data.SqlClient;
using static CPE_106_TimeManager.View.ToDoTrackerView;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace CPE_106_TimeManager.View
{
    /// <summary>
    /// Interaction logic for ToDoTrackerView.xaml
    /// </summary>
    public partial class ToDoTrackerView : UserControl
    {
        private string connectionString = "Data Source=jerome-laptop\\sqlexpress;Initial Catalog=timemanagerDB;Integrated Security=True";

        private ObservableCollection<TodoItem> todoItems = new ObservableCollection<TodoItem>();


        public class TodoItem
        {
            public bool IsComplete { get; set; }
            public string objectiveTitle { get; set; }
            public string Type { get; set; }
            public DateTime StartDate { get; set; }
            public string Priority { get; set; }
            public DateTime Deadline { get; set; }
        }



        public ToDoTrackerView()
        {
            InitializeComponent();

            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                                            SELECT o.objectiveTitle, ot.Type, tl.Startdate, pl.priorityLevelDescription, tl.Deadline, o.IsCompleteID
                                            FROM objective o
                                            INNER JOIN ObjectiveTypeTable ot ON o.ObjectiveID = ot.ObjectiveID
                                            INNER JOIN timelinetable tl ON o.ObjectiveID = tl.ObjectiveID
                                            INNER JOIN priorityLevel pl ON o.priorityID = pl.PriorityID
                                            WHERE o.UserID = (SELECT UserID FROM User_Info WHERE username = @Username)
                                            AND o.isCompleteID = 2";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TodoItem todoItem = new TodoItem();
                    todoItem.objectiveTitle = reader.GetString(0);
                    todoItem.Type = reader.GetString(1);
                    todoItem.StartDate = reader.GetDateTime(2);
                    todoItem.Priority = reader.GetString(3);
                    todoItem.Deadline = reader.GetDateTime(4);
                    todoItem.IsComplete = reader.GetByte(5) == 1 ? true : false;
                    todoItems.Add(todoItem);
                }
            }

            TodoListView.ItemsSource = todoItems;
        }


        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            todoItems.Clear();

            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                                SELECT o.objectiveTitle, ot.Type, tl.Startdate, pl.priorityLevelDescription, tl.Deadline, o.IsCompleteID
                                        FROM objective o
                                        INNER JOIN ObjectiveTypeTable ot ON o.ObjectiveID = ot.ObjectiveID
                                        INNER JOIN timelinetable tl ON o.ObjectiveID = tl.ObjectiveID
                                        INNER JOIN priorityLevel pl ON o.priorityID = pl.PriorityID
                                        WHERE o.UserID = (SELECT UserID FROM User_Info WHERE username = @Username)
                                        AND o.isCompleteID = 2";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TodoItem todoItem = new TodoItem();
                    todoItem.objectiveTitle = reader.GetString(0);
                    todoItem.Type = reader.GetString(1);
                    todoItem.StartDate = reader.GetDateTime(2);
                    todoItem.Priority = reader.GetString(3);
                    todoItem.Deadline = reader.GetDateTime(4);
                    todoItem.IsComplete = reader.GetByte(5) == 1 ? true : false;
                    todoItems.Add(todoItem);
                }
            }

            TodoListView.ItemsSource = null;
            TodoListView.ItemsSource = todoItems;
        }


        private void addToDo_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string query = "SELECT UserID FROM User_Info WHERE username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    object result = command.ExecuteScalar();
                    int userID = Convert.ToInt32(result);
                    int isCompleteID = 2;
                    int StatusID = 2;

                    query = "SELECT COUNT(*) FROM objective WHERE ObjectiveTitle = @ObjectiveTitle AND UserID = @UserID;";
                    using (SqlCommand comman = new SqlCommand(query, connection))
                    {
                        comman.Parameters.AddWithValue("@ObjectiveTitle", todoName.Text);
                        comman.Parameters.AddWithValue("@UserID", userID);
                        int count = (int)comman.ExecuteScalar();
                        if (count > 0)
                        {
                            // To-do already exists in the database, do not insert it again
                            MessageBox.Show("To-do already exists!");
                            return;
                        }

                    }

                        // Insert the new objective into the database
                        query = "INSERT INTO objective (ObjectiveTitle, PriorityID, isCompleteID, UserID, StatusID) " +
                                    "VALUES (@ObjectiveTitle, @PriorityID, @isCompleteID, @UserID, @StatusID);";

                    using (SqlCommand insertCommand = new SqlCommand(query, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ObjectiveTitle", todoName.Text);
                        insertCommand.Parameters.AddWithValue("@PriorityID", GetPriorityID());
                        insertCommand.Parameters.AddWithValue("@isCompleteID", isCompleteID);
                        insertCommand.Parameters.AddWithValue("@UserID", userID);
                        insertCommand.Parameters.AddWithValue("@StatusID", StatusID);



                        result = insertCommand.ExecuteScalar();
                        int objectiveID = (result == null) ? -1 : (int)result;

                        
                        // Insert the timeline into the database
                        

                        MessageBox.Show("New objective added with ID " + objectiveID);
                    }

                    

                    

                }
            }

        }

        private int GetPriorityID()
        {
            if (HighButton.IsChecked == true)
            {
                return 1;
            }
            else if (MediumButton.IsChecked == true)
            {
                return 2;
            }
            else
            {
                MessageBox.Show("No checbox was set: Priority automatically set as LOW");
                return 3;
            }
        }

       

        private void InsertTimeline(int objectiveId, DateTime startDate, DateTime deadlineDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                // Create a SQL command to insert the timeline
                SqlCommand command = new SqlCommand(
                    "INSERT INTO timelinetable (ObjectiveID, StartDate, Deadline) VALUES (@ObjectiveId, @StartDate, @Deadline);", connection);

                // Set the parameter values
                command.Parameters.AddWithValue("@ObjectiveId", objectiveId);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@Deadline", deadlineDate);

                // Execute the command
                command.ExecuteNonQuery();

                // Close the database connection
                connection.Close();
            }
        }

        private void addDates_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            // Get the ObjectiveID for the objective title entered by the user
            int objectiveId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ObjectiveID FROM objective WHERE objectiveTitle = @ObjectiveTitle AND UserID = (SELECT UserID FROM User_Info WHERE username = @Username)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ObjectiveTitle", todoName.Text);
                command.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        objectiveId = reader.GetInt16(0);
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM timelinetable WHERE ObjectiveID = @ObjectiveId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ObjectiveId", objectiveId);

                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("There's already a timeline for this objective.");
                    return;
                }
            }

            // Insert the timeline into the timelinetable
            InsertTimeline(objectiveId, StartDate.SelectedDate.Value, DeadlineDate.SelectedDate.Value);
            MessageBox.Show("Dates Successfully Added");
        }

        private string GetObjectiveType()
        {
            if (TaskButton.IsChecked == true)
            {
                return "Task";
            }
            else
            {
                return "Goal";
            }
        }

        private void AddObjectiveType_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            int objectiveId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ObjectiveID FROM objective WHERE objectiveTitle = @ObjectiveTitle AND UserID = (SELECT UserID FROM User_Info WHERE username = @Username)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ObjectiveTitle", todoName.Text);
                command.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        objectiveId = reader.GetInt16(0);
                    }
                }

            }
            // Check if the objective type already exists for this objective
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM ObjectiveTypeTable WHERE ObjectiveID = @ObjectiveID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ObjectiveID", objectiveId);

                int count = (int)command.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Objective type already exists for this objective");
                    return;
                }
            }

            // Insert the objective type into the ObjectiveTypeTable
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO ObjectiveTypeTable (ObjectiveID, Type) VALUES (@ObjectiveID, @Type)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ObjectiveID", objectiveId);
                command.Parameters.AddWithValue("@Type", GetObjectiveType());

                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Objective type added successfully");
                }
            }

        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string username = nameElement.Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (TodoItem item in todoItems)
                {
                    // Check if the IsComplete property has changed
                    if (item.IsComplete != false)
                    {
                        // Update the corresponding row in the database
                        string query = @"
                    UPDATE objective
                    SET IsCompleteID = 1
                    WHERE UserID = (SELECT UserID FROM User_Info WHERE username = @Username)
                    AND objectiveTitle = @ObjectiveTitle";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@ObjectiveTitle", item.objectiveTitle);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 1)
                        {
                            // Update the IsComplete property of the item
                            item.IsComplete = true;
                        }
                    }
                }

                Refresh_Click(sender, e);
            }

        }
    }
}
