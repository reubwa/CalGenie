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
    public sealed partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            InitializeComponent();
            LstEvents.ItemsSource = App.GlobalGeneratedEvents;
        }

        private async void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!App.TaskAdded && !App.CalendarAdded)
            {
                ContentDialog noo = new ContentDialog();

                noo.XamlRoot = XamlRoot;
                noo.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                noo.Title = "Oh dear!";
                noo.PrimaryButtonText = "OK";
                noo.Content = new ContentDialogContent();
                var result = await noo.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Frame.Navigate(typeof(ImportCalPage));
                }
            }
            else
            {
                App.GlobalGeneratedEvents.Clear();
                var generated = AllocationEngine.GenerateAllocation(App.GlobalEventsList, App.GlobalTasksList);
                CalendarEvent error = new CalendarEvent() { Summary = "Most productive time not set" };
                if (generated[0] == error)
                {
                    ContentDialog noo = new ContentDialog();

                    noo.XamlRoot = XamlRoot;
                    noo.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    noo.Title = "Oh dear!";
                    noo.PrimaryButtonText = "OK";
                    noo.Content = new MPTNotSet();
                    var result = await noo.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        Frame.Navigate(typeof(SettingsPage));
                    }
                }
                generated = generated.OrderBy(e => e.StartTime).ToList();
                foreach (var item in generated)
                {
                    App.GlobalGeneratedEvents.Add(item);
                }
            }
        }
    }
}
