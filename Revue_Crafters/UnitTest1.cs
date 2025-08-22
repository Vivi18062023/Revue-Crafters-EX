using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using Revue_Crafters.Models;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Revue_Crafters
{
    [TestFixture]
    public class RevueApiTests
    {
        private RestClient _client;
        private static string lastRevueId;
        private const string BaseUrl = "https://d2925tksfvgq8c.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI3MGFjOTIyOC1jZTgxLTQ4YTAtOTI4Yy1kN2I2YTE0ZWIwNWQiLCJpYXQiOiIwOC8yMi8yMDI1IDA2OjIyOjQ3IiwiVXNlcklkIjoiMGMwMTU1MDItNDU5YS00NTM1LTEzMzAtMDhkZGRlMWQ4YTY0IiwiRW1haWwiOiJ2aXZpODRAZXhhbXBsZTg0LmNvbSIsIlVzZXJOYW1lIjoidml2aTg0IiwiZXhwIjoxNzU1ODY1MzY3LCJpc3MiOiJSZXZ1ZU1ha2VyX0FwcF9Tb2Z0VW5pIiwiYXVkIjoiUmV2dWVNYWtlcl9XZWJBUElfU29mdFVuaSJ9.ZgIV4TrZxIWzyrv9Y4VajyKeayLuFaLY-lkPDZWC1d4";

            var options = new RestClientOptions(BaseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };
            _client = new RestClient(options);
        }

        [Test, Order(1)]
        public void Test_CreateRevue()
        {
            var request = new RestRequest("/api/Revue/Create", Method.Post);
            request.AddJsonBody(new
            {
                title = "My Test Revue",
                description = "This is a test description"
            });

            var response = _client.Execute<ApiResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Msg, Is.EqualTo("Successfully created!"));
        }

        [Test, Order(2)]
        public void Test_GetAllRevues()
        {
            var request = new RestRequest("/api/Revue/All", Method.Get);
            var response = _client.Execute<List<RevueDTO>>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Count, Is.GreaterThan(0));

            lastRevueId = response.Data.Last().Id;
        }

        [Test, Order(3)]
        public void Test_EditRevue()
        {
            var request = new RestRequest($"/api/Revue/Edit?revueId={lastRevueId}", Method.Put);
            request.AddJsonBody(new
            {
                title = "Updated Revue Title",
                description = "Updated description"
            });

            var response = _client.Execute<ApiResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Msg, Is.EqualTo("Edited successfully"));
        }

        [Test, Order(4)]
        public void Test_DeleteRevue()
        {
            var request = new RestRequest($"/api/Revue/Delete?revueId={lastRevueId}", Method.Delete);
            var response = _client.Execute<ApiResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Msg, Is.EqualTo("The revue is deleted!"));
        }

        [Test, Order(5)]
        public void Test_CreateRevue_MissingFields()
        {
            var request = new RestRequest("/api/Revue/Create", Method.Post);
            request.AddJsonBody(new { });

            var response = _client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void Test_EditRevue_NotExisting()
        {
            var request = new RestRequest($"/api/Revue/Edit?revueId=123456789", Method.Put);
            request.AddJsonBody(new
            {
                title = "Fake Revue",
                description = "This does not exist"
            });

            var response = _client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Does.Contain("There is no such revue!"));
        }

        [Test, Order(7)]
        public void Test_DeleteRevue_NotExisting()
        {
            var request = new RestRequest($"/api/Revue/Delete?revueId=123", Method.Delete);
            var response = _client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Does.Contain("There is no such revue!"));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _client?.Dispose();
        }
    }
}
