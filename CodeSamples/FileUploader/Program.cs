using GoogleDriveApi_DotNet;
using GoogleDriveApi_DotNet.Exceptions;
using MimeMapping;

GoogleDriveApi gDriveApi = await GoogleDriveApi.CreateBuilder()
    .SetCredentialsPath("credentials.json")
    .SetTokenFolderPath("_metadata")
    .SetApplicationName("QuickFilesLoad")
    .BuildAsync();


string filePath = "Files/TEST.docx";

try
{
    string fileId = gDriveApi.UploadFilePath(filePath, KnownMimeTypes.Docx);

    Console.WriteLine($"File has been successfuly uploded with ID({fileId})");
}
catch (CreateMediaUploadException ex)
{
    Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}