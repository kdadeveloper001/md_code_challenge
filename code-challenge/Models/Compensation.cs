using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    public class Compensation
    {
        [Key]
        public String CompensationId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        /// <summary>
        /// Salary in lowest localized currency.
        /// For example, $1.00 USD would be 100 and
        /// $0.01 would be 1.
        /// </summary>
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
