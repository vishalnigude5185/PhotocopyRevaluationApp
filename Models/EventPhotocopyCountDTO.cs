using PhotocopyRevaluationAppMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationAppMVC.Models
{
    [NotMapped]
    public class EventPhotocopyCountDTO
    {
        public string Event { get; set; }
        public int PhotocopyCount { get; set; }
    }
}

