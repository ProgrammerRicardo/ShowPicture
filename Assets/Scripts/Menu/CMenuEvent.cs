using UnityEngine;
using VRTK;

public class CMenuEvent : MonoBehaviour
{
    #region public fields

    [Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
    public GameObject leftController;
    [Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
    public GameObject rightController;

    #endregion

    #region private fields

    private GameObject m_DisplayInfoCanvas;

    #endregion

    #region event functions

    void Start()
    {
        m_DisplayInfoCanvas = GameObject.Find(CConstant.DISPLAY_INFO_CANVAS_OBJECT_NAME);
        m_DisplayInfoCanvas.SetActive(false);
        __bindControllerEvents();
    }

    void Update()
    {
        __updatePictureInfoCanvasTransform();
    }

    #endregion

    #region public functions

    public void onRadialMenuLeftClicked()
    {
        //test display relevant pictures
        var pictureList = CPictureManager.instance.pictureList;
        CPictureManager.instance.displayRelevantPictures(pictureList);
    }

    public void onRadialMenuRightClicked()
    {
        m_DisplayInfoCanvas.SetActive(!m_DisplayInfoCanvas.activeInHierarchy);
    }

    public void onRadialMenuUpClicked()
    {

    }

    public void onRadialMenuDownClicked()
    {
        gameObject.GetComponent<CSkyboxManager>().changeSkybox();
    }

    #endregion

    #region private functions

    private void __bindControllerEvents()
    {
        var leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents>();
        var rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents>();
        var rightPointer = rightController.GetComponent<VRTK_Pointer>();

        //NOTE: Trigger-扳机键  ButtonTwo-菜单键  Touchpad-触摸板  Grip-握持键
        leftControllerEvents.GripPressed += new ControllerInteractionEventHandler(__onLeftGripPressed);
        leftControllerEvents.ButtonTwoPressed += new ControllerInteractionEventHandler(__onLeftMenuPressed);
        leftControllerEvents.ButtonTwoReleased += new ControllerInteractionEventHandler(__onLeftMenuReleased);
        leftControllerEvents.TriggerReleased += new ControllerInteractionEventHandler(__onLeftTriggerReleased);
        rightPointer.PointerStateValid += new DestinationMarkerEventHandler(__onRightPointerStateValid);
    }

    private void __onRightPointerStateValid(object vSender, DestinationMarkerEventArgs vEvents)
    {
        if (vEvents.target.gameObject.layer != LayerMask.NameToLayer(CConstant.PICTURE_LAYER_NAME)) return;

        string targetName = vEvents.target.name;
        int index = int.Parse(targetName);
        m_DisplayInfoCanvas.GetComponent<CDisplayPictureInfo>().setDisplayInfo(index);
    }

    private void __onLeftMenuPressed(object vSender, ControllerInteractionEventArgs vEvents)
    {
        gameObject.GetComponent<CAutoSpeechRecognize>().OnRecordingStart();
    }

    private void __onLeftTriggerReleased(object vSender, ControllerInteractionEventArgs vEvents)
    {
        StartCoroutine(CPictureManager.instance.resetPictureRotation());
    }

    private void __onLeftMenuReleased(object vSender, ControllerInteractionEventArgs vEvents)
    {
        gameObject.GetComponent<CAutoSpeechRecognize>().OnRecordingStop();
    }

    private void __onLeftGripPressed(object vSender, ControllerInteractionEventArgs vEvents)
    {
        CPictureManager.instance.resetPicturePosition();
    }

    private void __updatePictureInfoCanvasTransform()
    {
        var controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();

        if (controllerLeftHand != null && m_DisplayInfoCanvas.transform.parent != controllerLeftHand.transform)
            m_DisplayInfoCanvas.transform.SetParent(controllerLeftHand.transform);

        if (Camera.main != null)
        {
            m_DisplayInfoCanvas.transform.LookAt(Camera.main.transform);
            m_DisplayInfoCanvas.transform.Rotate(Vector3.up, 180.0f);
        }
    }

    #endregion
}