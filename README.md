# Durable Functions Console

A web-based UI for managing instances and entities created with Azure Durable Functions via HTTP APIs.

## Supported Azure Functions Runtimes

Durable Functions Console is compatible with:
- ✅ **Azure Functions Runtime 2.x**
- ✅ **Azure Functions Runtime 3.x**
- ✅ **Azure Functions Runtime 4.x**

## Hosted Environments

We maintain two hosted environments, one for development and one for production. PRs can be tested in `Development` environment. 
Once merged to main, it will get deployed to `Production` environment.

| Environment | URL                                 |
|-------------|-------------------------------------|
| Development         | https://app-durable-functions-console-dev-001.azurewebsites.net      |
| Production        | https://app-durable-functions-console-prod-001.azurewebsites.net     |

## DockerHub
If you don't want to use the hosted environments, you can pull the Docker image from Docker Hub.

[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-Available-blue?logo=docker)](https://hub.docker.com/r/jaliyaudagedara/durable-functions-console)

### How to run
```bash
docker pull jaliyaudagedara/durable-functions-console:<tag>
docker run -d -p 8080:8080 jaliyaudagedara/durable-functions-console:<tag>
```

## Acknowledgements

Vibe coded using **GitHub Copilot**.