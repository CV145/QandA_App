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
    }
}
