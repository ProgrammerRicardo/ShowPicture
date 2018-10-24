using Newtonsoft.Json.Linq;
using System.IO;

public class CConfig
{
    public static string pictureDirectoryPath { get; set; }
    public static int maxRelevantPictureNum { get; set; }
    public static float initialPictureSize { get; set; }
    public static float relevantPictureDistanceToCenter { get; set; }
    public static float frontPictureDistanceToCenter { get; set; }

    public static void initConfig(string vConfigFilePath)
    {
        var jsonText = File.ReadAllText(vConfigFilePath);
        var obj = JObject.Parse(jsonText);
        pictureDirectoryPath = obj["pictureDirectoryPath"].ToString();
        maxRelevantPictureNum = obj["maxRelevantPictureNum"].ToObject<int>();
        initialPictureSize = obj["initialPictureSize"].ToObject<float>();
        relevantPictureDistanceToCenter = obj["relevantPictureDistanceToCenter"].ToObject<float>();
        frontPictureDistanceToCenter = obj["frontPictureDistanceToCenter"].ToObject<float>();
    }
}
