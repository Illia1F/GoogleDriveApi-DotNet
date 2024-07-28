
# Sample Code Snippets for GoogleDriveApi-DotNet

## Creating an Instance of GoogleDriveApi

First, create an instance of the `GoogleDriveApi` class using the fluent builder pattern with your credentials and token paths:

```csharp
GoogleDriveApi gDriveApi = await GoogleDriveApi.CreateBuilder()
    .SetCredentialsPath("credentials.json")
    .SetTokenFolderPath("_metadata")
    .SetApplicationName("[Your App Name]")
    .BuildAsync();
```

## Creating Folders

Create a new folder named "NewFolderName" in the root directory and then another folder named "NewFolderNameV2" inside the first folder.

```csharp
string newFolderId = gDriveApi.CreateFolder(folderName: "NewFolderName");

string newFolderId2 = gDriveApi.CreateFolder(folderName: "NewFolderNameV2", parentFolderId: newFolderId);

Console.WriteLine("New Folder ID: " + newFolderId);
Console.WriteLine("New Folder ID2: " + newFolderId2);
```

## Retrieving a List of Folders in the Root Directory

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

## Downloading Files from Google Drive

Download a file by its ID.

```csharp
string parentFolderId = "root";
string sourceFolderName = "FileDownloader Test Folder";
string? sourceFolderId = gDriveApi.GetFolderIdBy(sourceFolderName, parentFolderId);
if (sourceFolderId is null)
{
    Console.WriteLine($"Cannot find a folder with a name {sourceFolderName}.");
    return;
}

string fullFileNameToDownload = "Lesson_1.pdf";

string? fileId = gDriveApi.GetFileIdBy(fullFileNameToDownload, sourceFolderId);
if (fileId is null)
{
    Console.WriteLine($"Cannot find a file with a name {fullFileNameToDownload}.");
    return;
}

await gDriveApi.DownloadFileAsync(fileId);
```