using Gankx;
using XLua;

public class DateTimeExport
{
    public static void SetDateTime(int year, int month, int day, int hour, int min, int sec)
    {
        DateTimeService.instance.SetDateTime(year, month, day, hour, min, sec);
    }

    public static void GetStructedTime(LuaTable dateTime, int secondGap)
    {
        DateTimeService.instance.GetStructedTime(dateTime, secondGap);
    }
}