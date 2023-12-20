using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Factory_shifts.Tables
{
    [Table("shifts")]
    public class Shifts
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("day")]
        public DateTime Day { get; set; }
        [Column("letter")]
        public string Letter { get; set; }
        [Column("time_shift_id")]
        public long TimeShiftId { get; set; }

    }
}
