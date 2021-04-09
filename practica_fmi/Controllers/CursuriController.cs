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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(curs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Curs newCurs)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    db.Cursuri.Add(newCurs);
                    db.SaveChanges();
                    TempData["message"] = "Un curs nou a fost adaugat";
                    ViewBag.profs = GetProfesors();
                    ViewBag.students = GetStudents();
                    return RedirectToAction("Index");
                }
                return View(newCurs);
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
            ViewBag.profs = GetProfesors();
            ViewBag.students = GetStudents();
            return View(curs);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Curs reqCurs)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Curs toChange = db.Cursuri.Find(id);

                    toChange.Denumire = reqCurs.Denumire;
                    toChange.Profesors = reqCurs.Profesors;
                    toChange.Students = reqCurs.Students;
                    db.SaveChanges();
                    TempData["message"] = "Cursul a fost modificat";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(reqCurs);
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
            return RedirectToAction("Index");
        }

        private ICollection<Profesor> GetProfesors()
        {
            var profesori = (from prof in db.Profesors
                             select prof).AsQueryable().ToList();
            return profesori;
        }

        private ICollection<Student> GetStudents()
        {
            var students = (from student in db.Students
                            select student).AsQueryable().ToList();
            return students;
        }
    }
}