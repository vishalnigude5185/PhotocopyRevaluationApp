using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class SchemeRevaluationCountDTO {
        public string Scheme { get; set; }
        public int RevaluationCount { get; set; }
    }
}
