using System.Text;
using AdConvert.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace AdConvert.Services;

public interface IFileHandler
{
    Task<List<AdData>> ReadFileContentAsync(IBrowserFile file);
    string CreateText(List<AdData> ads);
}

public sealed class FileHandler : IFileHandler
{
    private readonly AdHandler adHandler;

    public FileHandler()
    {
        adHandler = new AdHandler();
    }

    public async Task<List<AdData>> ReadFileContentAsync(IBrowserFile file)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1250"));
        var content = await reader.ReadToEndAsync();

        var ads = adHandler.ConvertText(adHandler.ReplaceText(content));
        reader.Close();
        reader.Dispose();

        stream.Close();
        stream.Dispose();

        return ads;
    }

    public string CreateText(List<AdData>? ads)
    {
        if (ads is null)
        {
            return string.Empty;
        }
        var sb = new StringBuilder();
        foreach (var ad in ads)
        {
            sb.AppendLine($"{ad.StrongText}{ad.AdText ?? string.Empty}");
            if (ad.AdStyle != AdType.Header)
            {
                sb.AppendLine(ad.AdId);
            }
        }

        return sb.ToString() ?? "";
    }
}
