using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class AdminController : Controller
    {
        AarhusHostelDBEntities Db = new AarhusHostelDBEntities();
        // GET: Admin
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //
            if (Session["BurgerSession"] == null)
            {
                filterContext.Result = new RedirectResult("/LogInd");
            }
        }
        //*************** Mulighed liste************

        public ActionResult Index()
        {
            List<MulighederTable> MulighedListe = new List<MulighederTable>();
            MulighedListe = Db.MulighederTable.ToList();

            return View(MulighedListe);
        }
        //***************opret ny  Mulighed************
        public ActionResult OpretNyMulighed()
        {
            MulighederTable NyMulighed = new MulighederTable();

            return View(NyMulighed);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult OpretNyMulighed(MulighederTable NyMulighed, HttpPostedFileBase Mulighedfoto)
        {
            if (!ModelState.IsValid || Mulighedfoto == null)
            {
                ViewBag.Besked = "Noget gik galt,Prøv igen....";
                return View(NyMulighed);// retur til formularen - send den allerede- udfyldt med retur
            }
            // formularen er korrekt udfyldt - data gemmes i Db og fil uploads i korrekt mappe i webprojekt

            // Håndtere filen (snup filnavn til Db + gem filen i img mappe)
            string ImgNavn = System.IO.Path.GetFileName(Mulighedfoto.FileName);
            string ImgSti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/cards-foto"), ImgNavn);


            // gem filen
            Mulighedfoto.SaveAs(ImgSti);

            // gem i Db
            NyMulighed.MulighedFoto = ImgNavn; // fx hest.jpg
            Db.MulighederTable.Add(NyMulighed);
            Db.SaveChanges();



            TempData["Besked"] = "Muligheden er oprettet";


            return RedirectToAction("Index", "Admin");



        }



        //***************slet Mulighed************

        public ActionResult SletMuligheden(int? Id)
        {
            //Hvis der ikke en Id med, sparke tilbage til index side.
            if (Id == null)
                return RedirectToAction("Index");



            //Hente muligheden som måske skal slettes, og send den med til viewet.
            MulighederTable MulighedenSlettes = Db.MulighederTable.Find(Id);
            if (MulighedenSlettes == null) //hvis der er ikke mulighed som matcher Id
                return RedirectToAction("Index");

            return View(MulighedenSlettes);
        }
        [HttpPost]
        public ActionResult SletMuligheden(MulighederTable MulighedenSlettes)
        {
            //Hente muligheden som måske skal slettes. og sende den til viewet...
            //muligheden  har kun en id . så resten skla slås op med "find"
            MulighedenSlettes = Db.MulighederTable.Find(MulighedenSlettes.MulighedID);


            //MulighedTable SC = Db.MulighedTable.Find(Id);

            if (MulighedenSlettes == null)// hvis der er ikke muligheden som matcher id 
                return RedirectToAction("Index");

            // slet fysisk fil/img
            string ImgSti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/cards-foto"), MulighedenSlettes.MulighedFoto);


            // hvis der img i den angivet sti
            //if (System.IO.File.Exists(ImgSti))
            //    System.IO.File.Delete(ImgSti);// imgsti slettet


            // slet fra Db
            Db.MulighederTable.Remove(MulighedenSlettes);
            Db.SaveChanges();

            TempData["Besked"] = "Muligheden er slettet";
            return RedirectToAction("Index", "Admin");

        }
        public ActionResult RetMuligheden(int? Id)

        {
            //Hvis der ikke en Id med, sparke tilbage til index side.
            if (Id == null)
                return RedirectToAction("Index");


            //Hente Cartoon som måske skal Rettes, og send den med til viewet.
            MulighederTable MulighedenRettet = Db.MulighederTable.Find(Id);
            if (MulighedenRettet == null) //hvis der er ikke Cartoon som matcher Id
                return RedirectToAction("Index");


            return View(MulighedenRettet);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult RetMuligheden(MulighederTable MulighedenRettet, HttpPostedFileBase NytPhoto)
        {
            //hvis det er ikke udfyldt korrekt
            if (!ModelState.IsValid)
            {
                ViewBag.Besked = "Der er noget galt,Prøve igen.....";
                return View(MulighedenRettet);// return til formularen. send den allrede udfyldt med retur,
            }
            if (NytPhoto != null)
            {
                string imgsti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/cards-foto"), MulighedenRettet.MulighedFoto);

                //if (System.IO.File.Exists(imgsti))
                //    System.IO.File.Delete(imgsti);// imgsti slettet



                string ImgNavn = System.IO.Path.GetFileName(NytPhoto.FileName);
                imgsti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/cards-foto"), ImgNavn);

                NytPhoto.SaveAs(imgsti);//gemme selve filen
                MulighedenRettet.MulighedFoto = ImgNavn; // gemme navnet i modellen -som næste step sends til Db
            }
            Db.MulighederTable.AddOrUpdate(MulighedenRettet);
            Db.SaveChanges();

            TempData["Besked"] = "Muligheden er Rettet";
            return RedirectToAction("Index", "Admin");
        }

    }
}