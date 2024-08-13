//namespace MyAppWeb.Data
//{
//    public class SheetRepository
//    {
//    }
//}

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // Make sure to use Microsoft.Data.SqlClient
using Microsoft.Extensions.Configuration;
using MyAppWeb.ViewModels;
using System.Globalization;


namespace MyAppWeb.Data
{
    public class SheetRepository
    {
        private readonly string _connectionString;

        public SheetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public List<SheetViewModel> GetSheets()
        {
            var sheets = new List<SheetViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT EmpID, Name,Time, AttendanceState, DeviceName FROM Sheet", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timeValue = reader.GetString(reader.GetOrdinal("Time"));
                        var dateTimeParts = timeValue.Split(' ');

                        var timePart = dateTimeParts[1];
                        var attendanceState = reader.GetBoolean(reader.GetOrdinal("AttendanceState"));

                        TimeSpan timeIn = TimeSpan.Zero;
                        TimeSpan timeOut = TimeSpan.Zero;

                        //var timeIn = "-";
                        //var timeOut = "-";

                        // Convert timePart to TimeSpan
                        TimeSpan timeSpan;
                        if (TimeSpan.TryParse(timePart, out timeSpan))
                        {
                            if (attendanceState)                // Assuming true is for check-out
                            {
                                timeOut = timeSpan;
                            }
                            else                                // Assuming false is for check-in
                            {
                                timeIn = timeSpan;
                            }
                        }

                        sheets.Add(new SheetViewModel
                        {
                            EmpID = reader.GetInt32(reader.GetOrdinal("EmpID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            //Time = reader.GetString(reader.GetOrdinal("Time")),
                            //Time = reader.GetDateTime(reader.GetOrdinal("Time")),
                            //Date = DateTime.ParseExact(dateTimeParts[0], "dd/MM/yyyy", CultureInfo.InvariantCulture), // Extract and parse date
                            Date = dateTimeParts[0],
                            //Time = TimeSpan.Parse(dateTimeParts[1]), // Extract and parse time
                            TimeIn = timeIn,
                            TimeOut = timeOut,
                            //WorkCode = reader.GetString(reader.GetOrdinal("WorkCode")),
                            AttendanceState = reader.GetBoolean(reader.GetOrdinal("AttendanceState")),
                            //DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"))
                        });
                    }
                }
            }

            return sheets;
        }



