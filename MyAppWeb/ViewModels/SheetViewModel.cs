namespace MyAppWeb.ViewModels
{
    public class SheetViewModel
    {
        public int EmpID { get; set; }          // Corresponds to Emp ID
        public string Name { get; set; }        // Corresponds to Name
        //public string Time { get; set; }      // Corresponds to Time
        //public DateTime Time { get; set; }

        public string Date { get; set; }      // Extracted Date
        public TimeSpan Time { get; set; }      // Extracted Time

        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }

        public string? WorkCode { get; set; }    // Corresponds to Work Code
        public bool AttendanceState { get; set; } // Corresponds to Attendance State
        public string DeviceName { get; set; }  // Corresponds to Device Name
    }
}
