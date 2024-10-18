using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class EventPhotocopyCountDTO {
        public string Event { get; set; }
        public int PhotocopyCount { get; set; }
    }
}

