using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Models
{
    public class CursViewModel
    {
        public Curs Curs { get; set; }
        public IEnumerable<SelectListItem> AllProfIds { get; set; }
        public IEnumerable<SelectListItem> AllStudentIds { get; set; }

        private List<int> _selectedProfIds { get; set; }
        private List<int> _selectedStudentIds { get; set; }
        public List<int> SelectedProfIds
        {
            get
            {
                if(_selectedProfIds == null)
                {
                    _selectedProfIds = Curs.Profesors.Select(p => p.ProfesorId).ToList();
                }
                return _selectedProfIds;
            }
            set
            {
                _selectedProfIds = value;
            }
        }
        public List<int> SelectedStudentsIds
        {
            get
            {
                if(_selectedStudentIds == null)
                {
                    _selectedStudentIds = Curs.Students.Select(s => s.StudentId).ToList();
                }
                return _selectedStudentIds;
            }
            set
            {
                _selectedStudentIds = value;
            }
        }
    }
}