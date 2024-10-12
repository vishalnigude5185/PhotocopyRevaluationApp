using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationAppMVC.Models
{
    [NotMapped]
    public class EventRevaluationCountDTO
    {
        public string Event { get; set; }
        public int RevaluationCount { get; set; }
    }
}
