using System.Collections.Generic;
using System.Collections;
using Unity.Sentis;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Lays = Unity.Sentis.Layers;

/*
 *  YOLOv8n AR Inference Script (Snapdragon Spaces AR and Unity Sentis 1.3)
 *  =======================================================================
 *
 *  Place this script on the Main Camera.
 *
 *  - Assign your YOLOv8 model (.sentis file) to `modelAsset`.
 *  - Set up ARCameraManager and assign it to `_cameraManager`.
 *  - Create a RawImage UI element for preview and assign it to `displayImage`.
 *  - Assign classes.txt to `labelsAsset`, a bounding box sprite, and a font for labels.
 */

public class RunYOLO8n : MonoBehaviour
{
    [Header("Model and Labels")]
    public ModelAsset modelAsset;
    public TextAsset labelsAsset;

    [Header("AR Components")]
    [SerializeField] private ARCameraManager _cameraManager;

    [Header("UI Components")]
    public RawImage displayImage;
    public Sprite boxTexture;
    public Font font;
    public TextMeshProUGUI objectLabel;

    [Header("Inference Settings")]
    [SerializeField, Range(0, 1)] float iouThreshold = 0.5f;
    [SerializeField, Range(0, 1)] float scoreThreshold = 0.5f;

    const BackendType backend = BackendType.GPUCompute;

    Model model;
    IWorker engine;
    Ops ops;

    string[] labels;
    Texture2D cameraTexture;

    const int imageWidth = 640;
    const int imageHeight = 640;
    const int numClasses = 80;
    const int maxOutputBoxes = 64;

    public Button runMLButton;
    private Text buttonText;
    public APIManager apimanager;
    private List<string> last3Labels = new List<string>();


    List<GameObject> boxPool = new List<GameObject>();

    // You can define this as a member variable or inside the function.
    List<string> allowedItems = new List<string> {
    "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", 
    "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "backpack", 
    "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", 
    "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", 
    "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", 
    "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "potted plant", "bed", "dining table", "toilet", 
    "tv monitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", 
    "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush"
    };


    void Start()
    {
        Application.targetFrameRate = 60;
        //StartCoroutine(RunInferencePeriodically(1f));
        ops = WorkerFactory.CreateOps(backend, null);

        labels = labelsAsset.text.Split('\n');
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = labels[i].Trim();
        }


        model = ModelLoader.Load(modelAsset);
        ModifyModel();

        engine = WorkerFactory.CreateWorker(backend, model);

