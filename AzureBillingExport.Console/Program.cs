using System;
using System.IO;
using System.Linq;
using AzureBillingExport.Console.Services;
using CommandLine;
using CommandLine.Text;

namespace AzureBillingExport.Console
{
    class Program
    {
        //This is a sample console application that shows you how to grab a token from AAD for the current user of the app, and then get usage data for the customer with that token.
        //The same caveat remains, that the current user of the app needs to be part of either the Owner, Reader or Contributor role for the requested AzureSubID.
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.EndDate == DateTime.MinValue)
                    options.EndDate = DateTime.UtcNow.AddDays(-1);
                
                /*Setup API call to Usage API
                 Callouts:
                 * See the App.config file for all AppSettings key/value pairs
                 * You can get a list of offer numbers from this URL: http://azure.microsoft.com/en-us/support/legal/offer-details/
                 * See the Azure Usage API specification for more details on the query parameters for this API.
                 * The Usage Service/API is currently in preview; please use 2016-05-01-preview for api-version
                 */

                try
                {
                    var apiService = new ApiManagementService(options);
                    var ratePayload = apiService.GetRateCardPayload();
                    var totalPayload = apiService.GetUsagePayload();
                    var ms = ExcelService.CreateXLS(totalPayload, ratePayload);
                    if (options.UseMail)
                        MailService.SendMail(ms);
                    else
                        using (var fs = File.OpenWrite("billing.xlsx"))
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.CopyTo(fs);
                        }

                    System.Console.WriteLine("Billing Session Complete.");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(String.Format("{0} \n\n{1}", e.Message, e.InnerException != null ? e.InnerException.Message : ""));
                }
            }
            else
            {
                options.GetUsage();
                Environment.Exit(0);
            }
        }
    }
}
