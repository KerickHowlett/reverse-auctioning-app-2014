﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;

namespace MVC_DATABASE.Controllers
{
    public class RFPsController : Controller
    {
        private EVICEntities db = new EVICEntities();

        // GET: RFPs
        public async Task<ActionResult> Index()
        {
            var rFPs = db.RFPs.Include(r => r.RFI).Include(r => r.TEMPLATE);
            return View(await rFPs.ToListAsync());
        }

        // GET: RFPs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rfpcreate.rfp = await db.RFPs.FindAsync(id);
            if (rfpcreate.rfp == null)
            {
                return HttpNotFound();
            }
            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == id
                         select r;

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            return View(rfpcreate);
        }

        // GET: RFPs/Create
        public ActionResult Create()
        {
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "RFP"
                           select x;
            var expired = from y in db.RFIs
                          where y.EXPIRES <= DateTime.Now
                          select y;
            ViewBag.RFIID = new SelectList(expired, "RFIID", "RFIID");
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");
            return View();
        }

        public RFPCreate rfpcreate = new RFPCreate();

        // POST: RFPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RFPCreate model)
        {
            if (ModelState.IsValid)
            {
                model.rfp.RFI = await db.RFIs.FindAsync(model.rfiid);
                var rFP = new RFP { RFIID = model.rfiid, CATEGORY = model.rfp.RFI.CATEGORY, TEMPLATEID = model.templateid, CREATED = DateTime.Now, EXPIRES = model.rfp.EXPIRES};
                db.RFPs.Add(rFP);
                if (model.RFPInviteList != null)
                {
                    foreach (var x in model.RFPInviteList)
                    {
                        var rfpinvite = new RFPINVITE { RFPID = rFP.RFPID, Id = x };

                        db.RFPINVITEs.Add(rfpinvite);
                    }
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        //if we got this far something failed, reload form
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "RFP"
                           select x;
            var expired = from y in db.RFIs
                          where y.EXPIRES <= DateTime.Now
                          select y;
            ViewBag.RFIID = new SelectList(expired, "RFIID", "RFIID");
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");
            return View(model);
        }

        // GET: RFPs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rfpcreate.rfp = await db.RFPs.FindAsync(id);
            if (rfpcreate.rfp == null)
            {
                return HttpNotFound();
            }
            var vendorRFIQuery = from v in db.VENDORs
                                 join c in db.RFIINVITEs
                                 on v.Id equals c.Id
                                 where c.RFIID == (int)rfpcreate.rfp.RFIID
                                 select v;

            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == id
                         select r;

            vendorRFIQuery = vendorRFIQuery.Where(x => !result.Contains(x));
            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorRFIQuery, "Id", "Organization");

            return View(rfpcreate);
        }

        // POST: RFPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RFPCreate model)
        {
            if (ModelState.IsValid)
            {
                var rFP = model.rfp;

                if (model.RFPInviteList != null)
                {
                    foreach (var x in model.RFPInviteList)
                    {
                        var RFPInvite = new RFPINVITE { RFPID = rFP.RFPID, Id = x, OFFER_PATH = string.Empty };
                        db.RFPINVITEs.Add(RFPInvite);
                    }
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Details", "RFPs", new { id = rFP.RFPID});
            }
            //if we got this far something failed, reload form
            var vendorRFIQuery = from v in db.VENDORs
                                 join c in db.RFIINVITEs
                                 on v.Id equals c.Id
                                 where c.RFIID == (int)rfpcreate.rfp.RFIID
                                 select v;

            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == model.rfp.RFPID
                         select r;
            vendorRFIQuery = vendorRFIQuery.Where(x => !result.Contains(x));
            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorRFIQuery, "Id", "Organization");

            return View(rfpcreate);
        }

        RFPResponse responsemodel = new RFPResponse();

        public async Task<ActionResult> VendorResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            responsemodel.rfp = await db.RFPs.FindAsync(id);

            if (responsemodel.rfp == null)
            {
                return HttpNotFound();
            }

            responsemodel.inviteList = new List<RFPINVITE>();
            responsemodel.vendorlist = new List<VENDOR>();
            foreach (var x in db.RFPINVITEs.ToList())
            {

                if (x.RFPID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(x.Id);
                    responsemodel.inviteList.Add(x);
                    responsemodel.vendorlist.Add(vendor);
                }
            }



            return View(responsemodel);
        }

        public FileResult DownloadOffer(string path)
        {

            //select vendors Id from RFPINVITE
            var InviteId = from x in db.RFPINVITEs
                           where x.OFFER_PATH == path
                           select x.Id;
            //Get vendor items from Id
            VENDOR vendor = db.VENDORs.Find(InviteId.FirstOrDefault());
            //select RFPID
            var rfpId = from y in db.RFPINVITEs
                        where y.OFFER_PATH == path
                        select y.RFPID;

            string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfpId.FirstOrDefault().ToString());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);


        }
        //// GET: RFPs/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RFP rFP = await db.RFPs.FindAsync(id);
        //    if (rFP == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rFP);
        //}

        //// POST: RFPs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    RFP rFP = await db.RFPs.FindAsync(id);
        //    db.RFPs.Remove(rFP);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //Vendor RFPs

        RFPVendorRespond.RFPList respondmodel = new RFPVendorRespond.RFPList();

        
        public async Task<ActionResult> VendorRespond(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            respondmodel.rfp = await db.RFPs.FindAsync(id);

            if (respondmodel.rfp == null)
            {
                return HttpNotFound();
            }

            respondmodel.inviteList = new List<RFPINVITE>();

            foreach (var x in db.RFPINVITEs.ToList())
            {

                if (x.RFPID == id)
                {

                    respondmodel.inviteList.Add(x);

                }
            }
            return View(respondmodel);
        }

        RFPVendorIndex rfpvendorindex = new RFPVendorIndex();

        
        public ActionResult VendorIndex()
        {

            EVICEntities dbo = new EVICEntities();

            var user_ids = User.Identity.GetUserId();

            rfpvendorindex.rfplist = new List<RFP>();

            var VendorRFPQuery = from r in dbo.RFPs
                                   join i in dbo.RFPINVITEs
                                   on r.RFPID equals i.RFPID
                                   where i.Id == user_ids
                                   orderby r.RFPID
                                   select r;

            rfpvendorindex.rfplist = VendorRFPQuery.ToList();

            return View(rfpvendorindex);
        }
    }
}