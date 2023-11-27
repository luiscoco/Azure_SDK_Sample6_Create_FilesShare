using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager.Storage.Models;
using Azure.ResourceManager.Resources;
using NUnit.Framework;
using Azure.Core;
using Azure.ResourceManager;
using Azure;
using Azure.ResourceManager.Storage;

ArmClient armClient = new ArmClient(new DefaultAzureCredential());
SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

string rgName = "myRgName";
AzureLocation location = AzureLocation.WestEurope;
ArmOperation<ResourceGroupResource> operation = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName, new ResourceGroupData(location));
ResourceGroupResource resourceGroup = operation.Value;

//first we need to define the StorageAccountCreateParameters
StorageSku sku = new StorageSku(StorageSkuName.StandardGrs);
StorageKind kind = StorageKind.Storage;
StorageAccountCreateOrUpdateContent parameters = new StorageAccountCreateOrUpdateContent(sku, kind, location);
//now we can create a storage account with defined account name and parameters
StorageAccountCollection accountCollection = resourceGroup.GetStorageAccounts();
string accountName = "myaccount19999";
ArmOperation<StorageAccountResource> accountCreateOperation = await accountCollection.CreateOrUpdateAsync(WaitUntil.Completed, accountName, parameters);
StorageAccountResource storageAccount = accountCreateOperation.Value;

FileServiceResource fileService = await storageAccount.GetFileService().GetAsync();

FileShareCollection fileShareCollection1 = fileService.GetFileShares();
string fileShareName = "myfileshare19999";
FileShareData fileShareData = new FileShareData();
ArmOperation<FileShareResource> fileShareCreateOperation = await fileShareCollection1.CreateOrUpdateAsync(WaitUntil.Started, fileShareName, fileShareData);
FileShareResource fileShare1 = await fileShareCreateOperation.WaitForCompletionAsync();

FileShareCollection fileShareCollection2 = fileService.GetFileShares();
AsyncPageable<FileShareResource> response = fileShareCollection2.GetAllAsync();
await foreach (FileShareResource fileShare2 in response)
{
    Console.WriteLine(fileShare2.Id.Name);
}