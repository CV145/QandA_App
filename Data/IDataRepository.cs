using QandA_App.Data.Models;

namespace QandA_App.Data
{
    //DataRepository must implement these methods
    public interface IDataRepository
    {
        //Read data
        IEnumerable<QuestionGetManyResponse> GetQuestions();
        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search);
        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();
        QuestionGetSingleResponse GetQuestion(int questionId);
        bool QuestionExists(int questionId);
        AnswerGetResponse GetAnswer(int answerId);

        //Write data
        QuestionGetSingleResponse PostQuestion(QuestionPostRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(AnswerPostRequest answer);
    }
}
