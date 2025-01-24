using System.Collections.Generic;
using UnityEngine;

public class ApplicationStartup : MonoBehaviour
{
    [SerializeField] private GameObject m_cameraRoot;
    [SerializeField] private GameObject m_applicationSystemsHolder;
    [SerializeField] private CameraSettings m_cameraSettings;

    private void Awake()
    {
        AllSystems.Initialize(m_applicationSystemsHolder.GetComponents<IApplicationSystem>());

        AbstractCameraController cameraController = m_cameraRoot.AddComponent<AutoFollowPlayerCameraController>();
        InterpolatedCameraMovement cameraMovement = new InterpolatedCameraMovement();
        cameraMovement.Initialize(m_cameraRoot.transform, m_cameraSettings);
        cameraController.Initialize(cameraMovement, m_cameraSettings);
    }

    private void OnApplicationQuit()
    {
        AllSystems.Shutdown();
    }
}
