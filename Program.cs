using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace web21_03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var url = "https://api.hh.ru/vacancies?industry=7&per_page=30&page=0&text=sql&area=72&only_with_salary=1";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(url).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            JObject obj = JObject.Parse(content);

            //foreach (var item in obj["items"])
            //{
            //    Console.WriteLine(item["name"]);
            //    Console.WriteLine(item["salary"]);
            //    Console.WriteLine();
            //}

            int sumAverageSalary = 0;
            int count = 0;

            foreach (JObject item in obj["items"])
            {

                if (item["salary"]["from"].Type != JTokenType.Null)
                {
                    if (item["salary"]["to"].Type != JTokenType.Null)
                    {
                        Console.WriteLine(item["name"]);
                        int averageSalary = ((int)(item["salary"]["from"]) + (int)(item["salary"]["to"])) / 2;
                        Console.WriteLine(averageSalary);
                        sumAverageSalary += averageSalary;
                        count++;
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine(item["name"]);
                        Console.WriteLine(item["salary"]["from"]);
                        sumAverageSalary += (int)item["salary"]["from"];
                        count++;
                        Console.WriteLine();
                    }
                }
                else
                {
                    if (item["salary"]["to"].Type != JTokenType.Null)
                    {
                        Console.WriteLine(item["name"]);
                        Console.WriteLine(item["salary"]["to"]);
                        sumAverageSalary += (int)item["salary"]["to"];
                        count++;
                        Console.WriteLine();
                    }
                    else
                        continue;
                }
            }
            Console.WriteLine("Средняя зарплата по данным профессиям: " + sumAverageSalary / count);

            Console.WriteLine("--------------------------------");

            var vacancies = obj["items"]
                .Where(item => item["salary"]["from"].Type != JTokenType.Null || item["salary"]["to"].Type != JTokenType.Null)
                .Select(item =>
                {
                    int from = (int?)item["salary"]["from"] ?? 0;
                    int to = (int?)item["salary"]["to"] ?? 0;
                    return (from + to) / 2;
                });

            Console.WriteLine("Средняя зарплата по данным профессиям: " + vacancies.Average());

            client.Dispose();
        }

    }
}

