using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DailyReportSystem.Models
{
    public class ReportsIndexViewModel
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        public string Title { get; set; }

        [DisplayName("内容")]
        public string Content { get; set; }

    }
    public class ReportsCreateViewModel
    {
        [DisplayName("日付")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "日報の日付を入力してください。")]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        [Required(ErrorMessage = "タイトルを入力してください。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string Title { get; set; }

        [DisplayName("内容")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string Content { get; set; }
    }
    public class ReportsDetailsViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("氏名")]
        public string EmployeeName { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        public string Title { get; set; }

        [DisplayName("内容")]
        public string Content { get; set; }

        [DisplayName("作成日付")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("更新日付")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        //このレポートを作成した人なら、「この日報を編集する」リンクを出すために使用。
        public bool isReportCreater { get; set; }
    }
    public class ReportsEditViewModel
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("日付")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "日報の日付を入力してください。")]
        public DateTime? ReportDate { get; set; }

        [DisplayName("タイトル")]
        [Required(ErrorMessage = "タイトルを入力してください。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字を超えることはできません。")]
        public string Title { get; set; }

        [DisplayName("内容")]
        [Required(ErrorMessage = "内容を入力してください。")]
        public string Content { get; set; }

    }

}