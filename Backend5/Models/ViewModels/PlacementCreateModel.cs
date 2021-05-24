using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend5.Models.ViewModels
{
    public class PlacementCreateModel
    {
        public Int32 WardId { get; set; }

        [Required]
        public Int32 Bed { get; set; }
    }
}
