using GoogleDriveApi_DotNet;

GoogleDriveApi driveApi = await GoogleDriveApi.CreateBuilder()
    .SetCredentialsPath("credentials.json")
    .SetTokenFolderPath("_metadata")
    .SetApplicationName("[Your App Name]")
    .BuildAsync();

string newFolderId = driveApi.CreateFolder("NewFolderName");

Console.WriteLine("New Folder ID: " + newFolderId);