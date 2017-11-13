using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware;
namespace GruntyOS
{
    class Clock
    {
        public static string GetDateTimeString()
        {
            string date = "";

            date = RTC.Month.ToString() + "/" + RTC.DayOfTheMonth.ToString() + "/" + RTC.Year.ToString() + "   " + GetTimeString();
            return date;
        }
        public static string GetTimeString()
        {
            string time = RTC.Hour.ToString() + ":";
            if (RTC.Minute > 9)
            {
                time += RTC.Minute.ToString();
            }
            else
            {
                time += "0" + RTC.Minute.ToString();
            }
            return time;
        }
    }
}
