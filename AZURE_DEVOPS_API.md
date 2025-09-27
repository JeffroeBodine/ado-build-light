# Azure DevOps REST API Integration

This document describes the changes made to integrate with the Azure DevOps REST API instead of parsing build badge SVGs.

## Changes Made

### 1. Configuration Updates
- **appsettings.json**: Replaced `PipelineBadgeUrl` with Azure DevOps configuration section containing:
  - `Organization`: Your Azure DevOps organization name
  - `Project`: The project name containing the pipeline  
  - `PipelineId`: The numeric ID of the pipeline to monitor
  - `PersonalAccessToken`: Your Azure DevOps Personal Access Token

### 2. New Model Classes
- **ADOBuildLight.Models.AzureDevOpsModels.cs**: Created classes to deserialize Azure DevOps API responses:
  - `PipelineRunsResponse`: Root response object with count and array of runs
  - `PipelineRun`: Individual pipeline run with state, result, dates, etc.
  - `Pipeline`: Pipeline metadata
  - `AzureDevOpsConfig`: Configuration binding class

### 3. Updated Services

#### BuildStatusService
- Replaced HTML parsing with REST API calls
- Now uses JSON deserialization to get pipeline run data
- Returns the most recent pipeline run object instead of just status string
- Includes proper authentication headers for Azure DevOps API

#### BuildMonitorService  
- Updated to work with `PipelineRun` objects instead of status strings
- Added logic to determine overall status from `state` and `result` properties
- Enhanced status handling for Azure DevOps specific states:
  - `inProgress`: Shows yellow light for running builds
  - `completed` + `succeeded`: Shows green light for successful builds
  - `completed` + `failed`: Shows red light for failed builds
  - `completed` + `partiallySucceeded`: Shows orange light for partial success
  - `canceled`: Shows no light for cancelled builds

### 4. Updated Interfaces
- **IBuildStatusService**: Changed method signature from `GetBuildStatusAsync(string badgeUrl)` to `GetLatestPipelineRunAsync()`

## Setup Instructions

1. **Get a Personal Access Token**:
   - Go to Azure DevOps → User Settings → Personal Access Tokens
   - Create a token with "Build (Read)" permissions
   - Copy the token value

2. **Configure appsettings.json**:
   ```json
   {
     "AzureDevOps": {
       "Organization": "your-org-name",
       "Project": "your-project-name", 
       "PipelineId": "123",
       "PersonalAccessToken": "your-pat-token-here"
     }
   }
   ```

3. **Find your Pipeline ID**:
   - In Azure DevOps, navigate to your pipeline
   - The ID is visible in the URL: `https://dev.azure.com/org/project/_build?definitionId=123`

## Benefits of REST API Approach

1. **Real-time Status**: Get immediate status updates including in-progress builds
2. **More Information**: Access to detailed pipeline run data (start time, duration, etc.)  
3. **Better Reliability**: Direct API access instead of parsing HTML/SVG content
4. **Enhanced Status Mapping**: Support for all Azure DevOps pipeline states and results
5. **Authentication**: Secure access using Personal Access Tokens

## API Endpoint Used

The service calls the Azure DevOps REST API endpoint:
```
https://dev.azure.com/{organization}/{project}/_apis/pipelines/{pipelineId}/runs?api-version=7.1
```

This returns the most recent pipeline runs, with the latest run being the first item in the response array.