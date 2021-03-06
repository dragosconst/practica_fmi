using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;
using practica_fmi.Models;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
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
            SetAccesRights(null);
            var courses = db.Cursuri.OrderBy(c => c.Denumire).AsQueryable();

            var search = "";
            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim(); // ar trebui sa fie (parte din) denumirea unui curs

                List<int> cursIds = db.Cursuri.Where(c => c.Denumire.ToUpper().Contains(search.ToUpper()))
                                    .Select(c => c.CursId).ToList();
                List<int> profIds = db.Profesors.Where(p => p.Cursuri.Select(c => c.CursId).AsEnumerable()
                                    .Intersect(cursIds).Count() != 0)
                                    .Select(p => p.ProfesorId).ToList();
                return ProfSearch(search, cursIds, profIds);
            }
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            ViewBag.curs = courses;
            return View();
        }

        [Authorize(Roles = "Admin,Profesor,Student")]
        public ActionResult ProfSearch(string search, List<int> cursIds, List<int> profIds)
        {
            IEnumerable<Profesor> profesors = db.Profesors.Where(p => profIds.Contains(p.ProfesorId)).ToList();
            IEnumerable<Curs> cursuri = db.Cursuri.Where(c => cursIds.Contains(c.CursId)).ToList();
            ViewBag.profesors = profesors;
            ViewBag.cursuri = cursuri;
            ViewBag.search = search;
            return View("ProfSearch");
        }

        // toata lumea are voie sa vizualizeze un curs
        [Authorize(Roles = "Admin, Student, Profesor")]
        public ActionResult Show(int id)
        {
            Curs curs = db.Cursuri.Find(id);
            SetAccesRights(curs);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            return View(curs);
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

                    System.Random r = new System.Random();

                    toAdd.Denumire = newCurs.Curs.Denumire;
                    toAdd.Semestru = newCurs.Curs.Semestru;
                    toAdd.Profesors = selProfs;
                    toAdd.Students = selStudents;
                    toAdd.BackgroundId = r.Next(1, 14);
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

                // toata bataia asta de cap e necesara pt ca for some reason AllProfIds (si la studenti la fel)
                // e facut implicit null daca nu e valid modelstate, si asta crapa tot programu cu vechiul
                // reqCurs
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
                    EmptyProfsAndStudents(toChange);

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
                    toChange.Semestru = reqCurs.Curs.Semestru;
                    toChange.Profesors = selProfs;
                    toChange.Students = selStudents;
                    
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

                    // toata bataia asta de cap e necesara pt ca for some reason AllProfIds (si la studenti la fel)
                    // e facut implicit null daca nu e valid modelstate, si asta crapa tot programu cu vechiul
                    // reqCurs
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
            List<int> sids = new List<int>();
            foreach (var sect in toDelete.Sectiuni)
            {
                sids.Add(sect.SectiuneId);
            }
            foreach (int sid in sids)
            {
                Sectiune sectiune = db.Sectiuni.Find(sid);
                List<int> fids = new List<int>();
                foreach (var fm in sectiune.FileModels)
                {
                    fids.Add(fm.FileId);
                }
                foreach (int fid in fids)
                {
                    // delete from server
                    FileInfo fileInfo = new FileInfo(db.FileModels.Find(fid).FilePath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }

                    db.FileModels.Remove(db.FileModels.Find(fid));
                }
                db.Sectiuni.Remove(sectiune);
            }
            db.Cursuri.Remove(toDelete);
            TempData["message"] = "Cursul a fost șters";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Metoda ca sa gestionez drepturile de acces intre controllere
        [NonAction]
        private void SetAccesRights(Curs curs)
        {
            /**
             * Metoda e apelata doar din Index si Show.
             * In index, curs e null, dar in Show e cursul afisat.
             * */
            if(User.IsInRole("Admin"))
            {
                ViewBag.admin   = true;
                ViewBag.prof    = false;
                ViewBag.student = false;
            }
            else if(User.IsInRole("Profesor"))
            {
                string uid = User.Identity.GetUserId();
                Profesor profesor = db.Profesors.Where(p => p.UserId == uid)
                                    .ToList().First(); // e sigur un singur prof in lista
                if(curs == null || curs.Profesors.Contains(profesor))
                {
                    ViewBag.admin = false;
                    ViewBag.prof = true;
                    ViewBag.pid = profesor.ProfesorId;
                    ViewBag.student = false;
                }
                else
                {
                    ViewBag.faulty = true;
                }
            }
            else if (User.IsInRole("Student"))
            {
                string uid = User.Identity.GetUserId();
                Student student = db.Students.Where(s => s.UserId == uid)
                                  .ToList().First(); // e sigur un singur student in lista
                if(curs == null || curs.Students.Contains(student))
                {
                    ViewBag.admin = false;
                    ViewBag.prof = false;
                    ViewBag.student = true;
                    ViewBag.sid = student.StudentId;
                }
                else
                {
                    ViewBag.faulty = true;
                }
            }   
        }

        // Metoda utility pt editarea cursurilor, ca sa nu bage de doua ori aceiasi profi in baza de date
        [NonAction]
        private void EmptyProfsAndStudents(Curs curs)
        {
            foreach(Profesor prof in curs.Profesors)
            {
                prof.Cursuri.Remove(curs);
            }
            curs.Profesors = null;

            foreach(Student student in curs.Students)
            {
                student.Cursuri.Remove(curs);
            }
            curs.Students = null;
            db.SaveChanges();
        }
    }
}