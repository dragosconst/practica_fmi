using Microsoft.AspNet.Identity;
using practica_fmi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace practica_fmi.Controllers
{
    public class FileModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // id = id-ul sectiunii in care il bag
        [HttpPost]
        [Authorize(Roles = "Admin,Profesor")]
        public ActionResult AddFile(int id, HttpPostedFileBase file)
        {
            FileModel fm = new FileModel();
            Sectiune sectiune = db.Sectiuni.Find(id);
            if (file == null)
                return RedirectToAction("Show", "Cursuri", new { id = sectiune.Curs.CursId });
            fm.FileName = file.FileName;
            // momentan ii dam voie sa dea upload la ce vrea
            if (!System.IO.Directory.Exists(Server.MapPath("~/Files/" + User.Identity.GetUserId() + "/")))
                System.IO.Directory.CreateDirectory(Server.MapPath("~/Files/" + User.Identity.GetUserId() + "/"));
            string uploadFolder = Server.MapPath("~/Files/" + User.Identity.GetUserId() + "/");
            fm.FilePath = uploadFolder + fm.FileName;
            file.SaveAs(fm.FilePath); // save pe server
            fm.Date = DateTime.Now;
            fm.FileExtension = Path.GetExtension(file.FileName);
            fm.FileExtension = fm.FileExtension == ".rar" ? ".zip" : fm.FileExtension; // turn rar in zip
            if (fm.FileExtension != ".pdf" && fm.FileExtension != ".ppx" && fm.FileExtension != ".txt" && fm.FileExtension != ".zip")
                fm.FileExtension = ".other";

            if(User.IsInRole("Admin"))
            {
                fm.ProfNume = fm.ProfPrenume = "Admin";
            }
            else
            {
                string uid = User.Identity.GetUserId();
                Profesor profesor = (from prof in db.Profesors
                                     where prof.UserId == uid
                                     select prof).ToList().First();
                fm.ProfNume = profesor.Nume;
                fm.ProfPrenume = profesor.Prenume;
            }

            fm.Sectiune = sectiune;
            sectiune.FileModels.Add(fm);
            db.SaveChanges();
            return RedirectToAction("Show", "Cursuri", new { id = sectiune.Curs.CursId });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,Profesor")]
        public ActionResult Delete(int id)
        {
            FileModel fileModel = db.FileModels.Find(id);
            Sectiune sectiune = fileModel.Sectiune;

            // delete from server
            FileInfo fileInfo = new FileInfo(fileModel.FilePath);
            if(fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            sectiune.FileModels.Remove(fileModel);
            db.FileModels.Remove(fileModel);
            db.SaveChanges();
            return RedirectToAction("Show", "Cursuri", new { id = sectiune.Curs.CursId });
        }

        [Authorize(Roles = "Admin,Profesor,Student")]
        public FileResult Download(int id)
        {
            FileModel fileModel = db.FileModels.Find(id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileModel.FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileModel.FileName);
        }
    }
}