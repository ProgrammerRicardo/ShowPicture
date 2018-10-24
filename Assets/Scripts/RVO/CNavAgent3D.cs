using UnityEngine;

public class CNavAgent3D : MonoBehaviour
{
    [SerializeField] private long m_AgentId;
    [SerializeField] private Vector3 m_Velocity;
    [SerializeField] private Vector3 m_PreferredVelocity;

    private int m_MaxNeighbors = 10;
    private float m_NeighborDist = 20f;
    private float m_TimeHorizon = 5f;
    private float m_MaxSpeed = 60f;
    private float m_Radius = 1f;

    void Start()
    {
        targetPosition = transform.position;
        m_AgentId = CRVOSimulator.rvo_addAgentWithDetails(transform.position, m_NeighborDist, m_MaxNeighbors, m_TimeHorizon, m_Radius, m_MaxSpeed, m_Velocity);
    }

    void Update()
    {
        m_Velocity = CRVOSimulator.rvo_getAgentVelocity(m_AgentId);
        transform.position += m_Velocity * Time.deltaTime;

        __updatePreferredVelocity();
        __updateAgentRadius();

        CRVOSimulator.rvo_setAgentRadius(m_AgentId, m_Radius);
        CRVOSimulator.rvo_setAgentPrefVelocity(m_AgentId, m_PreferredVelocity);
        CRVOSimulator.rvo_setAgentPosition(m_AgentId, transform.position);
    }

    public Vector3 targetPosition { get; set; }

    private void __updatePreferredVelocity()
    {
        var distance = (targetPosition - transform.position).magnitude;

        if (distance > 0.1f * m_Radius)
        {
            var direction = (targetPosition - transform.position).normalized;
            var speed = Mathf.Min(0.1f * distance / Time.deltaTime, m_MaxSpeed);
            m_PreferredVelocity = direction * speed;
        }
        else
        {
            m_PreferredVelocity = Vector3.zero;
        }
    }

    private void __updateAgentRadius()
    {
        var sprite = CPictureManager.instance.pictureList[(int)m_AgentId].sprite;
        var spriteSize = sprite.rect.size;
        var localScale = transform.localScale;
        var size = new Vector2(spriteSize.x * localScale.x, spriteSize.y * localScale.y);
        m_Radius = 0.5f * size.magnitude / sprite.pixelsPerUnit;
    }
}