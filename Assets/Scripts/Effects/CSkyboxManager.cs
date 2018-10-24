using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkyboxManager : MonoBehaviour
{
    public Material[] skyboxMaterials;

    private int m_MaterialIndex = 0;
    private List<Color> m_InitialMaterialColor = new List<Color>();
    private float m_FadeStep = 0.0f;
    private bool m_IsChangeAllowed = true;

    private const string COLOR_PROPERTY_NAME = "_Tint";

    void Start()
    {
        foreach (Material mat in skyboxMaterials)
            m_InitialMaterialColor.Add(mat.GetColor(COLOR_PROPERTY_NAME));
    }

    void Update()
    {
        m_FadeStep = Mathf.Clamp(Time.deltaTime, 0.01f, 0.1f);
    }

    void OnDestroy()
    {
        for (int i = 0; i < skyboxMaterials.Length; ++i)
            skyboxMaterials[i].SetColor(COLOR_PROPERTY_NAME, m_InitialMaterialColor[i]);
    }

    public void changeSkybox()
    {
        if (!m_IsChangeAllowed) return;

        var tempIndex = m_MaterialIndex++;
        if (m_MaterialIndex >= skyboxMaterials.Length) m_MaterialIndex = 0;
        StartCoroutine(__changeSkyboxMaterial(skyboxMaterials[tempIndex], skyboxMaterials[m_MaterialIndex]));
    }

    private IEnumerator __changeSkyboxMaterial(Material vCurrentMat, Material vNextMat)
    {
        m_IsChangeAllowed = false;
        yield return StartCoroutine(__fadeOut(vCurrentMat));
        RenderSettings.skybox = vNextMat;
        yield return StartCoroutine(__fadeIn(vNextMat));
        m_IsChangeAllowed = true;
    }

    private IEnumerator __fadeOut(Material vMat)
    {
        vMat.SetColor(COLOR_PROPERTY_NAME, new Color(1.0f, 1.0f, 1.0f, 1.0f));
        while (true)
        {
            var color = vMat.GetColor(COLOR_PROPERTY_NAME);
            color.a -= m_FadeStep;
            color.r = color.g = color.b = color.a;
            vMat.SetColor(COLOR_PROPERTY_NAME, color);

            if (color.a < 0.0f) break;
            yield return 0;
        }
    }

    private IEnumerator __fadeIn(Material vMat)
    {
        vMat.SetColor(COLOR_PROPERTY_NAME, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        while (true)
        {
            var color = vMat.GetColor(COLOR_PROPERTY_NAME);
            color.a += m_FadeStep;
            color.r = color.g = color.b = color.a;
            vMat.SetColor(COLOR_PROPERTY_NAME, color);

            if (color.a > 1.0f) break;
            yield return 0;
        }
    }
}