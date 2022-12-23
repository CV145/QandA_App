using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace QandA_App.Data.Models
{
    public class QuestionPutRequest
    {
        [StringLength(100)]
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
