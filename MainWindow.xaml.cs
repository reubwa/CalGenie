using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CalGenie
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the initial selected item using its x:Name
            nvSample.SelectedItem = PgImportCal;
            contentFrame.Navigate(typeof(ImportCalPage));
        }

        private void NvSample_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var invokedItem = args.InvokedItemContainer;
                if (invokedItem != null)
                {
                    switch (invokedItem.Tag)
                    {
                        case "importCal":
                            contentFrame.Navigate(typeof(ImportCalPage));
                            break;
                        case "addTasks":
                            contentFrame.Navigate(typeof(ManageTasksPage));
                            break;
                        case "seeResults":
                            contentFrame.Navigate(typeof(ResultsPage));
                            break;
                    }
                }
            }
        }

        private void ContentFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(ImportCalPage))
            {
                nvSample.SelectedItem = PgImportCal;
            }
        }
    }
}
