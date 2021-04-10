using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredListAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IEnumerable;

            System.Diagnostics.Debug.WriteLine(list != null && list.GetEnumerator().MoveNext());
            // check against both null and available items inside the list
            return list != null && list.GetEnumerator().MoveNext();
        }
    }

    public class CursViewModel
    {
        public Curs Curs { get; set; }
        public IEnumerable<SelectListItem> AllProfIds { get; set; }
        public IEnumerable<SelectListItem> AllStudentIds { get; set; }

        private List<int> _selectedProfIds { get; set; }
        private List<int> _selectedStudentIds { get; set; }
        [RequiredList(ErrorMessage = "Trebuie măcar un profesor")]
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
        [RequiredList(ErrorMessage = "Trebuie măcar un student")]
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