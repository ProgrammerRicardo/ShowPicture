using System.Collections.Generic;
using UnityEngine;

public class CSelectRelevantPic
{
    public static List<CPicture> getSeacrchResultSet(Dictionary<GameObject, SImageInfo> vSelectRange, int vShowNum, EOptions vOption)
    {
        List<CPicture> SearchResultSet = new List<CPicture>();
        SearchResultSet.Clear();
        SImageInfo OrgData = new SImageInfo();
        CPictureManager pictureManager = CPictureManager.instance;
        List<CPicture> PictureList = pictureManager.pictureList;
        GameObject vOrgPic = PictureList[vShowNum].pictureObject;
        vSelectRange.TryGetValue(vOrgPic, out OrgData);
        string orgImgDes = OrgData.ImgDescription.value;
        SearchResultSet = getSeacrchResultSetByImgDescription(vSelectRange, orgImgDes);
        return SearchResultSet;
    }

    public static List<CPicture> getSeacrchResultSetByImgDescription(Dictionary<GameObject, SImageInfo> vSelectRange, string vImagDescribe)
    {
        List<CPicture> SearchResultSet = new List<CPicture>();
        SearchResultSet.Clear();
        CPictureManager pictureManager = CPictureManager.instance;
        List<CPicture> PictureList = pictureManager.pictureList;
        foreach (GameObject obj in vSelectRange.Keys)
        {
            
            SImageInfo TempData = new SImageInfo();
            vSelectRange.TryGetValue(obj, out TempData);
            if (vImagDescribe == TempData.ImgDescription.value)
            {
                int i = pictureManager.getPictureNum(vSelectRange, obj);
                if (i != -1)
                    SearchResultSet.Add(PictureList[i]);
            }
        }

        return SearchResultSet;
    }
}