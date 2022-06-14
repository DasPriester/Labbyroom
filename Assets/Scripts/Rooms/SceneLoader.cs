using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manage the loading of a scene from a save file
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SaveFile loadedFile;

    void Start()
    {
        if (loadedFile)
        {
            DeserializeGameData(loadedFile.data);
        } else
        {
            foreach (Recipe rec in Resources.LoadAll<Recipe>("Recipes"))
            {
                rec.unlocked = false;
            }
        }
    }

    /// <summary>
    /// Serialize current game state into json format
    /// </summary>
    /// <returns>Json representation of current game state</returns>
    public static string SeriaizeGameData()
    {
        GameData gd = new GameData();

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Inventory inv = FindObjectOfType<Inventory>();

        //Save

        //Player Transform
        gd.playerPosition = player.transform.position;
        gd.playerRotation = new Vector2(player.Pitch, player.Yaw);

        //Player Inventory
        gd.inventory = inv.Items;
        gd.currentSlot = inv.CurrentSlot;

        //Rooms and doors
        gd.roomData = new List<RoomData>();
        foreach (Room room in FindObjectsOfType<Room>())
        {
            RoomData rd = new RoomData
            {
                name = room.PrefabName,
                position = room.transform.position,

                wmData = new List<WallManagerData>()
            };
            foreach (WallManager wm in room.GetComponentsInChildren<WallManager>())
            {
                WallManagerData wmd = new WallManagerData
                {
                    doors = wm.doors,
                    wmid = wm.RoomWMID
                };

                rd.wmData.Add(wmd);
            }

            gd.roomData.Add(rd);
        }

        gd.ppData = new List<PortalPairData>();
        List<PortalComponent> connected = new List<PortalComponent>();
        foreach (PortalComponent portal in FindObjectsOfType<PortalComponent>())
        {
            if (!connected.Contains(portal))
            {
                PortalPairData ppd = new PortalPairData
                {
                    name = portal.PrefabName,
                    position1 = portal.transform.position,
                    rotation1 = portal.transform.rotation,
                    position2 = portal.linkedPortal.transform.position,
                    rotation2 = portal.linkedPortal.transform.rotation,
                    pcposition1 = portal.GetComponentInChildren<Camera>().transform.position,
                    pcrotation1 = portal.GetComponentInChildren<Camera>().transform.rotation,
                    pcposition2 = portal.linkedPortal.GetComponentInChildren<Camera>().transform.position,
                    pcrotation2 = portal.linkedPortal.GetComponentInChildren<Camera>().transform.rotation
                };

                foreach (MeshRenderer mr in portal.GetComponentsInChildren<MeshRenderer>())
                {
                    if (mr.gameObject.name == "_Wall")
                    {
                        ppd.material1 = mr.material.name.Replace(" (Instance)", "");
                        break;
                    }
                }
                foreach (MeshRenderer mr in portal.linkedPortal.GetComponentsInChildren<MeshRenderer>())
                {
                    if (mr.gameObject.name == "_Wall")
                    {
                        ppd.material2 = mr.material.name.Replace(" (Instance)","");
                        break;
                    }
                }

                gd.ppData.Add(ppd);
                connected.Add(portal);
                connected.Add(portal.linkedPortal);
            }
        }

        gd.z = PortalConnector.Z;

        //Objects
        gd.objectData = new List<ObjectData>();
        foreach (Moveable go in FindObjectsOfType<Moveable>())
        {
            ObjectData od = new ObjectData
            {
                name = go.PrefabName,
                position = go.transform.position,
                rotation = go.transform.rotation
            };

            RecipeInteractable ri = go.GetComponent<RecipeInteractable>();
            if (ri)
            {
                od.recipe = ri.Recipe;
            }

            gd.objectData.Add(od);
        }

        //Recipies
        gd.unlockedRecipies = new List<string>();
        foreach(Recipe rec in Resources.LoadAll<Recipe>("Recipes"))
        {
            if (rec.unlocked)
                gd.unlockedRecipies.Add(rec.name);
        }

        return JsonUtility.ToJson(gd);
    }

    /// <summary>
    /// Clear scene and create world from saved file
    /// </summary>
    /// <param name="data">Json representation of game state</param>
    public static void DeserializeGameData(string data)
    {
        foreach (Moveable mo in FindObjectsOfType<Moveable>())
        {
            Destroy(mo.gameObject);
        }

        foreach (Room room in FindObjectsOfType<Room>())
        {
            Destroy(room.gameObject);
        }

        GameData gd = JsonUtility.FromJson<GameData>(data);

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Inventory inv = FindObjectOfType<Inventory>();

        //Load

        //Player Transform
        player.transform.position = gd.playerPosition;
        player.Pitch = gd.playerRotation.x;
        player.Yaw = gd.playerRotation.y;

        //Player Inventory
        inv.Items = gd.inventory;
        inv.CurrentSlot = gd.currentSlot;

        //Rooms and doors
        foreach (RoomData rd in gd.roomData)
        {
            Room room = Instantiate(Resources.Load<Room>("Rooms/" + rd.name), rd.position, new Quaternion());

            foreach (Moveable mo in room.GetComponentsInChildren<Moveable>())
            {
                Destroy(mo.gameObject);
            }

            foreach (WallManagerData wmd in rd.wmData)
            {
                foreach (WallManager wm in room.GetComponentsInChildren<WallManager>())
                {
                    if (wmd.wmid == wm.RoomWMID)
                    {
                        wm.doors = wmd.doors;
                        wm.UpdateWall();
                    }
                }
            }
        }
        foreach (PortalPairData ppd in gd.ppData)
        {
            PortalComponent p1 = Instantiate(Resources.Load<PortalComponent>("Portals/" + ppd.name), ppd.position1, ppd.rotation1);
            PortalComponent p2 = Instantiate(Resources.Load<PortalComponent>("Portals/" + ppd.name), ppd.position2, ppd.rotation2);
            p1.GetComponentInChildren<Camera>().transform.position = ppd.pcposition1;
            p1.GetComponentInChildren<Camera>().transform.rotation = ppd.pcrotation1;
            p2.GetComponentInChildren<Camera>().transform.position = ppd.pcposition2;
            p2.GetComponentInChildren<Camera>().transform.rotation = ppd.pcrotation2;

            p1.linkedPortal = p2;
            p2.linkedPortal = p1;

            foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Wall")
                    mr.material = Resources.Load<Material>("Materials/" + ppd.material1);
            }
            foreach (MeshRenderer mr in p2.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Wall")
                    mr.material = Resources.Load<Material>("Materials/" + ppd.material2);
            }
            foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name != "_Hide")
                    mr.material.SetFloat("_TimeAppear", Time.time - 1000);
            }
            foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Hide")
                    mr.enabled = false;
            }
            foreach (MeshRenderer mr in p2.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Hide")
                    mr.enabled = false;
            }

            p1.GetComponentInChildren<DoorInteractable>().UpdateConnection();
            p2.GetComponentInChildren<DoorInteractable>().UpdateConnection();

        }

        PortalConnector.Z = gd.z;

        //Objects
        foreach (ObjectData od in gd.objectData)
        {
            if (!od.recipe)
            {
                Instantiate(Resources.Load("Prefabs/" + od.name), od.position, od.rotation);
            }
            else
            {
                RecipeInteractable rec = Instantiate(Resources.Load<RecipeInteractable>("Prefabs/" + od.name), od.position, od.rotation);
                rec.Recipe = od.recipe;
            }
        }

        //Recipies
        foreach (Recipe rec in Resources.LoadAll<Recipe>("Recipes"))
        {
            rec.unlocked = gd.unlockedRecipies.Contains(rec.name);
        }

        Physics.SyncTransforms();
    }

    /// <summary>
    /// Collection of all data of a save file
    /// </summary>
    [Serializable]
    private struct GameData
    {
        //Player Transform
        public Vector3 playerPosition;
        public Vector2 playerRotation;

        //Player Inventory
        public Item[] inventory;
        public int currentSlot;

        //Rooms and doors
        public List<RoomData> roomData;
        public List<PortalPairData> ppData;
        public int z;

        //Objects
        public List<ObjectData> objectData;

        //Recipies
        public List<string> unlockedRecipies;
    }

    /// <summary>
    /// Data about each object that can be picked up
    /// </summary>
    [Serializable]
    private struct ObjectData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public Recipe recipe;
    }

    /// <summary>
    /// Data about each room that has been created
    /// </summary>
    [Serializable]
    private struct RoomData
    {
        public Vector3 position;
        public string name;
        public List<WallManagerData> wmData;
    }

    /// <summary>
    /// Data about a single wall of a room
    /// </summary>
    [Serializable]
    private struct WallManagerData
    {
        public List<Vector2> doors;
        public int wmid;
    }

    /// <summary>
    /// Data about each pair of portals
    /// </summary>
    [Serializable]
    private struct PortalPairData
    {
        public Vector3 position1;
        public Quaternion rotation1;
        public Vector3 position2;
        public Quaternion rotation2;

        public Vector3 pcposition1;
        public Quaternion pcrotation1;
        public Vector3 pcposition2;
        public Quaternion pcrotation2;

        public bool open1;
        public bool open2;

        public string material1;
        public string material2;
        public string name;
    }
}