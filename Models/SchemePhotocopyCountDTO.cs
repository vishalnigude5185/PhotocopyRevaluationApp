using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationAppMVC.Models
{
    [NotMapped]
    public class SchemePhotocopyCountDTO
    {
        public string Scheme { get; set; }
        public int PhotocopyCount { get; set; }
    }
}
