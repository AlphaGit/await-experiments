using System.Threading.Tasks;

namespace TestAwaitApproaches
{
    public class SomeService
    {
        public Task<string> GetSomeValue()
        {
            return Task.Delay(1500).ContinueWith(_ => "Ok!");
        }

        public Task<string> GetAnotherValue(string initialValue)
        {
            return Task.Delay(1500).ContinueWith(_ => initialValue);
        }
    }
}
