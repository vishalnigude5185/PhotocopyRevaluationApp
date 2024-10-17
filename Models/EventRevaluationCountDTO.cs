using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class EventRevaluationCountDTO {
        public string Event { get; set; }
        public int RevaluationCount { get; set; }
    }
}
