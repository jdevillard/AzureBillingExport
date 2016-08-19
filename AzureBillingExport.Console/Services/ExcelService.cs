using System.IO;
using System.Linq;
using AzureBillingExport.Console.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table.PivotTable;

namespace AzureBillingExport.Console.Services
{
    internal static class ExcelService
    {
        public static MemoryStream CreateXLS(UsagePayload payload, RateCardPayload rate)
        {
            //Create the workbook
            var pkg = new ExcelPackage();
            var wsSummary = AddSummaryWorksheet(pkg);

            //Add the Content sheet
            var ws = pkg.Workbook.Worksheets.Add(Constants.ListingWorkSheet);
            WriteXLSHeader(ws);
            var i = 2;
            foreach (var v in payload.value.Select(u => u.properties))
            {
                ws.SetValue(i,1,v.usageStartTime);
                ws.SetValue(i,2,v.meterCategory);
                ws.SetValue(i,3,v.meterId);
                ws.SetValue(i,4,v.meterSubCategory);
                ws.SetValue(i,5,v.meterName);
                ws.SetValue(i,6,v.meterRegion);
                ws.SetValue(i,7,v.unit);
                ws.SetValue(i,8,v.quantity);

                //Meter Region
                if (v.infoFields != null && v.infoFields.meteredRegion != null)
                {
                    ws.SetValue(i,9,v.infoFields.meteredRegion);
                    ws.SetValue(i,10,v.infoFields.meteredService);
                    ws.SetValue(i,11,"");
                    ws.SetValue(i,12,v.infoFields.project);
                    ws.SetValue(i,13,"{}");
                    ws.SetValue(i,14,"{}");
                }
                else if (v.InstanceData != null)
                {
                    var uri = v.InstanceData.MicrosoftResources.resourceUri;

                    ws.SetValue(i,9,v.InstanceData.MicrosoftResources.location);
                    ws.SetValue(i,10,uri.Split('/')[6]);
                    ws.SetValue(i,11,uri.Split('/')[4]);
                    ws.SetValue(i,12,uri);
                    ws.SetValue(i,13,JsonConvert.SerializeObject(v.InstanceData.MicrosoftResources.tags));
                    ws.SetValue(i,14,JsonConvert.SerializeObject(v.InstanceData.MicrosoftResources.additionalInfo));
                }

                ws.SetValue(i,15,"");
                ws.SetValue(i,16,"");
                ws.SetValue(i,17,BillingService.CalculLineBillingFromArray(v, rate));
                i++;
            }
            AddPivotTable(pkg,i);

            var ws1 = pkg.Workbook.Worksheets.Add("Chart1");
            var pvt = CreatePivotTable(pkg, ws1, "A1", "test");
            pvt.RowFields.Add(pvt.Fields["Date Utilisation"]);
            pvt.DataFields.Add(pvt.Fields["valeur"]);
            ws1.Hidden = eWorkSheetHidden.Hidden;
            var chart = wsSummary.Drawings.AddChart("PvtChart", eChartType.ColumnStacked, pvt);
            chart.SetPosition(1, 0, 4, 0);
            chart.SetSize(600, 400);

            wsSummary.Cells["C2"].Value = payload.value[0].properties.usageStartTime;
            wsSummary.Cells["C3"].Value = payload.value.Last().properties.usageStartTime;
            
            var lastCell = pkg.Workbook.Worksheets[Constants.ListingWorkSheet].Dimension.End.Row;
            var dataRange = pkg.Workbook.Worksheets[Constants.ListingWorkSheet]
                .SelectedRange[1, 17, lastCell, 17];
            wsSummary.Cells["C4"].Formula = "SUM('"+Constants.ListingWorkSheet+"'!"+dataRange.Address+")";
            wsSummary.Cells["C4"].Style.Numberformat.Format = "#.## €";
            
            //  pkg.Save();
            var ms = new MemoryStream();
            pkg.SaveAs(ms);
            pkg.Dispose();

            return ms;
        }

        public static void AddPivotTable(ExcelPackage pkg, int i)
        {
            var ws = pkg.Workbook.Worksheets.Add(Constants.PivotTableWorkSheet);

            var pivotTable1 = CreatePivotTable(pkg, ws, "A1", Constants.PivotTableName);
            pivotTable1.RowFields.Add(pivotTable1.Fields["Catégorie du compteur"]);
            pivotTable1.RowFields.Add(pivotTable1.Fields["Sous-Catégorie"]);
            var dataField = pivotTable1.DataFields.Add(pivotTable1.Fields["valeur"]);

            var chart = ws.Drawings.AddChart("PivotChart", eChartType.Pie, pivotTable1);
            chart.SetPosition(1, 0, 4, 0);
            chart.SetSize(600, 400);
            chart.Name = "Consommation par Catégorie";
        }

        public static ExcelPivotTable CreatePivotTable(ExcelPackage pkg, ExcelWorksheet ws, string cell, string pivotTableName)
        {
            var lastCell = pkg.Workbook.Worksheets[Constants.ListingWorkSheet].Dimension.End.Row;

            var dataRange = pkg.Workbook.Worksheets[Constants.ListingWorkSheet]
                .SelectedRange[1, 1, lastCell, 17];
            dataRange.AutoFitColumns();
            return ws.PivotTables.Add(ws.Cells["A1"], dataRange, Constants.PivotTableName);
        }


        public static ExcelWorksheet AddSummaryWorksheet(ExcelPackage pkg)
        {
            var ws = pkg.Workbook.Worksheets.Add(Constants.SummaryWorkSheet);

            ws.Cells["B2"].Value = "Date début";
            ws.Cells["B3"].Value = "Date Fin";
            ws.Cells["B4"].Value = "Consommé";
            
            return ws;
        }
        public static void WriteXLSHeader(ExcelWorksheet ws)
        {
            var headers = new[]
            {
                "Date Utilisation"
                , "Catégorie du compteur"
                , "Id du Compteur"
                , "Sous-Catégorie"
                , "Nom du compteur"
                , "Region du compteur"
                , "Unité"
                , "Quantité consommée"
                , "Emplacement de la ressource"
                , "Service consommé"
                , "Groupe de ressource"
                , "Id d'instance"
                , "balise"
                , "Info Supplémentaire"
                , "Service Info1"
                , "Service Info2"
                , "valeur"
            };

            for (var i = 1; i < headers.Length + 1; i++)
            {
                ws.SetValue(1, i, headers[i - 1]);
            }
        }
    }
}