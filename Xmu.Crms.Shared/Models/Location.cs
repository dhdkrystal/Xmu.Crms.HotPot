using System.ComponentModel.DataAnnotations.Schema;

namespace Xmu.Crms.Shared.Models
{
    public class Location
    {
        public long Id { get; set; }

        [Column("class_id")]
        public long ClassInfoId { get; set; }

        [ForeignKey("ClassInfoId")]
        public ClassInfo ClassInfo { get; set; }

        [Column("seminar_id")]
        public long SeminarId { get; set; }

        [ForeignKey("SeminarId")]
        public Seminar Seminar { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }
        //0表示签到结束，1表示正在签到
        public int? Status { get; set; }
    }
}