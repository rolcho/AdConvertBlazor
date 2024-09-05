using System.Text;
using AdConvert.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace AdConvert.Services;

public interface IFileHandler
{
    Task<List<AdData>> ReadFileContentAsync(IBrowserFile file);
    Task CreateFileAsync(List<AdData> ads, string fileName);
}

public class FileHandler : IFileHandler
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

        await CreateFileAsync(ads, file.Name);

        return ads;
    }

    public async Task CreateFileAsync(List<AdData> ads, string fileName)
    {
        var sb = new StringBuilder();
        foreach (var ad in ads)
        {
            sb.AppendLine($"{ad.StrongText}{ad.AdText ?? string.Empty}");
            if (ad.AdStyle != AdType.Header)
            {
                sb.AppendLine(ad.AdId);
            }
        }

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        writer.Write(sb.ToString());

        await writer.FlushAsync();
        stream.Position = 0;

        await File.WriteAllBytesAsync($"NYERS-{fileName}", stream.ToArray());

        stream.Close();
        stream.Dispose();

        writer.Close();
        writer.Dispose();
    }
}
