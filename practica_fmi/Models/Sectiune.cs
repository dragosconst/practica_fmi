using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace practica_fmi.Models
{
    public class Sectiune
    {
        [Key]
        public int SectiuneId { get; set; }

        [Required(ErrorMessage = "Secțiunea are nevoie de o denumire")]
        public string Denumire { get; set; }

        [DataType(DataType.MultilineText)]
        public string Descriere { get; set; }

        public virtual Curs Curs { get; set; }
        public virtual ICollection<FileModel> FileModels { get; set; }
    }
}