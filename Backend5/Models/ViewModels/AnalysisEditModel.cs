using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend5.Models.ViewModels
{
    public class AnalysisEditModel
    {
        [Required]
        [MaxLength(200)]
        public String Type { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(200)]
        public String Status { get; set; }
    }
}
