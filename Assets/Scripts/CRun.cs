using System.Diagnostics;
using UnityEngine;

public class CRun : MonoBehaviour
{
    private int m_FrameCounter = 0;

    void Awake()
    {
        CConfig.initConfig(Application.dataPath + CConstant.CONFIG_FILE_PATH);

        var sw = new Stopwatch();
        sw.Start();
        var pictureGenerator = gameObject.GetComponent<CGeneratePicture>();
        pictureGenerator.generatePictureObjectsFromDirectory(CConfig.pictureDirectoryPath);
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("total loading time: {0} ms", sw.ElapsedMilliseconds));
    }

    void Update()
    {
        CPictureManager.instance.update();

        if (null != Camera.main)
            cameraPosition = Camera.main.transform.position;

        m_FrameCounter++;
    }

    public Vector3 cameraPosition { get; set; }
}
