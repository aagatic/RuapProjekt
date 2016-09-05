// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CallRequestResponseService
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                Console.WriteLine("Unesite kapacitet stadiona: ");
                string input_kapacitet = Console.ReadLine();
                int kapacitet;
                Int32.TryParse(input_kapacitet, out kapacitet);
                Console.WriteLine("\n");

                Console.WriteLine("Unesite temperaturu u gradu: ");
                string input_temp = Console.ReadLine();
                int temp;
                Int32.TryParse(input_temp, out temp);
                Console.WriteLine("\n");

                List<string> prognoza = new List<string> { "vedro", "oblačno", "olujna grmljavina", "kiša", "maglovito", "snijeg" };
                Console.WriteLine("Unesite vrijeme u gradu: ");
                Console.WriteLine("Moguci unosi su: ");
                prognoza.ForEach(i => Console.WriteLine("{0}", i));
                Console.WriteLine("\n");
                Console.Write("Vas unos:");
                string vrijeme = Console.ReadLine();
                Console.WriteLine("\n");

                Console.WriteLine("Da li je ovo utakmica između rivala?(0 - NE, 1 - DA)");
                string input_rival = Console.ReadLine();
                int rival;
                Int32.TryParse(input_rival, out rival);
                Console.WriteLine("\n");

                Console.WriteLine("Unesite broj stanovnika u gradu domaćina: ");
                string input_br_stan = Console.ReadLine();
                int br_stan;
                Int32.TryParse(input_br_stan, out br_stan);
                Console.WriteLine("\n");


                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Kapacitet", "Temperatura", "Vrijeme", "Rivali", "Broj_stanovnika"},
                                Values = new string[,] {{ "" + kapacitet, "" + temp, "" + vrijeme, "" + rival, "" + br_stan }}
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "4hgL26UX1NMeDyAfjeG0MU7WPizji/YCKt/rT7/N2rSKBPfPmsIGEUIMMOsIrSqXM/p11qhzCrc4uNz0cvXM8w=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/44b76b5ed0fc4b47bd3cce92361280e2/services/458e3c847562410aa0a33fc1a541a48d/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Broj gledatelja na utakmici: {0}", result.Substring(116,5));
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
