using Microsoft.AspNetCore.Mvc;
using MyAppWeb.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var file = Request.Form.Files[0];
            if (file != null && file.Length > 0)
            {
                // Set the license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

                var fileContent = new MemoryStream();
                await file.CopyToAsync(fileContent);

                using (var package = new ExcelPackage(fileContent))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    var data = new List<List<string>>();
                    for (int row = 1; row <= rowCount; row++)
                    {
                        var rowData = new List<string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            rowData.Add(worksheet.Cells[row, col].Text);
                        }
                        data.Add(rowData);
                    }

                    //ViewData["ExcelData"] = data;
                    TempData["ExcelData"] = JsonConvert.SerializeObject(data);
                    return View("DisplayData", data);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveToExcel([FromBody] List<List<string>> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < data[row].Count; col++)
                    {
                        worksheet.Cells[row + 1, col + 1].Value = data[row][col];
                    }
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "data.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        [HttpPost]
        public IActionResult ShowEmployees(string ExcelData)
        {
            //var data = ViewData["ExcelData"] as List<List<string>>;
            if (string.IsNullOrEmpty(ExcelData))
            {
                Console.WriteLine("Received ExcelData is empty or null.");
                return RedirectToAction("Index");
            }

            //Console.WriteLine($"Received ExcelData: {ExcelData}");

            try
            {
                //var data = JsonConvert.DeserializeObject<List<List<string>>>(ExcelData);
                //var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ExcelData);
                // Deserialize into List<List<string>> since the data is an array of arrays
                var data = JsonConvert.DeserializeObject<List<List<string>>>(ExcelData);



                // Extract headers (first row)
                var headers = data.FirstOrDefault();

                if (headers == null || headers.Count == 0)
                {
                    Console.WriteLine("Headers row is empty.");
                    return RedirectToAction("Index");
                }

                // Extract unique Employee IDs
                var uniqueEmployeeIds = data
                    .Skip(1) // Skip headers
                    .GroupBy(row => row[0]) // Assuming EmployeeId is the first column
                    .Select(group => new
                    {
                        EmployeeId = group.Key,
                        EmployeeName = group.FirstOrDefault()[1] // Assuming EmployeeName is the second column
                    })
                    .ToList();


                //// Extract unique Employee IDs
                //var uniqueEmployeeIds = data
                //    .GroupBy(entry => entry["EmployeeId"])
                //    .Select(group => group.First()) // Get the first entry in each group
                //    .Select(entry => new
                //    {
                //        EmployeeId = entry["EmployeeId"],
                //        EmployeeName = entry["EmployeeName"]
                //    })
                //    .ToList();

                // Debug: Print the unique employees
                foreach (var employee in uniqueEmployeeIds)
                {
                    Console.WriteLine($"EmployeeId: {employee.EmployeeId}, EmployeeName: {employee.EmployeeName}");
                }


                return View("ShowEmployees", uniqueEmployeeIds);
            }

            catch (JsonSerializationException ex)
            {
                // Debug: Print the serialization error
                Console.WriteLine($"JsonSerializationException: {ex.Message}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Debug: Print any other errors
                Console.WriteLine($"Exception: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        public IActionResult DisplayData(string searchString)
        {
            if (TempData["ExcelData"] != null)
            {
                var excelData = JsonConvert.DeserializeObject<List<List<string>>>(TempData["ExcelData"].ToString());
                TempData.Keep("ExcelData");

                var headers = excelData.FirstOrDefault();
                var data = excelData.Skip(1).ToList(); // Skip the header row

                if (!string.IsNullOrEmpty(searchString))
                {
                    excelData = excelData.Where(row => row.Any(cell => cell.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                // Combine headers and data for the view model
                var viewModel = new Tuple<List<string>, List<List<string>>>(headers, excelData);

                return View(excelData);
            }

            return RedirectToAction("Index");
        }
    }
}
     
