using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraAccess : MonoBehaviour
{
    public RawImage CameraRawImage;

    [SerializeField] private ARCameraManager _cameraManager;
    private Texture2D _cameraTexture;
    private XRCpuImage _lastCpuImage;

    // public void Awake()
    // {
    //     _cameraManager = FindFirstObjectByType<ARCameraManager>();
    //     if (_cameraManager)
    //     {
    //         Debug.Log("AR Camera Manager Found");
    //     }
    // }

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Camera Access Started");
        if (_cameraManager)
        {
            Debug.Log("AR Camera Manager exist");
        }
        _cameraManager.frameReceived += OnFrameReceived;
    }

    private void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log("Frame Received");
        _lastCpuImage = new XRCpuImage();
        if (!_cameraManager.TryAcquireLatestCpuImage(out _lastCpuImage))
        {
            Debug.Log("Cannot get CPU image");
            return;
        }

        UpdateCameraTexture(_lastCpuImage);
    }

    private unsafe void UpdateCameraTexture(XRCpuImage image)
    {

        Debug.Log("UpdateCameraTexture");
        var format = TextureFormat.RGBA32;

        if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
        {
            _cameraTexture = new Texture2D(image.width, image.height, format, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams(image, format);
        var rawTextureData = _cameraTexture.GetRawTextureData<byte>();

        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            image.Dispose();
        }

        _cameraTexture.Apply();
        CameraRawImage.texture = _cameraTexture;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}



