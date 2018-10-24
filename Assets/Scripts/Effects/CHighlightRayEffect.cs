using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HighlightingSystem;

public class CHighlightRayEffect : MonoBehaviour
{
    #region private fileds

    private GameObject m_RayPrefeb = null;
    private List<GameObject> m_TargetObjects = new List<GameObject>();
    private List<GameObject> m_Rays = new List<GameObject>();
    private bool m_IsRayHitTarget = false;

    private const float RAY_CAST_SPEED = 1.0f;
    private const float RAY_HIT_MIN_DISTANCE = 1.0f;
    private const float RAY_START_WIDTH = 0.001f;
    private const float RAY_END_WIDTH = 0.01f;
    private const float DEFAULT_SURVIVAL_TIME = 6.0f;
    private const string EFFECT_OBJECT_NAME = "RayEffect";

    #endregion

    #region event functions

    void Start()
    {
        __createRays();
        __createHighlights();
    }

    void Update()
    {
        __updateRays();
    }

    void OnDestroy()
    {
        __destroyRays();
        __destroyHighlights();
    }

    #endregion

    #region public functions

    public static void createEffect(List<GameObject> vTargetObjects, float vSurvivalTime = DEFAULT_SURVIVAL_TIME)
    {
        var existedObj = GameObject.Find(EFFECT_OBJECT_NAME);
        if (null != existedObj) Destroy(existedObj);

        var obj = new GameObject(EFFECT_OBJECT_NAME);
        var highlightRayEffect = obj.AddComponent<CHighlightRayEffect>();

        highlightRayEffect.init(vTargetObjects);
        Destroy(obj, vSurvivalTime);
    }

    public void init(List<GameObject> vTargetObjects)
    {
        m_RayPrefeb = (GameObject)Resources.Load(CConstant.RAY_PREFAB_FILE_PATH);
        m_TargetObjects = vTargetObjects;
    }

    #endregion

    #region private functions

    private void __createRays()
    {
        var controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
        var rayOrigin = (null != controllerLeftHand) ? controllerLeftHand.transform.position : Vector3.zero;

        for (int i = 0; i < m_TargetObjects.Count; ++i)
        {
            var ray = Instantiate(m_RayPrefeb) as GameObject;
            Vector3[] positions = { rayOrigin, rayOrigin };
            ray.GetComponent<LineRenderer>().SetPositions(positions);
            ray.transform.parent = this.gameObject.transform;
            m_Rays.Add(ray);
        }
    }

    private void __createHighlights()
    {
        foreach (var tgtObj in m_TargetObjects)
        {
            var highLighter = tgtObj.GetComponent<Highlighter>();
            if (null == highLighter) { highLighter = tgtObj.AddComponent<Highlighter>(); }
            highLighter.FlashingOn(Color.blue, Color.green, 1f);
        }
    }

    private void __destroyRays()
    {
        foreach (var ray in m_Rays) Destroy(ray);
    }

    private void __destroyHighlights()
    {
        foreach (var obj in m_TargetObjects)
        {
            var highLighter = obj.GetComponent<Highlighter>();
            if (null != highLighter) highLighter.Die();
        }
    }

    private void __updateRays()
    {
        if (m_TargetObjects.Count != m_Rays.Count) return;

        for (int i = 0; i < m_TargetObjects.Count; ++i)
        {
            var controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            var rayOrigin = (null != controllerLeftHand) ? controllerLeftHand.transform.position : Vector3.zero;

            var lineRenderer = m_Rays[i].GetComponent<LineRenderer>();

            var curPos = lineRenderer.GetPosition(1);
            var dstPos = m_TargetObjects[i].transform.position;
            var distance = (dstPos - curPos).magnitude;
            m_IsRayHitTarget = (distance < RAY_HIT_MIN_DISTANCE);

            var rayTarget = dstPos;
            if (!m_IsRayHitTarget)
            {
                var rayDir = (dstPos - curPos).normalized;
                rayTarget = curPos + rayDir * RAY_CAST_SPEED;
            }

            Vector3[] positions = { rayOrigin, rayTarget };
            lineRenderer.SetPositions(positions);
            lineRenderer.startWidth = RAY_START_WIDTH;
            lineRenderer.endWidth = RAY_END_WIDTH;
        }
    }

    #endregion
}
