using System.Collections.Generic;

namespace AzureBillingExport.Console.Models
{
    public class UsagePayload
    {
        public List<UsageAggregate> value { get; set; }

        public string nextLink { get; set; }
    }
}
