using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotocopyRevaluationAppMVC.Models
{
    // Define a class to store session-related information
    public class SessionData
    {
        [Key]
        [Required]
        public int SessionId { get; set; } // Unique identifier for the session

        // Optional Foreign Key to the ApplicationUser (if using Identity)
        [Required]
        [ForeignKey("UserId")]
        public virtual int ApplicationUserId { get; set; } // Navigation property to ApplicationUser if needed

        public virtual ApplicationUser ApplicationUser { get; set; }  // Navigation property to the ApplicationUser entity

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Session Start Time")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; // When the session was created

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Session Expiration Time")]
        public DateTimeOffset ExpiryTime { get; set; } // When the session expires

        [StringLength(100)]
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; } // IP address of the user

        [StringLength(100)]
        [Display(Name = "User Agent")]
        public string UserAgent { get; set; } // Browser or device details

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Activity Time")]
        public DateTimeOffset LastActivityAt { get; set; } = DateTimeOffset.UtcNow; // Tracks last user activity in the session

        [StringLength(128)]
        [Display(Name = "Session Token")]
        public string SessionToken { get; set; } // Optional session token or JWT

        [StringLength(450)]
        [Display(Name = "Connection ID")]
        public string ConnectionId { get; set; } // Used for SignalR or real-time notifications

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true; // If the session is still active

        [Display(Name = "Is Persistent")]
        public bool IsPersistent { get; set; } // If the session should be persisted (Remember Me option)

        [StringLength(450)]
        [Display(Name = "Role")]
        public string Role { get; set; } // User's role for authorization

        [NotMapped]
        public bool IsExpired => ExpiryTime < DateTimeOffset.UtcNow; // Calculated property to check if session is expired
    }
}
