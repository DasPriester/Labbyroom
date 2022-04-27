using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalComponent : MonoBehaviour
{
    [SerializeField] PortalComponent linkedPortal = null;
    [SerializeField] MeshRenderer screen = null;

    Camera playerCam;
    Camera portalCam;

    RenderTexture viewTexture;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            portalCam.targetTexture = viewTexture;

            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    private void Update()
    {
        screen.enabled = false;
        CreateViewTexture();

        Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        linkedPortal.portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        portalCam.Render();
        screen.enabled = true;
    }
}
