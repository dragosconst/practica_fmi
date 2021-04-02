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

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string Nume { get; set; }
        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string Prenume { get; set; }

        [Required(ErrorMessage = "Gradul didactic este obligatoriu")]
        public int GradDidactic { get; set; }
    }
}