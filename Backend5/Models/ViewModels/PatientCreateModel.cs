using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend5.Models.ViewModels
{
    public class PatientCreateModel
    {
        [Required]
        [MaxLength(200)]
        public String Name { get; set; }

        public String Address { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [MaxLength(10)]
        public String Gender { get; set; }
    }
}
