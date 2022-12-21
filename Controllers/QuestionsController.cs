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
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return _dataRepository.GetQuestions();
            }
            else
            {
                //call data repository question search
                return _dataRepository.GetQuestionsBySearch(search);
            }
        }
    }
}
