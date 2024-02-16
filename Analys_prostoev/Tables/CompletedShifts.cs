using System;

namespace Analys_prostoev.Tables
{
    public class CompletedShifts
    {
        public int Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Letter { get; set; }
    }
}
