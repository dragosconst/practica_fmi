using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using practica_fmi.Models;

namespace practica_fmi.Models
{
    public class Profesor : ApplicationUser
    {
        [Key]
        public int ProfesorId { get; set; }

        public string Nume { get; set; }
        public string Prenume { get; set; }

        public int GradDidactic { get; set; }
    }
}