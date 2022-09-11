using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Tables.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                db.SaveChanges();




                QueryCount(table.Id);
                return RedirectToAction("Index");
            }
            //---------------------------------------------------
            return View(table);

        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                QueryCount(table.Id);
                return RedirectToAction("Index");
            }
            return View(table);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Table table = db.Tables.Find(id);
            db.Tables.Remove(table);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public void QueryCount(int id)
        {
            //-------------------------------------updating quote

            using (InsuranceEntities db = new InsuranceEntities())
            {
                var result = db.Tables.Find(id);

                decimal quote = 50m;
                //based on age
                if (result.CarYear - DateTime.Now.Year < 18)
                {
                    quote += 100m;
                }
                else if (result.CarYear - DateTime.Now.Year > 18 && result.CarYear - DateTime.Now.Year < 25)
                {
                    quote += 50m;
                }
                else
                {
                    quote += 25m;
                }

                //based on cars age
                if (result.CarYear < 2000 || result.CarYear > 2015)
                {
                    quote += 25m;
                }

                //based on the car make
                if (result.CarMake == "Porsche")
                {
                    quote += 25m;
                    if (result.CarModel == "911 Carrera")
                    {
                        quote += 25m;
                    }
                }

                //based on speeding tickets
                for (int i = 0; i < result.SpeedingTickets; i++)
                {
                    quote += 10m;
                }

                //based on DUI
                if (result.DUI)
                {
                    quote += Decimal.Multiply(quote, 0.25m);
                }

                //based on coverage
                if (result.CoverageType)
                {
                    quote += Decimal.Multiply(quote, 0.5m);
                }
                result.Quote = quote;
                db.SaveChanges();

            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
