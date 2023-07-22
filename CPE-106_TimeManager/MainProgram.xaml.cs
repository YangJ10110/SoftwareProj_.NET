using CPE_106_TimeManager.View;
using CPE_106_TimeManager.ViewModel;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CPE_106_TimeManager
{
    /// <summary>
    /// Interaction logic for MainProgram.xaml
    /// </summary>
    public partial class MainProgram : Window
    {
        private void loadUserName(string displayUserName)
        {
            

        }

        public MainProgram()
        {
            
            InitializeComponent();

            RefreshUserName();
            
        }

        public void RefreshUserName()
        {
            // C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml
            // Load the XML file
            XDocument xmlDoc = XDocument.Load(@"C:\NEW CPE 106\CPE-106_TimeManager\CPE-106_TimeManager\username.xml");

            // Find the "Name" element and update its value
            XElement nameElement = xmlDoc.Element("Username").Element("Name");
            string displayUserName = nameElement.Value;

            username_text.Text = displayUserName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshUserName();
        }

        private void Username_BTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshUserName();
        }

        private void TodoTracker_btn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TodoTracker_btn_Click_1(object sender, RoutedEventArgs e)
        {
            ProgressView_View.Visibility = Visibility.Collapsed;
            SchedView_View.Visibility = Visibility.Collapsed;
            TodoTrackerView_View.Visibility = Visibility.Visible;          
        }

        private void ToDoTrackerView_IsHitTestVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Progress_btn_Click(object sender, RoutedEventArgs e)
        {
            TodoTrackerView_View.Visibility = Visibility.Collapsed;
            SchedView_View.Visibility = Visibility.Collapsed;
            ProgressView_View.Visibility = Visibility.Visible;
        }

        private void Schedule_btn_Click(object sender, RoutedEventArgs e)
        {
            TodoTrackerView_View.Visibility = Visibility.Collapsed;
            ProgressView_View.Visibility = Visibility.Collapsed;
            SchedView_View.Visibility = Visibility.Visible; 
        }
    }
}
