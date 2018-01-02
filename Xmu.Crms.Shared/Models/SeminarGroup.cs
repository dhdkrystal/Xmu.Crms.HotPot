using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xmu.Crms.Shared.Models
{
    public class SeminarGroup
    {
        public long Id { get; set; }

        [Column("seminar_id")]
        public long SeminarId { get; set; }

        [ForeignKey("SeminarId")]
        public Seminar Seminar { get; set; }

        [Column("class_id")]
        public long ClassId { get; set; }

        [ForeignKey("ClassId")]
        public ClassInfo ClassInfo { get; set; }

        public string Report { get; set; }

        public int? ReportGrade { get; set; }

        public int? PresentationGrade { get; set; }

        public int? FinalGrade { get; set; }

        [Column("leader_id")]
        public long? LeaderId { get; set; }

        [ForeignKey("LeaderId")]
        public UserInfo Leader { get; set; }

        public IList<SeminarGroupMember> SeminarGroupMembers { get; set; }

        public IList<SeminarGroupTopic> SeminarGroupTopics { get; set; }
    }
}