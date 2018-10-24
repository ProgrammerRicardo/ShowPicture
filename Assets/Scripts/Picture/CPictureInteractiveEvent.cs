using UnityEngine;
using VRTK;

public class CPictureInteractiveEvent : MonoBehaviour
{
    private CPicture m_Picture = null;
    private CRun m_CRunComp = null; //HACK: 命名
    private int m_FrameCounter = 0;

    void Start()
    {
        int index = int.Parse(gameObject.name);
        m_Picture = CPictureManager.instance.pictureList[index];
        m_CRunComp = GameObject.Find(CConstant.GAME_MANAGER_OBJECT_NAME).GetComponent<CRun>();
    }

    void Update()
    {
        if (m_Picture.isGrabbed)
        {
            var pictureObject = m_Picture.pictureObject;
            var agent = pictureObject.GetComponent<CNavAgent3D>();
            agent.targetPosition = pictureObject.transform.position;
        }
        else
        {
            const int updateInterval = 5;
            if (m_FrameCounter % updateInterval == 0)
            {
                if (m_Picture.isMoving) CUtility.adjustGameObjectToVisible(gameObject, m_CRunComp.cameraPosition);
                m_Picture.updateMovingState();
            }
        }

        m_FrameCounter++;
    }

    public void onGrab(object vSender, InteractableObjectEventArgs vEvents) //NOTE: OnGrab触发时VRTK会自动创建rigidbody组件
    {
        m_Picture.isGrabbed = true;
    }

    public void onUngrab(object vSender, InteractableObjectEventArgs vEvents)
    {
        m_Picture.isGrabbed = false;
        var rigidbody = GetComponent<Rigidbody>(); //NOTE: rigidbody开销很大，不需要时要销毁
        if (null != rigidbody) Destroy(rigidbody);
    }

    public void onUse(object vSender, InteractableObjectEventArgs vEvents)
    {
        string targetName = gameObject.name;
        int index = int.Parse(targetName);
        CPictureManager.instance.onPictureSelected(index);
    }
}
