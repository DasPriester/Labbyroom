using UnityEngine;

public class InconsitencyObject : MonoBehaviour
{
    Camera cam;
    Transform originalTransform;
    bool moved;
    MeshRenderer self;

    [Header("Move")]
    [SerializeField] bool move = false;
    [SerializeField] bool moveY = false;
    [SerializeField] float moveInconsistency = 0.1f;

    [Header("Rotate")]
    [SerializeField] bool rotate = false;
    [SerializeField] Vector3 rotateInconsistency = Vector3.one * 5f;

    [Header("Scale")]
    [SerializeField] bool scale = false;
    [SerializeField] bool keepOnFloor = true;
    [SerializeField] Vector3 scaleInconsistency = Vector3.one * 0.1f;

    [Header("Color")]
    [SerializeField] bool chageColor = false;
    [SerializeField] bool keepAspect = false;
    [SerializeField] Vector3 colorInconsistency = Vector3.one * 0.1f;

    [SerializeField] bool escalate = false;

    void Awake()
    {
        cam = Camera.main;
        originalTransform = transform;
        moved = false;
        self = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (CameraUtility.VisibleFromCamera(self, cam))
        {
            moved = false;
        }
        else if (!moved)
        {
            UpdateInconsitency();
            moved = true;
        }
    }

    private void UpdateInconsitency()
    {
        // Move
        if (move)
        {
            Vector3 oriPos = transform.position;
            do
            {
                transform.position = oriPos + Vector3.back * Random.Range(-1f, 1f) * moveInconsistency;
                transform.position = oriPos + Vector3.right * Random.Range(-1f, 1f) * moveInconsistency;
            } while (CameraUtility.VisibleFromCamera(self, cam));
        }

        if (moveY)
        {
            Vector3 oriPos = transform.position;
            do
            {
                transform.position = oriPos + Vector3.up * Random.Range(-1f, 1f) * moveInconsistency;
            } while (CameraUtility.VisibleFromCamera(self, cam));
        }

        // Rotate
        if (rotate)
        {
            transform.rotation = transform.rotation * Quaternion.AngleAxis(Random.Range(-1f, 1f) * rotateInconsistency.x, Vector3.right);
            transform.rotation = transform.rotation * Quaternion.AngleAxis(Random.Range(-1f, 1f) * rotateInconsistency.y, Vector3.up);
            transform.rotation = transform.rotation * Quaternion.AngleAxis(Random.Range(-1f, 1f) * rotateInconsistency.z, Vector3.forward);
        }

        // Scale
        if (scale)
        {
            Vector3 scl = transform.localScale;
            scl.x += Random.Range(-1f, 1f) * scaleInconsistency.x;
            scl.y += Random.Range(-1f, 1f) * scaleInconsistency.y;
            scl.z += Random.Range(-1f, 1f) * scaleInconsistency.z;
            if (keepOnFloor)
            {
                transform.position += (scl.y - transform.localScale.y) / 2 * Vector3.up;
            }
            transform.localScale = scl;
        }

        if (chageColor)
        {
            Color col = self.material.GetColor("_Color");
            if (keepAspect)
            {
                float r = Random.Range(-1f, 1f) * colorInconsistency.x;
                
                col.r = Mathf.Clamp(col.r + col.r * r, 0, 1);
                col.g = Mathf.Clamp(col.g + col.g * r, 0, 1);
                col.b = Mathf.Clamp(col.b + col.b * r, 0, 1);
            } else
            {
                col.r = Mathf.Clamp(col.r + Random.Range(-1f, 1f) * colorInconsistency.x, 0, 1);
                col.g = Mathf.Clamp(col.g + Random.Range(-1f, 1f) * colorInconsistency.y, 0, 1);
                col.b = Mathf.Clamp(col.b + Random.Range(-1f, 1f) * colorInconsistency.z, 0, 1);
            }
            self.material.SetColor("_Color", col);
        }
    }
}
