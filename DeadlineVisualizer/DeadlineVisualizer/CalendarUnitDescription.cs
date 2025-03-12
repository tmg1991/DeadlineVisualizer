namespace DeadlineVisualizer
{
    public class CalendarUnitDescription
    {
        public string Name { get; private set; }
        public CalendarUnits Unit { get; private set; }

        public CalendarUnitDescription(CalendarUnits calendarUnit)
        {
            Unit = calendarUnit;
            switch (Unit)
            {
                case CalendarUnits.CalendarWeeks:
                    Name = Resources.AppRes.CalendarWeeksString;
                    break;
                case CalendarUnits.Days:
                    Name = Resources.AppRes.DaysString;
                    break;
                default:
                    throw new ArgumentException($"There is no locale for {Unit}");
            }
        }
    }
}
