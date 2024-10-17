using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class SchemePhotocopyCountDTO {
        public string Scheme { get; set; }
        public int PhotocopyCount { get; set; }
    }
}
