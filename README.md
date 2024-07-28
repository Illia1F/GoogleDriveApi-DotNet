# GoogleDriveApi-DotNet

This C# library simplifies interaction with the Google Drive API. While it doesn't cover the entire API, it includes the most commonly used endpoints for easier access to Google Drive.

## Features

- Extract folder ID from a Google Drive folder URL.
- Get the folder name by its ID.
- Generate Google Drive folder URL by its id.
- Obtain the full path of a folder starting from the root using its ID. (Very slowly)
- Create folders in Google Drive.
- Check if the access token is expired.
- Refresh the access token if expired.
- Download files from Google Drive.
	For more detailed information, please refer to the following documents: [Downloading Files from Google Drive](DOCS/DownloadingFiles.md)
- Upload files to Google Drive. (Not yet)
- Delete files and folders in Google Drive. (Not yet)

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
   - Select `Desktop app` or `Web app` as the application type.
   - Click `Create`.
   - Download the JSON file containing your credentials.

### Setting Up Your Application

1. Place the downloaded `credentials.json` file in your project directory.
2. Initialize the `GoogleDriveApi` class with your credentials.

## Example Code

### Creating an Instance of GoogleDriveApi

First, create an instance of the `GoogleDriveApi` class using the fluent builder pattern with your credentials and token paths:

```csharp
GoogleDriveApi gDriveApi = await GoogleDriveApi.CreateBuilder()
	.SetCredentialsPath("credentials.json")
	.SetTokenFolderPath("_metadata")
	.SetApplicationName("[Your App Name]")
	.BuildAsync();
```

### Creating folders

Create a new folder named "NewFolderName" in the root directory and then another folder named "NewFolderNameV2" inside the first folder.

```csharp
string newFolderId = gDriveApi.CreateFolder(folderName: "NewFolderName");

string newFolderId2 = gDriveApi.CreateFolder(folderName: "NewFolderNameV2", parentFolderId: newFolderId);

Console.WriteLine("New Folder ID: " + newFolderId);
Console.WriteLine("New Folder ID2: " + newFolderId2);
```

### Retrieving a list of folders in the root directory

Retrieve and print all folders in the root directory and their children.

```csharp
// Retrieves a list of folders in the root directory
var folders = gDriveApi.GetFoldersBy(parentFolderId: "root");

for (int i = 0; i < folders.Count; i++)
{
   var folder = folders[i];

   Console.WriteLine($"{i + 1}. [{folder.name}] with ID({folder.id})");

   // Retrieves a list of subfolders within the current folder
   var subFolders = gDriveApi.GetFoldersBy(folder.id);
   for (int j = 0; j < subFolders.Count; j++)
   {
      var subFolder = subFolders[j];

      Console.WriteLine($"---|{j + 1}. [{subFolder.name}] with ID({subFolder.id})");
   }
}
```

## License

This project is licensed under the MIT License.

## Acknowledgements

- [Google Drive API](https://developers.google.com/drive)
- [Google API .NET Client Library](https://github.com/googleapis/google-api-dotnet-client)
