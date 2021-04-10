using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Profesor
    {
        public Profesor()
        {
            Cursuri = new HashSet<Curs>();
        }

        [Key]
        public int ProfesorId { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Emailul este obligatoriu!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string Nume { get; set; }
        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string Prenume { get; set; }

        [Required(ErrorMessage = "Gradul didactic este obligatoriu")]
        public int GradDidactic { get; set; }

        public virtual ICollection<Curs> Cursuri { get; set; }
    }
}