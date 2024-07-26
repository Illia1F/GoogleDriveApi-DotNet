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

## Example Code

### Creating an Instance of GoogleDriveApi

First, create an instance of the `GoogleDriveApi` class using the fluent builder pattern with your credentials and token paths:

```csharp
GoogleDriveApi driveApi = await GoogleDriveApi.CreateBuilder()
	.SetCredentialsPath("credentials.json")
	.SetTokenFolderPath("_metadata")
	.SetApplicationName("[Your App Name]")
	.BuildAsync();
```

## Class Documentation

### GoogleDriveApi

A class for interacting with the Google Drive API.

- `credentialsPath`: Path to the credentials JSON file.
- `tokenPath`: Path to the token JSON file.
- `applicationName`: The name of the application.

#### Methods

- `GoogleDriveApiBuilder SetCredentialsPath(string path)`: Sets the path to the credentials JSON file. [Documentation](https://developers.google.com/identity/protocols/oauth2)
- `GoogleDriveApiBuilder SetTokenPath(string path)`: Sets the path to the token JSON file. [Documentation](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth)
- `GoogleDriveApiBuilder SetApplicationName(string name)`: Sets the name of the application. [Documentation](https://developers.google.com/drive/api/v3/about-auth)
- `Task<GoogleDriveApi> BuildAsync()`: Builds and authorizes the GoogleDriveApi instance asynchronously. [Documentation](https://developers.google.com/identity/protocols/oauth2)
- `Task<string> CreateFolderAsync(string folderName, string parentFolderId = "root")`: Creates a folder in Google Drive asynchronously. [Documentation](https://developers.google.com/drive/api/v3/folder)
- `Task<bool> IsTokenExpiredAsync()`: Checks if the access token is expired asynchronously. [Documentation](https://developers.google.com/identity/protocols/oauth2)
- `Task RefreshTokenAsync()`: Refreshes the access token if it is expired asynchronously. [Documentation](https://developers.google.com/identity/protocols/oauth2)

## Acknowledgements

- [Google Drive API](https://developers.google.com/drive)
- [Google API .NET Client Library](https://github.com/googleapis/google-api-dotnet-client)
