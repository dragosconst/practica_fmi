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

        private List<int> _selectedProfIds { get; set; }
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
    }
}