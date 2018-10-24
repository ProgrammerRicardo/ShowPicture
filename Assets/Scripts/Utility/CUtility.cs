using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CUtility
{
    public static JObject convertToJObj(string vSourceString)
    {
        return (JObject)JsonConvert.DeserializeObject(vSourceString);
    }

    public static string normalizePath(string vPath)
    {
        string normalizedPath = vPath.Replace("\\", CConstant.FILE_SEPARATOR);

        if (!normalizedPath.EndsWith(CConstant.FILE_SEPARATOR))
            normalizedPath += CConstant.FILE_SEPARATOR;

        return normalizedPath;
    }

    public static ArrayList gameObjectList2ArrayList(List<GameObject> vList)
    {
        ArrayList arrayList = new ArrayList();
        arrayList.AddRange(vList);
        return arrayList;
    }

    public static List<GameObject> pictureList2GameObjectList(List<CPicture> vPictureList)
    {
        var gameObjectList = new List<GameObject>();
        foreach (var pic in vPictureList) gameObjectList.Add(pic.pictureObject);
        return gameObjectList;
    }

    public static Vector3 adjustToTargetPosition(Vector3 vOriginPosition, float vNormalizedDistanceTimes, bool isFurther)
    {
        Vector3 adjustPosition = vOriginPosition.normalized * vNormalizedDistanceTimes;
        Vector3 targetPosition = new Vector3(0, 0, 0);
        if (isFurther)
        {
            targetPosition = vOriginPosition + adjustPosition;
        }
        else
        {
            targetPosition = vOriginPosition - adjustPosition;
        }

        return targetPosition;
    }

    public static void adjustGameObjectToVisible(GameObject vGameObject, Vector3 vPos = default(Vector3))
    {
        vGameObject.transform.LookAt(vPos);
        vGameObject.transform.Rotate(0, 180, 0);
    }

    public static Vector3[] generateRandomSquarePositions(int vPositionCount, Vector3 vCenterPos = default(Vector3), float vCellSize = 15.0f)
    {
        var positions = new Vector3[vPositionCount];
        int len = Mathf.CeilToInt(Mathf.Sqrt(vPositionCount));
        int count = 0;
        for (int i = -len / 2; i <= len / 2; ++i)
        {
            for (int k = -len / 2; k <= len / 2; ++k)
            {
                Vector3 pos = new Vector3(i * vCellSize, 0.0f, k * vCellSize);
                pos.x += Random.Range(-vCellSize / 4, vCellSize / 4);
                pos.z += Random.Range(-vCellSize / 4, vCellSize / 4);
                pos += vCenterPos;

                if (count >= vPositionCount) break;
                positions[count++] = pos;
            }
        }

        return positions;
    }

    public static float calculateDeltaPhi(float vRadius, float vMaxUnitSize)
    {
        int maxUnitNum = Mathf.FloorToInt(Mathf.PI / Mathf.Asin(vMaxUnitSize / (2.0f * vRadius)));
        return 2.0f * Mathf.PI / maxUnitNum;
    }

    public static Vector3[] generateRandomCirclePositions(int vPositionCount, Vector3 vCenterPos = default(Vector3), float vCellSize = 15.0f)
    {
        var positions = new Vector3[vPositionCount];
        float phi = 0.0f, r = vCellSize;
        float deltaPhi = calculateDeltaPhi(r, vCellSize), deltaR = vCellSize;

        for (int i = 0; i < vPositionCount; ++i)
        {
            positions[i] = vCenterPos + new Vector3(r * Mathf.Cos(phi), 0, r * Mathf.Sin(phi));
            positions[i].x += Random.Range(-vCellSize / 4, vCellSize / 4);
            positions[i].z += Random.Range(-vCellSize / 4, vCellSize / 4);

            phi += deltaPhi;
            if (phi > (2.0f * Mathf.PI - 0.5f * deltaPhi))
            {
                phi = 0.0f;
                r += deltaR;
                deltaPhi = calculateDeltaPhi(r, vCellSize);
            }
        }

        return positions;
    }

    //TODO: 1. 处理输入参数不合法的情况  2. 根据vFrontDir自动调整图片位置
    public static Vector3[] generateHemisphericalPositions(int vPositionCount, Vector3 vCenterPos, Vector3 vFrontDir, float vSphereRadius, int vMaxNumPerLevel)
    {
        Vector3[] positions = new Vector3[vPositionCount];
        float maxUnitSize = 2.0f * vSphereRadius * Mathf.Sin(Mathf.PI / vMaxNumPerLevel);

        float phi = 0.0f, theta = 0.5f * Mathf.PI, r = vSphereRadius;
        float deltaPhi = 2.0f * Mathf.PI / vMaxNumPerLevel, deltaR = maxUnitSize;

        for (int i = 0; i < vPositionCount; ++i)
        {
            positions[i].x = r * Mathf.Sin(theta) * Mathf.Cos(phi);
            positions[i].z = r * Mathf.Sin(theta) * Mathf.Sin(phi);
            positions[i].y = r * Mathf.Cos(theta);
            positions[i] += vCenterPos;

            phi += deltaPhi;
            if (phi > (2.0f * Mathf.PI - 0.5f * deltaPhi))
            {
                phi = 0.0f;
                theta -= deltaPhi;
                deltaPhi = calculateDeltaPhi(r * Mathf.Sin(theta), maxUnitSize);
            }
            if (theta <= 0.1f * Mathf.PI)
            {
                theta = 0.5f * Mathf.PI;
                phi = 0.0f;
                r += deltaR;
                deltaPhi = calculateDeltaPhi(r * Mathf.Sin(theta), maxUnitSize);
            }
        }

        return positions;
    }

    public static void applyTextureLOD(int vOriginalLOD, int vTargetLOD, string vFilePath, ref Texture2D vioTexture)
    {
        if (vOriginalLOD == vTargetLOD) return;

        float scale = 1.0f;
        if (vOriginalLOD < vTargetLOD)
        {
            scale = Mathf.Pow(0.5f, vTargetLOD - vOriginalLOD);
        }
        else if (vOriginalLOD > vTargetLOD)
        {
            vioTexture = createTextureFromFile(vFilePath);
            scale = Mathf.Pow(0.5f, vTargetLOD);
        }
        CTextureScale.Bilinear(vioTexture, (int)(vioTexture.width * scale), (int)(vioTexture.height * scale));
    }

    public static int calculateTextureLOD(float vDistance) //TODO: 使用算法自动计算LOD
    {
        if (vDistance < 20) return 0;
        else if (vDistance < 50) return 1;
        else if (vDistance < 100) return 2;
        else if (vDistance < 200) return 3;
        else if (vDistance < 500) return 4;
        else if (vDistance < 800) return 5;
        else return 6;
    }

    public static Texture2D createTextureFromFile(string vFilePath)
    {
        string fileUrl = "file://" + vFilePath;
        Texture2D texture2D = new WWW(fileUrl).texture;
        return texture2D;
    }

    public static Vector3 calculateBoxColliderSize(Vector2 vTextureSize)
    {
        Vector3 boxColliderSize = new Vector3();
        boxColliderSize.x = vTextureSize.x / CConstant.PIXEL_PER_UNIT;
        boxColliderSize.y = vTextureSize.y / CConstant.PIXEL_PER_UNIT;
        boxColliderSize.z = 1.0f;
        return boxColliderSize;
    }
}
