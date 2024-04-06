using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers; // For AuthenticationHeaderValue and MediaTypeWithQualityHeaderValue
using System.Net.Http;        // For ProductInfoHeaderValue
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Data.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using WebApplication3.Data;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using WebApplication3.Migrations;

namespace WebApplication3
{
    public class AcronisTokenService
    {
        //private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseAPIUrl;
        private readonly string cachekey = "Access_Token";
        private readonly IMemoryCache _cache;
        private readonly string parentid = "5687532a-4df8-462b-a055-85de7f1d6e98";
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AcronisTokenService(IMemoryCache cache, DataContext context, UserManager<ApplicationUser> userManager)
        {

            _clientId = "c4411144-e048-4826-a56a-cf6e9a737350";
            _clientSecret = "isxcvtxgy5ugq2tewny2sr7cr436smyw5lq5xfpbtjqrq6zmno7y";
            string datacenter = "https://eu2-cloud.acronis.com";
            _baseAPIUrl = $"{datacenter}/api/2";
            _cache = cache;
            _context = context;
            _userManager = userManager;


        }
        public async Task<string> GetAccessToken()
        {
            if (_cache.TryGetValue(cachekey, out TokenResponse tokenResponse))
            {
                //token present
                string accesstoken = tokenResponse.access_token;
                return accesstoken;

            }
            else
            {

                //Access token not found 
                string accesstoken = await CreateToken();


                return accesstoken;

            }

        }

        public async Task<string> CreateToken()
        {
            string basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseAPIUrl}/idp/token");
            request.Headers.Add("Authorization", $"Basic {basicAuth}");

            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            request.Content = new FormUrlEncodedContent(requestData);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenInfo = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                string accessToken = tokenInfo.access_token;
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), // Set the absolute expiration to 3 hours
                    Priority = CacheItemPriority.Normal // Set the priority to Normal
                };


                _cache.Set(cachekey, accessToken, cacheEntryOptions);
                return accessToken;
            }
            else
            {
                // Handle authentication error (e.g., error response with a 400 status code)
                // You might want to throw an exception or log the error here.
            }

            return null; // Handle other cases or return null as needed.

        }

        public async Task<string> CreateTenant(string email)
        {
            if (_cache.TryGetValue(cachekey, out TokenResponse tokenResponse))
            {
                //token present
                string accesstoken = tokenResponse.access_token;
                string tenantid = await creatingtenant(accesstoken, email);
              
                return tenantid;

            }
            else
            {

                //Access token not found 
                string accesstoken = await CreateToken();
                string tenantid = await creatingtenant(accesstoken, email);
               

                return tenantid;

            }

        }

        public async Task<string> creatingtenant(string accesstoken, string username)
        {
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseAPIUrl}/tenants");
            request.Headers.Add("Authorization", $"Bearer {accesstoken}");

            // Body content for creating a tenant
            var bodyContent = new
            {
                name = username,
                parent_id = parentid,
                kind = "customer"
            };

            var json = JsonSerializer.Serialize(bodyContent);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                // Extract the "id" field from the response and store it in a variable
                if (responseObject.ContainsKey("id"))
                {
                    string tenantId = responseObject["id"].ToString();
                    return tenantId; // Return the tenant ID
                }
                else
                {
                    // Handle the case where the "id" field is not present in the response
                    // You might want to throw an exception or handle it accordingly.
                }
            }
            else
            {
                // Handle authentication error or other HTTP error responses
                // You might want to throw an exception or log the error here.
            }

            return null;
        }

        //check if tenantexsists 
        //get tenantid
        //if not exisist
        //create tenant
        //update account table

        public async Task<string> Gettenantid(string username)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                // Check if the TenantId is null or empty for the user
                if (string.IsNullOrEmpty(user.Tenantid))
                {
                    // TenantId is null or empty for the user
                    // You can handle this case accordingly, for example, return a default value or throw an exception.
                    return null;
                }
                else
                {
                    // TenantId exists for the user
                    return user.Tenantid;
                }
            }
            else
            {
                // Username doesn't exist
                // Handle this case accordingly, for example, return null or throw an exception.
                return null;
            }
        }

        public async void ManageOfferings(string status, string tenantid, string priceid, long quantity)
        {
            if (_cache.TryGetValue(cachekey, out TokenResponse tokenResponse))
            {
                //token present
                string accesstk = tokenResponse.access_token;
                setofferingAsync(accesstk, status, tenantid, priceid, quantity);
               

            }
            else
            {

                //Access token not found 
                string accesstk = await CreateToken();
                setofferingAsync(accesstk, status, tenantid, priceid, quantity);
            
            }


        }

        public async void OfferManager(string status, string tenantid, StripeList<SubscriptionItem> subscriptionItems)
        {
            if (_cache.TryGetValue(cachekey, out TokenResponse tokenResponse))
            {
                //token present
                string accesstk = tokenResponse.access_token;

                foreach (SubscriptionItem item in subscriptionItems)
                {
                    setofferingAsync(accesstk, status, tenantid, item.Price.Id, item.Quantity );
                }
                


            }
            else
            {
                string accesstk = await CreateToken();
                foreach (SubscriptionItem item in subscriptionItems)
                {
                    setofferingAsync(accesstk, status, tenantid, item.Price.Id, item.Quantity);
                }
                //Access token not found 
           
              

            }


        }



        public async void setofferingAsync(string accesstoken, string status, string tenantid, string priceid, long quantity)
        {
            if (status == "subscribe")
            {

                if (priceid == "price_1OqFGkDprfyvhQjoBJUkvUho")
                {
                    var httpClient = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseAPIUrl}/tenants/{tenantid}/offering_items");
                    request.Headers.Add("Authorization", $"Bearer {accesstoken}");
                    // Body content for creating an offering item
                    var bodyContent = new
                    {
                        offering_items = new[]
                          {
                                 new
                                     {
                                        name = "pg_base_workstations",
                                        application_id = "6e6d758d-8e74-3ae3-ac84-50eb0dff12eb",
                                        edition = "pck_per_gigabyte",
                                        usage_name = "workstations",
                                       
                                        tenant_id = tenantid,
                                        status = 1,
                                        locked = false,
                                        quota = new
                                        {
                                            value = quantity, // Ensure quantity is parsed to the appropriate type
                                            overage = (object)null,
                                            version = 0,
                                        },
                                        type = "count",
                                        infra_id = (object)null, // Assuming infra_id should be nullable
                                        measurement_unit = "quantity"
                                    }
                                }
                    };
                    var json = JsonSerializer.Serialize(bodyContent);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                    }
                }
            }

        }


        public async void ManageUsers(string tenantid, string Email)
        {
            if (_cache.TryGetValue(cachekey, out TokenResponse tokenResponse))
            {
                //token present
                string accesstk = tokenResponse.access_token;
                setuserAsync(tenantid, accesstk, Email);


            }
            else
            {

                //Access token not found 
                string accesstk = await CreateToken();
                setuserAsync(tenantid, accesstk, Email);

            }

        }


            //}

            public async void setuserAsync(string tenantid, string accesstoken, string Email)

        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseAPIUrl}/users");
            request.Headers.Add("Authorization", $"Bearer {accesstoken}");
            var bodyContent = new
            {
                tenant_id = tenantid,
                login = Email,

                contact = new
                {
                    email = Email, // Ensure quantity is parsed to the appropriate type

                }
            };

            var json = JsonSerializer.Serialize(bodyContent);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

            }

        }


    }




}







