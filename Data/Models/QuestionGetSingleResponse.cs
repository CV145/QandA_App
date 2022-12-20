namespace QandA_App.Data.Models
{
    //Model for Question  data returned from Question_GetSingle stored procedure
    public class QuestionGetSingleResponse
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }   
        public string Content { get; set; } 
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<AnswerGetResponse> Answers { get; set; }
    }
}
