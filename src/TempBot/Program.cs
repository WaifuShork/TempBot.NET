using System.Threading.Tasks;

namespace TempBot 
{
    internal static class Program 
    {
        private static async Task<int> Main()
        {
            return await TemplateBot.RunAsync();
        }
    }
}
