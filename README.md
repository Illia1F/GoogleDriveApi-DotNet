# GoogleDriveApi-DotNet

This C# library simplifies interaction with the Google Drive API. While it doesn't cover the entire API, it includes the most commonly used endpoints for easier access to Google Drive.

## Features

- Extract folder ID from a Google Drive folder URL.
- Get the folder name by its ID.
- Generate Google Drive folder URL by its path.
- Obtain the full path of a folder starting from the root using its ID.
- Create folders in Google Drive.
- Check if the access token is expired.
- Refresh the access token if expired.

> **Note:** This library is not a full reflection of the real Google Drive API but implements the most commonly used API endpoints to simplify interaction with Google Drive.

## Installation

Add the Google Drive API NuGet package to your project:

```bash
dotnet add package Google.Apis.Drive.v3
```

> Download this library or create our own implementation based on this template.

## Setup

1. **Create a Google Cloud Project** and enable the Google Drive API.
2. **Create OAuth 2.0 credentials** and download the JSON file.
3. **Set up your application to use these credentials**.

[Example from the internet](https://medium.com/geekculture/upload-files-to-google-drive-with-c-c32d5c8a7abc)

### Creating a Google Cloud Project and Enabling the Google Drive API

1. Go to the [Google Cloud Console](https://console.cloud.google.com/).
2. Click on the project dropdown at the top and select `New Project`.
3. Enter a project name and click `Create`.
4. Once the project is created, go to the `Navigation menu` > `APIs & Services` > `Library`.
5. Search for "Google Drive API" and click on it.
6. Click `Enable` to enable the Google Drive API for your project.

### Creating OAuth 2.0 Credentials

1. In the Google Cloud Console, go to `Navigation menu` > `APIs & Services` > `Credentials`.
2. Click on `Create Credentials` and select `OAuth 2.0 Client IDs`.
3. Configure the OAuth consent screen:
   - Select `External` and click `Create`.
   - Enter the necessary information (app name, user support email, etc.).
   - Add scopes if needed (usually, you can proceed with the default scope).
   - Click `Save and Continue` until the configuration is complete.
4. On the `Create OAuth client ID` page:
   - Select `Desktop app` as the application type.
   - Click `Create`.
   - Download the JSON file and save it as `credentials.json` in your project directory.

## Example Code

### Creating an Instance of GoogleDriveApi

First, create an instance of the `GoogleDriveApi` class using the fluent builder pattern with your credentials and token paths:

```csharp
GoogleDriveApi driveApi = await GoogleDriveApi.CreateBuilder()
	.SetCredentialsPath("credentials.json")
	.SetTokenFolderPath("_metadata")
	.SetApplicationName("[Your App Name]")
	.BuildAsync();

string newFolderId = driveApi.CreateFolder("NewFolderName");
Console.WriteLine("New Folder ID: " + newFolderId);
```

## Class Documentation

### GoogleDriveApi

A class for interacting with the Google Drive API.

#### Methods

- `GoogleDriveApiBuilder SetCredentialsPath(string path)`: Sets the path to the credentials JSON file. [Documentation](https://developers.google.com/identity/protocols/oauth2)
- `GoogleDriveApiBuilder SetTokenFolderPath(string path)`: Sets the path to the token JSON file. [Documentation](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth)
- `GoogleDriveApiBuilder SetApplicationName(string name)`: Sets the name of the application. [Documentation](https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Services.BaseClientService.Initializer#Google_Apis_Services_BaseClientService_Initializer_ApplicationName)
- `Task<GoogleDriveApi> BuildAsync()`: Builds and authorizes the GoogleDriveApi instance asynchronously. [Documentation](https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker?hl=en#Google_Apis_Auth_OAuth2_GoogleWebAuthorizationBroker_AuthorizeAsync_Google_Apis_Auth_OAuth2_ClientSecrets_System_Collections_Generic_IEnumerable_System_String__System_String_System_Threading_CancellationToken_Google_Apis_Util_Store_IDataStore_Google_Apis_Auth_OAuth2_ICodeReceiver_)
- `void TryRefreshToken()`: Refreshes the token if it is stale. [Documentation](https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Auth.OAuth2.UserCredential?hl=en#Google_Apis_Auth_OAuth2_UserCredential_RefreshTokenAsync_System_Threading_CancellationToken_)
- `string? GetFolderIdByPath(string path)`: Gets the folder ID by its path.
- `string? GetFolderId(string folderName, string parentId)`: Gets the folder ID by its name and parent ID.
- `string CreateFolder(string folderName, string parentFolderId = "root")`: Creates a folder in Google Drive.

## Acknowledgements

- [Google Drive API](https://developers.google.com/drive)
- [Google API .NET Client Library](https://github.com/googleapis/google-api-dotnet-client)
