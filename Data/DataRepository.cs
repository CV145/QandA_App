using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Identity.Client;
using QandA_App.Data.Models;
using static Dapper.SqlMapper;

namespace QandA_App.Data
{
    //Methods for reading/writing data to database
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            //Getting the connection string from appsettings.json
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public AnswerGetResponse GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<AnswerGetResponse>(
                    @"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @AnswerId",
                    new {AnswerId = answerId}
                    );
            }
        }


        //Declare a new database connection
        public IEnumerable<QuestionGetManyResponse> GetQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                /*automatically dispose the object defined in the using block after
                    *the program exits, so SqlConnection exists here only
                    *
                    *Ensures the connection is disposed
                    *
                    *SqlConnection
                    */
                connection.Open(); //open the connection

                //Execute stored procedure query
                //QuestionGetManyResponse is the model class to store the query results in
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany"
                    );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Dictionary of questions with key = question ID
                var questionDictionary = new Dictionary<int, QuestionGetManyResponse>();
                //Execute query using Dapper
                return connection.Query<QuestionGetManyResponse, AnswerGetResponse, QuestionGetManyResponse>
                    ("EXEC dbo.Question_GetMany_WithAnswers", map: (q, a) =>
                    {
                        QuestionGetManyResponse question;
                        if (!questionDictionary.TryGetValue(q.QuestionId, out question))
                        {
                            question = q;
                            question.Answers = new List<AnswerGetResponse>();
                            questionDictionary.Add(question.QuestionId, question);
                        }
                        question.Answers.Add(a);
                        return question;
                    },
                    splitOn: "QuestionId"
                    ).Distinct().ToList();
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //execute Question_GetMany_BySearch stored procedure
                //the stored procedures are in the database connected using the connection string
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany_BySearch @Search = @Search",
                    new { Search = search } //SQL passed in as a parameter
                    );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return connection.Query<QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany_BySearch_WithPaging @Search = @Search, @PageNumber = @PageNumber, @PageSize = @PageSize", parameters);
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Return a collection of records
                return connection.Query<QuestionGetManyResponse>(
                    "EXEC dbo.Question_GetUnanswered");
            }
        }

        //GET a question by id
        public QuestionGetSingleResponse GetQuestion(int questionId)
        {
            Console.WriteLine("Getting question with ID " + questionId);
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //Getting multiple results from Dapper query (2 stored procedures in a single database round trip), stored into a GridReader
                using (GridReader results = connection.QueryMultiple(@"EXEC dbo.Question_GetSingle @QuestionId = @QuestionId; EXEC dbo.Answer_Get_ByQuestionId @QuestionId = @QuestionId",
                    new { QuestionId = questionId }))
                {
                    //Read() the next Grid in results
                    var question = results.Read<QuestionGetSingleResponse>().FirstOrDefault();
                    if (question != null)
                    {
                        question.Answers = results.Read<AnswerGetResponse>().ToList();
                    }
                    return question;
                }
            }
        }

        public bool QuestionExists(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<bool>(
                    @"EXEC dbo.Question_Exists @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                    );
            }
        }

        public QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Execute Question_Post stored procedure, which returns questionID
                var questionId = connection.QueryFirst<int>(
                    @"EXEC dbo.Question_Post @Title = @Title, @Content = @Content,
                    @UserId = @UserId, @UserName = @UserName, @Created = @Created",
                    question
                    );
                return GetQuestion(questionId);
            }
        }

        public QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"EXEC dbo.Question_Put @QuestionId = @QuestionId, @Title = @Title, @Content = @Content",
                    new { QuestionId = questionId, question.Title, question.Content}
                    );
                return GetQuestion(questionId);
            }
        }

        public void DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Execute to run stored procedure
                connection.Execute(@"EXEC dbo.Question_Delete @QuestionId = @QuestionId",
                    new { QuestionId = questionId });
            }
        }

        public AnswerGetResponse PostAnswer(AnswerPostRequest answer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<AnswerGetResponse>(@"EXEC dbo.Answer_Post @QuestionId = @QuestionId,
                @Content = @Content, @UserId = @UserId, @UserName = @UserName, @Created = @Created", answer);
            }
        }

        IEnumerable<QuestionGetManyResponse> IDataRepository.GetQuestionsWithAnswers()
        {
            throw new NotImplementedException();
        }

        public AnswerGetResponse PostAnswer(AnswerPostFullRequest answer)
        {
            throw new NotImplementedException();
        }
    }
}
