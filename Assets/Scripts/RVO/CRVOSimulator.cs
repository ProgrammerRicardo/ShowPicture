using System.Runtime.InteropServices;
using UnityEngine;

public class CRVOSimulator : MonoBehaviour
{
    private const string RVO_DLL = "RVO3D";

    #region event functions

    void Awake()
    {
        rvo_init();
    }

    void Update()
    {
        rvo_setTimeStep(Time.deltaTime);
        //设置每个被碰撞物体新的速度，RVO的核心
        rvo_doStep();
    }

    void OnDestroy()
    {
        rvo_destroy();
    }

    #endregion

    #region RVO APIs

    [DllImport(RVO_DLL)] public static extern void rvo_init();
    [DllImport(RVO_DLL)] public static extern void rvo_doStep();
    [DllImport(RVO_DLL)] public static extern void rvo_destroy();
    [DllImport(RVO_DLL)] public static extern long rvo_addAgent(Vector3 position);
    [DllImport(RVO_DLL)] public static extern long rvo_addAgentWithDetails(Vector3 position, float neighborDist, long maxNeighbors, float timeHorizon, float radius, float maxSpeed, Vector3 velocity);
    [DllImport(RVO_DLL)] public static extern void rvo_removeAgent(long agentNo);
    [DllImport(RVO_DLL)] public static extern void rvo_setTimeStep(float timeStep);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentDefaults(float neighborDist, long maxNeighbors, float timeHorizon, float radius, float maxSpeed);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentMaxNeighbors(long agentNo, long maxNeighbors);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentMaxSpeed(long agentNo, float maxSpeed);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentNeighborDist(long agentNo, float neighborDist);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentPosition(long agentNo, Vector3 position);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentPrefVelocity(long agentNo, Vector3 prefVelocity);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentRadius(long agentNo, float radius);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentTimeHorizon(long agentNo, float timeHorizon);
    [DllImport(RVO_DLL)] public static extern void rvo_setAgentVelocity(long agentNo, Vector3 velocity);
    [DllImport(RVO_DLL)] public static extern long rvo_getNumAgents();
    [DllImport(RVO_DLL)] public static extern long rvo_getAgentAgentNeighbor(long agentNo, long neighborNo);
    [DllImport(RVO_DLL)] public static extern long rvo_getAgentMaxNeighbors(long agentNo);
    [DllImport(RVO_DLL)] public static extern long rvo_getAgentNumAgentNeighbors(long agentNo);
    [DllImport(RVO_DLL)] public static extern long rvo_getAgentNumORCAPlanes(long agentNo);
    [DllImport(RVO_DLL)] public static extern float rvo_getAgentMaxSpeed(long agentNo);
    [DllImport(RVO_DLL)] public static extern float rvo_getAgentNeighborDist(long agentNo);
    [DllImport(RVO_DLL)] public static extern float rvo_getAgentRadius(long agentNo);
    [DllImport(RVO_DLL)] public static extern float rvo_getAgentTimeHorizon(long agentNo);
    [DllImport(RVO_DLL)] public static extern float rvo_getGlobalTime();
    [DllImport(RVO_DLL)] public static extern float rvo_getTimeStep();
    [DllImport(RVO_DLL)] public static extern Vector3 rvo_getAgentPosition(long agentNo);
    [DllImport(RVO_DLL)] public static extern Vector3 rvo_getAgentPrefVelocity(long agentNo);
    [DllImport(RVO_DLL)] public static extern Vector3 rvo_getAgentVelocity(long agentNo);

    #endregion
}