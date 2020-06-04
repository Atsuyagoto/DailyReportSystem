using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DailyReportSystem.Models;
using Microsoft.AspNet.Identity;//追加


namespace DailyReportSystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reports
        public ActionResult Index()
        {
            // 日報のリストから、表示用のビューモデルのリストを作成
            List<ReportsIndexViewModel> indexViewModels = new List<ReportsIndexViewModel>();
            var reports = db.Reports
                .OrderByDescending(r => r.ReportDate)
                .ToList();
            foreach (Report report in reports)
            {
                ReportsIndexViewModel indexViewModel = new ReportsIndexViewModel
                {
                    Id = report.Id,
                    // 従業員のリストからこの日報のEmployeeIdで検索をかけて取得した従業員の名前を設定
                    EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName,
                    ReportDate = report.ReportDate,
                    Title = report.Title,
                    Content = report.Content
                };
                indexViewModels.Add(indexViewModel);
            }

            return View(indexViewModels);
        }

        // GET: Reports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            ReportsDetailsViewModel detailsViewModel = new ReportsDetailsViewModel
            {
                Id = report.Id,
                ReportDate = report.ReportDate,
                Title = report.Title,
                Content = report.Content,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,
            };
            detailsViewModel.EmployeeName = db.Users.Find(report.EmployeeId).EmployeeName;
            detailsViewModel.isReportCreater = User.Identity.GetUserId() == report.EmployeeId;

            return View(detailsViewModel);
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View(new ReportsCreateViewModel());
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportDate,Title,Content")] ReportsCreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                Report report = new Report()
                {
                    ReportDate = createViewModel.ReportDate,
                    Title = createViewModel.Title,
                    Content = createViewModel.Content,
                    //現在ログイン中のUserIdを取得し、EmployeeIdとして設定
                    EmployeeId = User.Identity.GetUserId(),
                    //作成時は現在の時刻に設定
                    UpdatedAt = DateTime.Now,
                    //作成時は現在の時刻に設定
                    CreatedAt = DateTime.Now
                };

                //Contextに新しいオブジェクト追加
                db.Reports.Add(report);
                //実際のDBに反映
                db.SaveChanges();
                // TempDataにフラッシュメッセージを入れておく。
                TempData["flush"] = "日報を登録しました。";
                //indexにRedirect（ページ遷移）
                return RedirectToAction("Index");
            }

            return View(createViewModel);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }

            //本人の日報でなければ表示しないように、それをEdit.cshtmlにTempDataで伝える。
            if (report.EmployeeId != User.Identity.GetUserId())
            {
                TempData["wrong_person"] = "true";
            }
            ReportsEditViewModel editViewModel = new ReportsEditViewModel
            {
                Id = report.Id,
                ReportDate = report.ReportDate,
                Title = report.Title,
                Content = report.Content
            };
            return View(editViewModel);
        }

        // POST: Reports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ReportDate,Title,Content")] ReportsEditViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                Report report = db.Reports.Find(editViewModel.Id);
                report.ReportDate = editViewModel.ReportDate;
                report.Title = editViewModel.Title;
                report.Content = editViewModel.Content;
                report.UpdatedAt = DateTime.Now;
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();

                // TempDataにフラッシュメッセージを入れておく。
                TempData["flush"] = "日報を編集しました。";
                return RedirectToAction("Index");
            }
            return View(editViewModel);
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Report report = db.Reports.Find(id);
            db.Reports.Remove(report);
            db.SaveChanges();
            return RedirectToAction("Index");
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
