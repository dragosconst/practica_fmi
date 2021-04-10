using practica_fmi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Controllers
{
    public class CursuriController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cursuri
        [Authorize(Roles = "Admin, Student, Profesor")]
        public ActionResult Index()
        {
            var curses = (from curs in db.Cursuri
                          orderby curs.Denumire
                          select curs).AsQueryable();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            ViewBag.curs = curses;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            Curs curs = new Curs();
            var cursViewModel = new CursViewModel
            {
                Curs = curs,
            };

            var allProfsList = db.Profesors.ToList();
            cursViewModel.AllProfIds = allProfsList.Select(o => new SelectListItem
            {
                Text = o.Nume + " " + o.Prenume,
                Value = o.ProfesorId.ToString()
            });
            var allStudentsList = db.Students.ToList();
            cursViewModel.AllStudentIds = allStudentsList.Select(o => new SelectListItem
            {
                Text = o.Nume + " " + o.Prenume,
                Value = o.StudentId.ToString()
            });
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(cursViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(CursViewModel newCurs)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Curs toAdd = new Curs();
                    var allProfs = db.Profesors.ToList();
                    List<Profesor> selProfs = new List<Profesor>();
                    foreach (var prof in allProfs)
                    {
                        if (newCurs.SelectedProfIds.Contains(prof.ProfesorId))
                        {
                            selProfs.Add(prof);
                        }
                    }
                    var allStds = db.Students.ToList();
                    List<Student> selStudents = new List<Student>();
                    foreach(var student in allStds)
                    {
                        if (newCurs.SelectedStudentsIds.Contains(student.StudentId))
                        {
                            selStudents.Add(student);
                        }
                    }

                    toAdd.Denumire = newCurs.Curs.Denumire;
                    toAdd.Profesors = selProfs;
                    toAdd.Students = selStudents;
                    db.Cursuri.Add(toAdd);
                    db.SaveChanges();
                    TempData["message"] = "Un curs nou a fost adaugat";
                    return RedirectToAction("Index");
                }
                System.Diagnostics.Debug.WriteLine("not valid");
                CursViewModel _newCurs = new CursViewModel();
                _newCurs.AllProfIds = newCurs.AllProfIds;
                _newCurs.AllStudentIds = newCurs.AllStudentIds;
                _newCurs.Curs = newCurs.Curs;
                if (_newCurs.AllProfIds == null)
                {
                    var allProfsList = db.Profesors.ToList();
                    _newCurs.AllProfIds = allProfsList.Select(o => new SelectListItem
                    {
                        Text = o.Nume + " " + o.Prenume,
                        Value = o.ProfesorId.ToString()
                    });
                }
                if (_newCurs.AllStudentIds == null)
                {
                    var allStudentsList = db.Students.ToList();
                    _newCurs.AllStudentIds = allStudentsList.Select(o => new SelectListItem
                    {
                        Text = o.Nume + " " + o.Prenume,
                        Value = o.StudentId.ToString()
                    });
                }
                return View(_newCurs);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                TempData["message"] = "Eroare la adăugarea cursului";
                return View(newCurs);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)    
        {
            Curs curs = db.Cursuri.Find(id);
            var cursViewModel = new CursViewModel
            {
                Curs = db.Cursuri.Include("Profesors").Include("Students").First(c => c.CursId == id),
            };
            if (cursViewModel.Curs == null)
                return HttpNotFound();

            var allProfsList = db.Profesors.ToList();
            cursViewModel.AllProfIds = allProfsList.Select(o => new SelectListItem
            {
                Text = o.Nume + " " + o.Prenume,
                Value = o.ProfesorId.ToString()
            });

            var allStudentsList = db.Students.ToList();
            cursViewModel.AllStudentIds = allStudentsList.Select(o => new SelectListItem
            {
                Text = o.Nume + " " + o.Prenume,
                Value = o.StudentId.ToString()
            });

            return View(cursViewModel);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, CursViewModel reqCurs)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Curs toChange = db.Cursuri.Find(id);
                    db.Cursuri.Remove(toChange); // perhaps not the greatest idea
                    db.SaveChanges();

                    var allProfs = db.Profesors.ToList();
                    List<Profesor> selProfs = new List<Profesor>();
                    foreach(var prof in allProfs)
                    {
                        if(reqCurs.SelectedProfIds.Contains(prof.ProfesorId))
                        {
                            selProfs.Add(prof);
                        }
                    }
                    var allStds = db.Students.ToList();
                    List<Student> selStudents = new List<Student>();
                    foreach (var student in allStds)
                    {
                        if (reqCurs.SelectedStudentsIds.Contains(student.StudentId))
                        {
                            selStudents.Add(student);
                        }
                    }

                    toChange.Denumire = reqCurs.Curs.Denumire;
                    toChange.Profesors = selProfs;
                    toChange.Students = selStudents;

                    db.Cursuri.Add(toChange);
                    db.SaveChanges();
                    TempData["message"] = "Cursul a fost modificat";
                    return RedirectToAction("Index");
                }
                else
                {
                    CursViewModel _reqCurs = new CursViewModel();
                    _reqCurs.AllProfIds = reqCurs.AllProfIds;
                    _reqCurs.AllStudentIds = reqCurs.AllStudentIds;
                    _reqCurs.Curs = reqCurs.Curs;
                    if (_reqCurs.AllProfIds == null)
                    {
                        var allProfsList = db.Profesors.ToList();
                        _reqCurs.AllProfIds = allProfsList.Select(o => new SelectListItem
                        {
                            Text = o.Nume + " " + o.Prenume,
                            Value = o.ProfesorId.ToString()
                        });
                    }
                    if (_reqCurs.AllStudentIds == null)
                    {
                        var allStudentsList = db.Students.ToList();
                        _reqCurs.AllStudentIds = allStudentsList.Select(o => new SelectListItem
                        {
                            Text = o.Nume + " " + o.Prenume,
                            Value = o.StudentId.ToString()
                        });
                    }

                    return View(_reqCurs);
                }
            }
            catch(DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                TempData["message"] = "Eroare la adaugarea profesorului";
                return View(reqCurs);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Curs toDelete = db.Cursuri.Find(id);
            db.Cursuri.Remove(toDelete);
            TempData["message"] = "Cursul a fost șters";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        private IEnumerable<SelectListItem> GetProfesors()
        {
            var profesori = (from prof in db.Profesors
                             select prof).AsQueryable().ToList();
            var selectList = new List<SelectListItem>();

            foreach(var prof in profesori)
            {
                selectList.Add(new SelectListItem
                {
                    Text = prof.Nume.ToString() + " " + prof.Prenume.ToString(),
                    Value = prof.ProfesorId.ToString()
                });
            }
            return selectList;
        }

        [NonAction]
        private ICollection<SelectListItem> GetStudents()
        {
            var students = (from student in db.Students
                            select student).AsQueryable().ToList();
            var selectList = new List<SelectListItem>();

            foreach (var std in students)
            {
                selectList.Add(new SelectListItem
                {
                    Text = std.Nume.ToString() + " " + std.Prenume.ToString(),
                    Value = std.StudentId.ToString()
                });
            }
            return selectList;
        }
    }
}