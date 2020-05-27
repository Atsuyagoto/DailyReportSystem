using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DailyReportSystem.Models;

using System.Threading.Tasks; // 追加
using Microsoft.AspNet.Identity; // 追加
using Microsoft.AspNet.Identity.Owin; // 追加
using Microsoft.AspNet.Identity.EntityFramework; // 追加


namespace DailyReportSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // 以下のフィールド、コンストラクタ、SignInManger、UserManagerを追加
        // このアプリケーション用のユーザーのサインインを管理するSignInManager
        private ApplicationSignInManager _signInManager;
        // このアプリケーション用のユーザー情報の管理をするUserManager
        private ApplicationUserManager _userManager;
        public EmployeesController()
        {

        }
        public EmployeesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Employees
        public ActionResult Index()
        {
            // ビューに送るためのEmployeesIndexViewModelのリストを作成
            List<EmployeesIndexViewModel> employees = new List<EmployeesIndexViewModel>();
            // ユーザー一覧を、作成日時が最近のものから順にしてリストとして取得
            List<ApplicationUser> users = db.Users.OrderByDescending(u => u.CreatedAt).ToList();
            // ユーザーのリストを、EmployeesIndexViewModelのリストに変換
            foreach (ApplicationUser applicationUser in users)
            {
                // EmployeesIndexViewModelをApplicationUserから必要なプロパティだけ抜き出して作成
                EmployeesIndexViewModel employee = new EmployeesIndexViewModel
                {
                    Email = applicationUser.Email,
                    EmployeeName = applicationUser.EmployeeName,
                    DeleteFlg = applicationUser.DeleteFlg,
                    Id = applicationUser.Id

                };
                // 作成したEmployeesIndexViewModelをリストに追加
                employees.Add(employee);
            }
            // 作成したリストをIndexビューに送る
            return View(employees);
        }

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Employees/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
    
        // GET: Employees/Create
        public ActionResult Create()
        {
            return View(new EmployeesCreateViewModel());
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EmployeeName,Email,Password,AdminFlag")]
                                    EmployeesCreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                // ビューから受け取ったEmployeesCreateViewModelからユーザー情報を作成
                ApplicationUser applicationUser =
                    new ApplicationUser
                    {
                // IdentityアカウントのUserNameにはメールアドレスを入れる必要がある
                UserName = model.Email,
                        Email = model.Email,
                        EmployeeName = model.EmployeeName,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        DeleteFlg = 0
                    };

                // ユーザー情報をDBに登録
                var result = await UserManager.CreateAsync(applicationUser, model.Password);
                // DB登録に成功した場合
                if (result.Succeeded)
                {
                    // TempDataにフラッシュメッセージを入れておく。
                    TempData["flush"] = String.Format("{0}さんを登録しました。", applicationUser.EmployeeName);

                    return RedirectToAction("Index", "Employees");
                }
                //DB登録に失敗したらエラー登録
                AddErrors(result);
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        // エラーがある場合、エラーメッセージを追加する
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Employees/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeName,CreatedAt,UpdatedAt,DeleteFlg,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
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
