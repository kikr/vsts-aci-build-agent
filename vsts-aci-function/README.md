# vsts-aci-function

## StartVSTSBuildAgent / StopVSTSBuildAgent

Creates or destroys an Azure Container Instance that runs the VSTS Build Agent

* Dockerfile is in the `vsts-build-agent-from-scratch` folder
* You'll need to provide AppSettings in your Function App in [local.settings.json](https://github.com/kikr/vsts-aci-build-agent/blob/master/vsts-aci-function/Functions/local.settings.json)
* Creates Container Groups in a Resource Group defined in local.settings.json.
* Places VSTS agents in a pool named in your local.settings.json
* Uses a Service Principal as the other Functions to interact with your Azure Subscription

### Usage

1. Create a Resource Group (need to match with the one in local.settings.json) to hold your build agent containers

2. Create a [Function App](https://docs.microsoft.com/en-us/azure/azure-functions/) in Azure

3. Create a Service Principal to be used with the function

```bash
# Create a Service Principal
az ad sp create-for-rbac -n vsts-aci-function
```

4. Publish the function to Azure

* Open `vsts-aci-function.sln` in VS2017
* Right-click the `Functions` project
* Hit `Publish` and select your your function app

> Note! [By default your local.settings.json file is not published](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#local-settings-file). You have to set seperate configs for publishing, or use publish-command flag to publish your local settings.
