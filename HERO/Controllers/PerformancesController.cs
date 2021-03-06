﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HERO.Models;
using HERO.Models.Objects;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace HERO.Controllers
{
    public class PerformancesController : Controller
    {
        private GymContext db;

        public PerformancesController(GymContext context)
        {
            db = context;
        }

        // GET: Performances
        public async Task<ActionResult> Index()
        {
            return View(await db.Performances.ToListAsync());
        }

        // GET: Performances/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Performance performance = await db.Performances.FindAsync(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            return View(performance);
        }

        // GET: Performances/Create
        public ActionResult Create(int? classId)
        {
            Class cls = db.Classes.Find(classId);
            ViewData["Class"] = db.Classes.Find(classId);
            ViewData["WOD"] = cls.WOD;
            return View();
        }

        // POST: Performances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ScoreInput,Description,Prescribed")] Performance performance, int classId)
        {
            Class cls = await db.Classes.FindAsync(classId);

            string userId = HttpContext.User.Identity.GetUserId();
            Athlete athlete = db.Athletes.Single(a => a.ApplicationUserId.Equals(userId));

            switch(cls.WOD.Scoring)
            {
                case WODScoring.TotalReps:
                    performance.ScoreActual = Convert.ToDouble(performance.ScoreInput);
                    break;
                case WODScoring.TotalRounds:
                    performance.ScoreActual = Convert.ToDouble(performance.ScoreInput);
                    break;
                case WODScoring.TotalTime:
                    TimeSpan time;
                    if (!TimeSpan.TryParseExact(performance.ScoreInput, @"mm\:ss", CultureInfo.CurrentCulture, out time))
                    {
                        throw new Exception("TimeSpan parse failed.");
                    }
                    performance.ScoreActual = Convert.ToDouble(time.TotalSeconds);
                    break;
            }

            performance.Class = cls;
            performance.Athlete = athlete;
            performance.WOD = cls.WOD;

            cls.Performances.Add(performance);
            athlete.Performances.Add(performance);

            ModelState.Remove("Description");
            ModelState.Remove("Prescribed");
            if (ModelState.IsValid)
            {
                db.Performances.Add(performance);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { controller = "Performances" } );
            } else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                throw new Exception(String.Join(", ", errorMessages));
            }
        }

        // GET: Performances/Edit/5
        public async Task<ActionResult> Edit(int? id, int? classId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Performance performance = await db.Performances.FindAsync(id);

            if (performance == null)
            {
                return HttpNotFound();
            }

            Class cls = db.Classes.Find(classId);
            ViewData["Class"] = db.Classes.Find(classId);
            ViewData["WOD"] = cls.WOD;
            
            return View(performance);
        }

        // POST: Performances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Score")] Performance performance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(performance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(performance);
        }

        // POST: Performances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, int classId)
        {
            Performance performance = await db.Performances.FindAsync(id);
            string userId = HttpContext.User.Identity.GetUserId();
            Athlete athlete = await db.Athletes.SingleAsync(a => a.ApplicationUserId.Equals(userId));
            Class cls = await db.Classes.FindAsync(classId);

            athlete.Performances.Remove(performance);
            cls.Performances.Remove(performance);
            db.Performances.Remove(performance);

            await db.SaveChangesAsync();
            return RedirectToAction("Details", new { controller = "Classes", id = classId });
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
