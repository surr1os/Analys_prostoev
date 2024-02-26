using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Analys_prostoev.Tables
{
    [Table("analysis")]
    public class Analysis
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("date_start")]
        public DateTime DateStart { get; set; }
        [Column("date_finish")]
        public DateTime DateFinish { get; set; }
        [Column("region")]
        public string Region { get; set; }
        [Column("period")]
        public int Period { get; set; }
        [Column("category_one")]
        public string CategoryOne { get; set; }
        [Column("category_two")]
        public string CategoryTwo { get; set; }
        [Column("category_hird")]
        public string CategoryThird { get; set; }
        [Column("reason")]
        public string Reason { get; set; }
        [Column("status")]
        public int? Status { get; set; }
        [Column("is_manual")]
        public bool IsManual { get; set; }
        [Column("shifts")]
        public string Shifts { get; set; }

    }
}
