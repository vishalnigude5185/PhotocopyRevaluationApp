using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationApp.Models;

public partial class Revaluation {
    [Key]
    public int SNO { get; set; }

    public string EventName { get; set; } = null!;

    public string Scheme { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string SubPostCode { get; set; } = null!;

    public string PinNumber { get; set; } = null!;

    public string StudentName { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public long PgiRefNo { get; set; }

    public int Amount { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string BillerId { get; set; } = null!;

    public int IsPaid { get; set; }
}
