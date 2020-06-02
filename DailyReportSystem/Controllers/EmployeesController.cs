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
    [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーが取得できなければ、NotFoundエラーページへ
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesDetailsViewModel employee = new EmployeesDetailsViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName,
                CreatedAt = applicationUser.CreatedAt,
                UpdatedAt = applicationUser.UpdatedAt
            };

            // RoleがAdminかどうか確認し、そうなら"管理者"、違うなら"一般"にする
            employee.Role = UserManager.IsInRole(applicationUser.Id, "Admin") ?
                "管理者" : "一般";

            return View(employee);
        }

        // POST: Employees/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。

        // GET: Employees/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View(new EmployeesCreateViewModel());
        }

        // POST: Employee/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Email,EmployeeName,Password,AdminFlag")]
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
                    // Roleを追加する
                    var roleManager = new RoleManager<ApplicationRole>(
                        new RoleStore<ApplicationRole>(new ApplicationDbContext())
                        );


                    // AdminロールがDBに存在しなければ
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        // AdminロールをDBに作成
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                    }

                    // mode.AdminFlagの内容によって、処理をswitchで変える。
                    switch (model.AdminFlag)
                    {
                        case RolesEnum.Admin:
                            // Adminロールをユーザーに対して設定
                            await UserManager.AddToRoleAsync(applicationUser.Id, "Admin");
                            break;
                    }


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
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesEditViewModel employee = new EmployeesEditViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName
            };

            //従業員の権限(role)がAdminならAdminに、そうでなければNormalにする。
            if (UserManager.IsInRole(applicationUser.Id, "Admin"))
            {
                employee.AdminFlag = RolesEnum.Admin;
            }
            else
            {
                employee.AdminFlag = RolesEnum.Normal;
            }

            return View(employee);
        }
        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
    [Bind(Include = "Id,Email,EmployeeName,Password,AdminFlag")] EmployeesEditViewModel employee)
        {

            if (ModelState.IsValid)
            {
                // DBからidのユーザーを検索し取得。そのユーザーに対し変更をする
                ApplicationUser applicationUser = db.Users.Find(employee.Id);
                // IdentityアカウントのUserNameにはメールアドレスを入れる必要がある
                applicationUser.UserName = employee.Email;
                applicationUser.Email = employee.Email;
                applicationUser.EmployeeName = employee.EmployeeName;
                applicationUser.UpdatedAt = DateTime.Now;
                // passwordが空でなければパスワード変更する。
                if (!String.IsNullOrEmpty(employee.Password))
                {
                    // パスワードの入力検証
                    var result = await UserManager.PasswordValidator.ValidateAsync(employee.Password);
                    // パスワードの検証に失敗したら、エラーを追加しEditビューをもう一度描画
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return View(employee);
                    }
                    // パスワードはハッシュ化したものをDBに登録する必要があるので、PasswordHasherでハッシュ化する
                    applicationUser.PasswordHash = UserManager.PasswordHasher.HashPassword(employee.Password);
                }
                // StateをModifiedにしてUPDATE文を行うように設定
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();

                // mode.AdminFlagの内容によって、処理をswitchで変える。
                switch (employee.AdminFlag)
                {
                    case RolesEnum.Admin:
                        //すでに管理者権限を持っているならbreakしてswitchを抜ける。
                        if (UserManager.IsInRole(applicationUser.Id, "Admin"))
                            break;
                        //Adminロールをユーザーに対して設定
                        UserManager.AddToRole(applicationUser.Id, "Admin");
                        break;

                    default:
                        //管理者以外が選ばれている時に、管理者権限を持っていた場合、管理者権限を消す。
                        if (UserManager.IsInRole(applicationUser.Id, "Admin"))
                        {
                            UserManager.RemoveFromRole(applicationUser.Id, "Admin");
                        }
                        break;
                }

                // TempDataにフラッシュメッセージを入れておく。TempDataは現在のリクエストと次のリクエストまで存在
                TempData["flush"] = String.Format("{0}さんの情報を更新しました。", applicationUser.EmployeeName);

                return RedirectToAction("Index", "Employees");
            }

            return View(employee);
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
            // idが無い場合、不正なリクエストとして処理
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーが取得できなければ、NotFoundエラーページへ
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // ビューモデルにデータを詰め替える
            EmployeesDeleteViewModel employee = new EmployeesDeleteViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                EmployeeName = applicationUser.EmployeeName,
                CreatedAt = applicationUser.CreatedAt,
                UpdatedAt = applicationUser.UpdatedAt
            };

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // DBからidで検索して該当するユーザーを取得
            ApplicationUser applicationUser = db.Users.Find(id);
            // ユーザーを論理削除
            applicationUser.DeleteFlg = 1;
            // StateをModifiedにしてUPDATE文を行うように設定
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();

            // TempDataにフラッシュメッセージを入れておく。TempDataは現在のリクエストと次のリクエストまで存在
            TempData["flush"] = String.Format("{0}さんの情報を削除しました。", applicationUser.EmployeeName);

            return RedirectToAction("Index", "Employees");
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
