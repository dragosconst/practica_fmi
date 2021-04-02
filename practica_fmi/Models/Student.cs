using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string Nume { get; set; }
        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string Prenume { get; set; }

        [Required(ErrorMessage = "Emailul este obligatoriu")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Anul de studiu este obligatoriu")]
        public int AnStudiu { get; set; }
    }
}