using UnityEngine;

public class CRealtimeReflection : MonoBehaviour
{
    private ReflectionProbe m_ReflectionProbe;
    private int m_Counter = 0;
    private const int UPDATE_INTERVAL = 5;

    void Awake()
    {
        m_ReflectionProbe = GetComponent<ReflectionProbe>();
    }

    void Update()
    {
        if ((m_Counter++) % UPDATE_INTERVAL != 0) return;

        float floorPosY = GameObject.Find(CConstant.FLOOR_OBJECT_NAME).transform.position.y;

        if (null == Camera.main) return;
        m_ReflectionProbe.transform.position = new Vector3(
            Camera.main.transform.position.x,
            2 * floorPosY - Camera.main.transform.position.y,
            Camera.main.transform.position.z
        );

        m_ReflectionProbe.RenderProbe();
    }
}