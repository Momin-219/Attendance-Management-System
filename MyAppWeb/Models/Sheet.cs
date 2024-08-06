namespace MyAppWeb.Models
{
    public class Sheet
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string WorkCode { get; set; }
        public string AttendanceState { get; set; }
        public string DeviceName { get; set; }
    }
}
