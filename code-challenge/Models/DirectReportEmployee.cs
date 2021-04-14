using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    public class DirectReportEmployee
    {
        [Key]
        public String EmployeeId { get; set; }
    }
}
