using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace GoogleDriveApi_DotNet;

public class GoogleDriveApi
{
    private string _credentialsPath = "credentials.json";
    private string _tokenFolderPath = "_metadata";
    private string? _applicationName;
    private DriveService? _service;
    private UserCredential? _credential;

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
    /// <exception cref="InvalidOperationException"></exception>
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

    public string? GetFolderIdByPath(string path)
    {
        string[] folders = path.Split('/');
        string parentId = "root";
        string? folderId = null;

        foreach (var folder in folders)
        {
            folderId = GetFolderId(folder, parentId);
            if (folderId == null)
            {
                return null;
            }

            parentId = folderId; // Move to the next folder level
        }

        return folderId;
    }

    public string? GetFolderId(string folderName, string parentId)
    {
        TryRefreshToken();

        FilesResource.ListRequest listRequest = _service.Files.List();
        listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and '{parentId}' in parents and trashed=false";
        listRequest.Fields = "files(id, name)";

        IList<GoogleFile> files = listRequest.Execute().Files;

        if (files != null && files.Count > 0)
        {
            return files[0].Id;
        }
        else
        {
            return null;
        }
    }

    public string CreateFolder(string folderName, string parentFolderId = "root")
    {
        TryRefreshToken();

        var driveFolder = new GoogleFile()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = [parentFolderId]
        };

        var request = _service.Files.Create(driveFolder);
        GoogleFile file = request.Execute();

        return file.Id;
    }
}
