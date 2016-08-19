using System.Collections.Generic;

namespace AzureBillingExport.Console.Models
{
    public class Resource
    {
        public string MeterId { get; set; }
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeterSubCategory { get; set; }
        public string Unit { get; set; }
        public Dictionary<double, double> MeterRates { get; set; }
        public string EffectiveDate { get; set; }
        public List<string> MeterTags { get; set; }
        public string MeterRegion { get; set; }
        public double IncludedQuantity { get; set; }

    }
}