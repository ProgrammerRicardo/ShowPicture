using UnityEngine;
using UnityEngine.UI;

public class CDisplayPictureInfo : MonoBehaviour
{
    public Text m_DescribeText;
    public Text m_ArtistText;
    public Text m_LocalText;
    public Text m_TimeText;
    public Text m_IndexText;
    public Text m_TotalPictureNumText;
    public Image m_Image;
    public Sprite m_IntialSprite;

    private CPictureManager m_PictureManager;

    void Start()
    {
        m_PictureManager = CPictureManager.instance;

        clearDisplayInfo();
    }

    public void setDisplayInfo(int vPictureIndex)
    {
        if (vPictureIndex < 0 || vPictureIndex >= m_PictureManager.pictureList.Count)
            return;

        var picture = m_PictureManager.pictureList[vPictureIndex];

        SImageInfo Data = m_PictureManager.imageInfoDic[picture.pictureObject];
        m_ArtistText.text = Data.Artist.value.ToString();
        m_DescribeText.text = Data.ImgDescription.value.ToString();
        m_LocalText.text = Data.LocationInfo.country.ToString() + Data.LocationInfo.province.ToString() + Data.LocationInfo.city.ToString() + Data.LocationInfo.district.ToString();
        m_TimeText.text = Data.DateTimeOriginal.value.ToString();
        m_IndexText.text = picture.index.ToString();
        m_TotalPictureNumText.text = m_PictureManager.pictureList.Count.ToString();
        m_Image.sprite = picture.pictureObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void clearDisplayInfo()
    {
        m_ArtistText.text = "";
        m_DescribeText.text = "";
        m_LocalText.text = "";
        m_TimeText.text = "";
        m_IndexText.text = "";
        m_TotalPictureNumText.text = "";
        m_Image.sprite = m_IntialSprite;
    }
}