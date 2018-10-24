using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPictureManager
{
    #region private fields

    private static CPictureManager m_Instance = null;

    private CPicture m_SelectedPicture = null;
    private List<CPicture> m_PictureList = new List<CPicture>();
    private ArrayList m_ArtistList = new ArrayList();
    private ArrayList m_LocationList = new ArrayList();
    private ArrayList m_ImgDescriptionList = new ArrayList();
    private Dictionary<GameObject, SImageInfo> m_ImageInfoDic = new Dictionary<GameObject, SImageInfo>();
    private int m_FrameCounter = 0;

    #endregion

    #region public functions

    public static CPictureManager instance
    {
        get
        {
            if (null == m_Instance)
                m_Instance = new CPictureManager();
            return m_Instance;
        }
    }

    public List<CPicture> pictureList { get { return m_PictureList; } }

    public ArrayList artistList { get { return m_ArtistList; } }

    public ArrayList locationList { get { return m_LocationList; } }

    public ArrayList imgDescriptionList { get { return m_ImgDescriptionList; } }

    public Dictionary<GameObject, SImageInfo> imageInfoDic { get { return m_ImageInfoDic; } }

    public int getPictureNum(Dictionary<GameObject, SImageInfo> vDictionary, GameObject vGameObject)
    {
        int i = 0;
        foreach (GameObject obj in vDictionary.Keys)
        {
            if (vGameObject == obj)
                return i;
            ++i;
        }
        return -1;
    }

    public CPicture selectedPicture { get { return m_SelectedPicture; } }

    public void displayRelevantPictures(List<CPicture> vRelevantPictures)
    {
        if (vRelevantPictures.Count > CConfig.maxRelevantPictureNum)
            vRelevantPictures = vRelevantPictures.GetRange(0, CConfig.maxRelevantPictureNum);
        Debug.Log("maxRelevantPictureNum: " + CConfig.maxRelevantPictureNum);

        var viewDir = Camera.main.transform.forward;
        var frontDir = new Vector3(viewDir.x, 0, viewDir.z).normalized;
        var centerPos = Camera.main.transform.position;

        Vector3[] positions = CUtility.generateHemisphericalPositions(vRelevantPictures.Count, centerPos, frontDir, CConfig.relevantPictureDistanceToCenter, 10);
        for (int i = 0; i < vRelevantPictures.Count; ++i)
        {
            GameObject obj = vRelevantPictures[i].pictureObject;
            vRelevantPictures[i].displayPosition = positions[i];
            vRelevantPictures[i].isDisplayedAsRelatedPicture = true;
            obj.GetComponent<CNavAgent3D>().targetPosition = positions[i];
        }

        CHighlightRayEffect.createEffect(CUtility.pictureList2GameObjectList(vRelevantPictures));
    }

    public void onPictureSelected(int vIndex)
    {
        var motionManager = GameObject.Find(CConstant.GAME_MANAGER_OBJECT_NAME).GetComponent<CMotionManager>();
        var pic = m_PictureList[vIndex];

        if (pic.isSelected)
        {
            motionManager.movePictureBack(vIndex);
            m_SelectedPicture = null;
        }
        else
        {
            if (null != m_SelectedPicture) motionManager.movePictureBack(m_SelectedPicture.index);
            motionManager.movePictureFront(vIndex);
            m_SelectedPicture = pic;
        }
    }

    public void resetPicturePosition()
    {
        foreach (CPicture picture in m_PictureList)
        {
            if (picture.pictureObject.transform.position != picture.initialPosition)
            {
                picture.pictureObject.GetComponent<CNavAgent3D>().targetPosition = picture.initialPosition;
                picture.isDisplayedAsRelatedPicture = false;
                picture.isSelected = false;
            }
        }
    }

    public IEnumerator resetPictureRotation()
    {
        yield return 2; //NOTE: 延迟执行，保证在执行该方法之前，cameraPosition已被更新
        var pos = GameObject.Find(CConstant.GAME_MANAGER_OBJECT_NAME).GetComponent<CRun>().cameraPosition;
        foreach (var pic in m_PictureList)
            CUtility.adjustGameObjectToVisible(pic.pictureObject, pos);
    }

    public void update()
    {
        if (null == Camera.main) return;
        var cameraPos = Camera.main.transform.position;

        float nearstDistance = float.MaxValue;
        CPicture nearestPicture = null;

        foreach (var pic in m_PictureList)
        {
            var pos = pic.pictureObject.transform.position;
            var distance = (cameraPos - pos).magnitude;
            var originalLOD = pic.lod;
            var targetLOD = CUtility.calculateTextureLOD(distance);
            if (originalLOD != targetLOD && distance < nearstDistance)
            {
                nearstDistance = distance;
                nearestPicture = pic;
            }
        }

        __updatePictureSize(nearestPicture, nearstDistance);

        m_FrameCounter++;
    }

    public void setPictureLOD(CPicture vPicture, int vTargetLOD)
    {
        if (null == vPicture || vPicture.lod == vTargetLOD) return;
        var originalLOD = vPicture.lod;
        var texture = vPicture.pictureObject.GetComponent<SpriteRenderer>().sprite.texture;
        CUtility.applyTextureLOD(originalLOD, vTargetLOD, vPicture.textureFilePath, ref texture);
        vPicture.lod = vTargetLOD;
        vPicture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        vPicture.setSize(CConfig.initialPictureSize);
        vPicture.pictureObject.GetComponent<BoxCollider>().size = CUtility.calculateBoxColliderSize(new Vector2(texture.width, texture.height));
    }

    public void setPictureLOD(int vPictureIndex, int vTargetLOD)
    {
        if (vPictureIndex < 0 || vPictureIndex >= m_PictureList.Count) return;
        setPictureLOD(m_PictureList[vPictureIndex], vTargetLOD);
    }

    private void __updatePictureSize(CPicture vPicture, float vDistance)
    {
        if (null == vPicture) return;
        var targetLOD = CUtility.calculateTextureLOD(vDistance);
        setPictureLOD(vPicture, targetLOD);
    }

    #endregion
}