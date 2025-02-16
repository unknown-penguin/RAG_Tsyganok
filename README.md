# RAG Tsyhanok

## Overview

This project is a C# application that reads text files, processes them, and interacts with Azure OpenAI services.

## Prerequisites

- .NET 9.0 SDK
- Azure OpenAI account and API key
- Configuration files (appsettings.json)

## Configuration

The `appsettings.json` file contains configuration settings for the application. Update the following fields with your specific values:

```json
{
  "Documents": {
    "FolderPath": "insert-here-path-to-files"
  },
  "AzureOpenAI": {
    "Endpoint": "insert-here-endpoint",
    "ApiKey": "insert-here-api-key",
    "EmbeddingModel": {
      "DeploymentId": "text-embedding-ada-002",
      "TaskName": "embeddings",
      "ApiVersion": "2024-05-01-preview"
    },
    "ChatCompletionModel": {
      "DeploymentId": "gpt-4o",
      "TaskName": "completions",
      "ApiVersion": "2024-08-01-preview"
    }
  }
}
