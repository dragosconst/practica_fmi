using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Curs
    {
        public Curs()
        {
            Profesors = new HashSet<Profesor>();
            Students = new HashSet<Student>();
        }

        [Key]
        public int CursId { get; set; }

        [Required(ErrorMessage = "Denumirea e obligatorie")]
        public string Denumire { get; set; }
        
        public virtual ICollection<Profesor> Profesors { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}