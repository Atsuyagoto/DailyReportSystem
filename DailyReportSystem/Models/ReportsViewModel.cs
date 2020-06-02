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
}