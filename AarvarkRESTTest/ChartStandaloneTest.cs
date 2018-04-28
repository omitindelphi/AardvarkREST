using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using AardvarkREST;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
//using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Configuration.EnvironmentVariables;
//using Microsoft.Extensions.Configuration;

//using Microsoft.Extensions.Options;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions;
using Microsoft.EntityFrameworkCore;
using AardvarkREST.Models;
using System.Net.Http.Headers;
//using System.Text;
//using Swashbuckle.AspNetCore.Swagger;
//using System.IO;

//using Microsoft.AspNetCore.Http;
//using Moq;
using Newtonsoft.Json;

namespace AardvarkRESTIntegrationTest
{
    public class ChartStandaloneTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        // Arrangements
        public ChartStandaloneTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<WFContext>();

            var serviceProvider = new ServiceCollection()
               .AddEntityFrameworkSqlServer()
               .BuildServiceProvider();

            //var connectionStringsAppSettings = new ConnectionStringsAppSettings();
            //configuration.GetSection("ConnectionStrings").Bind(connectionStringsAppSettings);

            _server = new TestServer(new WebHostBuilder()
                //.ConfigureServices(s => s.AddSingleton<IStartupConfigurationService, TestStartupConfigurationService>());
                //.AddDbContext<WFContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
                //.ConfigureAppConfiguration(configuration);
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private async Task<string> GetApiValuesResponseString(string querystring = "")
        {
            return await GetApiResponseString("api/values");
        }

        private async Task<string> GetApiResponseString(string api, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> GetApiResponse(string api, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;
            var response = await _client.GetAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return response;
        }

        private async Task<string> GetApiAnyResponse(string api, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;
            var response = await _client.GetAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return ((int)response.StatusCode).ToString() + " " + response.ReasonPhrase + "; " + content;
            // return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> PostApiResponseString(string api, string body, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;

            StringContent contentBody = new StringContent(body);
            contentBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync(request, contentBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            return response;
        }

        private async Task<HttpResponseMessage> SendDeleteRequest(string api, string body, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;

            StringContent contentBody = new StringContent(body);
            contentBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.DeleteAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return response;
        }

        private async Task<HttpResponseMessage> SendDeleteByIdRequest(string api, int id)
        {
            string request;
            request = api + id.ToString();
            StringContent contentBody = new StringContent("");
            contentBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.DeleteAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return response;
        }

        private async Task<HttpResponseMessage> SendUpdateRequest(string api, string body, string querystring = "")
        {
            string request;
            if (!string.IsNullOrEmpty(querystring))
            {
                request = api + "?" + querystring;
            }
            else
                request = api;

            StringContent contentBody = new StringContent(body);
            contentBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PutAsync(request, contentBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            return response;
        }

        /*
        [Fact]
        public async Task ReturnApiValuesGivenEmptyQueryString()
        {
            // Act
            var responseString = await GetApiValuesResponseString();

            // Assert
            Assert.Equal("[\"value1\",\"value2\"]",
                responseString);
        }
        */

        [Fact]
        public async Task Return_001_SuccessReadingExistingCharts()
        {
            // Act
            var responseString = await GetApiResponseString("api/WFCharts");

            // Assert
            WFChart[] expectedCharts = JsonConvert.DeserializeObject<WFChart[]>(responseString);

            //string expected = "[{\"chartId\":1,\"chartName\":\"TestChart\",\"chartDescription\":null},{\"chartId\":2,\"chartName\":\"SecondChart\",\"chartDescription\":null}]";
            Assert.True(expectedCharts.Count() > 0, "Empty list of WF Charts");
        }

        [Fact]
        public async Task Return_002_SuccessReadingExistingChartSingleByName()
        {
            // Act
            var actual = await GetApiResponseString("api/WFCharts/TestChart");

            // Assert
            //string expected = "{\"chartId\":1,\"chartName\":\"TestChart\",\"chartDescription\":null}";
            WFChart expectedChart = new WFChart { ChartId = 1, ChartName = "TestChart", ChartDescription = "" };
            WFChart actualChart = JsonConvert.DeserializeObject<WFChart>(actual);
            Assert.Equal(expectedChart.ChartId, actualChart.ChartId);
            Assert.Equal(expectedChart.ChartName, actualChart.ChartName);

        }

        [Fact]
        public async Task Return_003_FailureReadingNonExistingChartSingleByName()
        {
            // Act
            string MissChart = "Abracadabra";
            string response = await GetApiAnyResponse("api/WFCharts/" + MissChart);
            string expected = response;
            // Assert
            Assert.Equal(expected, string.Format("404 Not Found; \"Not found Chart Name '{0}'\"", MissChart));
        }

        [Fact]
        public async Task Return_004_SuccessCreateDeleteNewChart()
        {
            string newChart = "ZZZChart";
            await CreateDeleteChart(newChart);
        }


        [Fact]
        public async Task Return_005_SuccessCreateDeleteChartWithSpace()
        {
            string newChart = "YYY Chart";
            await CreateDeleteChart(newChart);
        }

        [Fact]
        public async Task Return_006_SuccessCreateDeleteInjectionAttack()
        {
            string newChart = "XXX';drop table owf.WFTask";
            await CreateDeleteChart(newChart);
        }

        [Fact]
        public async Task Return_007_SuccessCreateDeleteInjectionQuoteAttack()
        {
            string newChart = "XXX\";drop table owf.WFTask"; // quote char in the name
            await CreateChartFailureCheck(newChart);
        }

        [Fact]
        public async Task Return_008_SuccessCreateDeleteInjectionQuote2Attack()
        {
            string newChart = "XXX\\\";drop table owf.WFTask"; // quote char in the name
            await CreateDeleteChart(newChart);
        }


        [Fact]
        public async Task Return_009_FailureDeleteNonExistingChart()
        {
            string newChart = "oooDeleteAttempt";
            var ret = await SendDeleteRequest("api/WFCharts/" + newChart, "");
            var responseContent = await ret.Content.ReadAsStringAsync();
            int returnedDelCode = (int)ret.StatusCode;
            Assert.True(returnedDelCode == 404, "Returned code is not 404 (Not found) for deleting non-existing item");
        }

        [Fact]
        public async Task Return_010_SuccessUpdateChartDescriptionByBody()
        {
            // Act
            var originalChartSerialization = await GetApiResponseString("api/WFCharts/TestChart");
            WFChart returnedChart = JsonConvert.DeserializeObject<WFChart>(originalChartSerialization);

            string returnedDesc = returnedChart.ChartDescription;
            string updatedDesc = returnedDesc + "_1234567890";
            WFChart updatedChart = new WFChart { ChartId = 0, ChartName = returnedChart.ChartName, ChartDescription = updatedDesc };
            updatedChart.ChartName = returnedChart.ChartName;
            string sendBody = JsonConvert.SerializeObject(updatedChart);

            var respObj = SendUpdateRequest("api/WFCharts", sendBody);
            var updateContent = await respObj.Result.Content.ReadAsStringAsync();
            int returnedCode = (int)respObj.Result.StatusCode;
            Assert.True(returnedCode == 200, "Not returned Success from update attempt");
            WFChart updateReturnedChart = JsonConvert.DeserializeObject<WFChart>(updateContent);
            Assert.True(updateReturnedChart.ChartName == updatedChart.ChartName, "Update request returned mismatched ChartName");
            Assert.True(updateReturnedChart.ChartDescription == updatedChart.ChartDescription, "Update request returned mismatched ChartDescription");

            var restoreObj = SendUpdateRequest("api/WFCharts", originalChartSerialization);
            int restoreCode = (int)restoreObj.Result.StatusCode;
            Assert.True(restoreCode == 200, "Chart was not restored to original state");
        }

        [Fact]
        public async Task Return_011_SuccessUpdateChartDescriptionByRoute()
        {
            // Act
            string chartUpdate = "TestChart";
            var originalChartSerialization = await GetApiResponseString("api/WFCharts/" + chartUpdate);
            WFChart originalChart = JsonConvert.DeserializeObject<WFChart>(originalChartSerialization);

            string returnedDesc = originalChart.ChartDescription;
            string updatedDesc = returnedDesc + "_1234567890";

            var respObj = await SendUpdateRequest("api/WFCharts/" + chartUpdate + "/" + updatedDesc, "");
            var responseContent = await respObj.Content.ReadAsStringAsync();
            int returnedUpdCode = (int)respObj.StatusCode;
            Assert.True((returnedUpdCode >= 200 && returnedUpdCode <= 299), "Update Chart by route not success");
            WFChart updateReturnedChart = JsonConvert.DeserializeObject<WFChart>(responseContent);
            Assert.True(updateReturnedChart.ChartName == chartUpdate, "returned wrong ChartName");
            Assert.True(updateReturnedChart.ChartDescription == updatedDesc, "returned wrong ChartDescription");

            var restoreObj = SendUpdateRequest("api/WFCharts", originalChartSerialization);
            int restoreCode = (int)restoreObj.Result.StatusCode;
            Assert.True(restoreCode == 200, "Chart was not restored to original state");
        }

        private async Task CreateChartFailureCheck(string newChart)
        {
            string newChartJson = "{\"ChartId\":3,\"ChartName\":\"" + newChart + "\",\"ChartDescription\":null}";
            var returnedPost = await PostApiResponseString("api/WFCharts", newChartJson);

            int returnedCode = (int)returnedPost.StatusCode;
            Assert.True((returnedCode >= 400) && (returnedCode <= 499), "Returned code for Create post do not indicate failure when expected");
        }

        private async Task CreateDeleteChart(string newChart)
        {
            string newChartJson = "{\"ChartId\":3,\"ChartName\":\"" + newChart + "\",\"ChartDescription\":null}";
            var returnedPost = await PostApiResponseString("api/WFCharts", newChartJson);

            int returnedCode = (int)returnedPost.StatusCode;
            Assert.True((returnedCode >= 200) && (returnedCode <= 299), "Returned code for Create post do not indicate success");

            string actual = await returnedPost.Content.ReadAsStringAsync();

            var actualChart = JsonConvert.DeserializeObject<WFChart>(actual);
            var chartId = actualChart.ChartId;
            Assert.NotEmpty(chartId.ToString());

            string expected = newChartJson;
            WFChart expectedChart = JsonConvert.DeserializeObject<WFChart>(expected);
            Assert.Equal(expectedChart.ChartName, actualChart.ChartName);
            Assert.Equal(expectedChart.ChartDescription, actualChart.ChartDescription);

            var ret = await SendDeleteByIdRequest("api/WFCharts/DeleteById/", chartId);

            int returnedDelCode = (int)ret.StatusCode;
            Assert.True((returnedDelCode >= 200) && (returnedDelCode <= 299), "Returned code for Delete send do not indicate success");
        }

        private async Task CreateDeleteChartByName(string newChart)
        {
            string newChartJson = "{\"ChartId\":3,\"ChartName\":\"" + newChart + "\",\"ChartDescription\":null}";
            var returnedPost = await PostApiResponseString("api/WFCharts", newChartJson);

            int returnedCode = (int)returnedPost.StatusCode;
            Assert.True((returnedCode >= 200) && (returnedCode <= 299), "Returned code for Create post do not indicate success");

            string actual = await returnedPost.Content.ReadAsStringAsync();

            var actualChart = JsonConvert.DeserializeObject<WFChart>(actual);
            var chartId = actualChart.ChartId;
            Assert.NotEmpty(chartId.ToString());

            string expected = newChartJson;
            WFChart expectedChart = JsonConvert.DeserializeObject<WFChart>(expected);
            Assert.Equal(expectedChart.ChartName, actualChart.ChartName);
            Assert.Equal(expectedChart.ChartDescription, actualChart.ChartDescription);

            var ret = await SendDeleteRequest("api/WFCharts/", newChart);

            int returnedDelCode = (int)ret.StatusCode;
            Assert.True((returnedDelCode >= 200) && (returnedDelCode <= 299), "Returned code for Delete send do not indicate success");
        }

        [Fact]
        public async Task Return_012_ConflictUpdateDescriptionByBodyAndRouteRouteWin()
        {

            // Act
            string chartUpdate = "TestChart";
            var originalChartSerialization = await GetApiResponseString("api/WFCharts/" + chartUpdate);
            WFChart originalChart = JsonConvert.DeserializeObject<WFChart>(originalChartSerialization);

            string originalDesc = originalChart.ChartDescription;
            string routeDesc = "_RouteDescription";
            string bodyDesc = "_BodyDescription";

            WFChart updatedBodyChart = new WFChart { ChartId = 0, ChartName = originalChart.ChartName, ChartDescription = bodyDesc };
            string bodyWithDescription = JsonConvert.SerializeObject(updatedBodyChart);

            var respObj = await SendUpdateRequest("api/WFCharts/" + chartUpdate + "/" + routeDesc, bodyWithDescription);
            int returnedCode = (int)respObj.StatusCode;
            Assert.True((returnedCode >= 200) && (returnedCode <= 299), "Returned code for Update send do not indicate success");

            var updateContent = await respObj.Content.ReadAsStringAsync();
            WFChart respChart = JsonConvert.DeserializeObject<WFChart>(updateContent);
            //cleaning up 
            Assert.True((respChart.ChartDescription == routeDesc), "In case of discrepancy between body and route preference to route");
            var cleanObj = await SendUpdateRequest("api/WFCharts/" + chartUpdate + "/", "");
            var cleanContent = await cleanObj.Content.ReadAsStringAsync();

        }

        [Fact]
        public async Task Return_013_SuccessReadingExistingTasksFromCharts()
        {
            // Act
            var responseString = await GetApiResponseString("api/WFTask/TestChart");

            // Assert
            WFTask[] expectedTasks = JsonConvert.DeserializeObject<WFTask[]>(responseString);
            /*
                        [{"taskId":1,"chartName":"TestChart","taskName":"InTask","taskDescription":null},{"taskId":2,"chartName":"TestChart","taskName":"Process","taskDescription":null},{"taskId":3,"chartName":"TestChart","taskName":"Final","taskDescription":null}]
            */
            Assert.True(expectedTasks.Count() > 0, "Empty list of WF tasks from TestChart");
        }

        [Fact]
        public async Task Return_014_SuccessReadingSpecificTaskFromCharts()
        {
            var response = await GetApiResponseString("api/WFTask/TestChart/Task/InTask");

            WFTask expectedTask = JsonConvert.DeserializeObject<WFTask>(response);

            Assert.True(expectedTask.TaskName.Equals("InTask"), "Read unexpected task from TestChart workflow ");
        }

        [Fact]
        public async Task Return_015_SuccessCreateDeleteTask()
        {
            string aTask = "Abracadaber";
            var returnedPost = await PostApiResponseString("api/WFTask/TestChart/Task/"+aTask, "");
            int returnedCode = (int)returnedPost.StatusCode;
            Assert.True((returnedCode >= 200) && (returnedCode <= 299), "Returned code for Create post do not indicate success");

            string actual = await returnedPost.Content.ReadAsStringAsync();
            WFTask actualTask = JsonConvert.DeserializeObject<WFTask>(actual);

            int chartId = actualTask.ChartId;
            int taskId = actualTask.TaskId;
            var deletedResponse = await SendDeleteRequest("api/WFTask/"+ chartId.ToString() + "/Task/" + taskId.ToString(), "");
            int deletedcode = (int)deletedResponse.StatusCode;

            Assert.True((deletedcode >= 200) && (deletedcode <= 299), "Unexpected return from Delete Task request: " + deletedcode.ToString());
        }


        [Fact]
        public async Task Return_016_SuccessCreateUpdateTask()
        {
            string aTask = "Abracadaber";
            var returnedPost = await PostApiResponseString("api/WFTask/TestChart/Task/" + aTask, "");
            int returnedCode = (int)returnedPost.StatusCode;
            Assert.True((returnedCode >= 200) && (returnedCode <= 299), "Returned code for Create post do not indicate success");

            string created = await returnedPost.Content.ReadAsStringAsync();
            WFTask createdTask = JsonConvert.DeserializeObject<WFTask>(created);

            int chartId = createdTask.ChartId;
            int taskId = createdTask.TaskId;

            createdTask.TaskDescription = "Amazed Zovirax";
            var updatedResp = SendUpdateRequest("api/WFTask/" + createdTask.ChartName + "/Task/" + createdTask.TaskName + "/Description/" + createdTask.TaskDescription, "");
            int updatedCode = (int)updatedResp.Result.StatusCode;
            Assert.True((updatedCode >= 200) && (updatedCode <= 299), "Unexpected return from Update Task request: " + updatedCode.ToString());

            string updatedContent = await updatedResp.Result.Content.ReadAsStringAsync();
            WFTask updatedTask = JsonConvert.DeserializeObject<WFTask>(updatedContent);
            Assert.True((updatedTask.TaskId == createdTask.TaskId) && (updatedTask.TaskDescription == createdTask.TaskDescription));

            var deletedResponse = await SendDeleteRequest("api/WFTask/" + chartId.ToString() + "/Task/" + taskId.ToString(), "Mismatch on task update");
            int deletedcode = (int)deletedResponse.StatusCode;


            Assert.True((deletedcode >= 200) && (deletedcode <= 299), "Unexpected return from Delete Task request: " + deletedcode.ToString());
        }

        private async  Task<WFRoute> PostRoute(string ChartName, string aTaskFrom, string aTaskTo, string aRouteCode)
        {
            var returnedPost01 = await PostApiResponseString("api/WFRoute/" + ChartName + "/TaskFrom/" + aTaskFrom + "/TaskTo/" + aTaskTo + "/RouteCode/" + aRouteCode, "");
            int returnedCode01 = (int)returnedPost01.StatusCode;
            Assert.True((returnedCode01 >= 200) && (returnedCode01 <= 299), string.Format("Returned code for Create Route From '{0}' To '{1}' for Chart '{2}' post do not indicate success", aTaskFrom, aTaskTo, ChartName));
            string updatedRoute01 = await returnedPost01.Content.ReadAsStringAsync();
            WFRoute Route01 = JsonConvert.DeserializeObject<WFRoute>(updatedRoute01);
            return Route01;
        }

        private async Task<WFRoute> PostShortRoute(string ChartName, string aTaskFrom, string aTaskTo)
        {
            var returnedPost01 = await PostApiResponseString("api/WFRoute/" + ChartName + "/TaskFrom/" + aTaskFrom + "/TaskTo/" + aTaskTo , "");
            int returnedCode01 = (int)returnedPost01.StatusCode;
            Assert.True((returnedCode01 >= 200) && (returnedCode01 <= 299), string.Format("Returned code for Create Short Route From '{0}' To '{1}' for Chart '{2}' post do not indicate success", aTaskFrom, aTaskTo, ChartName));
            string updatedRoute01 = await returnedPost01.Content.ReadAsStringAsync();
            WFRoute Route01 = JsonConvert.DeserializeObject<WFRoute>(updatedRoute01);
            return Route01;
        }

        [Fact]
        public async Task Return_017_SuccessCreateRoute()
        {
            string aRoute = "Ok";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";
            string aTask03 = "Final";

            WFRoute Route01 = await PostRoute(aChart, aTask01, aTask02, aRoute);
            Assert.True(Route01.TaskFrom == aTask01, "Unexpected starting task for route");
            Assert.True(Route01.RouteCode == aRoute, "Unexpected starting Route Code");

            WFRoute Route02 = await PostRoute(aChart, aTask02, aTask03, aRoute);
            Assert.True(Route02.TaskTo == aTask03, "Unexpected final task for route");
            Assert.True(Route02.RouteCode == aRoute, "Unexpected final Route Code");
        }

        [Fact]
        public async Task Return_018_SuccessReadingAllRoutesFromChart()
        {
            string aRoute = "Ok";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";
            string aTask03 = "Final";
            WFRoute Route01 = await PostRoute(aChart, aTask01, aTask02, aRoute);
            WFRoute Route02 = await PostRoute(aChart, aTask02, aTask03, aRoute);

            var responseString = await GetApiResponseString("api/WFRoute/TestChart");
            WFRoute[] actualRoutes = JsonConvert.DeserializeObject<WFRoute[]>(responseString);
            Assert.True(actualRoutes.Count() > 0, "Empty list of WF routes from TestChart");
        }

        [Fact]
        public async Task Return_019_SuccessDeleteRoute()
        {
            string aRoute = "Ok";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";
            string aTask03 = "Final";
            WFRoute Route01 = await PostRoute(aChart, aTask01, aTask02, aRoute);
            WFRoute Route02 = await PostRoute(aChart, aTask02, aTask03, aRoute);
            var deletedResponse = await SendDeleteRequest(string.Format("api/WFRoute/{0}/TaskFrom/{1}/TaskTo/{2}", aChart, aTask02, aTask03), "");
            int deletedcode = (int)deletedResponse.StatusCode;
            Assert.True((deletedcode >= 200) && (deletedcode <= 299), string.Format("Returned code for Delete Route From '{0}' To '{1}' for Chart '{2}' post do not indicate success", aTask02, aTask03, aChart));

            var responseString = await GetApiResponseString("api/WFRoute/" + aChart);
            WFRoute[] actualRoutes = JsonConvert.DeserializeObject<WFRoute[]>(responseString);
            Assert.True(actualRoutes.Count() == 1, "Wrong number of returned routes");
        }

        private async void RemoveAllRoutes()
        {
            string aRoute = "Ok";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";
            string aTask03 = "Final";
            Task<WFRoute> Task01 = PostRoute(aChart, aTask01, aTask02, aRoute);
            Task01.Wait();
            WFRoute Route01 = Task01.Result;

            Task<WFRoute> Task02 = PostRoute(aChart, aTask02, aTask03, aRoute);
            Task02.Wait();
            WFRoute Route02 = Task02.Result;
            
            Task delTask01 = SendDeleteRequest(string.Format("api/WFRoute/{0}/TaskFrom/{1}/TaskTo/{2}", aChart, aTask02, aTask03), "");
            delTask01.Wait();
            
            Task delTask02 = SendDeleteRequest(string.Format("api/WFRoute/{0}/TaskFrom/{1}/TaskTo/{2}", aChart, aTask01, aTask02), "");
            delTask02.Wait();

            Task<string> controlTask = GetApiResponseString("api/WFRoute/" + aChart);
            controlTask.Wait();
            var responseString = controlTask.Result;
            WFRoute[] actualRoutes = JsonConvert.DeserializeObject<WFRoute[]>(responseString);

            Assert.True(actualRoutes.Count() == 0, "Wrong number of returned routes in no-route chart");
        }

        [Fact]
        public async Task Return_020_SuccessUpdateRoute()
        {
            string aRoute = "Ok";
            string sLongRoute = "OkTest";
            string sAnotherRoute = "OkAnother";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";

            RemoveAllRoutes();

            WFRoute RouteA02 = await PostRoute(aChart, aTask01, aTask02, sLongRoute);
            WFRoute RouteZ02 = await PostShortRoute(aChart, aTask01, aTask02);

            var updatedResp = SendUpdateRequest("api/WFRoute/" + aChart + "/TaskFrom/" + aTask01 + "/TaskTo/" + aTask02 + "/RouteCode/" + sAnotherRoute, "");
            int updatedCode = (int)updatedResp.Result.StatusCode;
            Assert.True((updatedCode >= 200) && (updatedCode <= 299), "Unexpected return from Update Task request: " + updatedCode.ToString());
            string updatedContent = await updatedResp.Result.Content.ReadAsStringAsync();
            WFRoute RouteAnother = JsonConvert.DeserializeObject<WFRoute>(updatedContent);

            Assert.True(RouteAnother.RouteCode == sAnotherRoute, "unexpected route after PUT update");
            Assert.True(RouteA02.RouteCode == sLongRoute, "Unexpected route after route creation");
            Assert.True(RouteZ02.RouteCode == aRoute, "Unexpected route after route update");
        }

        [Fact]
        public async Task Return_021_SuccessSingleRoute()
        {
            string aRoute = "Ok";
            string sLongRoute = "OkTest";
            string sAnotherRoute = "OkAnother";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";

            RemoveAllRoutes();

            await Task.WhenAll();
            WFRoute RouteA02 = await PostRoute(aChart, aTask01, aTask02, sLongRoute);
            await Task.WhenAll();

            var updatedResp = await GetApiResponseString("api/WFRoute/" + aChart + "/TaskFrom/" + aTask01 + "/TaskTo/" + aTask02 , "");
            WFRoute RouteAnother = JsonConvert.DeserializeObject<WFRoute>(updatedResp);

            Assert.True(RouteAnother.RouteCode == sLongRoute, string.Format("Obtained route {0}, expected {1}", RouteAnother.RouteCode, sLongRoute));

        }

        private async Task<WFItemStatus> ItemDropOff(string ChartName, string TaskName, string ItemName)
        {
            var returnedItem01 = await PostApiResponseString("api/WFItem/" + ChartName + "/TaskActionPut/" + TaskName + "/Item/" + ItemName, "");
            string insertedItemString = await returnedItem01.Content.ReadAsStringAsync();
            WFItemStatus ItemStatus = JsonConvert.DeserializeObject<WFItemStatus>(insertedItemString);
            return ItemStatus;
        }

        [Fact]
        public async Task Return_030_SuccessInsertItem()
        {
            string aRoute = "Ok";
            //string sLongRoute = "OkTest";
            //string sAnotherRoute = "OkAnother";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";

            string aItemName = Guid.NewGuid().ToString();

            RemoveAllRoutes();

            Task<WFRoute> TaskRouteA02 = PostRoute(aChart, aTask01, aTask02, aRoute);
            TaskRouteA02.Wait();
            
            Task<WFItemStatus> inserted = ItemDropOff(aChart, aTask01, aItemName);
            inserted.Wait();
            WFItemStatus ItemStatus01 = inserted.Result;
            Assert.True(ItemStatus01.ItemTaskStatus == WFItemTaskStatusValue.Completed, string.Format("Obtained status {0}, expected {1}", ItemStatus01.ItemTaskStatus, WFItemTaskStatusValue.Completed));
        }

        [Fact]
        public async Task Return_031_SuccessViewItemStatus()
        {
            string aRoute = "Ok";
            string aChart = "TestChart";
            string aTask01 = "InTask";
            string aTask02 = "Process";

            string aItemName = Guid.NewGuid().ToString();

            RemoveAllRoutes();

            Task<WFRoute> TaskRouteA02 = PostRoute(aChart, aTask01, aTask02, aRoute);
            TaskRouteA02.Wait();

            Task<WFItemStatus> inserted = ItemDropOff(aChart, aTask01, aItemName);
            inserted.Wait();
            WFItemStatus ItemStatus01 = inserted.Result;

            string allItemTasksString =  await GetApiResponseString("api/WFItem/" + aChart + "/Item/" + aItemName, "");
            WFItemStatus[] listItemStatuses = JsonConvert.DeserializeObject<WFItemStatus[]>(allItemTasksString);

            Assert.True(listItemStatuses.Count() >= 2, string.Format("Number of tasks {0}, expected {1}", listItemStatuses.Count(), 2));
        }


    }
}
