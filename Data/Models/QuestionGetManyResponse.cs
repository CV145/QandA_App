namespace QandA_App.Data.Models
{
    public class QuestionGetManyResponse
    {
        //Properties that match the fields output from Question_GetMany stored procedure
        //Dapper automatically maps data from database to this class
        //Property names must match table columns
        public int QuestionId { get; set; }
        public string Title { get; set; }   
        public string Content { get; set; } 
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
