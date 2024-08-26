using System;
using AdConvert.Models;

namespace AdConvert.Services;

public class AdTextHandler
{
    private static readonly Dictionary<string, string> ConversionTable =
        new()
        {
            { "Ö", "." },
            { "Ń", "Á" },
            { "…", "É" },
            { "Õ", "Í" },
            { "”", "Ó" },
            { "÷", "Ö" },
            { "<0x0150>", "Ő" },
            { "ŕ", "Ú" },
            { "‹", "Ü" },
            { "<0x0170>", "Ű" },
            { "Ř", "Ű" },
            { "Š", "á" },
            { "ť", "é" },
            { "Ū", "í" },
            { "ů", "ó" },
            { "Ų", "ö" },
            { "<0x0151>", "ő" },
            { "ķ", "ú" },
            { "Ł", "ü" },
            { "<0x0171>", "ű" },
            { "<ANSI-WIN>\n", "" },
        };

    private static AdType StyleSelector(string line)
    {
        if (line.Contains("KNEGA\\") || line.Contains("PNEGATIV\\"))
        {
            return AdType.HighlightBackground;
        }
        if (line.Contains("KKERETES\\") || line.Contains("PKERETES\\"))
        {
            return AdType.HighlightFrame;
        }
        if (line.Contains("NEGATIV\\"))
        {
            return AdType.BlackBackground;
        }
        if (line.Contains("FKERETES\\"))
        {
            return AdType.BlackFrame;
        }
        if (line.Contains("SARGA\\"))
        {
            return AdType.YellowBackground;
        }
        if (line.Contains("SZURKE\\"))
        {
            return AdType.GrayBackground;
        }
        if (line.Contains("KISERO\\"))
        {
            return AdType.LightRedBackground;
        }
        if (line.Contains("KARIKAS\\"))
        {
            return AdType.Circle;
        }
        if (line.Contains("APRO_fej") || line.Contains("APRÓ fej"))
        {
            return AdType.Header;
        }
        return AdType.Plain;
    }

    private static readonly string[] idSeparator = ["sorszam>", "SOR>"];

    public static List<AdData> ConvertText(string text)
    {
        foreach (var item in ConversionTable)
        {
            text = text.Replace(item.Key, item.Value);
        }

        var lines = text.Split("\n");
        var currentAd = new AdData();
        List<AdData> adList = [];

        foreach (var line in lines)
        {
            var shortLine = line.Replace("<pstyle:APRO\\:", "");

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

            if (currentAd.AdText != String.Empty)
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
