using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PortalComponent : MonoBehaviour
{
    [SerializeField] public PortalComponent linkedPortal = null;
    [SerializeField] public MeshRenderer screen = null;
    [SerializeField] public float width = 1f;

    Camera playerCam;
    Camera portalCam;

    RenderTexture viewTexture;
    MeshFilter screenMeshFilter;

    List<PortalTraveller> trackedTravellers;

    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;
    public int recursionLimit = 5;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller>();
        screenMeshFilter = screen.GetComponent<MeshFilter>();
        screen.material.SetInt("displayMask", 1);
    }

    private void LateUpdate()
    {
        if (linkedPortal)
        {
            linkedPortal.ProtectScreenFromClipping(playerCam.transform.position);
            for (int i = 0; i < trackedTravellers.Count; i++)
            {
                PortalTraveller traveller = trackedTravellers[i];
                Transform travellerT = traveller.transform;
                var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

                Vector3 offsetFromPortal = travellerT.position - transform.position;
                int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
                int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

                if (portalSide != portalSideOld)
                {
                    traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                    linkedPortal.OnTravellerEnterPortal(traveller);
                    trackedTravellers.Remove(traveller);
                    i--;

                }
                else
                {
                    traveller.previousOffsetFromPortal = offsetFromPortal;
                }
            }
        }
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            viewTexture = new RenderTexture(Screen.width, Screen.height, 8);
            portalCam.targetTexture = viewTexture;

            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    private void Update()
    {
        if (linkedPortal)
        {
            if (!CameraUtility.VisibleFromCamera(linkedPortal.screen, playerCam))
            {
                return;
            }

            CreateViewTexture();

            var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
            var renderPositions = new Vector3[recursionLimit];
            var renderRotations = new Quaternion[recursionLimit];

            int startIndex = 0;
            portalCam.projectionMatrix = playerCam.projectionMatrix;
            for (int i = 0; i < recursionLimit; i++)
            {
                if (i > 0)
                {
                    // No need for recursive rendering if linked portal is not visible through this portal
                    if (!CameraUtility.BoundsOverlap(screenMeshFilter, linkedPortal.screenMeshFilter, portalCam))
                    {
                        break;
                    }
                }
                localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
                int renderOrderIndex = recursionLimit - i - 1;
                renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
                renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

                portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
                startIndex = renderOrderIndex;
            }

            screen.enabled = false;
            linkedPortal.screen.material.SetInt("displayMask", 0);

            for (int i = startIndex; i < recursionLimit; i++)
            {
                portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
                SetNearClipPlane();
                linkedPortal.ProtectScreenFromClipping(portalCam.transform.position);
                if (portalCam.transform.eulerAngles != Vector3.zero)
                {
                    portalCam.Render();
                }

                if (i == startIndex)
                {
                    linkedPortal.screen.material.SetInt("displayMask", 1);
                }
            }

            screen.enabled = true;
        }
    }
    

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    float ProtectScreenFromClipping(Vector3 viewPoint)
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = screen.transform;
        float dist = Vector3.Distance(screenT.position, viewPoint);
        bool inFrontOfPortal = dist < screenT.localScale.x / 1.5;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
        screenThickness = inFrontOfPortal ? screenThickness : screenT.localScale.y / 2;
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void SetNearClipPlane()
    {
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (linkedPortal)
        {
            Gizmos.color = Color.green;
            Vector3 a = screen.transform.position;
            Vector3 b = linkedPortal.screen.transform.position;

            Vector3 am = a;
            Vector3 bi = b;
            am.Scale(Vector3.one * 0.9f);
            bi.Scale(Vector3.one * 0.1f);
            am = am + bi;

            Vector3 ai = a;
            Vector3 bm = b;
            ai.Scale(Vector3.one * 0.1f);
            bm.Scale(Vector3.one * 0.9f);
            bm = bm + ai;

            DrawArrow(am, bm);

            Gizmos.color = Color.blue;
            DrawArrow(a - transform.forward, a);
            DrawArrow(b, b + linkedPortal.screen.transform.forward);


            Gizmos.color = Color.red;
            DrawArrow(a + transform.forward, a);
            DrawArrow(b, b - linkedPortal.screen.transform.forward);
        }
    }

    static void DrawArrow(Vector3 a, Vector3 b)
    {
        Gizmos.DrawLine(a, b);
        Vector3 dir = (a - b).normalized;
        Vector3 n = Vector3.Cross(dir, (SceneView.lastActiveSceneView.camera.transform.position - b).normalized);
        n.Scale(Vector3.one * 0.5f);
        Gizmos.DrawLine(b + dir + n, b);
        Gizmos.DrawLine(b + dir - n, b);
        Gizmos.DrawLine(a, b);
    }
}
