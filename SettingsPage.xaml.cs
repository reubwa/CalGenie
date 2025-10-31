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
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CalGenie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("mostProductiveTimeStart"))
            {
                TmStart.SelectedTime = (TimeSpan)localSettings.Values["mostProductiveTimeStart"];
            }

            if (localSettings.Values.ContainsKey("mostProductiveTimeEnd"))
            {
                TmEnd.SelectedTime = (TimeSpan)localSettings.Values["mostProductiveTimeEnd"];
            }
        }

        private void TmStart_OnSelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["mostProductiveTimeStart"] = TmStart.SelectedTime;
        }

        private void TmEnd_OnSelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["mostProductiveTimeEnd"] = TmEnd.SelectedTime;
        }
    }
}
