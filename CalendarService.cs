using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;

namespace CalGenie
{
    public class CalendarEvent
    {
        public string Summary { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
    }
    public class CalendarService
    {
        private static readonly HttpClient client = new HttpClient();
        


        ///<summary>
        ///Fetches and parses a remotely-hosted .ics file and returns events in a specific day
        ///</summary>
        /// <param name="icsUrl">The public URL of the .ics file</param>
        /// <param name="targetDay">The desired day to return events for</param>
        /// <returns>A list of events in the specified day</returns>
        public async Task<List<CalendarEvent>> GetEventsForDayAsync(string icsUrl, DateTime targetDay)
        {
            var eventsList = new List<CalendarEvent>();
            try
            {
                string icsData = await client.GetStringAsync(icsUrl);
                var calendar = Calendar.Load(icsData);
                var searchStart = new CalDateTime(targetDay.Date);
                var searchEnd = new CalDateTime(targetDay.Date.AddDays(1));
                var occurances = calendar.GetOccurrences(searchStart);
                var boundedOcc = occurances.TakeWhile(occ => occ.Period.StartTime < searchEnd);
                occurances = boundedOcc.ToList();
                foreach (var occurence in occurances)
                {
                    var origEvent = occurence.Source as Ical.Net.CalendarComponents.CalendarEvent;
                    if (origEvent != null)
                    {
                        eventsList.Add(new CalendarEvent
                        {
                            StartTime = origEvent.DtStart.Value,
                            EndTime = origEvent.DtEnd.Value,
                            Summary = origEvent.Summary,
                            Description = origEvent.Description
                        });
                    }
                }

                return eventsList.OrderBy(e => e.StartTime).ToList();
            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {e.Message}");
                return eventsList;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {e.Message}");
                return eventsList;
            }
        }
    }
}
