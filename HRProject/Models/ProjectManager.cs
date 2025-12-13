using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRProject.Models
{
    public class ProjectManager
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Navigation – one project has many requirements
        public ICollection<ProjectRequirement> Requirements { get; set; }
            = new List<ProjectRequirement>();
    }
}

