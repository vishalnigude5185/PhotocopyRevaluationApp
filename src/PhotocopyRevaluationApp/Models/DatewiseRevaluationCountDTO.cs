using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class DatewiseRevaluationCountDTO {
        public DateTime CreatedDate { get; set; }
        public int RevaluationCount { get; set; }
    }
}
