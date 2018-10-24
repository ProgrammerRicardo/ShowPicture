using UnityEngine;

public class CMotionManager : MonoBehaviour
{
    #region private fields

    private CPictureManager m_PictureManager = null;

    #endregion

    #region event functions

    void Start()
    {
        m_PictureManager = CPictureManager.instance;
    }

    #endregion

    #region public functions

    public void movePictureBack(int vPicIndex)
    {
        CPicture pic = m_PictureManager.pictureList[vPicIndex];
        pic.isSelected = false;

        var agent = pic.pictureObject.GetComponent<CNavAgent3D>();
        if (pic.isDisplayedAsRelatedPicture)
        {
            agent.targetPosition = pic.displayPosition;
        }
        else
        {
            agent.targetPosition = pic.initialPosition;
        }

        pic.adjustToBackHighlighter();
    }

    public void movePictureFront(int vPicIndex)
    {
        CPicture pic = m_PictureManager.pictureList[vPicIndex];
        pic.isSelected = true;

        var viewDir = Camera.main.transform.forward;
        var direction = new Vector3(viewDir.x, 0, viewDir.z).normalized;
        var targetPosition = Camera.main.transform.position + CConfig.frontPictureDistanceToCenter * direction;
        CUtility.adjustGameObjectToVisible(pic.pictureObject, Camera.main.transform.position);
        pic.pictureObject.GetComponent<CNavAgent3D>().targetPosition = targetPosition;
        pic.adjustToFrontHighlighter();
    }

    #endregion
}