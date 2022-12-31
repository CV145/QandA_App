using Microsoft.Extensions.Caching.Memory;
using QandA_App.Data.Models;

namespace QandA_App.Data
{
    public class QuestionCache : IQuestionCache
    {
        //Create a memory cache
        private MemoryCache _cache { get; set; }
        public QuestionCache(MemoryCache cache)
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                //Limit the memory the cache takes up on web server
                SizeLimit = 100
            });
        }

        //Question-1 <- cache key
        private string GetCacheKey(int questionId) => $"Question-{questionId}";

        //Get a cached question - will return null if question doesn't exist in cache
        public QuestionGetSingleResponse Get(int questionId)
        {
            QuestionGetSingleResponse question;
            _cache.TryGetValue(GetCacheKey(questionId), out question);
            return question;
        }

        //Remove a cached question
        public void Remove(int questionId)
        {
            _cache.Remove(GetCacheKey(questionId));
        }

        //Add a new cached question
        public void Set(QuestionGetSingleResponse question)
        {
            //Question size = 1, Memory cache limit = 100 questions
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cache.Set(GetCacheKey(question.QuestionId), question, cacheEntryOptions);
        }
    }
}
