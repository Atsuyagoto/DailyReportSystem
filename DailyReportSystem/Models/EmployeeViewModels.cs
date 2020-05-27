using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


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

}