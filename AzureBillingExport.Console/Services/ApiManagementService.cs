using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using AzureBillingExport.Console.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace AzureBillingExport.Console.Services
{
    public class ApiManagementService
    {
        private readonly Options options_;

        public ApiManagementService(Options options)
        {
            options_ = options;
        }

        public string GetOAuthTokenFromAAD()
        {
            AuthenticationResult result;

            var authenticationContext = new AuthenticationContext(String.Format("{0}/{1}",
                ConfigurationManager.AppSettings["ADALServiceURL"],
                ConfigurationManager.AppSettings["TenantDomain"]));

            //Ask the logged in user to authenticate, so that this client app can get a token on his behalf
            if (options_.UseInteractiveAccount)
                result = authenticationContext.AcquireToken(String.Format("{0}/", ConfigurationManager.AppSettings["ARMBillingServiceURL"]),
                    ConfigurationManager.AppSettings["ClientID"],
                    new Uri(ConfigurationManager.AppSettings["ADALRedirectURL"]));
            else
            {
                var clientCredentials = new ClientCredential(ConfigurationManager.AppSettings["ClientID2"], ConfigurationManager.AppSettings["AppKey"]);
                result = authenticationContext.AcquireToken(String.Format("{0}/", ConfigurationManager.AppSettings["ARMBillingServiceURL"])
                    , clientCredentials);
            }

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        public UsagePayload GetUsagePayload()
        {
            //Get the AAD token to get authorized to make the call to the Usage API

            string token = GetOAuthTokenFromAAD();

            var startDate = options_.StartDate.ToString("yyyy-MM-dd");
            var endDate = options_.EndDate.ToString("yyyy-MM-dd");
            // Build up the HttpWebRequest
            string requestUrl = String.Format("{0}/{1}/{2}/{3}",
                ConfigurationManager.AppSettings["ARMBillingServiceURL"],
                "subscriptions",
                ConfigurationManager.AppSettings["SubscriptionID"],
                "providers/Microsoft.Commerce/UsageAggregates?api-version=2015-06-01-preview&reportedstartTime=" + startDate + "+00%3a00%3a00Z&reportedEndTime=" + endDate + "+00%3a00%3a00Z&showDetails=true");
            
            var payload = CallManagementApi(requestUrl, token); 
            var totalPayload = payload;
            var totalline = payload.value.Count;
            System.Console.WriteLine("Total Line : {0:000000}", totalline);
            while (!String.IsNullOrEmpty(payload.nextLink))
                {
                payload = CallManagementApi(payload.nextLink, token);
                totalPayload.value.AddRange(payload.value);

                totalline += payload.value.Count;
                System.Console.WriteLine("Total Line : {0:000000}", totalline);
            }

            return totalPayload;
        }

        public static UsagePayload CallManagementApi(string uri, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            // Add the OAuth Authorization header, and Content Type header
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            request.ContentType = "application/json";

            System.Console.WriteLine("Calling Usage service...");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            System.Console.WriteLine($"Usage service response status: {response.StatusDescription}");
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var usageResponse = readStream.ReadToEnd();
            UsagePayload payload = JsonConvert.DeserializeObject<UsagePayload>(usageResponse);
            
            readStream.Dispose();
            response.Dispose();
            return payload;
        }
        public RateCardPayload GetRateCardPayload()
        {
            var offer = "MS-AZR-0111p";
            var currency = "EUR";
            var local = "fr-fr";
            var regionInfo = "FR";

            string token = GetOAuthTokenFromAAD();

            string requestUrl = String.Format("{0}/{1}/{2}/{3}",
                      ConfigurationManager.AppSettings["ARMBillingServiceURL"],
                      "subscriptions",
                      ConfigurationManager.AppSettings["SubscriptionID"],
                      "providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq '"+offer+"' and Currency eq '"+currency+"' and Locale eq '"+local+"' and RegionInfo eq '"+regionInfo+"'");
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);

            // Add the OAuth Authorization header, and Content Type header
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            request.ContentType = "application/json";
            
            var response = (HttpWebResponse)request.GetResponse();
            System.Console.WriteLine($"RateCard service response status: {response.StatusDescription}");
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var rateCardResponse = readStream.ReadToEnd();
            // Convert the Stream to a strongly typed RateCardPayload object.  
            // You can also walk through this object to manipulate the individuals member objects. 
            var payload = JsonConvert.DeserializeObject<RateCardPayload>(rateCardResponse);

            return payload;
        }
    }
}
