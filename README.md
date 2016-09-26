# AzureBillingExport
Program Console that connect to an Azure Subscription to extract billing information into Excel file

This program can : 
- connect to your azure subscription using interactive account or service account
- extract billing information from range date
- create a XSLX file that summarize all the data (the format try to be the same as the one given by microsoft)
- Send you an email with the Billing file sent as attachment

## Get Started

To start, you've to configure the app.config : 

```xml
<appSettings>
   <!-- Service root URL for ADAL authentication service WITH NO TRAILING SLASH! -->
   <add key="ADALServiceURL" value="https://login.microsoftonline.com" />
   <!-- Redirect URL for ADAL authentication service MUST MATCH YOUR AAD APP CONFIGURATION! -->
   <add key="ADALRedirectURL" value="https://localhost/" />
   <!-- Service root URL for ARM/Billing service WITH NO TRAILING SLASH!  -->
   <add key="ARMBillingServiceURL" value="https://management.azure.com" />
   <!-- DNS name for your Azure AD tenant -->
   <add key="TenantDomain" value="{INSERT Tenant Domain here}" />
   <!-- GUID of Azure Subscription that is trusting AAD tenant specified above -->
   <add key="SubscriptionID" value="{INSERT YOUR AZURE SUBSCRIPTION ID HERE}" />
   <!-- GUID for AAD application configured as Native Client App in AAD tenant specified above -->
   <add key="ClientId" value="{INSERT YOUR AD CLIENT ID HERE}" />                
 
   <!-- If you've created an Service Account -->
   <add key="AppKey" value="{INSERT YOUR AZURE APP KEY HERE}" />
   <add key="ClientID2" value="{INSERT YOUR AD CLIENT ID HERE}" />
    
   <add key="SendGridApiKey" value="{INSERT SENDGRID API KEY HERE}" />
   <add key="BillingEmailSender" value="{INSERT AN SENDER HERE}" />
   <add key="BillingEmails" value="{INSERT DESTINATION EMAIL HERE USING '|'}" />
 </appSettings>
```

The first part about ADAL configuration is detailed in [Microsoft Samples](https://github.com/Azure-Samples/billing-dotnet-usage-api)  
The second part allow you to configure service account. This part is usefull if you want to deploy this program as a webjob for example.
The third part allow you to configure mail service using SendGrid.

The program contains an help page : 

![alt HelpImage](https://jeremiedevillard.files.wordpress.com/2016/08/image3.png)

To launch the program, 

```cmd
AzureBillingExport.Console.exe –u –start-date 2016-08-01
```

where -u indicate interactive mode and -start-date indicate the date to start getting information until now.

this will launch an interactive windows to log into your subscription : 

![alt login](https://jeremiedevillard.files.wordpress.com/2016/08/image4.png)

*(Remember: you account need to get at least Reader Permissions on All the subscription)*

at the end the program display the number of line polled with the API :
![alt endProgram](https://jeremiedevillard.files.wordpress.com/2016/08/image6.png)

## Excel Sheet

The excel file is composed of 3 sheet :
- Summary : with the date requested, total amount consumed by the subscription and a Chart
- BillingDetails : a table with the same representation as the billing.csv available on account.windowsazure.com
- PivotTable : with a Pivot on the different Azure Service


Sheet 1 : 
![alt sheet1](https://jeremiedevillard.files.wordpress.com/2016/08/image7.png)

Sheet 2 : 
![alt sheet2](https://jeremiedevillard.files.wordpress.com/2016/08/image8.png)

Sheet 3 :  
![alt sheet3](https://jeremiedevillard.files.wordpress.com/2016/08/image9.png)

Link to my [blog post](https://jeremiedevillard.wordpress.com/2016/08/19/azure-billing-export-comment-recrer-votre-consommation-azure/)