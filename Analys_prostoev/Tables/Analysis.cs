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
        [Column("shifts")]
        public string Shifts { get; set; }

    }
}
