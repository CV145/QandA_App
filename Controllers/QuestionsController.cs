using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QandA_App.Data;
using QandA_App.Data.Models;

namespace QandA_App.Controllers
{
    //[controller] is substituted with questions
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        //Reference to repository
        private readonly IDataRepository _dataRepository;

        //Constructor - dependency injection to pass in data repository instance
        public QuestionsController(IDataRepository dataRepository)
        {
            //Reference to data repository
            _dataRepository = dataRepository;
        }

        //GET response. Data from query parameters is automatically mapped to action method params (model binding)
        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search, bool includeAnswers, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeAnswers)
                {
                    return _dataRepository.GetQuestionsWithAnswers();
                }
                else
                {
                    return _dataRepository.GetQuestions();
                }
            }
            else
            {
                //call data repository question search and paging
                return _dataRepository.GetQuestionsBySearchWithPaging(search, page, pageSize);
            }
        }

        [HttpGet("unanswered")]
        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            return _dataRepository.GetUnansweredQuestions();
        }

        //HTTP GET has an attribute parameter placed in subpath:
        //api/questions/3, questionId = 3
        //ActionResult can return a NotFoundResult if there is no QuestionGetSingleResponse
        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            //call the data repository to get the question
            var question = _dataRepository.GetQuestion(questionId);

            //return http status 404 if question not found
            if (question == null)
            {
                //ActionResult
                return NotFound(); 
            }

            //return question in response with status 200
            return question;
        }


        //Action method to handle POST requests to api/questions
        [HttpPost]
        public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            Console.WriteLine("Posting new question...");
            Console.WriteLine("Title: " + questionPostRequest.Title);
            Console.WriteLine("Content: " + questionPostRequest.Content);
            //call the data repository to save the question
            var savedQuestion = _dataRepository.PostQuestion(new QuestionPostFullRequest
            {
                //Use info contained in the post request
                Title = questionPostRequest.Title,
                Content= questionPostRequest.Content,
                UserId = "1",
                UserName = "bob.test@test.com",
                Created= DateTime.UtcNow,
            });

            Console.WriteLine("Question posted");
            Console.WriteLine("Saved title: " + savedQuestion.Title);
            Console.WriteLine("Saved content: " + savedQuestion.Content);
            Console.WriteLine("Saved id: " + savedQuestion.QuestionId);

            //return HTTP status code 201 - signify resource has been created
            return CreatedAtAction(nameof(GetQuestion), new { questionId = savedQuestion.QuestionId }, savedQuestion);
        }

        //Action method for updating a question
        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
        {
            //get the question from the data repository
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                //return HTTP status code 404 if question is not found
                return NotFound();
            }

            //update the question model
            questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ?
                question.Title : questionPutRequest.Title;
            questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ?
                question.Content : questionPutRequest.Content;

            //call the data repository with the updated question model to update the question
            var savedQuestion = _dataRepository.PutQuestion(questionId, questionPutRequest);

            //return the saved question
            return savedQuestion;
        }

        //Action method for deleting a question
        //Question ID included at the end of the path
        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            _dataRepository.DeleteQuestion(questionId);
            return NoContent();
        }

        [HttpPost("answer")]
        public ActionResult<AnswerGetResponse> PostAnswer(AnswerPostRequest answerPostRequest)
        {
            var questionExists = _dataRepository.QuestionExists(answerPostRequest.QuestionId.Value);
            if (!questionExists)
            {
                return NotFound();
            }
            var savedAnswer = _dataRepository.PostAnswer(new AnswerPostFullRequest
            {
                QuestionId = answerPostRequest.QuestionId.Value,
                Content = answerPostRequest.Content,
                UserId = "1",
                UserName = "bob.test@test.com",
                Created = DateTime.UtcNow
            });
            return savedAnswer;
        }
    }
}
