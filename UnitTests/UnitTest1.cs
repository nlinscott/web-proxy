using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace UnitTests
{
    public class Tests
    {
        private class OpenApiVerificationModel<T>
        {
            public string OpenAPIUrl
            {
                get;
                set;
            }

            public string Path
            {
                get;
                set;
            }

            public T Body
            {
                get;
                set;
            }
        }

        private class OpenApiResponseModel
        {
            public Dictionary<string, string> Responses
            {
                get;
                set;
            }
        }

        private class MyOrderModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("petId")]
            public int PetId { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("shipDate")]
            public string ShipDate { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("complete")]
            public bool Complete { get; set; }
        }

        private class JunkOrderModel
        {
            [JsonProperty("someRandomNumbers")]
            public int SomeRandomNumbers { get; set; }

            [JsonProperty("petId")]
            public string PetId { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("shipDate")]
            public string ShipDate { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("complete")]
            public bool Complete { get; set; }
        }

        [Test]
        public async Task VerifySchema_CreateOrder_Success()
        {
            MyOrderModel orderModel = new MyOrderModel()
            {
                Id = 123,
                PetId = 154,
                Quantity = 1,
                ShipDate = "2023-06-01T14:54:50.346Z",
                Status = "approved",
                Complete = false
            };

            OpenApiVerificationModel<MyOrderModel> model = new OpenApiVerificationModel<MyOrderModel>()
            {
                Path = "/store/order",
                OpenAPIUrl = "https://raw.githubusercontent.com/swagger-api/swagger-petstore/master/src/main/resources/openapi.yaml",
                Body = orderModel
            };

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.PostAsJsonAsync("https://localhost:7072/verify", model);

                Assert.True(resp.IsSuccessStatusCode);

                OpenApiResponseModel response = await resp.Content.ReadFromJsonAsync<OpenApiResponseModel>();

                Assert.IsNotEmpty(response.Responses);

                Assert.True(response.Responses.TryGetValue("200", out string exampleJson));

                JObject jsonObject = JObject.Parse(exampleJson);

                IEnumerable<string> propertyNames = jsonObject.Properties().Select(p => p.Name).ToList();

                Assert.IsNotEmpty(propertyNames);

                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Id), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.PetId), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Status), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.ShipDate), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Quantity), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Complete), StringComparer.InvariantCultureIgnoreCase));

                MyOrderModel result = JsonConvert.DeserializeObject<MyOrderModel>(exampleJson);

                Assert.NotZero(result.Id);
                Assert.NotZero(result.PetId);
                Assert.NotZero(result.Quantity);
                Assert.NotNull(result.Status);
                Assert.NotNull(result.ShipDate);
                Assert.True(DateTimeOffset.TryParse(result.ShipDate, out _));
            }
        }

        [Test]
        public async Task VerifySchema_CreateOrder_Failure()
        {
            JunkOrderModel orderModel = new JunkOrderModel()
            {
                SomeRandomNumbers = 123,
                PetId = "1",
                Quantity = 1,
                ShipDate = "2023-06-01T14:54:50.346Z",
                Status = "approved",
                Complete = false
            };

            OpenApiVerificationModel<JunkOrderModel> model = new OpenApiVerificationModel<JunkOrderModel>()
            {
                Path = "/store/order",
                OpenAPIUrl = "https://raw.githubusercontent.com/swagger-api/swagger-petstore/master/src/main/resources/openapi.yaml",
                Body = orderModel
            };

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.PostAsJsonAsync("https://localhost:7072/verify", model);

                string errors = await resp.Content.ReadAsStringAsync();

                Assert.True(resp.IsSuccessStatusCode);

                OpenApiResponseModel response = await resp.Content.ReadFromJsonAsync<OpenApiResponseModel>();

                Assert.IsNotEmpty(response.Responses);

                Assert.True(response.Responses.TryGetValue("200", out string exampleJson));

                JObject jsonObject = JObject.Parse(exampleJson);

                IEnumerable<string> propertyNames = jsonObject.Properties().Select(p => p.Name).ToList();

                Assert.IsNotEmpty(propertyNames);

                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Id), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.PetId), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Status), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.ShipDate), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Quantity), StringComparer.InvariantCultureIgnoreCase));
                Assert.True(propertyNames.Contains(nameof(MyOrderModel.Complete), StringComparer.InvariantCultureIgnoreCase));

                MyOrderModel result = JsonConvert.DeserializeObject<MyOrderModel>(exampleJson);

                Assert.NotZero(result.Id);
                Assert.NotZero(result.PetId);
                Assert.NotZero(result.Quantity);
                Assert.NotNull(result.Status);
                Assert.NotNull(result.ShipDate);
                Assert.True(DateTimeOffset.TryParse(result.ShipDate, out _));
            }
        }
    }
}