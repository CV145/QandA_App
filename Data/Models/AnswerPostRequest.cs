using System.ComponentModel.DataAnnotations;

namespace QandA_App.Data.Models
{
    //T? = Nullable<T>
    public class AnswerPostRequest
    {
        [Required]
        public int? QuestionId { get; set; }
        [Required]
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
