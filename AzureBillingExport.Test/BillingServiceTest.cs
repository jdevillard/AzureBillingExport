using System;
using System.Collections.Generic;
using AzureBillingExport.Console.Models;
using AzureBillingExport.Console.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureBillingExport.Test
{
    [TestClass]
    public class BillingServiceTest
    {
        /*
        "MeterRates": {
                           "0": 0.08812485,
                           "1024": 0.080956800,   
                           "51200": 0.070837200,   800 /  52 000
                           "512000": 0.065777400,
                           "1024000": 0.060717600,
                           "5120000": 0.055657800
                     }
                     */
        [TestMethod]
        public void TestArrayWithOneValue()
        {
            var props = new Properties()
            {
                quantity = 100, 
                meterId = "1",
            };
            var ratePayload = new RateCardPayload()
            {
                Meters = new List<Resource>()
                {
                    new Resource()
                    {
                        MeterId = "1", 
                        MeterRates = new Dictionary<double, double>()
                        {
                            {0,10}
                        }
                    }
                }
            };
            var result = BillingService.CalculLineBilling(props, ratePayload);
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void TestArrayWithOneValueInArray()
        {
            var props = new Properties()
            {
                quantity = 100,
                meterId = "1",
            };
            var ratePayload = new RateCardPayload()
            {
                Meters = new List<Resource>()
                {
                    new Resource()
                    {
                        MeterId = "1",
                        MeterRates = new Dictionary<double, double>()
                        {
                            {0,10}
                        }
                    }
                }
            };
            var result = BillingService.CalculLineBillingFromArray(props, ratePayload);
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void TestArrayWithTwoValue()
        {
            var props = new Properties()
            {
                quantity = 100,
                meterId = "1",
            };
            var ratePayload = new RateCardPayload()
            {
                Meters = new List<Resource>()
                {
                    new Resource()
                    {
                        MeterId = "1",
                        MeterRates = new Dictionary<double, double>()
                        {
                            {0,10},
                            {50, 8 }
                        }
                    }
                }
            };
            var result = BillingService.CalculLineBillingFromArray(props, ratePayload);
            Assert.AreEqual(50 * 10 + 50 * 8, result);
        }

        [TestMethod]
        public void TestArrayWithOneValueUpper()
        {
            var props = new Properties()
            {
                quantity = 100,
                meterId = "1",
            };
            var ratePayload = new RateCardPayload()
            {
                Meters = new List<Resource>()
                {
                    new Resource()
                    {
                        MeterId = "1",
                        MeterRates = new Dictionary<double, double>()
                        {
                            {0,10},
                            {50, 8 },
                            {101, 6 }

                        }
                    }
                }
            };
            var result = BillingService.CalculLineBillingFromArray(props, ratePayload);
            Assert.AreEqual(50 * 10 + 50 * 8, result);
        }
    }
}
