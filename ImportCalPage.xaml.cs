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
    public sealed partial class ImportCalPage : Page
    {
        public ImportCalPage()
        {
            InitializeComponent();
            LstEvents.ItemsSource = App.GlobalEventsList;
            CalDate.Date = DateTimeOffset.Now;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtURL.Text == "")
            {
                TestButton1TeachingTip.IsOpen = true;
            }
            string calendarURL = TxtURL.Text;
            TimeOnly time = new TimeOnly(DateTime.Now.Hour,DateTime.Now.Minute);
            DateOnly date = new DateOnly(CalDate.Date.Value.Year,CalDate.Date.Value.Month,CalDate.Date.Value.Day);
            DateTime selectedDate = new DateTime(date,time);
            var calendarService = new CalendarService();
            List<CalendarEvent> eventsForToday = await calendarService.GetEventsForDayAsync(calendarURL, selectedDate);
            foreach (var ev in eventsForToday)
            {
                App.GlobalEventsList.Add(ev);
            }

            App.CalendarAdded = true;
        }
    }
}
