using System;
using XLua;

namespace Gankx
{
    public class DateTimeService : Singleton<DateTimeService>
    {
        private DateTime myDateTime;

        public void SetDateTime(int year, int month, int day, int hour, int min, int sec)
        {
            myDateTime = new DateTime(year, month, day, hour, min, sec);
        }

        public void GetStructedTime(LuaTable timeTable, int secondGap)
        {
            if (null == timeTable)
            {
                return;
            }

            var time = myDateTime.AddSeconds(secondGap);
            timeTable.SetInPath("year", time.Year);
            timeTable.SetInPath("month", time.Month);
            timeTable.SetInPath("day", time.Day);
            var dayOfWeek = (int) time.DayOfWeek;
            timeTable.SetInPath("wday", dayOfWeek);
            timeTable.SetInPath("hour", time.Hour);
            timeTable.SetInPath("min", time.Minute);
            timeTable.SetInPath("sec", time.Second);

            timeTable.Dispose();
        }
    }
}