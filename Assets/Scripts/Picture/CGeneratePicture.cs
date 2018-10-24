using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class CGeneratePicture : MonoBehaviour
{
    #region public fields 

    [Tooltip("The prefab used to create the picture object.")]
    public GameObject picturePrefab;

    #endregion

    #region private fields

    private Vector3 m_CenterPosition = new Vector3(0, 0, 0);
    private Vector3[] m_InitialPicturePositions = null;
    private SImageInfo[] m_ImageInfoSet = null;
    private List<string> m_FilePathSet = new List<string>();

    private int m_ThreadCount = 0;
    private ManualResetEvent m_ThreadFinishEvent = null;

    #endregion

    #region event functions

    #endregion

    #region public functions

    public void generatePictureObjectsFromDirectory(string vDirectoryPath)
    {
        if (!Directory.Exists(vDirectoryPath))
        {
            Debug.Log("input directory path does not exist, the default path will be used instead.");
            vDirectoryPath = Application.dataPath + CConstant.DEFAULT_PICTURE_DIRECTORY;
        }
        vDirectoryPath = CUtility.normalizePath(vDirectoryPath);
        var directoryInfo = new DirectoryInfo(vDirectoryPath);

        var allowedFileExtensions = new HashSet<string> { ".jpg", ".png", ".jpeg", ".bmp", ".gif" };
        foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*"))
        {
            string fileExtension = Path.GetExtension(fileInfo.Name).ToLower();
            if (allowedFileExtensions.Contains(fileExtension))
                m_FilePathSet.Add(vDirectoryPath + fileInfo.Name);
        }

        var pictureCount = m_FilePathSet.Count;
        m_InitialPicturePositions = CUtility.generateRandomCirclePositions(pictureCount, new Vector3(0, 50, 0), 15.0f);
        m_ImageInfoSet = new SImageInfo[pictureCount];

        __readImageInfo(pictureCount);

        for (int i = 0; i < pictureCount; ++i)
            __initPictureObject(i);

        __createArtistList();
        __createLocationList();
        __createUserCommentList();
    }

    #endregion

    #region private functions

    private void __readImageInfo(int vCount)
    {
        m_ThreadFinishEvent = new ManualResetEvent(false);
        m_ThreadCount = SystemInfo.processorCount;
        m_ThreadCount = m_ThreadCount < vCount ? m_ThreadCount : vCount;
        var sliceCount = vCount / m_ThreadCount;
        for (int i = 0; i < m_ThreadCount; ++i)
        {
            int startIndex = i * sliceCount;
            int endIndex = startIndex + sliceCount;
            if (i == (m_ThreadCount - 1)) endIndex = vCount;

            ThreadPool.QueueUserWorkItem(new WaitCallback(__readImageInfoThread), new int[] { startIndex, endIndex });
        }
        m_ThreadFinishEvent.WaitOne();
    }

    private void __readImageInfoThread(object vObject) //NOTE: 使用多线程读取图片数据，注意UnityEngine的很多方法只能在主线程中调用
    {
        var range = (int[])(vObject);
        var startIndex = range[0];
        var endIndex = range[1];
        for (int i = startIndex; i < endIndex; ++i)
        {
            var filePath = m_FilePathSet[i];
            CImageInfoReader reader = new CImageInfoReader();
            m_ImageInfoSet[i] = reader.readImageInfo(filePath);
        }
        if (Interlocked.Decrement(ref m_ThreadCount) == 0)
        {
            m_ThreadFinishEvent.Set();
        }
    }

    private void __initPictureObject(int vIndex)
    {
        var pictureObject = Instantiate(picturePrefab) as GameObject; //NOTE: Instantiate只能在主线程调用，不能用多线程的方法
        pictureObject.name = vIndex.ToString();
        pictureObject.transform.parent = GameObject.Find(CConstant.PICTURE_OBJECT_PARENT_NAME).transform;
        pictureObject.transform.position = m_InitialPicturePositions[vIndex];
        CUtility.adjustGameObjectToVisible(pictureObject);

        var imageInfo = m_ImageInfoSet[vIndex];
        var texture2D = new Texture2D(imageInfo.Width, imageInfo.Height);
        texture2D.LoadImage(imageInfo.RawData);
        imageInfo.RawData = null;
        var lod = CUtility.calculateTextureLOD(m_InitialPicturePositions[vIndex].magnitude);
        CUtility.applyTextureLOD(0, lod, "", ref texture2D);
        pictureObject.GetComponent<BoxCollider>().size = CUtility.calculateBoxColliderSize(new Vector2(texture2D.width, texture2D.height));

        CPicture picture = new CPicture();
        picture.pictureObject = pictureObject;
        picture.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        picture.initialPosition = m_InitialPicturePositions[vIndex];
        picture.lastPosition = picture.initialPosition;
        picture.index = vIndex;
        picture.textureFilePath = m_FilePathSet[vIndex];
        picture.lod = lod;
        picture.setSize(CConfig.initialPictureSize);

        CPictureManager.instance.pictureList.Add(picture);
        CPictureManager.instance.imageInfoDic.Add(pictureObject, imageInfo);
    }

    private void __createArtistList()
    {
        SImageInfo tempInfo = new SImageInfo();
        ArrayList artistList = CPictureManager.instance.artistList;
        var picExifInfoDic = CPictureManager.instance.imageInfoDic;

        foreach (GameObject obj in picExifInfoDic.Keys)
        {
            picExifInfoDic.TryGetValue(obj, out tempInfo);
            if (tempInfo.Artist.value != "")
            {
                string artist = tempInfo.Artist.value;
                if (artistList.Contains(artist))
                    continue;
                else
                    artistList.Add(artist);
            }
        }
    }

    private void __createLocationList()
    {
        SImageInfo tempInfo = new SImageInfo();
        ArrayList locationList = CPictureManager.instance.locationList;
        var picExifInfoDic = CPictureManager.instance.imageInfoDic;

        foreach (GameObject obj in picExifInfoDic.Keys)
        {
            picExifInfoDic.TryGetValue(obj, out tempInfo);
            string city = tempInfo.LocationInfo.city;
            if (tempInfo.LocationInfo.city != "")
            {
                if (locationList.Contains(city))
                    continue;
                else
                    locationList.Add(city);
            }
        }
    }

    private void __createUserCommentList()
    {
        SImageInfo tempInfo = new SImageInfo();
        ArrayList imgDescriptionList = CPictureManager.instance.imgDescriptionList;
        var picExifInfoDic = CPictureManager.instance.imageInfoDic;

        foreach (GameObject obj in picExifInfoDic.Keys)
        {
            picExifInfoDic.TryGetValue(obj, out tempInfo);
            string description = tempInfo.ImgDescription.value;
            if (description != "")
            {
                if (!imgDescriptionList.Contains(description))
                {
                    imgDescriptionList.Add(description);
                }
            }
        }
    }

    #endregion
}
