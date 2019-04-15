# Updating Json Configuration with Dynamic values in C#

## Configuration Management using Json  

This is sample code to use JSON for application configuration with placeholder for dynamic values which can be updated during application startup.

This sample code updates placeholders in JSON which represent that property value depend on environment variable with actual environment value during application startup. This is a common scenario in any application's configuration management to be able to create and use dynamic configuration. Dynamic configuration allows application behavior to be modified when environment or dependencies change.

## Json Example
```
{
  "Sources": [
        {
          "SourceId": "123-RandomSourceId",
          "DataSource": {
            "FolderPath": "ENV[BaseFolderPathInStorage]/Data",
            "Connection": {
            "HttpConfiguration": {
              "HttpEndpoint": "ENV[ExternalBootStartAPI]/bootstart/products",
              "CertName": "ENV[LocalStoreAuthCertName]"
            }
            "Type": "CertAuthHttp"
          }
        }      
    ]
}
```

In above example, by making HttpEndpoint configurable application can easily switch between different dependency endpoints e.g. Test/Production etc.

## Reading and Updating JSON

Using NewtonSoft.Json , C# application can deserialize JSON into JToken or JObject. Code will then recurse through every Property inside the json, check for placeholder and update property's valyue with dynamic or environment specific values.



