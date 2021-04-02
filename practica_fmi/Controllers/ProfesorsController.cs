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
    }
}