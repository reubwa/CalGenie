using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CalGenie
{
    public class AllocationEngine
    {
        public static List<TimeOnly> GetMostProductiveTime()
        {
            List<TimeOnly> result = new List<TimeOnly>();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            TimeSpan timeStart = new TimeSpan();
            TimeSpan timeEnd = new TimeSpan();
            TimeOnly toS = new TimeOnly();
            TimeOnly toE = new TimeOnly();
            TimeOnly nu = new TimeOnly(0);
            bool hasStart = false;
            bool hasEnd = false;
            if (localSettings.Values.ContainsKey("mostProductiveTimeStart"))
            {
                timeStart = (TimeSpan)localSettings.Values["mostProductiveTimeStart"];
                toS = new TimeOnly(timeStart.Hours,timeStart.Minutes,0);
                hasStart = true;
            }

            if (localSettings.Values.ContainsKey("mostProductiveTimeEnd"))
            {
                timeEnd = (TimeSpan)localSettings.Values["mostProductiveTimeEnd"];
                toE = new TimeOnly(timeEnd.Hours, timeEnd.Minutes, 0);
                hasEnd = true;
            }
            if(hasStart&&hasEnd)
            {
                result.Add(toS);
                result.Add(toE);
            } else if (hasEnd && !hasStart)
            {
                result.Add(nu);
                result.Add(toE);
            }else if (hasStart && !hasEnd)
            {
                result.Add(toS);
                result.Add(nu);
            }
            else
            {
                result.Add(nu);
                result.Add(nu);
            }
            return result;
        }

        public static List<CalendarEvent> GenerateAllocation(ObservableCollection<CalendarEvent> Events, ObservableCollection<Task> Tasks)
        {
            List<CalendarEvent> generated = new List<CalendarEvent>();
            List<TimeOnly> mostProductiveTime = GetMostProductiveTime();
            TimeOnly mostProductiveTimeStart = new TimeOnly();
            TimeOnly mostProductiveTimeEnd = new TimeOnly();
            TimeOnly duff = new TimeOnly(0);
            foreach (var item in mostProductiveTime)
            {
                if (mostProductiveTime.IndexOf(item) == 0 && item != duff)
                {
                    mostProductiveTimeStart = item;
                    continue;
                }
                if (mostProductiveTime.IndexOf(item) == 1 && item != duff)
                {
                    mostProductiveTimeEnd = item;
                    continue;
                }

                if (mostProductiveTime.IndexOf(item) == 0 && item == duff)
                {
                    continue;
                }

                if (mostProductiveTime.IndexOf(item) == 1 && item == duff)
                {
                    CalendarEvent errorCalendarEvent = new CalendarEvent(){Summary = "Most productive time not set"};
                    List<CalendarEvent> errorList = new List<CalendarEvent>();
                    errorList.Add(errorCalendarEvent);
                    return errorList;
                }
            }

            foreach (var calEv in Events)
            {
                generated.Add(calEv);
            }

            DateOnly mptd = new DateOnly(Events[0].StartTime.Year, Events[0].StartTime.Month, Events[0].StartTime.Day);
            DateTime mpts = new DateTime(mptd,mostProductiveTimeStart);
            DateTime mpte = new DateTime(mptd, mostProductiveTimeEnd);
            var freetime = GetFreeTimeSlots(Events, mpts, mpte);
        OOLCONTROL:
            foreach (var slot in freetime)
            {
            OUPCONTROL:
                foreach (var calTask in Tasks)
                {
                    if (slot.Duration >= calTask.Duration)
                    {
                        CalendarEvent calEvent = new CalendarEvent()
                        {
                            Summary = calTask.Name,
                            StartTime = slot.StartTime,
                            EndTime = slot.StartTime + calTask.Duration
                        };
                        generated.Add(calEvent);
                        slot.StartTime += calTask.Duration;
                        Tasks.Remove(calTask);
                        goto OUPCONTROL;
                    }
                    if (slot.Duration < calTask.Duration) { goto OLCONTROL; }

                    if (slot.Duration == calTask.Duration)
                    {
                        CalendarEvent calEvent = new CalendarEvent()
                        {
                            Summary = calTask.Name,
                            StartTime = slot.StartTime,
                            EndTime = slot.StartTime + calTask.Duration
                        };
                        generated.Add(calEvent);
                        Tasks.Remove(calTask);
                        freetime.Remove(slot);
                        goto OOLCONTROL;
                    }
                }
            OLCONTROL:
                continue;
            }
            return generated;
        }
        /// <summary>
        /// Finds all free time slots (gaps) between events within a given time range.
        /// This method automatically handles and merges overlapping events.
        /// </summary>
        /// <param name="events">The list of calendar events.</param>
        /// <param name="searchStart">The start of the period to check for gaps.</param>
        /// <param name="searchEnd">The end of the period to check for gaps.</param>
        /// <returns>A list of FreeTimeSlot objects representing the gaps.</returns>
        public static List<FreeTimeSlot> GetFreeTimeSlots(ObservableCollection<CalendarEvent> events, DateTime searchStart, DateTime searchEnd)
        {
            var gaps = new List<FreeTimeSlot>();
            if (events == null || events.Count == 0)
            {
                // No events, the whole period is a gap
                gaps.Add(new FreeTimeSlot { StartTime = searchStart, EndTime = searchEnd });
                return gaps;
            }

            // 1. Sort events by start time
            var sortedEvents = events.OrderBy(e => e.StartTime).ToList();

            // 2. Merge overlapping events into "busy blocks"
            var busyBlocks = new List<CalendarEvent>();
            if (sortedEvents.Count > 0)
            {
                // Start with the first event
                var currentBlock = new CalendarEvent
                {
                    StartTime = sortedEvents[0].StartTime,
                    EndTime = sortedEvents[0].EndTime
                };

                foreach (var ev in sortedEvents.Skip(1))
                {
                    if (ev.StartTime < currentBlock.EndTime)
                    {
                        // Overlap detected: extend the current block if this event ends later
                        if (ev.EndTime > currentBlock.EndTime)
                        {
                            currentBlock.EndTime = ev.EndTime;
                        }
                    }
                    else
                    {
                        // No overlap: add the completed block and start a new one
                        busyBlocks.Add(currentBlock);
                        currentBlock = new CalendarEvent { StartTime = ev.StartTime, EndTime = ev.EndTime };
                    }
                }
                // Add the last block
                busyBlocks.Add(currentBlock);
            }

            // 3. Find gaps between the search boundaries and the busy blocks
            var currentPointer = searchStart;

            foreach (var block in busyBlocks)
            {
                // If there's time between the pointer and the start of the block
                if (block.StartTime > currentPointer)
                {
                    gaps.Add(new FreeTimeSlot { StartTime = currentPointer, EndTime = block.StartTime });
                }
                // Move the pointer to the end of the current busy block
                currentPointer = block.EndTime;
            }

            // 4. Check for a final gap after the last event
            if (searchEnd > currentPointer)
            {
                gaps.Add(new FreeTimeSlot { StartTime = currentPointer, EndTime = searchEnd });
            }

            return gaps;
        }
    }
    public class FreeTimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;

        public override string ToString()
        {
            return $"Free from {StartTime:HH:mm} to {EndTime:HH:mm} ({Duration.TotalMinutes} mins)";
        }
    }
}
