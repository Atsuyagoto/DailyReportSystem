using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using System.ComponentModel; // 追記
using System.ComponentModel.DataAnnotations; // 追記
using System; // 追記

namespace DailyReportSystem.Models
{
    public class ApplicationUser : IdentityUser//プロパティを4つApplicationUserクラスに追加
    {
        [DisplayName("従業員名")]
        [Required(ErrorMessage = "氏名を入力してください。")]
        [StringLength(20, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string EmployeeName { get; set; } // 名前

        [DisplayName("作成日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } // データの作成された日時

        [DisplayName("更新日付")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; } // データの更新された日時

        public int DeleteFlg { get; set; } // Userの削除フラグ

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }
    }
    // ApplicationUser クラスにさらにプロパティを追加すると、ユーザーのプロファイル データを追加できます。詳細については、https://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
   

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}