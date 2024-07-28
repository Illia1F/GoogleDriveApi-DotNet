using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleDriveApi_DotNet.Helpers;

namespace GoogleDriveApi_DotNet;

public class GoogleDriveApi
{
    public const string RootFolderId = "root";
    private string _credentialsPath = "credentials.json";
    private string _tokenFolderPath = "_metadata";
    private string? _applicationName;
    private DriveService? _service;
    private UserCredential? _credential;

    public DriveService Provider => _service ?? throw new InvalidOperationException("The GoogleDriveApi is not initialized and authorized.");
    public bool IsTokenShouldBeRefreshed => _credential?.Token?.IsStale ?? false;

    public GoogleDriveApi() { }

    public static GoogleDriveApiBuilder CreateBuilder() => new GoogleDriveApiBuilder();

    public class GoogleDriveApiBuilder
    {
        private readonly GoogleDriveApi _googleDriveService = new GoogleDriveApi();

        /// <summary>
        /// Sets the path to the credentials JSON file. Default value is "credentials.json".
        /// **Note**: Place the downloaded JSON file in your project directory.
        /// <para>Documentation: https://developers.google.com/identity/protocols/oauth2</para>
        /// </summary>
        /// <param name="path">The path to the credentials JSON file.</param>
        /// <returns>The builder instance.</returns>
        public GoogleDriveApiBuilder SetCredentialsPath(string path)
        {
            _googleDriveService._credentialsPath = path;
            return this;
        }

        /// <summary>
        /// Sets the path to the token JSON file folder where it will store the token in a file. Default value is "_metadata".
        /// <para>Documentation: https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth</para>
        /// </summary>
        /// <param name="folderPath">The path to the token JSON file.</param>
        /// <returns>The builder instance.</returns>
        public GoogleDriveApiBuilder SetTokenFolderPath(string folderPath)
        {
            _googleDriveService._tokenFolderPath = folderPath;
            return this;
        }

        /// <summary>
        /// Sets the name of the application. Default value is null.
        /// <para>Documentation: https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Services.BaseClientService.Initializer#Google_Apis_Services_BaseClientService_Initializer_ApplicationName</para>
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <returns>The builder instance.</returns>
        public GoogleDriveApiBuilder SetApplicationName(string name)
        {
            _googleDriveService._applicationName = name;
            return this;
        }

        /// <summary>
        /// Builds and authorizes the GoogleDriveApi instance asynchronously.
        /// <para>Documentation: https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker?hl=en#Google_Apis_Auth_OAuth2_GoogleWebAuthorizationBroker_AuthorizeAsync_Google_Apis_Auth_OAuth2_ClientSecrets_System_Collections_Generic_IEnumerable_System_String__System_String_System_Threading_CancellationToken_Google_Apis_Util_Store_IDataStore_Google_Apis_Auth_OAuth2_ICodeReceiver_</para>
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the authorized GoogleDriveApi instance.</returns>
        public async Task<GoogleDriveApi> BuildAsync()
        {
            await _googleDriveService.AuthorizeAsync();
            return _googleDriveService;
        }
    }

    private async Task AuthorizeAsync()
    {
        using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
        {
            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None,
                new FileDataStore(_tokenFolderPath, true));
        }

