using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PhotocopyRevaluationApp.Enums;

namespace PhotocopyRevaluationApp.Models {
    public class Notification {
        [Key]
        public int NotificationId { get; set; }  // Primary Key

        // ApplicationUser Information
        [Required]
        [ForeignKey("ApplicationUser")]  // Foreign key to the user who receives the notification
        public int ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }  // Navigation property to the ApplicationUser entity

        // Notification Content
        [Required]
        [StringLength(250)]  // Limiting the length of the message
        public string Message { get; set; }  // The content of the notification

        // Type of Notification (Info, Warning, Alert, etc.)
        [Required]
        [EnumDataType(typeof(NotificationType))]  // Ensure the Type is from the NotificationType enum
        public NotificationType Type { get; set; }  // Enum to classify the notification

        // Priority (Low, Normal, High)
        [Required]
        [EnumDataType(typeof(NotificationPriority))]  // Ensure the Priority is from the NotificationPriority enum
        public NotificationPriority Priority { get; set; }  // Enum for priority

        // Date and Time
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // When the notification was created

        public DateTime? ReadAt { get; set; }  // When the notification was read (nullable)

        // Status
        [NotMapped]  // This field is derived, so no need to store it in the database
        public bool IsRead => ReadAt.HasValue;  // True if ReadAt is not null

        // Link to Details (Optional, useful for action-based notifications)
        [StringLength(500)]  // Optional field for action URL
        public string ActionUrl { get; set; }  // URL for the action or details

        // Expiration Date
        public DateTime? ExpiresAt { get; set; }  // Optional expiration date for the notification
    }
}