        public List<SheetViewModel> GetAttendanceRecordsByEmpId(int empId)
        {
            var attendanceRecords = new List<SheetViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT EmpID, Name,Time, AttendanceState FROM Sheet WHERE EmpID = @EmpID", connection);
                command.Parameters.AddWithValue("@EmpID", empId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timeValue = reader.GetString(reader.GetOrdinal("Time"));
                        var dateTimeParts = timeValue.Split(' ');

                        var timePart = dateTimeParts[1];
                        var attendanceState = reader.GetBoolean(reader.GetOrdinal("AttendanceState"));

                        TimeSpan timeIn = TimeSpan.Zero;
                        TimeSpan timeOut = TimeSpan.Zero;

                        //var timeIn = "-";
                        //var timeOut = "-";

                        // Convert timePart to TimeSpan
                        TimeSpan timeSpan;
                        if (TimeSpan.TryParse(timePart, out timeSpan))
                        {
                            if (attendanceState)                // Assuming true is for check-out
                            {
                                timeOut = timeSpan;
                            }
                            else                                // Assuming false is for check-in
                            {
                                timeIn = timeSpan;
                            }
                        }

                        attendanceRecords.Add(new SheetViewModel
                        {
                            EmpID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            //Time = reader.GetString(reader.GetOrdinal("Time")),
                            //Time = reader.GetDateTime(reader.GetOrdinal("Time")),
                            /* Date = DateTime.ParseExact(dateTimeParts[0], "MM,dd,yyyy", CultureInfo.InvariantCulture),*/ // Extract and parse date
                            Date = dateTimeParts[0],
                            //Time = TimeSpan.Parse(dateTimeParts[1]), // Extract and parse time
                            TimeIn = timeIn,
                            TimeOut = timeOut,
                            //WorkCode = reader.GetString(3),
                            AttendanceState = reader.GetBoolean(reader.GetOrdinal("AttendanceState")),
                            //DeviceName = reader.GetString(5)
                        });
                    }
                }
            }

            return attendanceRecords;
        }




        public Dictionary<int, Dictionary<string, TimeSpan>> CalculateHoursWorked(IEnumerable<SheetViewModel> sheets)
        {
            var hoursWorked = new Dictionary<int, Dictionary<string, TimeSpan>>();

            // Group by employee and date
            var groupedSheets = sheets
                .GroupBy(s => new { s.EmpID, s.Date })
                .ToList();

            foreach (var group in groupedSheets)
            {
                int empId = group.Key.EmpID;
                string date = group.Key.Date;
                var empHours = new Dictionary<string, TimeSpan>();

                TimeSpan firstTimeIn = TimeSpan.Zero;
                TimeSpan lastTimeOut = TimeSpan.Zero;

                foreach (var sheet in group)
                {
                    if (sheet.TimeIn != TimeSpan.Zero)
                    {
                        if (firstTimeIn == TimeSpan.Zero || sheet.TimeIn < firstTimeIn)
                        {
                            firstTimeIn = sheet.TimeIn;
                        }
                    }

                    if (sheet.TimeOut != TimeSpan.Zero)
                    {
                        lastTimeOut = sheet.TimeOut;
                    }
                }

                if (firstTimeIn != TimeSpan.Zero && lastTimeOut != TimeSpan.Zero)
                {
                    empHours[date] = lastTimeOut - firstTimeIn;
                }

                if (!hoursWorked.ContainsKey(empId))
                {
                    hoursWorked[empId] = new Dictionary<string, TimeSpan>();
                }

                hoursWorked[empId][date] = empHours.ContainsKey(date) ? empHours[date] : TimeSpan.Zero;
            }

            return hoursWorked;
        }


        //public Dictionary<int, Dictionary<string, TimeSpan>> CalculateHoursSpent(IEnumerable<SheetViewModel> sheets)
        //{
        //    var hoursSpent = new Dictionary<int, Dictionary<string, TimeSpan>>();

        //    foreach (var sheet in sheets)
        //    {
        //        if (!hoursSpent.ContainsKey(sheet.EmpID))
        //        {
        //            hoursSpent[sheet.EmpID] = new Dictionary<string, TimeSpan>();
        //        }
        //        //Console.WriteLine("MMomin -> ", hoursSpent);
        //        var empHours = hoursSpent[sheet.EmpID];
        //        var dateKey = sheet.Date;

        //        if (!empHours.ContainsKey(dateKey))
        //        {
        //            empHours[dateKey] = TimeSpan.Zero;
        //        }

        //        var currentDateHours = empHours[dateKey];
        //        if (sheet.TimeIn != TimeSpan.Zero && sheet.TimeOut != TimeSpan.Zero)
        //        {
        //            var hoursForDay = sheet.TimeOut - sheet.TimeIn;
        //            empHours[dateKey] += hoursForDay;
        //        }
        //    }
        //    //Console.WriteLine("Momin", hoursSpent);
        //    return hoursSpent;

        //}


        //public IEnumerable<SheetViewModel> GetAttendanceRecordsByEmpId(int empId)
        //{
        //    var attendanceRecords = new List<SheetViewModel>();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var command = new SqlCommand("SELECT EmpID, Name,Time, AttendanceState FROM Sheet WHERE EmpID = @EmpID", connection);
        //        command.Parameters.AddWithValue("@EmpID", empId);
        //        connection.Open();

        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var timeValue = reader.GetString(reader.GetOrdinal("Time"));
        //                var dateTimeParts = timeValue.Split(' ');
        //                //Console.WriteLine(dateTimeParts[0]);

        //                attendanceRecords.Add(new SheetViewModel
        //                {
        //                    EmpID = reader.GetInt32(0),
        //                    Name = reader.GetString(1),
        //                    //Time = reader.GetString(reader.GetOrdinal("Time")),
        //                    //Time = reader.GetDateTime(reader.GetOrdinal("Time")),
        //                    /* Date = DateTime.ParseExact(dateTimeParts[0], "MM,dd,yyyy", CultureInfo.InvariantCulture),*/ // Extract and parse date
        //                    Date= dateTimeParts[0],
        //                    Time = TimeSpan.Parse(dateTimeParts[1]), // Extract and parse time
        //                    //WorkCode = reader.GetString(3),
        //                    AttendanceState = reader.GetBoolean(reader.GetOrdinal("AttendanceState")),
        //                    //DeviceName = reader.GetString(5)
        //                }) ;
        //            }
        //        }
        //    }

        //    return attendanceRecords;
        //}


    }
}

