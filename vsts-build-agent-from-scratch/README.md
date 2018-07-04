# vsts-build-agent

This image contains the [Visual Studio Team Services Build and Release Agent](https://github.com/Microsoft/vsts-agent) for Linux.

See [vsts-agent-docker](https://github.com/Microsoft/vsts-agent-docker) for the Docker-based Hosted Linux Agent.

## Run Docker Visual Studio Team Services (VSTS) agent locally

Instructions how to run the VSTS agent on your computer with nothing but Docker installed. Well these instructions assume that the VSTS agent image resides in private Azure image registry, so you have to install Azure CLI as well.

### Prerequisites
Start by installing the following applications:

* Docker CE
* Azure CLI

### Configure VSTS account for the agent

VSTS agent uses an existing VSTS account (will be referred as "agent account") in communicating with the VSTS server
		
1. Using admin account, add agent account as an Administrator of the Agent Pool that your agent's going to use. (_Project Settings > Agent Pools_)
2. Using agent account, generate Personal Access Token (PAT) with scopes: **Agent Pools (read, manage)**, **Build (Read and Execute)**
3. Set the PAT as a value for VSTS_TOKEN-paramater in Docker run-command below

### Start the agent

Next we'll start the agent from command line

1. Login to Azure Container Registry
```bash
az acr login -n someAzureRegistry
```
2. Now fill in the missing information in the below command and execute it
```bash
docker run -e VSTS_TOKEN=<PAT> -e VSTS_AGENT=<Agent Name> -e VSTS_POOL=<Agent Pool Name> -e VSTS_ACCOUNT=<MY VSTS owner account> -it someAzureRegistry.azurecr.io/john/my-image
```
			
> Notes! In the docker run-command, VSTS_ACCOUNT is actually the project owner's account name and not the agent account's name.
			
### Result
		
Agent should now be waiting for jobs and you should also be able see your agent listed in the VSTS Agent Pool
