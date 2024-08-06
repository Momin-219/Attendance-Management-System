using System.Collections.Generic;
using MyAppWeb.Models;

namespace MyAppWeb.ViewModels
{
    public class IndexViewModel
    {
        public List<SheetViewModel> Sheets { get; set; } = new List<SheetViewModel> { };
        public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel> { };  
    }
}

