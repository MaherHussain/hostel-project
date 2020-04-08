using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class LogIndController : Controller
    {
        AarhusHostelDBEntities Db = new AarhusHostelDBEntities();
        public ActionResult Index()
        {
            //slet en evt. brugersession når man lander på logind side- ekstra sikkerhed og 
            Session.Remove("BurgerSession");
            return View();
        }
        [HttpPost]
        public ActionResult Index(BrugerTable LogIndBruger)
        {
            //hvis modellen er udfyldt med fejl/mangler- send bruger tilbage til viwet ( her er samme side )
            if (!ModelState.IsValid)
                return View();


            // find ud af om der er et match mellem logind input og brugernavn/password i tablen
            BrugerTable BrugerMatch = new BrugerTable();
            BrugerMatch = Db.BrugerTable.Where(b => b.BrugerNavn == LogIndBruger.BrugerNavn && b.PassWord == LogIndBruger.PassWord).FirstOrDefault();

            // brugermatch == null .....der var ikke noget match
            if (BrugerMatch == null)
            {

                // her er mere sikerhed for logind da vi sletter sission hvis der noget forkert.....
                Session.Remove("BurgerSession");


                ViewBag.Besked = "UPS! forkert brugernavn og/eller Password Prøv igen";
                return View(); // retun samme side med beskden.
            }


            // der blev fundet et match =login = session
            //der blev lavet en session - som er den vi "måler" på, om brugen skal have adgang til admin

            Session["BurgerSession"] = BrugerMatch.Navn;


            return RedirectToAction("Index", "Admin"); // bruger sendes til startsiden på ADMIN
        }
    }
}