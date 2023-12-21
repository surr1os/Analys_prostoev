using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Analys_prostoev.Tables
{
    [Table("time_shifts")]
    public class TimeShifts
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("time_begin")]
        public TimeSpan? TimeBegin { get; set; }
        [Column("time_end")]
        public TimeSpan? TimeEnd { get; set; }
    }
}
