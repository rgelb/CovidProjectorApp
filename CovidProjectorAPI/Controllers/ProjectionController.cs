using CovidProjectorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CovidProjectorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectionController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public ProjectionController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [HttpGet]
        [Route("CaliforniaCounties")]
        public async Task<IEnumerable<CountyAverageModel>> CaliforniaCounties(string limitToCounties = "")
        {

            string url = "https://data.ca.gov/api/3/action/datastore_search_sql?sql=SELECT * from \"926fd08f-cc91-4828-af38-bd45de97f8c3\" WHERE \"date\" > '{date}'";
            url = url.Replace("{date}", DateTime.Today.AddDays(-15).ToString("MM/dd/yyyy"));


            //string url = "https://data.ca.gov/api/3/action/datastore_search?resource_id=926fd08f-cc91-4828-af38-bd45de97f8c3&limit=1000";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {

                var statsDict = new Dictionary<string, List<CountyDayModel>>();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var stateModel  = await JsonSerializer.DeserializeAsync<CountiesCovidDataModel>(responseStream);

                foreach (Record item in stateModel.result.records)
                {
                    var dayModel = new CountyDayModel
                    {
                        Name = item.county,
                        Cases = Convert.ToInt32(item.newcountconfirmed),
                        Deaths = Convert.ToInt32(item.newcountdeaths),
                        Date = item.date
                    };

                    List<CountyDayModel> statRecords;
                    if (statsDict.ContainsKey(dayModel.Name.ToLower())) {
                        statRecords = statsDict[dayModel.Name.ToLower()];
                    } 
                    else
                    {
                        statRecords = new List<CountyDayModel>();
                        statsDict.Add(dayModel.Name.ToLower(), statRecords);
                    }
                    statRecords.Add(dayModel);
                }


                var avgModels = new List<CountyAverageModel>();
                // now get averages
                foreach (string countyName in statsDict.Keys)
                {
                    var avgModel = new CountyAverageModel { Name = countyName };

                    double value = statsDict[countyName].OrderByDescending(d => d.Date).Take(14).Average(t => t.Cases);
                    avgModel.AverageCases14 = (int) Math.Ceiling(value);

                    value = statsDict[countyName].OrderByDescending(d => d.Date).Take(14).Average(t => t.Deaths);
                    avgModel.AverageDeaths14 = (int)Math.Ceiling(value);

                    value = statsDict[countyName].OrderByDescending(d => d.Date).Take(7).Average(t => t.Cases);
                    avgModel.AverageCases7 = (int)Math.Ceiling(value);

                    value = statsDict[countyName].OrderByDescending(d => d.Date).Take(7).Average(t => t.Deaths);
                    avgModel.AverageDeaths7 = (int)Math.Ceiling(value);

                    value = statsDict[countyName].OrderByDescending(d => d.Date).Take(3).Average(t => t.Cases);
                    avgModel.AverageCases3 = (int)Math.Ceiling(value);

                    value = statsDict[countyName].OrderByDescending(d => d.Date).Take(3).Average(t => t.Deaths);
                    avgModel.AverageDeaths3 = (int)Math.Ceiling(value);

                    avgModels.Add(avgModel);
                }

                if (!string.IsNullOrWhiteSpace(limitToCounties)) {
                    var countiesToReturn = limitToCounties.ToLower().Split(",", StringSplitOptions.TrimEntries).ToList();

                    avgModels.RemoveAll(c => !countiesToReturn.Contains(c.Name));

                }

                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                avgModels.ForEach(z => { z.Name = myTI.ToTitleCase(z.Name); });

                return avgModels.OrderBy(c => c.Name);
            }
            else
            {
                return new List<CountyAverageModel>();
            }
        }

        [HttpGet]
        [Route("State")]
        public async Task<StateAverageModel> State(string state) {

            if (string.IsNullOrWhiteSpace(state)) {
                throw new Exception("Did I tell you not to include the state?");
            }


            string url = $"https://api.covidtracking.com/v1/states/{state}/daily.json";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var stateModels = await JsonSerializer.DeserializeAsync<List<StateRecord>>(responseStream);

            var avgModel = new StateAverageModel { Name = state.ToUpper() };

            double value = stateModels.OrderByDescending(d => d.dateModified).Take(14).Average(t => t.positiveIncrease);
            avgModel.AverageCases14 = (int)Math.Ceiling(value);

            value = stateModels.OrderByDescending(d => d.dateModified).Take(14).Average(t => t.deathIncrease);
            avgModel.AverageDeaths14 = (int)Math.Ceiling(value);

            value = stateModels.OrderByDescending(d => d.dateModified).Take(7).Average(t => t.positiveIncrease);
            avgModel.AverageCases7 = (int)Math.Ceiling(value);

            value = stateModels.OrderByDescending(d => d.dateModified).Take(7).Average(t => t.deathIncrease);
            avgModel.AverageDeaths7 = (int)Math.Ceiling(value);

            value = stateModels.OrderByDescending(d => d.dateModified).Take(3).Average(t => t.positiveIncrease);
            avgModel.AverageCases3 = (int)Math.Ceiling(value);

            value = stateModels.OrderByDescending(d => d.dateModified).Take(3).Average(t => t.deathIncrease);
            avgModel.AverageDeaths3 = (int)Math.Ceiling(value);

            return avgModel;
        }

        [HttpGet]
        [Route("Country")]
        public async Task<CountryAverageModel> Country() {
            string url = $"https://api.covidtracking.com/v1/us/daily.json";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var countryModels = await JsonSerializer.DeserializeAsync<List<CountryRecord>>(responseStream);

            var avgModel = new CountryAverageModel { Name = "US" };

            double value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(14).Average(t => t.positiveIncrease);
            avgModel.AverageCases14 = (int)Math.Ceiling(value);

            value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(14).Average(t => t.deathIncrease);
            avgModel.AverageDeaths14 = (int)Math.Ceiling(value);

            value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(7).Average(t => t.positiveIncrease);
            avgModel.AverageCases7 = (int)Math.Ceiling(value);

            value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(7).Average(t => t.deathIncrease);
            avgModel.AverageDeaths7 = (int)Math.Ceiling(value);

            value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(3).Average(t => t.positiveIncrease);
            avgModel.AverageCases3 = (int)Math.Ceiling(value);

            value = countryModels.OrderByDescending(d => d.DateCheckedAsDate).Take(3).Average(t => t.deathIncrease);
            avgModel.AverageDeaths3 = (int)Math.Ceiling(value);

            return avgModel;

        }
    }
}
