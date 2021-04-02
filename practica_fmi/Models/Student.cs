using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Student : ApplicationUser
    {
        [Key]
        public int StudentId { get; set; }

        public string Nume { get; set; }
        public string Prenume { get; set; }
        public int AnStudiu { get; set; }
    }
}