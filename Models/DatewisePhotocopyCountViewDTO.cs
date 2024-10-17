using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationApp.Models {
    [NotMapped]
    public class DatewisePhotocopyCountViewDTO {
        public DateTime CreatedDate { get; set; }
        public int PhotocopyCount { get; set; }
    }
}
