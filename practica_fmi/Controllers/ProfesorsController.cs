using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using practica_fmi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace practica_fmi.Controllers
{
    public class ProfesorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profesors
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            var profs = (from prof in db.Profesors
                         select prof).AsQueryable();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            ViewBag.profs = profs;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            Profesor profesor = new Profesor();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(profesor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Profesor profesor)
        {
            try
            {
                if(ModelState.IsValid)
                {

                    var ph = new Microsoft.AspNet.Identity.PasswordHasher();
                    var UserManager = new UserManager<Profesor>(new UserStore<Profesor>(db));

                    // TODO: find better way to generate pass
                    profesor.UserName = profesor.Email;
                    var profCreated = UserManager.Create(profesor, profesor.Nume + profesor.Prenume);
                    if (profCreated.Succeeded)
                    {
                        UserManager.AddToRole(profesor.Id, "Profesor");
                    }
                    // db.Profesors.Add(profesor);
                    //db.SaveChanges();
                    TempData["message"] = "Profesorul a fost adăugat";
                    return RedirectToAction("Index");
                }

                TempData["message"] = "Eroare la adaugarea profesorului";
                return View(profesor);
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
                return View(profesor);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Profesor profesor = (from prof in db.Profesors
                                 where prof.ProfesorId == id
                                 select prof).First();
            
            return View(profesor);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int id, Profesor reqProf)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Profesor profesor = (from prof in db.Profesors
                                         where prof.ProfesorId == id
                                         select prof).First();
        
                    var UserManager = new UserManager<Profesor>(new UserStore<Profesor>(db));

                    profesor.Nume = reqProf.Nume;
                    profesor.Prenume = reqProf.Prenume;
                    profesor.Email = reqProf.Email;
                    //profesor.UserName = profesor.Email;

                    await UserManager.UpdateAsync(profesor);

                    TempData["message"] = "Profesorul a fost modificat";
                    return RedirectToAction("Index");
                }
                return View(reqProf);
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
                TempData["message"] = "Eroare la adaugarea profesorului";
                return View(reqProf);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Profesor profesor = (from prof in db.Profesors
                                 where prof.ProfesorId == id
                                 select prof).First();
            db.Profesors.Remove(profesor);
            db.SaveChanges();
            TempData["message"] = "Profesorul a fost șters";
            return RedirectToAction("Index");
        }
    }
}