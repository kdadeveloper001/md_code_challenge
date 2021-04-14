using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    /// <summary>
    /// Provides reporting metrics for the specified employee
    /// </summary>
    public class ReportingStructure
    {
        /// <summary>
        /// Employee being reported on.
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Cumulative number of employees under the specified <see cref="ReportingStructure.Employee"/>
        /// For example:
        ///                     John Lennon
        ///                     /               \
        ///             Paul McCartney         Ringo Starr
        ///                                     /        \
        ///                             Pete Best     George Harrison
        ///                             
        /// In this case, the employee 'John Lennon' would have 4 direct reports.
        /// </summary>
        public int NumberOfDirectReports { get; set; }
    }
}
