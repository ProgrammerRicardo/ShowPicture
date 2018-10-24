using UnityEngine;
using Wit.BaiduAip.Speech;

public class CAutoSpeechRecognize : MonoBehaviour
{

    public string APIKey = "KW1UGmcFLDeFniZW5XpU9LTR";
    public string SecretKey = "hQt6ImPlwFtRf2OdNnGK9kjGUMdcWZSR";

    private AudioClip _clipRecord;
    private Asr _asr;

    // Microphone is not supported in Webgl
#if !UNITY_WEBGL

    void Start()
    {
        _asr = new Asr(APIKey, SecretKey);
        StartCoroutine(_asr.GetAccessToken());
    }

    public void OnRecordingStart()
    {
        Debug.Log("Listening...");
        _clipRecord = Microphone.Start(null, false, 30, 16000);
    }

    public void OnRecordingStop()
    {
        Debug.Log("Recognizing...");
        Microphone.End(null);
        Debug.Log("end record");
        var data = Asr.ConvertAudioClipToPCM16(_clipRecord);
        StartCoroutine(_asr.Recognize(data, s =>
        {
            if (s.result != null && s.result.Length > 0)
            {
                Debug.Log(s.result[0]);
                doAsrInstrutionResponse(s.result[0]);
            }
            else
            {
                Debug.Log("未识别到声音");
            }
        }));
    }

    public void doAsrInstrutionResponse(string AsrResult)
    {
        if (AsrResult.Contains("还原") || AsrResult.Contains("复位"))
        {
            CPictureManager.instance.resetPicturePosition();
        }
        else if (AsrResult.Contains("放大"))
        {
            Debug.Log("放大成功");
        }
        else if (AsrResult.Contains("查找") || AsrResult.Contains("展示"))
        {
            string[,] PicinfoArray = new string[20, 2] { { "狗", "dog" }, { "猫", "cat" }, { "瓶", "bottle" }, { "羊", "sheep" }, { "狗", "dog" }, { "鸟", "bird" },
                { "自行车", "bicycle" }, { "人", "person" }, { "飞机", "aeroplane" }, { "电视", "tvmonitor" }, { "火车", "train" }, { "船", "boat" }, { "摩托", "motorbike" },
                { "马", "horse" }, { "牛", "cow" },{ "沙发", "sofa" }, { "公交车", "bus" }, { "椅子", "chair" }, { "餐桌", "diningtable" }, { "狗", "dog" },  };

            for (int j = 0; j < PicinfoArray.GetLength(0); j++)
            {
                if (AsrResult.Contains(PicinfoArray[j, 0]))
                {
                    var picInfo = PicinfoArray[j, 1];
                    CPictureManager.instance.resetPicturePosition();
                    var relatedPictureSet = CSelectRelevantPic.getSeacrchResultSetByImgDescription(CPictureManager.instance.imageInfoDic, picInfo);
                    CPictureManager.instance.displayRelevantPictures(relatedPictureSet);
                    Debug.Log("relatedPictureSet:" + relatedPictureSet.Count);

                }
            }
        }
    }
#endif
}
