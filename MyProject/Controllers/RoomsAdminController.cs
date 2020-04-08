using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class RoomsAdminController : Controller
    {
        AarhusHostelDBEntities Db = new AarhusHostelDBEntities();
        // GET: RoomsAdmin
        public ActionResult Index()
        {
            List<VaerlseTable> VaerlseListe = new List<VaerlseTable>();
            VaerlseListe = Db.VaerlseTable.ToList();
            return View(VaerlseListe);
        }



        public ActionResult OpretNytVaerelse()
        {
            VaerlseTable NytVaerelse = new VaerlseTable();


            return View(NytVaerelse);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult OpretNytVaerelse(VaerlseTable NytVaerelse)
        {

            //Debug.WriteLine("Stage 1");
            if (!ModelState.IsValid)
            {
                //Debug.WriteLine("Fejl 1");
                ViewBag.Besked = "Noget gik galt,Prøv igen....";

                return View(NytVaerelse);// retur til formularen - send den allerede- udfyldt med retur
            }
            //Debug.WriteLine("Stage 1 after");
            List<FotoTable> MoreFotos = new List<FotoTable>();
            //Debug.WriteLine(MoreFotos.ToList());
            //Debug.WriteLine(Request.Files.Count);
            for (int i = 0; i < Request.Files.Count; i++)
            {
                //Debug.WriteLine(i);
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    //Debug.WriteLine(file.FileName);
                    FotoTable fileDetail = new FotoTable()
                    {
                        FotoNavn = fileName,
                        ItemID_Fk = NytVaerelse.ItemID

                    };
                    MoreFotos.Add(fileDetail);

                    var path = System.IO.Path.Combine(Server.MapPath("~/Content/Img/"), fileDetail.FotoNavn);
                    file.SaveAs(path);
                }
            }

            NytVaerelse.FotoTable = MoreFotos;


            Db.VaerlseTable.Add(NytVaerelse);
            Db.SaveChanges();



            TempData["Besked"] = "Værelset  er oprettet";
            return RedirectToAction("Index", "RoomsAdmin");

        }
        public ActionResult RetVaerelset(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VaerlseTable RetVaerelset = Db.VaerlseTable.Include(s => s.FotoTable).SingleOrDefault(x => x.ItemID == id);
            if (RetVaerelset == null)
            {
                return HttpNotFound();
            }
            return View(RetVaerelset);
        }
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetVaerelset(VaerlseTable RetVaerelset)
        {
            if (ModelState.IsValid)
            {

                //New Files
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        FotoTable NytFoto = new FotoTable()
                        {
                            FotoNavn = fileName,
                            //Extension = Path.GetExtension(fileName),
                            //FotoID = Guid.NewGuid(),
                            ItemID_Fk = RetVaerelset.ItemID
                        };
                        var path = System.IO.Path.Combine(Server.MapPath("~/Content/Img/"), NytFoto.FotoNavn);
                        file.SaveAs(path);

                        Db.Entry(NytFoto).State = EntityState.Added;
                    }
                }

                Db.Entry(RetVaerelset).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(RetVaerelset);
        }





        //***************slet Værelset************

        public ActionResult SletVaerelset(int? Id)
        {
            //Hvis der ikke en Id med, sparke tilbage til index side.
            if (Id == null)
                return RedirectToAction("Index");



            //Hente muligheden som måske skal slettes, og send den med til viewet.
            VaerlseTable VaerelseSlettes = Db.VaerlseTable.Find(Id);
            if (VaerelseSlettes == null) //hvis der er ikke mulighed som matcher Id
                return RedirectToAction("Index");

            return View(VaerelseSlettes);
        }
        [HttpPost]
        public ActionResult SletVaerelset(VaerlseTable VaerelseSlettes)
        {
            //Hente muligheden som måske skal slettes. og sende den til viewet...
            //muligheden  har kun en id . så resten skla slås op med "find"
            VaerelseSlettes = Db.VaerlseTable.Find(VaerelseSlettes.ItemID);


            //MulighedTable SC = Db.MulighedTable.Find(Id);

            if (VaerelseSlettes == null)// hvis der er ikke muligheden som matcher id 
                return RedirectToAction("Index");

            //foreach (var item in VaerelseSlettes.FotoTable)
            //{

            //    string ImgSti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/"), item.FotoNavn);
            //    if (System.IO.File.Exists(ImgSti))
            //    {
            //        System.IO.File.Delete(ImgSti);
            //    }
            //}

            //slet fysisk fil / img
            //string ImgSti = System.IO.Path.Combine(Server.MapPath("~/Content/Img/"), );


            // hvis der img i den angivet sti
            //if (System.IO.File.Exists(ImgSti))
            //    System.IO.File.Delete(ImgSti);// imgsti slettet


            // slet fra Db
            Db.VaerlseTable.Remove(VaerelseSlettes);
            Db.SaveChanges();

            TempData["Besked"] = "Værelset er slettet";
            return RedirectToAction("Index", "RoomsAdmin");

        }
        //***************slet Foto************

        public ActionResult SletFoto(int? Id)
        {
            //Hvis der ikke en Id med, sparke tilbage til index side.
            if (Id == null)
                return RedirectToAction("RetVaerelset");



            //Hente muligheden som måske skal slettes, og send den med til viewet.
            FotoTable FotoSlettes = Db.FotoTable.Find(Id);
            var id = FotoSlettes.VaerlseTable.ItemID;
            if (FotoSlettes == null) //hvis der er ikke mulighed som matcher Id
                return RedirectToAction("RetVaerelset", new { @Id = FotoSlettes.ItemID_Fk });

            return View(FotoSlettes);
        }
        [HttpPost]
        public ActionResult SletFoto(FotoTable FotoSlettes)
        {



            //Hente muligheden som måske skal slettes. og sende den til viewet...
            //muligheden  har kun en id . så resten skla slås op med "find"
            FotoSlettes = Db.FotoTable.Find(FotoSlettes.FotoID);


            //MulighedTable SC = Db.MulighedTable.Find(Id);

            if (FotoSlettes == null)// hvis der er ikke muligheden som matcher id 
                return RedirectToAction("RetVaerelset");

            // slet fysisk fil/img
            string ImgSti = System.IO.Path.Combine(Server.MapPath("~/Content/Img"), FotoSlettes.FotoNavn);
            var id = FotoSlettes.VaerlseTable.ItemID.ToString();

            // hvis der img i den angivet sti
            //if (System.IO.File.Exists(ImgSti))
            //    System.IO.File.Delete(ImgSti);// imgsti slettet


            // slet fra Db
            Db.FotoTable.Remove(FotoSlettes);
            Db.SaveChanges();


            return RedirectToAction("RetVaerelset", "RoomsAdmin", new { @Id = FotoSlettes.ItemID_Fk });

        }
    }
}