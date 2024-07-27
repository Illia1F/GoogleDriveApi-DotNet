namespace GoogleDriveApi_DotNet.Helpers;

public static class MimeTypeHelper
{
    public static bool IsGDriveMimeType(string mimeType)
    {
        return mimeType.StartsWith("application/vnd.google-apps");
    }

    public static string? GetExportMimeTypeBy(string mimeType)
    {
        return mimeType switch
        {
            "application/vnd.google-apps.document" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.google-apps.spreadsheet" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.google-apps.presentation" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.google-apps.drawing" => "image/png",
            _ => null,
        };
    }

    public static string? GetExtensionBy(string mimeType)
    {
        return MimeMapping.MimeUtility.GetExtensions(mimeType).FirstOrDefault();
    }
}