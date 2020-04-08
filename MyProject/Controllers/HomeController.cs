using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        AarhusHostelDBEntities Db = new AarhusHostelDBEntities();
        // GET: Home

        public ActionResult Index()
        {
            List<MulighederTable> MulighederList = new List<MulighederTable>();
            MulighederList = Db.MulighederTable.ToList();
            return View(MulighederList);
        }
        public ActionResult BestilVaerelse()
        {

            List<VaerlseTable> ModelRoom = new List<VaerlseTable>();
            //var ViewModel = new AarhusHostel.Models.VaelserViewModel();

            //ViewModel.VaerlseListe = Db.VaerlseTable.ToList();

            //ViewModel.FotoListe = Db.FotoTable.ToList();

            ModelRoom = Db.VaerlseTable.ToList(); //Udtrækker værelser med Id =1

            return View(ModelRoom);
        }
    }
}