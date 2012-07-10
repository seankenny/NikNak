using System;

namespace NikNak.Models
{
    public class BloodGlucoseReading
    {
        public int Id { get; set; }
        public double ReadingValue { get; set; }
        public DateTime? ReadingDateTime { get; set; }
    }
}