        _service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = _credential,
            ApplicationName = _applicationName,
        });
    }

    /// <summary>
    /// Refreshes the token by calling to RefreshTokenAsync.
    /// <para>Documentation: https://cloud.google.com/dotnet/docs/reference/Google.Apis/latest/Google.Apis.Auth.OAuth2.UserCredential?hl=en#Google_Apis_Auth_OAuth2_UserCredential_RefreshTokenAsync_System_Threading_CancellationToken_</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the GoogleDriveApi is not initialized and authorized.</exception>
    public void TryRefreshToken()
    {
        if (_credential is null)
        {
            throw new InvalidOperationException("The GoogleDriveApi is not initialized and authorized.");
        }

        if (IsTokenShouldBeRefreshed)
        {
            _credential.RefreshTokenAsync(CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
    }

    /// <inheritdoc cref="Internal_GetFolderIdByAsync"/>
    public string? GetFolderIdBy(string folderName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_GetFolderIdByAsync(folderName, parentFolderId)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc cref="Internal_GetFolderIdByAsync"/>
    public Task<string?> GetFolderIdByAsync(string folderName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_GetFolderIdByAsync(folderName, parentFolderId);
    }

    /// <summary>
    /// Retrieves the ID of a folder by its name within a specified parent folder. 
    /// Default value  for <paramref name="parentFolderId"/> is "root".
    /// </summary>
    /// <param name="folderName">The name of the folder to search for.</param>
    /// <param name="parentFolderId">(optional) The ID of the parent folder to search within (default is "root").</param>
    /// <returns>The ID of the folder if found; otherwise, null.</returns>
    private async Task<string?> Internal_GetFolderIdByAsync(string folderName, string parentFolderId)
    {
        var listRequest = Provider.Files.List();
        listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and '{parentFolderId}' in parents and trashed=false";
        listRequest.Fields = "files(id, name)";
        listRequest.PageSize = 1;

        var result = await listRequest.ExecuteAsync();
        GoogleFile? file = result.Files.FirstOrDefault();

        return file?.Id;
    }

    /// <inheritdoc cref="Internal_GetFoldersByAsync"/>
    public List<(string id, string name)> GetFoldersBy(string parentFolderId, int pageSize = 50)
    {
        TryRefreshToken();

        return Internal_GetFoldersByAsync(parentFolderId, pageSize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc cref="Internal_GetFoldersByAsync"/>
    public Task<List<(string id, string name)>> GetFoldersByAsync(string parentFolderId, int pageSize = 50)
    {
        TryRefreshToken();

        return Internal_GetFoldersByAsync(parentFolderId, pageSize);
    }

    /// <summary>
    /// Retrieves a list of folders within a specified parent folder.
    /// </summary>
    /// <param name="parentFolderId">The ID of the parent folder to search within.</param>
    /// <param name="pageSize">(optional) The number of results to retrieve per page (default is 50).</param>
    /// <returns>A list of tuples, each containing the ID and name of a folder.</returns>
    private async Task<List<(string id, string name)>> Internal_GetFoldersByAsync(string parentFolderId, int pageSize)
    {
        var allFolders = new List<GoogleFile>();
        string? pageToken = null;
        string qSelector = $"mimeType='application/vnd.google-apps.folder' and '{parentFolderId}' in parents and trashed=false";
        const string fields = "nextPageToken, files(id, name)";
        do
        {
            var listRequest = Provider.Files.List();
            listRequest.Q = qSelector;
            listRequest.Fields = fields;
            listRequest.PageSize = pageSize;
            listRequest.PageToken = pageToken;

            var result = await listRequest.ExecuteAsync();
            if (result.Files is not null)
            {
                allFolders.AddRange(result.Files);
            }

            pageToken = result.NextPageToken;
        } while (pageToken is not null);

        return allFolders
            .Select(f => (f.Id, f.Name))
            .ToList();
    }

    /// <inheritdoc cref="Internal_CreateFolderAsync"/>
    public string CreateFolder(string folderName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_CreateFolderAsync(folderName, parentFolderId)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc cref="Internal_CreateFolderAsync"/>
    public Task<string> CreateFolderAsync(string folderName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_CreateFolderAsync(folderName, parentFolderId);
    }

    /// <summary>
    /// Creates a new folder in Google Drive.
    /// </summary>
    /// <param name="folderName">The name of the folder to create.</param>
    /// <param name="parentFolderId">(optional) The ID of the parent folder where the new folder will be created (default is "root").</param>
    /// <returns>The ID of the created folder.</returns>
    private async Task<string> Internal_CreateFolderAsync(string folderName, string parentFolderId)
    {
        var driveFolder = new GoogleFile()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = new [] { parentFolderId }
        };

        var request = Provider.Files.Create(driveFolder);
        GoogleFile file = await request.ExecuteAsync();

        return file.Id;
    }

    /// <inheritdoc cref="Internal_GetFileIdByAsync"/>
    public string? GetFileIdBy(string fullFileName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_GetFileIdByAsync(fullFileName, parentFolderId)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc cref="Internal_GetFileIdByAsync"/>
    public Task<string?> GetFileIdByAsync(string fullFileName, string parentFolderId = RootFolderId)
    {
        TryRefreshToken();

        return Internal_GetFileIdByAsync(fullFileName, parentFolderId);
    }

    /// <summary>
    /// Gets the file ID by its name and parent folder ID.
    /// </summary>
    /// <param name="fullFileName">The name of the file with an extension to search for.</param>
    /// <param name="parentFolderId">The ID of the parent folder where the file is located. Use "root" for the root directory.</param>
    /// <returns>The file ID if found, otherwise null.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the GoogleDriveApi is not initialized and authorized.</exception>
    private async Task<string?> Internal_GetFileIdByAsync(string fullFileName, string parentFolderId)
    {
        var request = Provider.Files.List();
        request.Q = $"name = '{fullFileName}' and '{parentFolderId}' in parents and trashed = false";
        request.Fields = "files(id, name)";
        request.PageSize = 1;

        var result = await request.ExecuteAsync();
        var file = result.Files.FirstOrDefault();

        return file?.Id;
    }

    /// <summary>
    /// Downloads a file from Google Drive by its file ID.
    /// </summary>
    /// <param name="fileId">The ID of the file to download.</param>
    /// <param name="saveToPath">The local path where the file will be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the GoogleDriveApi is not initialized and authorized.</exception>
    public async Task DownloadFileAsync(string fileId, string saveToPath = "Downloads")
    {
        TryRefreshToken();

        // Ensure the folderPath directory exists
        Directory.CreateDirectory(saveToPath);

        var request = Provider.Files.Get(fileId);
        GoogleFile file = request.Execute();
        string fileName = Path.GetFileNameWithoutExtension(file.Name);
        string? fileMimeType = file.MimeType;

        // Check for a specific Google Workplace Mime Types
        bool isGoogleSpecificMimeType = MimeTypeHelper.IsGDriveMimeType(fileMimeType);
        if (isGoogleSpecificMimeType)
        {
            fileMimeType = MimeTypeHelper.GetExportMimeTypeBy(fileMimeType);
            if (fileMimeType is null)
            {
                throw new InvalidOperationException($"Unsupported mime type ({file.MimeType})");
            }
        }

        string? extension = MimeTypeHelper.GetExtensionBy(fileMimeType);
        if (extension is null)
        {
            throw new InvalidOperationException($"Unsupported mime type ({file.MimeType})");
        }

        string fullPath = Path.Combine(saveToPath, $"{fileName}.{extension}");

        if (isGoogleSpecificMimeType)
        {
            await ExportGoogleFileAsync(fileId, fileMimeType, fullPath);
        }
        else
        {
            await DownloadBinaryFileAsync(fileId, fullPath);
        }
    }

    /// <summary>
    /// Exports a Google-specific file (like Google Docs, Sheets, Slides) to a specified MIME type.
    /// </summary>
    /// <param name="fileId">The ID of the file to export.</param>
    /// <param name="exportMimeType">The MIME type to which the file should be exported.</param>
    /// <param name="fullFilePath">The full path where the exported file will be saved.</param>
    private async Task ExportGoogleFileAsync(string fileId, string exportMimeType, string fullFilePath)
    {
        var request = Provider.Files.Export(fileId, exportMimeType);
        var streamFile = new MemoryStream();

        request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    Console.WriteLine(progress.BytesDownloaded);
                    break;
                case DownloadStatus.Completed:
                    Console.WriteLine("Export complete.");
                    SaveStream(streamFile, fullFilePath);
                    break;
                case DownloadStatus.Failed:
                    Console.WriteLine("Export failed.");
                    Console.WriteLine($"Error: {progress.Exception.Message}");
                    break;
            }
        };

        await request.DownloadAsync(streamFile);
    }

    /// <summary>
    /// Downloads a binary file from Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to download.</param>
    /// <param name="fullFilePath">The full path where the downloaded file will be saved.</param>
    private async Task DownloadBinaryFileAsync(string fileId, string fullFilePath)
    {
        var request = Provider.Files.Get(fileId);
        var streamFile = new MemoryStream();

        request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    Console.WriteLine(progress.BytesDownloaded);
                    break;
                case DownloadStatus.Completed:
                    Console.WriteLine("Download complete.");
                    SaveStream(streamFile, fullFilePath);
                    break;
                case DownloadStatus.Failed:
                    Console.WriteLine("Download failed.");
                    Console.WriteLine($"Error: {progress.Exception.Message}");
                    break;
            }
        };

        await request.DownloadAsync(streamFile);
    }

    private static void SaveStream(MemoryStream stream, string fullFilePath)
    {
        using (var fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
        {
            stream.WriteTo(fileStream);
        }
    }
}