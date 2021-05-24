using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend5.Models
{
    public class Diagnosis
    {
        public Int32 DiagnosisId { get; set; }

        public Int32 PatientId { get; set; }

        public Patient Patient { get; set; }

        [Required]
        [MaxLength(200)]
        public String Type { get; set; }

        [MaxLength(300)]
        public String Complications { get; set; }

        [MaxLength(500)]
        public String Details { get; set; }
    }
}
