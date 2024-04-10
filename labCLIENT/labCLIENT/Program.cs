using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace labCLIENT
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:49343/api/");

                var responseTask = client.GetAsync("climb");
                responseTask.Wait();

                var GetResult = responseTask.Result;
                if (GetResult.IsSuccessStatusCode)
                {
                    var readTask = GetResult.Content.ReadAsAsync<SearchResultLine[]>();
                    readTask.Wait();

                    var climbs = readTask.Result;

                    foreach(SearchResultLine climb in climbs)
                    {
                        Console.WriteLine($"{ climb.Id} - { climb.ClimberName} - { climb.ClimberSurname} - { climb.Mountain} ");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
