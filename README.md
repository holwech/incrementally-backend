# incrementally-backend
[![Build Status](https://dev.azure.com/incrementally/incrementally/_apis/build/status/holwech.incrementally-backend?branchName=master)](https://dev.azure.com/incrementally/incrementally/_build/latest?definitionId=1&branchName=master)

This is the backend service for Incrementally.


## Local development

To develop locally, clone this project to your local workspace. Install [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) and [CosmosDB local emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) by following the guidelines. The CosmosDB emulator requires you to add a local secret (can be done in Visual Studio), containing the key provided in the emulator. The secrets.json should be on the following format:

```json
{
  "CosmosDBKey": "YOUR_LOCAL_COSMOSDB_KEY"
}
```

## Local development towards production

To run a local backend towards the hosted database, your account has to be added to the Azure key vault.
