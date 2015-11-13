using SURLY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDBMS.Models;

namespace WebDBMS.Controllers
{
    public class HomeController : Base
    {

        [HttpGet]
        public ActionResult Command()
        {
            CreateRestOfTables();
            return View();
        }

        [HttpPost]
        public PartialViewResult Command(string query)
        {
            var ret = WebDataBase.db.parse(query);
            Tables table = new Tables();
            table.myTables = ret;
            return PartialView("~/Views/Home/TablePartial.cshtml", table);
        }

        [HttpGet]
        public ActionResult ListTables()
        {
            Tables table = new Tables();
            table.myTables = WebDataBase.db.Tables;
            return View(table);
        }

        [HttpGet]
        public ActionResult Tutorial()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult CreateGameTut(string query)
        {
            WebDataBase.db.parse(query);
            var ret = WebDataBase.db.parse("Select from Game");
            Tables table = new Tables();
            table.myTables = ret;
            return PartialView("~/Views/Home/TablePartial.cshtml", table);
        }

        [HttpPost]
        public PartialViewResult CreateMovieTut(string query)
        {
            WebDataBase.db.parse(query);
            var ret = WebDataBase.db.parse("Select from Movie");
            Tables table = new Tables();
            table.myTables = ret;
            return PartialView("~/Views/Home/TablePartial.cshtml", table);
        }

        [HttpPost]
        public PartialViewResult ReturTutInput(string query)
        {
            var ret = WebDataBase.db.parse(query);
            Tables table = new Tables();
            table.myTables = ret;
            return PartialView("~/Views/Home/TablePartial.cshtml", table);
        }

    }
}