using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] private Wall Wall = null;
    [SerializeField] private Vector3 Dimensions = Vector3.one;

    private List<Wall> walls = new List<Wall>();
    [SerializeField] private List<Vector2> doors = new List<Vector2>();

    public bool AddDoor(Vector3 pos, float width)
    {
        Vector3 inv = transform.InverseTransformPoint(pos);
        Vector2 door = new Vector2(inv.x + Dimensions.x / 2, width);

        if (door.x < 0 || door.x + door.y > Dimensions.x)
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

        doors.Add(door);
        UpdateWall();

        return true;
    }

    public void DrawDoor(Vector3 pos, float width, Material mat)
    {
        Vector3 inv = transform.InverseTransformPoint(pos);
        Vector2 door = new Vector2(inv.x + Dimensions.x / 2, width);

        if (door.x < 0 || door.x + door.y > Dimensions.x)
        {
            return;
        }

        foreach (Vector2 d in doors)
        {
            if (Mathf.Abs(door.x - d.x) < (door.y + d.y) / 2)
            {
                return;
            }
        }

        Vector3 left = transform.position - transform.right * Dimensions.x / 2;
        mat.SetVector("_DoorPos", left + door.x * transform.right + Dimensions.z / 2 * transform.forward);
        mat.SetVector("_DoorDim", new Vector3(width, Dimensions.y));
    }

    private Camera playerCamera;

    void Start()
    {
        UpdateWall();
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawCube(Vector3.zero, Dimensions);


        Gizmos.color = Color.red;

        doors.Sort((p1, p2) => p1.x.CompareTo(p2.x));

        Vector3 left = - transform.right * Dimensions.x / 2;
        Vector3 right = transform.right * Dimensions.x / 2;

        foreach (Vector2 door in doors)
        {
            Vector3 leftPoint = left + (door.x - door.y / 2) * Vector3.right;
            Vector3 rightPoint = left + (door.x + door.y / 2) * Vector3.right;

            Vector3 center = (leftPoint + rightPoint) / 2;
            Vector3 dimensions = new Vector3((rightPoint - leftPoint).x, Dimensions.y, Dimensions.z);
            Gizmos.DrawWireCube(center, dimensions);
        }
    }

    private void UpdateWall()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        walls.Clear();
        doors.Sort((p1, p2) => p1.x.CompareTo(p2.x));

        Vector3 left = transform.position - transform.right * Dimensions.x / 2;
        Vector3 right = transform.position + transform.right * Dimensions.x / 2;

        float at = 0f;

        Vector3 leftPoint, rightPoint, center, dimensions;
        Wall w;

        foreach (Vector2 door in doors)
        {
            leftPoint = left + at * transform.right;
            rightPoint = left + (door.x - door.y / 2) * transform.right;

            center = (leftPoint + rightPoint) / 2;
            dimensions = new Vector3((door.x - door.y / 2) - at, Dimensions.y, Dimensions.z);
            Wall.transform.localScale = dimensions;

            w = Instantiate(Wall, center, transform.rotation, transform);
            w.manager = this;
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
        walls.Add(w);

        Wall.transform.localScale = Vector3.one;
        Wall.transform.position = Vector3.zero;
    }
}
