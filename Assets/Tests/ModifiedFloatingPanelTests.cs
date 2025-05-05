using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.ARFoundation;

public class ModifiedFloatingPanelTests
{
    GameObject testGO, cameraGO, panelGO;

    [SetUp]
    public void SetUp()
    {
        // Camera
        cameraGO = new GameObject("MainCamera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = Vector3.zero;
        cameraGO.transform.forward = Vector3.forward;

        // Panel
        panelGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        panelGO.name = "Panel";
        panelGO.transform.position = new Vector3(0, 0, 5);

        // Controller
        testGO = new GameObject("FloatingPanelController");
        var controller = testGO.AddComponent<ModifiedFloatingPanelController>();
        controller.m_Camera = cameraGO.transform;
        controller.m_PlayerSpacePanel = panelGO.transform;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGO);
        Object.DestroyImmediate(cameraGO);
        Object.DestroyImmediate(panelGO);
    }

    [UnityTest]
    public IEnumerator PanelMovesToCorrectPositionInFrontOfCamera()
    {
        var controller = testGO.GetComponent<ModifiedFloatingPanelController>();
        float expectedDistance = controller.m_DistanceFromCamera;

        // Let one frame of LateUpdate run
        yield return new WaitForEndOfFrame();

        float actualDistance = Vector3.Distance(controller.m_Camera.position, controller.m_PlayerSpacePanel.position);
        Assert.That(actualDistance, Is.EqualTo(expectedDistance).Within(0.1f), "Panel should be placed in front of the camera at the correct distance.");
    }

    [UnityTest]
    public IEnumerator PanelFacesCamera()
    {
        var controller = testGO.GetComponent<ModifiedFloatingPanelController>();

        // Let one frame of LateUpdate run
        yield return new WaitForEndOfFrame();

        Vector3 toCamera = controller.m_Camera.position - controller.m_PlayerSpacePanel.position;
        float angle = Vector3.Angle(controller.m_PlayerSpacePanel.forward, toCamera);

        Assert.That(angle, Is.LessThan(10f), "Panel should be rotated to face the camera.");
    }

    [UnityTest]
    public IEnumerator Awake_SetsCameraIfNull()
    {
        // Set up with no camera assigned
        Object.DestroyImmediate(cameraGO); // Remove tagged camera to test fallback
        cameraGO = new GameObject("NewCamera");
        cameraGO.tag = "MainCamera";
        Camera camComponent = cameraGO.AddComponent<Camera>(); // Needed for Camera.main to work

        var newGO = new GameObject("NewController");
        var controller = newGO.AddComponent<ModifiedFloatingPanelController>();

        // Simulate Unity lifecycle
        controller.Awake();

        yield return null;

        Assert.IsNotNull(controller.m_Camera, "Camera should be auto-assigned from Camera.main if null.");
        Object.DestroyImmediate(newGO);
    }

    [UnityTest]
    public IEnumerator HandlesMissingReferencesWithoutCrashing()
    {
        var controller = testGO.GetComponent<ModifiedFloatingPanelController>();
        controller.m_Camera = null;
        controller.m_PlayerSpacePanel = null;

        // Should not crash or throw exceptions
        yield return new WaitForEndOfFrame();

        Assert.Pass("No exceptions thrown with null references.");
    }
}
