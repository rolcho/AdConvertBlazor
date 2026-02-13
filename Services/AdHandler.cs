using AdConvert.Models;

namespace AdConvert.Services;

public class StringPair
{
    public required string Original { get; set; }
    public required string Fixed { get; set; }
}

public sealed class AdHandler
{
    private static readonly Dictionary<string, string> ConversionTable =
        new()
        {
            { "<0x0150>", "Ő" },
            { "<0x0170>", "Ű" },
            { "<0x0151>", "ő" },
            { "<0x0171>", "ű" },
            { "<ANSI-WIN>\n", "" },
            { "<ANSI-WIN>\r", "" },
            { "\r", "" },
        };

    private static readonly Dictionary<string, AdType> StyleTable =
        new()
        {
            { "APRO_fej", AdType.Header },
            { "APRÓ fej", AdType.Header },
            { "KNEGA\\", AdType.HighlightBackground },
            { "PNEGATIV\\", AdType.HighlightBackground },
            { "KKERETES\\", AdType.HighlightFrame },
            { "PKERETES\\", AdType.HighlightFrame },
            { "NEGATIV\\", AdType.BlackBackground },
            { "FKERETES\\", AdType.BlackFrame },
            { "SARGA\\", AdType.YellowBackground },
            { "SZURKE\\", AdType.GrayBackground },
            { "KISERO\\", AdType.LightRedBackground },
            { "KARIKAS\\", AdType.Circle },
        };

    private static readonly List<StringPair> FixedStyleStrings =
    [
        new StringPair { Original = "<pstyle:APRÓ\\:", Fixed = "" },
        new StringPair { Original = "<pstyle:APRO\\:", Fixed = "" },
        new StringPair { Original = "KERETES apróhird", Fixed = "FKERETES" },
        new StringPair { Original = "KERETES apró SORSZÁM", Fixed = "fkeretesSOR" },
        new StringPair { Original = "NEGA apró", Fixed = "NEGATIV" },
        new StringPair { Original = "Nega APRÓ", Fixed = "negativ" },
        new StringPair { Original = "Nega SORSZÁM", Fixed = "negativSOR" },
        new StringPair { Original = "SÁRGA apróhird", Fixed = "SARGA" },
        new StringPair { Original = "SÁRGA apró", Fixed = "sarga" },
        new StringPair { Original = "APRÓ sorszám", Fixed = "sorszam" },
        new StringPair { Original = "PIFKERETES", Fixed = "PKERETES" },
        new StringPair { Original = "PIKERETES", Fixed = "pkeretes" },
    ];

    private static AdType StyleSelector(string line)
    {
        AdType? adType = StyleTable.FirstOrDefault(style => line.Contains(style.Key)).Value;
        return adType ?? AdType.Plain;
    }

    private static readonly string[] idSeparator = ["sorszam>", "SOR>"];

    public string ReplaceText(string text)
    {
        foreach (var item in ConversionTable)
        {
            text = text.Replace(item.Key, item.Value);
        }

        return text;
    }

    public string FixStyles(string line, List<StringPair> stringPairs)
    {
        foreach (var stringPair in stringPairs)
        {
            line = line.Replace(stringPair.Original, stringPair.Fixed);
        }

        return line;
    }

    public List<AdData> ConvertText(string text)
    {
        var lines = text.Split("\n");
        var currentAd = new AdData();
        List<AdData> adList = [];

        foreach (var line in lines)
        {
            var shortLine = FixStyles(line, FixedStyleStrings);

            if (shortLine.Contains("sorszam>") || shortLine.Contains("SOR>"))
            {
                currentAd.AdId = shortLine.Split(idSeparator, StringSplitOptions.None)[1];
                adList.Add(currentAd);
                currentAd = new AdData();
                continue;
            }

            currentAd.AdStyle = StyleSelector(shortLine);

            if (currentAd.AdStyle == AdType.Header)
            {
                var headerTextIndex = shortLine.IndexOf('>') + 1;
                currentAd.StrongText = shortLine[headerTextIndex..];
                adList.Add(currentAd);
                currentAd = new AdData();
                continue;
            }

            if (currentAd.AdText is not "")
            {
                currentAd.AdText += " " + shortLine;
                continue;
            }

            var adContentIndex = shortLine.IndexOf('>') + 1;
            var adContent = shortLine[adContentIndex..];
            var strongTextEnding = adContent.IndexOf(' ') + 1;

            currentAd.StrongText = adContent[..strongTextEnding];
            currentAd.AdText = adContent[strongTextEnding..];
        }

        return adList;
    }
}
