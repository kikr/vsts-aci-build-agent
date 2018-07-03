# vsts-build-agent

This image contains the [Visual Studio Team Services Build and Release Agent](https://github.com/Microsoft/vsts-agent) for Linux.

See [vsts-agent-docker](https://github.com/Microsoft/vsts-agent-docker) for the Docker-based Hosted Linux Agent.

## Usage - Docker

As a thin wrapper over the base scripts, most configuration is done via environment variables. Per the VSTS agent docs, "Any command line argument can be specified as an environment variable. Use the format `VSTS_AGENT_INPUT_<ARGUMENT_NAME>`. For example: `VSTS_AGENT_INPUT_PASSWORD`

> The full list of options can be found by running `config.sh --help`

So an example to run an agent named `docker-0` in an agent pool named `Docker` would look like:

```bash
docker run --rm -it -e VSTS_AGENT_INPUT_URL=https://noelbundick.visualstudio.com -e VSTS_AGENT_INPUT_AUTH=pat -e VSTS_AGENT_INPUT_TOKEN=S4GGVbTQs58h6NbmJBY7cn98CKoyQSC1CSWMmCIx3aMkOhRppLDh -e VSTS_AGENT_INPUT_POOL=Docker -e VSTS_AGENT_INPUT_AGENT=docker-0 vsts-build-agent
```

## Usage - Azure Container Instance

This image can also be used to run a build agent on [Azure Container Instances](https://azure.microsoft.com/en-us/services/container-instances/). An example that runs an agent named `vsts-agent-0` in an agent pool named `AzureContainerInstance` would look like:

```bash
az container create -n vsts-agent-0 -g vsts --cpu 2 --memory 3.5 --image acanthamoeba/vsts-build-agent -e VSTS_AGENT_INPUT_URL=https://noelbundick.visualstudio.com VSTS_AGENT_INPUT_AUTH=pat VSTS_AGENT_INPUT_TOKEN=S4GGVbTQs58h6NbmJBY7cn98CKoyQSC1CSWMmCIx3aMkOhRppLDh VSTS_AGENT_INPUT_POOL=AzureContainerInstance VSTS_AGENT_INPUT_AGENT=vsts-agent-0
```

## Run Docker VSTS agent locally

	### Configure VSTS account for the agent

		VSTS agent uses an existing VSTS account (will be referred as "agent account") in communicating with the VSTS server
		
		1. Using admin account, add agent account as an Administrator of the Agent Pool that your agent's going to use. (Project Settings > Agent Pools)
		2. Using agent account, generate Personal Access Token (PAT) with scopes: Agent Pools (read, manage), Build (Read and Execute)
		3. Set the PAT as a value for VSTS_TOKEN-paramater in command below

	### Start the agent

		Next we'll start the agent from command line

		1. Login to Azure Container Registry
            ```bash
                az acr login -n packregistry
            ```
		2. Now fill in the missing information in the below command and execute it
         ```bash
			docker run -e VSTS_TOKEN=<PAT> -e VSTS_AGENT=<Agent Name> -e VSTS_POOL=<Agent Pool Name> -e VSTS_ACCOUNT=myVstsOwnerAccount -it packregistry.azurecr.io/john/my-image
        ```
			
		> Notes! In the docker run-command, VSTS_ACCOUNT is actually the project owner's account name and not the agent account's name.
			
	### Result
		
		Agent should now be waiting for jobs and you should also be able see your agent listed in the VSTS Agent Pool
