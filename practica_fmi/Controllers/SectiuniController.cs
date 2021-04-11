using Microsoft.Security.Application;
using practica_fmi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Controllers
{
    public class SectiuniController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult New(int id)
        {
            Curs curs = db.Cursuri.Find(id);
            Sectiune sectiune = new Sectiune();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            ViewBag.curs = curs;
            return View(sectiune);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult New(int id, Sectiune newSec)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Curs curs = db.Cursuri.Find(id);
                    System.Diagnostics.Debug.WriteLine(newSec.Descriere);
                    newSec.Descriere = Sanitizer.GetSafeHtmlFragment(newSec.Descriere); // for security
                    db.Sectiuni.Add(newSec);
                    curs.Sectiuni.Add(newSec);
                    newSec.Curs = curs;
                    TempData["message"] = "Secțiune adăugată";
                    db.SaveChanges();
                    return RedirectToAction("Show", "Cursuri", new { id = id });
                }
                ViewBag.curs = db.Cursuri.Find(id);
                return View(newSec);
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
                TempData["message"] = "Eroare la adăugarea secțiunii";
                ViewBag.curs = db.Cursuri.Find(id);
                return View(newSec);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Sectiune toEdit = db.Sectiuni.Find(id);
            Curs curs = toEdit.Curs;
            ViewBag.curs = curs;
            return View(toEdit);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Sectiune reqSect)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Sectiune toModify = db.Sectiuni.Find(id);
                    toModify.Denumire = reqSect.Denumire;
                    toModify.Descriere = Sanitizer.GetSafeHtmlFragment(reqSect.Descriere);
                    db.SaveChanges();
                    return RedirectToAction("Show", "Cursuri", new { id = toModify.Curs.CursId });
                }
                ViewBag.curs = db.Cursuri.Find(reqSect.Curs.CursId);
                System.Diagnostics.Debug.WriteLine("YOOOOOOOOOOOOOOOOOOOOOOOO");
                return View(reqSect);
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
                TempData["message"] = "Eroare la modificarea secțiunii";
                return View(reqSect);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Sectiune toRemove = db.Sectiuni.Find(id);
            int cursId = toRemove.Curs.CursId;
            db.Sectiuni.Remove(toRemove);
            db.SaveChanges();
            return RedirectToAction("Show", "Cursuri", new { id = cursId });
        }
    }
}