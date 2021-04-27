using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using practica_fmi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profesors
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            SetAccesRights();
            var students = (from student in db.Students
                         select student).AsQueryable();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            ViewBag.students = students;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            SetAccesRights();
            Student student = new Student();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(student);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var ph = new Microsoft.AspNet.Identity.PasswordHasher();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var newUser = new ApplicationUser();
                    newUser.Email = student.Email;
                    newUser.UserName = student.Email;

                    // TODO: find better way to generate pass
                    var userCreated = UserManager.Create(newUser, student.Nume + student.Prenume);
                    if (userCreated.Succeeded)
                    {
                        UserManager.AddToRole(newUser.Id, "Student");
                        student.UserId = newUser.Id;
                    }
                    db.Students.Add(student);
                    db.SaveChanges();
                    TempData["message"] = "Studentul a fost adăugat";
                    return RedirectToAction("Index");
                }

                TempData["message"] = "Eroare la adăugarea studentului";
                return View(student);
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
                TempData["message"] = "Eroare la adăugarea studentului";
                return View(student);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            SetAccesRights();
            Student student = (from std in db.Students
                                 where std.StudentId == id
                                 select std).First();

            return View(student);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int id, Student reqStudent)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Student student = (from std in db.Students
                                         where std.StudentId == id
                                         select std).First();

                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                    student.Nume = reqStudent.Nume;
                    student.Prenume = reqStudent.Prenume;
                    var user = UserManager.FindByEmail(student.Email);
                    student.Email = reqStudent.Email;
                    user.Email = student.Email; // change email of associated user, too

                    await UserManager.UpdateAsync(user);

                    db.SaveChanges();
                    TempData["message"] = "Studentul a fost modificat";
                    return RedirectToAction("Index");
                }
                return View(reqStudent);
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
                TempData["message"] = "Eroare la adăugarea studentului";
                return View(reqStudent);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Student student = (from std in db.Students
                                 where std.StudentId == id
                                 select std).First();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = UserManager.FindByEmail(student.Email);
            db.Users.Remove(user);
            db.Students.Remove(student);
            db.SaveChanges();
            TempData["message"] = "Studentul a fost șters";
            return RedirectToAction("Index");
        }


        // Metoda ca sa gestionez drepturile de acces intre controllere
        private void SetAccesRights()
        {
            // deoarece CRUD pe studenti poate face doar adminul, e imposibil ca metoda sa fie
            // apelata de altcineva decat de un admin
            ViewBag.admin = true;
        }
    }
}