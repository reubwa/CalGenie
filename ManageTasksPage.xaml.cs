using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CalGenie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageTasksPage : Page
    {
        public ManageTasksPage()
        {
            InitializeComponent();
            LstEvents.ItemsSource = App.GlobalTasksList;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TimeSpan duration = new TimeSpan(hours:(int)NewTaskDurHours.Value,minutes:(int)NewTaskDurMinutes.Value,0);
            Task nt = new Task(){Duration = duration,Name = NewTaskName.Text};
            App.GlobalTasksList.Add(nt);
            AddFly.Hide();
            NewTaskDurHours.Value = 0;
            NewTaskDurMinutes.Value = 0;
            NewTaskName.Text = "";
            App.TaskAdded = true;
        }

        private void LstEvents_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstEvents.SelectedItem != null)
            {
                BtEdit.IsEnabled = true;
                BtRemove.IsEnabled = true;
            }
        }

        private void EditFly_OnOpened(object? sender, object e)
        {
            Task selectedTask = (Task)LstEvents.SelectedItem;
            EditTaskName.Text = selectedTask.Name;
            EditTaskDurHours.Value = selectedTask.Duration.Hours;
            EditTaskDurMinutes.Value = selectedTask.Duration.Minutes;
        }

        private void ButtonBaseEd_OnClick(object sender, RoutedEventArgs e)
        {
            Task selectedTask = (Task)LstEvents.SelectedItem;
            Task editedTask = new Task(){Name = EditTaskName.Text,Duration = new TimeSpan((int)EditTaskDurHours.Value,(int)EditTaskDurMinutes.Value,0)};
            foreach (var task in App.GlobalTasksList)
            {
                if (task == selectedTask)
                {
                    App.GlobalTasksList[App.GlobalTasksList.IndexOf(task)] = editedTask;
                    break;
                }
            }
            EditFly.Hide();
        }

        private void BtRemove_OnClick(object sender, RoutedEventArgs e)
        {
            Task selectedTask = (Task)LstEvents.SelectedItem;
            foreach (var task in App.GlobalTasksList)
            {
                if (task == selectedTask)
                {
                    App.GlobalTasksList.RemoveAt(App.GlobalTasksList.IndexOf(task));
                    break;
                }
            }

            if (App.GlobalTasksList.Count < 1)
            {
                App.TaskAdded = false;
            }
        }
    }
}
