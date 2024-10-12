using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationAppMVC.Models
{
    [NotMapped]
    public class DatewiseRevaluationCountDTO
    {
        public DateTime CreatedDate { get; set; }
        public int RevaluationCount { get; set; }
    }
}
