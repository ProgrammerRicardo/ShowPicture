using UnityEngine;
using HighlightingSystem;

public class CPicture
{
    #region private fields

    private int m_Index = -1;
    private bool m_IsSelected = false;
    private bool m_IsGrabbed = false;
    private bool m_IsMoving = false;
    private bool m_IsDisplayedAsRelatedPicture = false; //HACK: 命名需要修改
    private Highlighter m_HighLighter;

    #endregion

    #region public functions

    public GameObject pictureObject { get; set; }
    public Sprite sprite { get { return pictureObject.GetComponent<SpriteRenderer>().sprite; } set { pictureObject.GetComponent<SpriteRenderer>().sprite = value; } }
    public Vector3 initialPosition { get; set; }
    public Vector3 displayPosition { get; set; }
    public bool isSelected { get { return m_IsSelected; } set { m_IsSelected = value; } }
    public bool isGrabbed { get { return m_IsGrabbed; } set { m_IsGrabbed = value; } }
    public bool isMoving { get { return m_IsMoving; } set { m_IsMoving = value; } }
    public bool isDisplayedAsRelatedPicture { get { return m_IsDisplayedAsRelatedPicture; } set { m_IsDisplayedAsRelatedPicture = value; } }
    public int index { get { return m_Index; } set { m_Index = value; } }
    public int lod { get; set; }
    public string textureFilePath { get; set; }
    public Vector3 lastPosition { get; set; }

    public void updateMovingState()
    {
        var curPosition = pictureObject.transform.position;
        if ((lastPosition - curPosition).magnitude < 0.01) m_IsMoving = false;
        else m_IsMoving = true;
        lastPosition = curPosition;
    }

    public void adjustToFrontHighlighter()
    {
        m_HighLighter = pictureObject.GetComponent<Highlighter>();
        if (m_HighLighter == null)
        {
            m_HighLighter = pictureObject.AddComponent<Highlighter>();
        }
        m_HighLighter.FlashingOn(Color.blue, Color.cyan, 1f);
    }

    public void adjustToBackHighlighter()
    {
        m_HighLighter = pictureObject.GetComponent<Highlighter>();
        if (m_HighLighter != null)
        {
            m_HighLighter.Die();
        }
    }

    public void setSize(float vSize)
    {
        float size = Mathf.Max(sprite.rect.width / CConstant.PIXEL_PER_UNIT, sprite.rect.height / CConstant.PIXEL_PER_UNIT) * pictureObject.transform.localScale.x;
        float scale = vSize / size;
        pictureObject.transform.localScale *= scale;
    }

    #endregion
}