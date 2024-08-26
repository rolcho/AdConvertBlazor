namespace AdConvert.Models;

public class AdData
{
    public string AdId { get; set; } = String.Empty;
    public string StrongText { get; set; } = String.Empty;
    public string AdText { get; set; } = String.Empty;
    public AdType AdStyle { get; set; } = AdType.Plain;

    public static readonly Dictionary<AdType, string> Styles =
        new()
        {
            { AdType.Plain, "text-black p-y-4 border-b-2 border-black w-full" },
            { AdType.HighlightBackground, "bg-red-600 text-white p-4 w-full" },
            { AdType.YellowBackground, "text-black p-4 bg-yellow-400 w-full" },
            { AdType.BlackBackground, "text-white p-4 bg-black w-full" },
            { AdType.GrayBackground, "p-4 bg-gray-300 w-full" },
            { AdType.BlackFrame, "border-2 border-black p-4 w-full" },
            { AdType.HighlightFrame, "border-2 border-red-600 p-4 w-full" },
            {
                AdType.Header,
                "p-4 bg-red-600 text-white w-full text-center uppercase border-b-4 border-black w-full"
            },
            { AdType.Circle, "p-6 border-4 border-pink-600 rounded-full w-full" },
            { AdType.LightRedBackground, "text-black p-4 bg-red-200 w-full" },
            { AdType.Error, "text-red-600 p-4 w-full" },
        };
}
