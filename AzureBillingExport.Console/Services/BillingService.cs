using System;
using System.Collections.Generic;
using System.Linq;
using AzureBillingExport.Console.Models;
using Newtonsoft.Json;

namespace AzureBillingExport.Console.Services
{
    public static class BillingService
    {
        public static double CalculLineBilling(Properties prop, RateCardPayload rate)
        {
            var meterBase = rate.Meters.First(m => m.MeterId == prop.meterId);
            var count = prop.quantity * meterBase.MeterRates.First().Value;
            return count;
        }


        /**
                * 
                   "MeterRates": {
                           "0": 0.08812485,
                           "1024": 0.080956800,   
                           "51200": 0.070837200,   800 /  52 000
                           "512000": 0.065777400,
                           "1024000": 0.060717600,
                           "5120000": 0.055657800
                     }
                * 
                * */
        public static double CalculLineBillingFromArray(Properties prop, RateCardPayload rate)
        {
            double count = 0;
            var quantityToAdd = prop.quantity;
            var meterBase = rate.Meters.First(m => m.MeterId == prop.meterId).MeterRates.ToArray();
            foreach (var keyValuePair in meterBase.Reverse())
            {
                if (quantityToAdd > keyValuePair.Key)
                {
                    count += (quantityToAdd - keyValuePair.Key)*keyValuePair.Value;
                    quantityToAdd = keyValuePair.Key;
                }
            }
            return count;
        }
    }
}