        if (_cameraManager != null)
        {
            _cameraManager.frameReceived += OnFrameReceived;
        }
        Transform buttonTextTransform = runMLButton.gameObject.transform.GetChild(0);
        buttonText = buttonTextTransform.GetComponent<Text>();


    }
    //IEnumerator RunInferencePeriodically(float interval)
    //{
    //    while (true)
    //    {
    //        if (cameraTexture != null) // Ensure texture exists
    //            ExecuteML(cameraTexture);
    //        yield return new WaitForSeconds(interval);
    //    }
    //}
    Coroutine inferenceCoroutine;

    public void runML()
    {
        if (inferenceCoroutine != null)
        {
            StopCoroutine(inferenceCoroutine);
            inferenceCoroutine = null;
        }
        else
        {
            inferenceCoroutine = StartCoroutine(RunInferencePeriodically(2f));
        }

        UpdateDetectButtonUI();
        StartCoroutine(EnableButtonAfterDelay(3f));
    }

    void UpdateDetectButtonUI()
    {
        if (inferenceCoroutine != null)
        {
            buttonText.text = "Stop";
        }
        else
        {
            buttonText.text = "Detect";
        }

        runMLButton.enabled = false;
    }

    IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        runMLButton.enabled = true;
    }


    IEnumerator RunInferencePeriodically(float interval)
    {
        while (true)
        {
            if (cameraTexture != null)
            {
                ExecuteML(cameraTexture);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    void ModifyModel()
    {
        model.AddConstant(new Lays.Constant("0", new int[] { 0 }));
        model.AddConstant(new Lays.Constant("1", new int[] { 1 }));
        model.AddConstant(new Lays.Constant("4", new int[] { 4 }));
        model.AddConstant(new Lays.Constant("classes_plus_4", new int[] { numClasses + 4 }));
        model.AddConstant(new Lays.Constant("maxOutputBoxes", new int[] { maxOutputBoxes }));
        model.AddConstant(new Lays.Constant("iouThreshold", new float[] { iouThreshold }));
        model.AddConstant(new Lays.Constant("scoreThreshold", new float[] { scoreThreshold }));

        model.AddLayer(new Lays.Slice("boxCoords0", "output0", "0", "4", "1"));
        model.AddLayer(new Lays.Transpose("boxCoords", "boxCoords0", new int[] { 0, 2, 1 }));
        model.AddLayer(new Lays.Slice("scores0", "output0", "4", "classes_plus_4", "1"));
        model.AddLayer(new Lays.ReduceMax("scores", new[] { "scores0", "1" }));
        model.AddLayer(new Lays.ArgMax("classIDs", "scores0", 1));

        model.AddLayer(new Lays.NonMaxSuppression("NMS", "boxCoords", "scores",
            "maxOutputBoxes", "iouThreshold", "scoreThreshold",
            centerPointBox: Lays.CenterPointBox.Center));

        model.outputs.Clear();
        model.AddOutput("boxCoords");
        model.AddOutput("classIDs");
        model.AddOutput("NMS");
        model.AddOutput("scores");
    }

    void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage)) return;

        var format = TextureFormat.RGBA32;
        cameraTexture ??= new Texture2D(cpuImage.width, cpuImage.height, format, false);

        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, format);
        var rawTextureData = cameraTexture.GetRawTextureData<byte>();
        cpuImage.Convert(conversionParams, rawTextureData);
        cpuImage.Dispose();

        cameraTexture.Apply();
        displayImage.texture = cameraTexture;

        //ExecuteML(cameraTexture);
    }
    void ExecuteML(Texture sourceTexture)
    {
        ClearAnnotations();

        using var input = TextureConverter.ToTensor(sourceTexture, imageWidth, imageHeight, 3);
        engine.Execute(input);

        var boxCoords = engine.PeekOutput("boxCoords") as TensorFloat;
        var NMS = engine.PeekOutput("NMS") as TensorInt;
        var classIDs = engine.PeekOutput("classIDs") as TensorInt;
        var scores = engine.PeekOutput("scores") as TensorFloat;

        using var boxIDs = ops.Slice(NMS, new int[] { 2 }, new int[] { 3 }, new int[] { 1 }, new int[] { 1 });
        using var boxIDsFlat = boxIDs.ShallowReshape(new TensorShape(boxIDs.shape.length)) as TensorInt;
        using var output = ops.Gather(boxCoords, boxIDsFlat, 1);
        using var labelIDs = ops.Gather(classIDs, boxIDsFlat, 2);
        using var gatheredScores = ops.Gather(scores, boxIDsFlat, 1);

        output.MakeReadable();
        labelIDs.MakeReadable();
        gatheredScores.MakeReadable();

        float scaleX = displayImage.rectTransform.rect.width / imageWidth;
        float scaleY = displayImage.rectTransform.rect.height / imageHeight;

        string firstDetectedLabelThisFrame = null;

        for (int n = 0; n < output.shape[1]; n++)
        {
            float currentConfidence = gatheredScores[0, n];
            string currentLabel = labels[labelIDs[0, 0, n]];

            DrawBox(
                new Vector2(output[0, n, 0], output[0, n, 1]),
                new Vector2(output[0, n, 2], output[0, n, 3]),
                currentLabel,
                n,
                scaleX,
                scaleY
            );

            if (firstDetectedLabelThisFrame == null && allowedItems.Contains(currentLabel, StringComparer.OrdinalIgnoreCase))
            {
                firstDetectedLabelThisFrame = currentLabel;
            }
        }

        // Only update last3Labels once per ExecuteML call
        if (firstDetectedLabelThisFrame != null)
        {
            UpdateLabel(firstDetectedLabelThisFrame);
        }

        StartCoroutine(DeleteBoundingBoxes());
        CheckConsecutiveLabels();
    }

    IEnumerator DeleteBoundingBoxes()
    {
        yield return new WaitForSeconds(2f);
        ClearAnnotations();
    }
    void CheckConsecutiveLabels()
    {
        if (last3Labels.Count < 3)
            return;

        if (last3Labels.All(label => label == last3Labels[0]))
        {
            runML();
            apimanager.startGASCoroutine();
            last3Labels.Clear(); // Optional: clear after triggering
        }
    }


    void DrawBox(Vector2 center, Vector2 size, string label, int id, float scaleX, float scaleY)
    {
        var panel = (id < boxPool.Count) ? boxPool[id] : CreateNewBox();
        panel.SetActive(true);
        panel.transform.localPosition = new Vector3(center.x * scaleX - displayImage.rectTransform.rect.width / 2, -(center.y * scaleY - displayImage.rectTransform.rect.height / 2));
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * scaleX, size.y * scaleY);
        panel.GetComponentInChildren<Text>().text = label;
    }

    GameObject CreateNewBox()
    {
        var panel = new GameObject("ObjectBox", typeof(RectTransform), typeof(Image));
        panel.GetComponent<Image>().sprite = boxTexture;
        panel.transform.SetParent(displayImage.transform, false);

        var txtObj = new GameObject("Label", typeof(RectTransform), typeof(Text));
        txtObj.transform.SetParent(panel.transform, false);
        var txt = txtObj.GetComponent<Text>();
        txt.font = font; txt.fontSize = 20;

        boxPool.Add(panel);
        return panel;
    }

    void ClearAnnotations() => boxPool.ForEach(box => box.SetActive(false));

    void UpdateLabel(string l)
    {
        objectLabel.text = l;
        apimanager.SetDetectedObject(l);

        last3Labels.Add(l);
        if (last3Labels.Count > 3)
            last3Labels.RemoveAt(0);
    }



    void OnDestroy()
    {
        engine?.Dispose();
        ops?.Dispose();
    }
}
