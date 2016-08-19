using System.IO;
using System.Linq;
using AzureBillingExport.Console.Models;
using CsvHelper;
using Newtonsoft.Json;

namespace AzureBillingExport.Console.Services
{
    internal static class CsvService
    {
        public static void CreateCSV(UsagePayload payload, RateCardPayload rate)
        {
            using (var fileStream = File.CreateText("billing.csv"))
            using (var csv = new CsvHelper.CsvWriter(fileStream))
            {
                csv.Configuration.QuoteAllFields = true;

                var jsonSerializerSetting = new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore};

                WriteHeader(csv);
                csv.NextRecord();
                foreach (var v in payload.value.Select(u => u.properties))
                {
                    csv.WriteField(v.usageStartTime);
                    csv.WriteField(v.meterCategory);
                    csv.WriteField(v.meterId);
                    csv.WriteField(v.meterSubCategory);
                    csv.WriteField(v.meterName);
                    csv.WriteField(v.meterRegion);
                    csv.WriteField(v.unit);
                    csv.WriteField(v.quantity);

                    //Meter Region
                    if (v.infoFields != null && v.infoFields.meteredRegion != null)
                    {
                        csv.WriteField(v.infoFields.meteredRegion);
                        csv.WriteField(v.infoFields.meteredService);
                        csv.WriteField("");
                        csv.WriteField(v.infoFields.project);
                        csv.WriteField("{}");
                        csv.WriteField("{}");
                    }
                    else if (v.InstanceData != null)
                    {
                        var uri = v.InstanceData.MicrosoftResources.resourceUri;

                        csv.WriteField(v.InstanceData.MicrosoftResources.location);
                        csv.WriteField(uri.Split('/')[6]);
                        csv.WriteField(uri.Split('/')[4]);
                        csv.WriteField(uri);
                        csv.WriteField(JsonConvert.SerializeObject(v.InstanceData.MicrosoftResources.tags, jsonSerializerSetting));
                        csv.WriteField(JsonConvert.SerializeObject(v.InstanceData.MicrosoftResources.additionalInfo, jsonSerializerSetting));
                    }

                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField(BillingService.CalculLineBilling(v, rate));
                    csv.NextRecord();
                }

            }
        }

        private static void WriteHeader(CsvWriter csv)
        {
            csv.WriteField("Date Utilisation");
            csv.WriteField("Catégorie du compteur");
            csv.WriteField("Id du Compteur");
            csv.WriteField("Sous-Catégorie");
            csv.WriteField("Nom du compteur");
            csv.WriteField("Region du compteur");
            csv.WriteField("Unité");
            csv.WriteField("Quantité consommée");
            csv.WriteField("Emplacement de la ressource");
            csv.WriteField("Service consommé");
            csv.WriteField("Groupe de ressource");
            csv.WriteField("Id d'instance");
            csv.WriteField("balise");
            csv.WriteField("Info Supplémentaire");
            csv.WriteField("Service Info1");
            csv.WriteField("Service Info2");
            csv.WriteField("valeur");
        }
    }
}