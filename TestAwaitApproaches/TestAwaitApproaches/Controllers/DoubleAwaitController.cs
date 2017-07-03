using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestAwaitApproaches.Controllers
{
    [Route("double_await")]
    public class DoubleAwaitController: Controller
    {
        private readonly SomeService _someService;

        public DoubleAwaitController(SomeService someService)
        {
            _someService = someService;
        }

        [HttpGet(nameof(await_service_twice))]
        public async Task<string> await_service_twice()
        {
            var value = await _someService.GetSomeValue();

            return await _someService.GetAnotherValue(value);
        }

        [HttpGet(nameof(wait_on_first_result))]
        public async Task<string> wait_on_first_result()
        {
            var valueTask = _someService.GetSomeValue();
            valueTask.Wait();

            return await _someService.GetAnotherValue(valueTask.Result);
        }

        [HttpGet(nameof(wait_on_both_results))]
#pragma warning disable 1998
        public async Task<string> wait_on_both_results()
#pragma warning restore 1998
        {
            var valueTask = _someService.GetSomeValue();
            valueTask.Wait();

            var resultTask = _someService.GetAnotherValue(valueTask.Result);
            resultTask.Wait();

            return resultTask.Result;
        }

        [HttpGet(nameof(use_result_then_await))]
        public async Task<string> use_result_then_await()
        {
            var value = _someService.GetSomeValue().Result;

            return await _someService.GetAnotherValue(value);
        }

        [HttpGet(nameof(use_result_then_result))]
#pragma warning disable 1998
        public async Task<string> use_result_then_result()
#pragma warning restore 1998
        {
            var value = _someService.GetSomeValue().Result;

            return _someService.GetAnotherValue(value).Result;
        }
    }
}
