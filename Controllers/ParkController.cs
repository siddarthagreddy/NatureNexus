using NatureNexus.Data;
using NatureNexus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NatureNexus.Controllers
{
    public class ParkController : Controller
    {
        private readonly NatureNexusContext _context;
        public ParkController(NatureNexusContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Search(string statename, string activityname, string topicname, string parkname)
        {
            statename = (statename == null) ? "" : statename;
            activityname = (activityname == null) ? "" : activityname;
            topicname = (topicname == null) ? "" : topicname;
            parkname = (parkname == null) ? "" : parkname;

            List<Park> plist = new List<Park>();
            if (statename == "" && activityname == "" && topicname == "" && parkname == "")
            {
                plist = await _context.Parks.Include(p => p.states).ToListAsync();
            }
            else
            {
                plist = await _context.Parks
                            .Include(p => p.activities)
                            .Include(p => p.topics)
                            .Include(p => p.states)
                            .Where(p => p.activities.Any(s => s.activity.name.Contains(activityname)))
                            .Where(p => p.topics.Any(s => s.topic.name.Contains(topicname)))
                            .Where(p => p.states.Any(s => s.state.ID.Contains(statename)))
                            .Where(p => p.fullName.Contains(parkname))
                            .ToListAsync();
            }


            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = _context.Activities.Select(p => p.name).ToList();
            List<string> tnames = _context.Topics.Select(p => p.name).ToList();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;
            ViewBag.tnames = tnames;

            return View(plist);
        }
        public IActionResult Thanks(string message)
        {
            ViewBag.dispm = message;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> tn = await _context.Topics.Select(p => p.name).ToListAsync();
            List<string> an = await _context.Activities.Select(p => p.name).ToListAsync();
            ViewBag.tnames = tn;
            ViewBag.anames = an;
            ViewBag.statedict = dict;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("url,fullName,parkCode,description,statenames,activitynames,topicnames")] CreatePark newp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Park newpark = new Park()
                    {
                        ID = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                        fullName = newp.fullName,
                        parkCode = newp.parkCode,
                        description = newp.description,
                        url = newp.url
                    };
                    _context.Parks.Add(newpark);
                    if (newp.activitynames != null)
                    {
                        foreach (string str in newp.activitynames)
                        {
                            Activity a = _context.Activities.Where(p => p.name == str).FirstOrDefault();
                            _context.ParkActivities.Add(new ParkActivity()
                            {
                                park = newpark,
                                activity = a
                            });
                        }
                    }
                    if (newp.statenames != null)
                    {
                        foreach (string str in newp.statenames)
                        {
                            State s = _context.States.Where(p => p.ID == str).FirstOrDefault();
                            _context.StateParks.Add(new StatePark()
                            {
                                park = newpark,
                                state = s
                            });
                        }
                    }
                    if (newp.topicnames != null)
                    {
                        foreach (string str in newp.topicnames)
                        {
                            Topic t = _context.Topics.Where(p => p.name == str).FirstOrDefault();
                            _context.ParkTopics.Add(new ParkTopic()
                            {
                                park = newpark,
                                topic = t
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Thanks), new { message = "Thanks for helping us to grow our database!" });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> tn = await _context.Topics.Select(p => p.name).ToListAsync();
            List<string> an = await _context.Activities.Select(p => p.name).ToListAsync();
            ViewBag.tnames = tn;
            ViewBag.anames = an;
            ViewBag.statedict = dict;
            return View(newp);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Parks
                .Include(s => s.activities)
                    .ThenInclude(e => e.activity)
                .Include(s => s.topics)
                    .ThenInclude(e => e.topic)
                .Include(s => s.states)
                    .ThenInclude(e => e.state)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID.Equals(id));

            if (p == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Details: " + p.parkCode;
            return View(p);
        }

        public async Task<IActionResult> Edit(string id)
        {
            Park parkToUpdate = _context.Parks.Where(p => p.ID == id).FirstOrDefault();
            List<string> park_a = _context.ParkActivities.Where(p => p.park == parkToUpdate).Select(p => p.activity.name).ToList();
            List<string> park_t = _context.ParkTopics.Where(p => p.park == parkToUpdate).Select(p => p.topic.name).ToList();
            List<string> park_s = _context.StateParks.Where(p => p.park == parkToUpdate).Select(p => p.state.ID).ToList();

            CreatePark cp_edit = new CreatePark()
            {
                ID = parkToUpdate.ID,
                fullName = parkToUpdate.fullName,
                parkCode = parkToUpdate.parkCode,
                url = parkToUpdate.url,
                description = parkToUpdate.description,
                topicnames = park_t,
                activitynames = park_a,
                statenames = park_s
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = await _context.Activities.Select(p => p.name).ToListAsync();
            List<string> tnames = await _context.Topics.Select(p => p.name).ToListAsync();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;
            ViewBag.tnames = tnames;

            return View(cp_edit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("url,fullName,parkCode,description,statenames,activitynames,topicnames")] CreatePark modifiedp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Fetch the park that needs to be updated
                    Park ptobeupdated = _context.Parks
                        .Include(p => p.activities)
                        .Include(p => p.topics)
                        .Include(p => p.states)
                        .Where(p => p.ID == id)
                        .FirstOrDefault();

                    ptobeupdated.url = modifiedp.url;
                    ptobeupdated.fullName = modifiedp.fullName;
                    ptobeupdated.parkCode = modifiedp.parkCode;
                    ptobeupdated.description = modifiedp.description;

                    ptobeupdated.activities.Clear();

                    foreach (string aname in modifiedp.activitynames)
                    {
                        Activity a = _context.Activities.Where(a => a.name == aname).FirstOrDefault();
                        ParkActivity pa = new ParkActivity()
                        {
                            park = ptobeupdated,
                            activity = a
                        };
                        ptobeupdated.activities.Add(pa);
                    }


                    ptobeupdated.topics.Clear();

                    foreach (string tname in modifiedp.topicnames)
                    {
                        Topic t = _context.Topics.Where(t => t.name == tname).FirstOrDefault();
                        ParkTopic pt = new ParkTopic()
                        {
                            park = ptobeupdated,
                            topic = t
                        };
                        ptobeupdated.topics.Add(pt);
                    }

                    ptobeupdated.states.Clear();

                    foreach (string sname in modifiedp.statenames)
                    {
                        State s = _context.States.Where(s => s.ID == sname).FirstOrDefault();
                        StatePark sp = new StatePark()
                        {
                            park = ptobeupdated,
                            state = s
                        };
                        ptobeupdated.states.Add(sp);
                    }
                    _context.Update(ptobeupdated);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Thanks), new { message = "Thanks! The record has been edited." });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                // Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = await _context.Activities.Select(p => p.name).ToListAsync();
            List<string> tnames = await _context.Topics.Select(p => p.name).ToListAsync();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;
            ViewBag.tnames = tnames;
            return View(modifiedp);
        }

        public async Task<IActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Parks
                .Include(s => s.activities)
                    .ThenInclude(e => e.activity)
                .Include(s => s.topics)
                    .ThenInclude(e => e.topic)
                .Include(s => s.states)
                    .ThenInclude(e => e.state)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID.Equals(id));

            if (p == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Delete: " + p.parkCode;

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Park ptobedeleted = await _context.Parks
                .Include(p => p.activities)
                .Include(p => p.topics)
                .Include(p => p.states)
                .Where(p => p.ID == id)
                .FirstOrDefaultAsync();

            if (ptobedeleted == null)
            {
                return RedirectToAction(nameof(Search));
            }

            try
            {
                _context.Parks.Remove(ptobedeleted);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Thanks), new { message = "Thanks! The record has been deleted." });
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        public IActionResult Explore()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (Activity i in _context.Activities)
            {
                dict.Add(i.ID, i.name);
            }
            ViewBag.adict = dict;
            return View();
        }

        [HttpPost]
        public JsonResult Explore(String id)
        {
            List<object> chartTable = new List<object>();
            List<string> statelist = _context.States.Select(s => s.ID).ToList();
            List<int> pcount = new List<int>();
            string aname = _context.Activities.Where(a => a.ID == id).Select(a => a.name).FirstOrDefault();
            foreach (string s in statelist)
            {
                int parkCount = 0;
                if (id == "All")
                {
                    parkCount = _context.StateParks
                    .Where(p => p.state.ID == s)
                    .Select(p => p.park)
                    .Count();
                }
                else
                {
                    parkCount = _context.StateParks
                    .Where(p => p.state.ID == s)
                    .Select(p => p.park)
                    .Where(p => p.activities.Any(s => s.activity.ID == id))
                    .Count();
                }
                pcount.Add(parkCount);
            }
            chartTable.Add(statelist);
            chartTable.Add(pcount);
            chartTable.Add(aname);
            return Json(chartTable);
        }
    }
}
