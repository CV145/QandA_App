using System.ComponentModel.DataAnnotations;

namespace QandA_App.Data.Models
{
    public class QuestionPostRequest
    {
        [Required]
        [StringLength(100)] //A title containing more than 100 characters would cause a database error
        public string Title { get; set; }
        //Error message to validation on content property
        [Required(ErrorMessage = "Please include some content for the question")]
        public string Content { get; set; }
    }
}
