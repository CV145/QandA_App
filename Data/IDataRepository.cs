using QandA_App.Data.Models;

namespace QandA_App.Data
{
    //DataRepository must implement these methods
    public interface IDataRepository
    {
        //Read data
        IEnumerable<QuestionGetManyResponse> GetQuestions();
        IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers(); //Get all questions from database and their answers
        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search);
        //Return questions in pages
        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize);
        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();
        QuestionGetSingleResponse GetQuestion(int questionId);
        bool QuestionExists(int questionId);
        AnswerGetResponse GetAnswer(int answerId);

        //Write data
        QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(AnswerPostFullRequest answer);
    }
}
