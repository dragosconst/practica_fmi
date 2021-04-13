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
        [Authorize(Roles = "Admin")]
        public ActionResult AddFile(int id, HttpPostedFileBase file)
        {
            FileModel fm = new FileModel();
            Sectiune sectiune = db.Sectiuni.Find(id);
            if (file == null)
                return RedirectToAction("Show", "Cursuri", new { id = sectiune.Curs.CursId });
            fm.FileName = file.FileName;
            // momentan ii dam voie sa dea upload la ce vrea
            string uploadFolder = Server.MapPath("~/Files/");
            fm.FilePath = uploadFolder + fm.FileName;
            file.SaveAs(fm.FilePath); // save pe server
            fm.Date = DateTime.Now;
            fm.ProfNume = fm.ProfPrenume = "Admin"; // momentan

            fm.Sectiune = sectiune;
            sectiune.FileModels.Add(fm);
            db.SaveChanges();
            return RedirectToAction("Show", "Cursuri", new { id = sectiune.Curs.CursId });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
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