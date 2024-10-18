using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationApp.Models;

public partial class Photocopy {
    [Key]
    public int SNO { get; set; }

    public string? EventName { get; set; }

    public string? Scheme { get; set; }

    public string? Subject { get; set; }

    public string? SubPostCode { get; set; }

    public string? PinNumber { get; set; }

    public string? StudentName { get; set; }

    public string? OrderId { get; set; }

    public long? PgiRefNo { get; set; }

    public int? Amount { get; set; }

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? IsPaid { get; set; }

    public string? BillerId { get; set; }
}
