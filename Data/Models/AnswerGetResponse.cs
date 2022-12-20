namespace QandA_App.Data.Models
{
    //From Answer_Get_ByQuestionId stored procedure
    public class AnswerGetResponse
    {
        public int AnswerId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
