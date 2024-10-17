namespace PhotocopyRevaluationApp.Models {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Log {
        [Key]  // Primary Key
        public int Id { get; set; }

        [Required]  // Non-nullable field
        public DateTime Timestamp { get; set; }

        [Required]  // Non-nullable field
        [MaxLength(50)]  // Max length of 50 for Log Level
        public string Level { get; set; }

        [Required]  // Non-nullable field
        public string Message { get; set; }

        // Nullable field for storing exception details if any
        public string Exception { get; set; }

        // Nullable field for storing context in JSON or other formats
        public string Context { get; set; }

        // Nullable field for storing user ID if applicable
        [MaxLength(255)]  // Max length of 255 for userId
        public string UserId { get; set; }

        // Nullable field for storing action that caused the log entry
        [MaxLength(255)]  // Max length of 255 for action
        public string Action { get; set; }

        // Nullable field for storing source location (e.g., module or class)
        [MaxLength(255)]  // Max length of 255 for location
        public string Location { get; set; }

        // Nullable field for storing IP address, maximum length of 50 characters
        [MaxLength(50)]
        public string IpAddress { get; set; }

        // Nullable field for storing correlation ID for request tracking
        [MaxLength(255)]
        public string CorrelationId { get; set; }

        // Nullable field for the duration in milliseconds (optional)
        public int? DurationMs { get; set; }

        // Nullable field for storing any custom data (as JSON or XML)
        public string CustomData { get; set; }

        // Field to store when the log entry was created, defaulting to current date/time
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]  // Default value set at the database level
        public DateTime CreatedAt { get; set; }
    }
}
