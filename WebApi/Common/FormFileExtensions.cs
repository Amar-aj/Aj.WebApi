namespace WebApi.Common;

public class FormFileExtensions
{
    public static async Task<(byte[] bytes, string fileExtension, string fileType)> GetFileDetailsAsync(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return (null, null, null);
        }

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        var fileName = formFile.FileName;
        var fileExtension = Path.GetExtension(fileName);
        var fileType = formFile.ContentType;

        return (memoryStream.ToArray(), fileExtension, fileType);
    }
}
