using System;
using System.Collections.Generic;

#nullable disable

namespace WorldManagementApi.Models
{
    public partial class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Gender { get; set; }
        public double? Luck { get; set; }
        public double? Health { get; set; }
        public double? Hunger { get; set; }
        public double? Security { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public DateTime? DestructionDate { get; set; }
        public string IdentificationTags { get; set; }
    }
}
