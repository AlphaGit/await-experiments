using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestAwaitApproaches.Controllers
{
    public class TestController : Controller
    {
        private readonly SomeService _someService;

        public TestController(SomeService someService)
        {
            _someService = someService;
        }

        [HttpGet("await_service")]
        public async Task<string> await_service()
        {
            return await _someService.GetSomeValue();
        }

        [HttpGet("return_task_from_service")]
        public Task<string> return_task_from_service()
        {
            return _someService.GetSomeValue();
        }

        [HttpGet("wait_service")]
        public string wait_service()
        {
            var task = _someService.GetSomeValue();
            task.Wait();
            return task.Result;
        }
    }
}
