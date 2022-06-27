using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of a surface manager for a flat wall
/// </summary>
public class WallManager : SurfaceManager
{
    [SerializeField] private WallInteractable Wall = null;
    [SerializeField] private Vector3 Dimensions = Vector3.one;
    [SerializeField] private Hideable Preview = null;
    [SerializeField] private int roomWMID = 0;
    private Hideable preview = null;
    private float previewCooldown = 0;
    private bool isTemporary = false;

    private readonly List<WallInteractable> walls = new List<WallInteractable>();
    public List<Vector2> doors = new List<Vector2>();


    public int RoomWMID { get => roomWMID; set => roomWMID = value; }
    public bool IsTemporary { get => isTemporary; set => isTemporary = value; }

    void Start()
    {
        UpdateWall();

        foreach (WallInteractable wall in walls)
        {
            foreach (MeshRenderer mr in wall.GetComponentsInChildren<MeshRenderer>())
                mr.material.renderQueue = 2999;
        }

        preview = Instantiate(Preview, transform);
    }

    private void Update()
    {
        if (preview)
        {
            if (previewCooldown > 0)
            {
                previewCooldown -= Time.deltaTime;
                preview.GetComponent<CanvasGroup>().alpha = previewCooldown;
            }
            else
            {
                preview.Hide();
                previewCooldown = 0;
            }
        }
    }

    /// <summary>
    /// Adds the portal to the wall 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="roomType"></param>
    /// <param name="portalType"></param>
    /// <returns>Returns true if the door fitted else false</returns>
    public override bool AddDoor(Vector3 pos, Room roomType, PortalComponent portalType, bool temporary)
    {
        Vector3 inv = transform.InverseTransformPoint(pos);
        Vector2 door = new Vector2(inv.x + Dimensions.x / 2, portalType.width);

        if (DoorFits(door))
        {
            doors.Add(door);
            UpdateWall();

            Vector3 left = transform.position - transform.right * Dimensions.x / 2;

            if (temporary)
            {
                PlayerController.hasTempPath = true;
            }

            InsertPortal(left + door.x * transform.right - transform.up * Dimensions.y / 2, transform.rotation, roomType, portalType, Wall.GetComponent<MeshRenderer>().sharedMaterial, temporary, GetComponentInParent<Room>(), door, this);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check wether a door would fit at a given position
    /// </summary>
    /// <param name="door">Poition of the door</param>
    /// <returns>True if the door would fit else false</returns>
    private bool DoorFits(Vector2 door)
    {
        if (door.x - door.y / 2 < 0 || door.x + door.y / 2 > Dimensions.x)
        {
            return false;
        }

        foreach (Vector2 d in doors)
        {

            if (Mathf.Abs(door.x - d.x) < (door.y + d.y) / 2)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Update and enable preview of the door to be added 
    /// </summary>
    /// <param name="pos">Position of hit detection</param>
    /// <param name="portalType">Type of portal to preview</param>
    public override void OnViewedAtWithKey(Vector3 pos, float width)
    {
        Vector3 inv = transform.InverseTransformPoint(pos);
        Vector2 door = new Vector2(inv.x + Dimensions.x / 2, width);

        if (DoorFits(door))
        {
            Vector3 position = pos - 0.1f * transform.forward;
            position.y = Dimensions.y / 2;
            Vector3 scale = Vector3.one;
            scale.y = Dimensions.y;
            scale.x = width;

            preview.transform.position = position;
            preview.transform.localScale = scale;
            preview.transform.rotation = transform.rotation;

            previewCooldown = 1;
            preview.Show();
        }
    }

    /// <summary>
    /// Unity gizmos of wall location preview
    /// </summary>
    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawCube(Vector3.zero, Dimensions);


        Gizmos.color = Color.red;

        doors.Sort((p1, p2) => p1.x.CompareTo(p2.x));

        Vector3 left = - Vector3.right * Dimensions.x / 2;
        Vector3 right = Vector3.right * Dimensions.x / 2;

        foreach (Vector2 door in doors)
        {
            Vector3 leftPoint = left + (door.x - door.y / 2) * Vector3.right;
            Vector3 rightPoint = left + (door.x + door.y / 2) * Vector3.right;

            Vector3 center = (leftPoint + rightPoint) / 2;
            Vector3 dimensions = new Vector3((rightPoint - leftPoint).x, Dimensions.y, Dimensions.z);
            Gizmos.DrawWireCube(center, dimensions);
        }
    }

    /// <summary>
    /// Split and update the wall for each portal
    /// </summary>
    public void UpdateWall()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<WallInteractable>())
                Destroy(child.gameObject);
        }
        walls.Clear();
        doors.Sort((p1, p2) => p1.x.CompareTo(p2.x));

        Vector3 left = transform.position - transform.right * Dimensions.x / 2;
        Vector3 right = transform.position + transform.right * Dimensions.x / 2;

        float at = 0f;

        Vector3 leftPoint, rightPoint, center, dimensions;
        WallInteractable w;

        foreach (Vector2 door in doors)
        {
            leftPoint = left + at * transform.right;
            rightPoint = left + (door.x - door.y / 2) * transform.right;

            center = (leftPoint + rightPoint) / 2;
            dimensions = new Vector3((door.x - door.y / 2) - at, Dimensions.y, Dimensions.z);
            Wall.transform.localScale = dimensions;

            w = Instantiate(Wall, center, transform.rotation, transform);
            w.manager = this;
            w.IsTemporary = isTemporary;
            walls.Add(w);

            at = door.x + door.y / 2;
        }

        leftPoint = left + at * transform.right;
        rightPoint = right;

        center = (leftPoint + rightPoint) / 2;
        dimensions = new Vector3(Dimensions.x - at, Dimensions.y, Dimensions.z);
        Wall.transform.localScale = dimensions;

        w = Instantiate(Wall, center, transform.rotation, transform);
        w.manager = this;
        w.IsTemporary = isTemporary;
        walls.Add(w);

        Wall.transform.localScale = Vector3.one;
        Wall.transform.position = Vector3.zero;
    }
}
