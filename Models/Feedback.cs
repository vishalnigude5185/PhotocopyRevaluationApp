namespace PhotocopyRevaluationAppMVC.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }          // Unique identifier for the feedback
        public string UserName { get; set; }          // Name of the user providing feedback
        public string Email { get; set; }             // Email of the user (optional)
        public string Comments { get; set; }          // User's feedback or comments
        public int Rating { get; set; }               // Rating (1-5 or any scale)
        public DateTime SubmittedOn { get; set; }     // Date when feedback was submitted

        // Optional properties
        public bool IsResolved { get; set; }          // Flag to check if the feedback has been addressed
        public string Response { get; set; }          // Response from the team or admin
    }
}
