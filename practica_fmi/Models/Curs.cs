using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Curs
    {
        public int CursId { get; set; }

        public string Denumire { get; set; }

        public virtual ICollection<Profesor> Profesors { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}