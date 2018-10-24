using System;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;

public enum EOptions { LOCATION, ARTIST, IMGDESCRIPTION, DATETIMEORIGINAL };

public struct SCommonTag
{
    public string tagName;
    public string value;

    public override string ToString()
    {
        return ("<TagName: " + tagName + "  Value: " + value + ">");
    }
}

public struct SLocationInfo
{
    public string country;
    public string province;
    public string city;
    public string district;

    public override string ToString()
    {
        return ("<Country: " + country + "  Province: " + province + "  City: " + city + "  District: " + district + ">");
    }
}

public struct SImageInfo
{
    public byte[] RawData;
    public int Width;
    public int Height;

    public SCommonTag GPSLatitudeRef;
    public SCommonTag GPSLatitude;
    public SCommonTag GPSLongitudeRef;
    public SCommonTag GPSLangitude;
    public SCommonTag Artist;
    public SCommonTag DateTimeOriginal;
    public SCommonTag ImgDescription;

    public SLocationInfo LocationInfo;

    public void init()
    {
        RawData = null;
        Width = 0;
        Height = 0;

        GPSLatitudeRef.tagName = "GPSLatitudeRef";
        GPSLatitudeRef.value = "";
        GPSLatitude.tagName = "GPSLatitude";
        GPSLatitude.value = "";
        GPSLongitudeRef.tagName = "GPSLongitudeRef";
        GPSLongitudeRef.value = "";
        GPSLangitude.tagName = "GPSLangitude";
        GPSLangitude.value = "";
        Artist.tagName = "Artist";
        Artist.value = "";
        DateTimeOriginal.tagName = "DateTimeOriginal";
        DateTimeOriginal.value = "";
        ImgDescription.tagName = "ImgDescription";
        ImgDescription.value = "";

        LocationInfo.country = "";
        LocationInfo.city = "";
        LocationInfo.province = "";
        LocationInfo.district = "";
    }

    public override string ToString()
    {
        string strInfo = "";
        strInfo += "\r\nGPSLatitudeRef:      " + GPSLatitudeRef.ToString();
        strInfo += "\r\nGPSLatitude:         " + GPSLatitude.ToString();
        strInfo += "\r\n GPSLongitudeRef:    " + GPSLongitudeRef.ToString();
        strInfo += "\r\n GPSLangitude:       " + GPSLangitude.ToString();
        strInfo += "\r\n Artist:             " + Artist.ToString();
        strInfo += "\r\n ImgDescription:     " + ImgDescription.ToString();
        strInfo += "\r\n LocationInfo:       " + LocationInfo.ToString();
        strInfo += "\r\n";
        return strInfo;
    }
}

public class CImageInfoReader
{
    public SImageInfo readImageInfo(string vImagePath)
    {
        FileStream fs = new FileStream(vImagePath, FileMode.Open);
        int fileLength = (int)fs.Length;
        byte[] fileDate = new byte[fs.Length];

        fs.Read(fileDate, 0, fileLength);
        System.Drawing.Image image = System.Drawing.Image.FromStream(fs);

        SImageInfo ExifInfoData = new SImageInfo();
        ExifInfoData.init();
        ExifInfoData.RawData = fileDate;
        ExifInfoData.Width = image.Width;
        ExifInfoData.Height = image.Height;

        int[] PropertyIdList = image.PropertyIdList;
        PropertyItem[] PropertyItemList = new PropertyItem[PropertyIdList.Length];

        System.Text.ASCIIEncoding AsciiValue = new System.Text.ASCIIEncoding();
        int index = 0;

        if (PropertyIdList.Length != 0)
        {
            foreach (int PropertyId in PropertyIdList)
            {
                PropertyItem TempItem = image.GetPropertyItem(PropertyId);
                PropertyItemList[index++] = TempItem;

                Int32 HexPropertyId = TempItem.Id;
                switch (HexPropertyId)
                {
                    case 0x0001:
                        ExifInfoData.GPSLatitudeRef.value = AsciiValue.GetString(TempItem.Value).Substring(0, AsciiValue.GetString(TempItem.Value).Length - 1);
                        break;
                    case 0x0002:
                        ExifInfoData.GPSLatitude.value = parseGpsInfoFromPropertyItem(TempItem);
                        break;
                    case 0x0003:
                        ExifInfoData.GPSLongitudeRef.value = AsciiValue.GetString(TempItem.Value).Substring(0, AsciiValue.GetString(TempItem.Value).Length - 1);
                        break;
                    case 0x0004:
                        ExifInfoData.GPSLangitude.value = parseGpsInfoFromPropertyItem(TempItem);
                        break;
                    case 0x013b:
                        ExifInfoData.Artist.value = AsciiValue.GetString(TempItem.Value);
                        ExifInfoData.Artist.value = ExifInfoData.Artist.value.Substring(0, ExifInfoData.Artist.value.Length - 1);
                        break;
                    case 0x9003:
                        ExifInfoData.DateTimeOriginal.value = AsciiValue.GetString(TempItem.Value);
                        ExifInfoData.DateTimeOriginal.value = ExifInfoData.DateTimeOriginal.value.Substring(0, ExifInfoData.DateTimeOriginal.value.Length - 1);
                        break;
                    case 0x010e:
                        ArrayList DescriptionList = new ArrayList();
                        string ImgDescription = AsciiValue.GetString(TempItem.Value);
                        ImgDescription = ImgDescription.Substring(0, ImgDescription.Length - 1);
                        if (ImgDescription.Length == 0)
                        {
                            //do nothing
                        }
                        else
                        {
                            string[] DescriptionStringArray = System.Text.RegularExpressions.Regex.Split(ImgDescription, "; ");
                            ExifInfoData.ImgDescription.value = DescriptionStringArray[0];
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        image.Dispose();
        image = null;
        fs.Close();

        return ExifInfoData;
    }

    private string mapExifInfoToString(string vExifTagName, string vValue)
    {
        string MappingResult = null;
        switch (vExifTagName)
        {
            case "GPSLatitudeRef":
                switch (vValue)
                {
                    case "N":
                        MappingResult = "N";
                        break;
                    case "S":
                        MappingResult = "S";
                        break;
                }
                break;
            case "GPSLongitudeRef":
                switch (vValue)
                {
                    case "E":
                        MappingResult = "E";
                        break;
                    case "W":
                        MappingResult = "W";
                        break;
                }
                break;
            default:
                break;

        }
        return MappingResult;
    }

    private string parseGpsInfoFromPropertyItem(PropertyItem vGpsProperty)
    {
        string GpsInfo = null;
        if (vGpsProperty.Value.Length != 24)
        {
            GpsInfo = "";
        }
        else
        {
            //degrees
            double d = BitConverter.ToUInt32(vGpsProperty.Value, 0) * 1.0d / BitConverter.ToUInt32(vGpsProperty.Value, 4);
            //minutes
            double m = BitConverter.ToUInt32(vGpsProperty.Value, 8) * 1.0d / BitConverter.ToUInt32(vGpsProperty.Value, 12);
            //seconds
            double s = BitConverter.ToUInt32(vGpsProperty.Value, 16) * 1.0d / BitConverter.ToUInt32(vGpsProperty.Value, 20);
            //如果录入的Gps信息有误
            if (d.ToString() == "NaN" || m.ToString() == "NaN" || s.ToString() == "NaN")
            {
                GpsInfo = "";
            }
            else
            {
                //格式构建
                GpsInfo = (d.ToString() + " " + m.ToString() + " " + s.ToString());
            }
        }
        return GpsInfo;
    }
}