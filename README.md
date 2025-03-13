# url-shortener
url shortener

## Infrastructure as Code

### Download Azure CLI
https://learn.microsoft.com/em-use/cli/azure

### Log in into Azure
```bash
az login
```

### Create Resource Group

```bash
az group create --name urlshortener-dev --location mexicocentral
```

### What if
```bash
az deployment group what-if --resource-group urlshortener-dev --template-file infrastructure/main.bicep
```

### Deploy
```bash
az deployment group create --resource-group urlshortener-dev --template-file infrastructure/main.bicep
```

### Create user for GH Actions

```bash
az ad sp create-for-rbac --name "Github-Actions-SP" --role contributor --scopes /subscriptions/{SubscriptionId} --sdk-auth
```

### Apply to Custom Contributor Role
```bash
az ad sp create-for-rbac --name "Github-Actions-SP" --role 'infra_deploy' --scopes /subscriptions/{SubscriptionId} --sdk-auth
```

#### Configure a federated identity credential on an app
https://learn.microsoft.com/en-us/entra/workload-id/workload-identity-federation-create-trust?pivots=identity-wif-apps-methods-azp#configure-a-federated-identity-credential-on-an-app

## Get Azure Publish Profile

```bash
az webapp deployment list-publishing-profiles --name {AppServiceName} --resource-group {ResourceGroup} --xml
```