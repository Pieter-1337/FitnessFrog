using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        
        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };

            SetupActivitiesSelectListItems();

            return View(entry);
        }

      

        //Use this action method when "Posting from a form" the 2 decorator attributes specify the origin and the form method which is to be associated with
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);


            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                TempData["Message"] = "Your entry was successfully added!";
                return RedirectToAction("Index");
            }

            //Onderstaande wordt gebruikt voor het genereren van een dropdown menu voor de activities
            SetupActivitiesSelectListItems();

            return View(entry);
        }

     

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO Get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);

            //TODO a status of "not found" if the entry was not found
            if(entry == null)
            {
                return HttpNotFound();
            }

            //Todo Populate the activities select list items ViewBar Property
            SetupActivitiesSelectListItems();
            //TODO pass the entry in the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //validate the entry
            ValidateEntry(entry);

            //if the entry is valid
            // 1) Use te repository to update the entry
            // 2) Redirect the user to the "Entries" List page
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["Message"] = "Your entry was successfully edited!";
                return RedirectToAction("Index");
            }


            //Todo populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //todo retrieve entry for the provided id parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);

            //Todo return not found if entry is not found
            if(entry == null)
            {
                return HttpNotFound();
            }

            //Todo pass entry to view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Todo delete the entry
            _entriesRepository.DeleteEntry(id);
            TempData["Message"] = "Your entry was successfully deleted!";

            //Redirect the user to the entries list page
            return RedirectToAction("Index");

            
        }

        private void ValidateEntry(Entry entry)
        {
            //If there aren't any Duration field validation errors
            //Then make sure that the duration is greater than "0"
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}