using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin; // 追加
using Microsoft.AspNet.Identity.EntityFramework; // 追加


namespace DailyReportSystem.Models
{
    // ファイル名とクラス名が違うので注意しましょう。
    public class EmployeesIndexViewModel
    {
        public string Id { get; set; } // IdentityのId

        [DisplayName("メールアドレス")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        public int DeleteFlg { get; set; } // Userの削除フラグ
    }
    public enum RolesEnum
    {
        [Display(Name = "一般")]
        Normal = 1,
        [Display(Name = "管理者")]
        Admin = 2
    }

    public class EmployeesCreateViewModel
    {
        [DisplayName("メールアドレス")]
        [EmailAddress]
        [Required(ErrorMessage = "メールアドレスを入力してください。")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        [Required(ErrorMessage = "氏名を入力してください。")]
        [StringLength(20, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string EmployeeName { get; set; } // 名前

        [DataType(DataType.Password)]
        [DisplayName("パスワード")]
        [Required(ErrorMessage = "パスワードを入力してください。")]
        public string Password { get; set; }

        [DisplayName("権限")]
        [Required(ErrorMessage = "権限を選択してください。")]
        public RolesEnum? AdminFlag { get; set; } // 管理者フラグ
    }
    public class EmployeesEditViewModel
    {
        public string Id { get; set; }

        [DisplayName("メールアドレス")]
        [EmailAddress]
        [Required(ErrorMessage = "メールアドレスを入力してください。")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        [Required(ErrorMessage = "氏名を入力してください。")]
        [StringLength(20, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string EmployeeName { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("パスワード")]
        public string Password { get; set; }

        [DisplayName("権限")]
        [Required(ErrorMessage = "権限を選択してください。")]
        public RolesEnum? AdminFlag { get; set; } // 管理者フラグ
    }
    public class EmployeesDetailsViewModel
    {
        public string Id { get; set; }

        [DisplayName("メールアドレス")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        [DisplayName("登録日時")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("権限")]
        public string Role { get; set; }
    }
    public class EmployeesDeleteViewModel
    {
        public string Id { get; set; }

        [DisplayName("メールアドレス")]
        public string Email { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        [DisplayName("登録日時")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("権限")]
        public string Role { get; set; }

    }

}