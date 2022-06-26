using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
        Inventory inv = player.GetComponent<Inventory>();

        //Save

        gd.hasTempPath = PlayerController.hasTempPath;

        //Player Transform
        gd.playerPosition = player.transform.position;
        gd.rotation = player.transform.rotation;
        gd.playerRotation = new Vector2(player.Pitch, player.Yaw);

        //Player Inventory
        gd.inventory = JsonHelper.ToJson(inv.Items);
        gd.currentSlot = inv.CurrentSlot;

        //Tutorial
        gd.step = GameObject.Find("TutorialManager").GetComponent<TutorialManager>().step;

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
                    portal1 = new PortalData(portal),
                    portal2 = new PortalData(portal.linkedPortal)
                };

                gd.ppData.Add(ppd);
                connected.Add(portal);
                connected.Add(portal.linkedPortal);
            }
        }

        gd.z_temp = PortalConnector.ZTemp;
        gd.z_perm = PortalConnector.ZPerm;

        //Objects
        gd.objectData = new List<ObjectData>();
        foreach (Moveable go in FindObjectsOfType<Moveable>())
        {
            ObjectData od = new ObjectData
            {
                name = go.PrefabName,
                position = go.transform.position,
                rotation = go.transform.rotation,
                tag = go.transform.tag
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

        //Quests

        //gd.quests = FindObjectOfType<QuestManager>().Quests;
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

        PlayerController.hasTempPath = gd.hasTempPath;

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Inventory inv = player.GetComponent<Inventory>();
        TutorialManager tm = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();

        //Load
        //Player Transform
        player.transform.SetPositionAndRotation(gd.playerPosition, gd.rotation);
        player.Pitch = gd.playerRotation.x;
        player.Yaw = gd.playerRotation.y;

        //Player Inventory
        inv.Items = JsonHelper.FromJson<Item>(gd.inventory);
        inv.CurrentSlot = gd.currentSlot;
        inv.RefreshUI();

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
            PortalComponent p1 = Instantiate(Resources.Load<PortalComponent>("Portals/" + ppd.name), ppd.portal1.position, ppd.portal1.rotation);
            PortalComponent p2 = Instantiate(Resources.Load<PortalComponent>("Portals/" + ppd.name), ppd.portal2.position, ppd.portal2.rotation);
            p1.GetComponentInChildren<Camera>().transform.SetPositionAndRotation(ppd.portal1.pcposition, ppd.portal1.pcrotation);
            p2.GetComponentInChildren<Camera>().transform.SetPositionAndRotation(ppd.portal2.pcposition, ppd.portal2.pcrotation);

            p1.IsTemporary = ppd.portal1.temp;
            p2.IsTemporary = ppd.portal2.temp;

            p1.linkedPortal = p2;
            p2.linkedPortal = p1;

            p1.Door = ppd.portal1.door;
            p2.Door = ppd.portal2.door;

            foreach (MeshRenderer mr in p1.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Wall")
                    mr.material = Resources.Load<Material>("Materials/" + ppd.portal1.material);
            }
            foreach (MeshRenderer mr in p2.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Wall")
                    mr.material = Resources.Load<Material>("Materials/" + ppd.portal2.material);
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


            foreach (Room rm in GameObject.FindObjectsOfType<Room>())
            {
                float roomZ = Mathf.Round(rm.transform.position.z / 100);
                float roomX = Mathf.Round(rm.transform.position.x / 100);

                float portal1Z = Mathf.Round(p1.transform.position.z / 100);
                float portal1X = Mathf.Round(p1.transform.position.x / 100);
                if (roomZ == portal1Z && roomX == portal1X) {
                    p1.Room = rm;

                    foreach (WallManager wm in rm.GetComponentsInChildren<WallManager>())
                    {
                        if (wm.RoomWMID == ppd.portal1.wmid)
                            p1.WallManager = wm;
                    }

                    break;
                }
            }

            foreach (Room rm in GameObject.FindObjectsOfType<Room>())
            {
                float roomZ = Mathf.Round(rm.transform.position.z / 100);
                float roomX = Mathf.Round(rm.transform.position.x / 100);


                float portal2Z = Mathf.Round(p2.transform.position.z / 100);
                float portal2X = Mathf.Round(p2.transform.position.x / 100);
                if (roomZ == portal2Z && roomX == portal2X)
                {
                    p2.Room = rm;

                    foreach (WallManager wm in rm.GetComponentsInChildren<WallManager>())
                    {
                        if (wm.RoomWMID == ppd.portal2.wmid)
                            p2.WallManager = wm;
                    }

                    break;
                }
            }

            if (ppd.portal1.open)
                p1.GetComponentInChildren<Animator>().SetTrigger("ToggleTrigger");
            if (ppd.portal2.open)
                p2.GetComponentInChildren<Animator>().SetTrigger("ToggleTrigger");

            p1.GetComponentInChildren<DoorInteractable>().UpdateConnection();
            p2.GetComponentInChildren<DoorInteractable>().UpdateConnection();

        }

        PortalConnector.ZTemp = gd.z_temp;
        PortalConnector.ZPerm = gd.z_perm;

        //Objects
        GameObject summon;
        foreach (ObjectData od in gd.objectData)
        {
            if (!od.recipe)
            {
                summon = Instantiate(Resources.Load("Prefabs/" + od.name), od.position, od.rotation) as GameObject;
                summon.tag = od.tag;
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

        //Tutorial
        tm.step = gd.step;
        tm.Refresh();

        Physics.SyncTransforms();

        //Quests

        //FindObjectOfType<QuestManager>().AddQuest(gd.quests);
    }

    /// <summary>
    /// Collection of all data of a save file
    /// </summary>
    [Serializable]
    private struct GameData
    {
        public bool hasTempPath;

        //Player Transform
        public Vector3 playerPosition;
        public Vector2 playerRotation;
        public Quaternion rotation;

        //Player Inventory
        public string inventory;
        public int currentSlot;

        //Tutorial
        public int step;

        //Rooms and doors
        public List<RoomData> roomData;
        public List<PortalPairData> ppData;
        public int z_temp;
        public int z_perm;

        //Objects
        public List<ObjectData> objectData;

        //Recipies
        public List<string> unlockedRecipies;

        //Quests
        //public Dictionary<Quest, bool> quests;
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
        public string tag;
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
        public PortalData portal1;
        public PortalData portal2;
        public string name;
    }

    [Serializable] struct PortalData
    {
        public Vector3 position;
        public Quaternion rotation;

        public Vector3 pcposition;
        public Quaternion pcrotation;

        public bool temp;
        public bool open;
        public string material;
        public Vector2 door;
        public int wmid;

        public PortalData(PortalComponent portal)
        {
            position = portal.transform.position;
            rotation = portal.transform.rotation;
            pcposition = portal.GetComponentInChildren<Camera>().transform.position;
            pcrotation = portal.GetComponentInChildren<Camera>().transform.rotation;

            temp = portal.IsTemporary;
            material = null;
            open = portal.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open");
            door = portal.Door;
            if (portal.WallManager)
                wmid = portal.WallManager.RoomWMID;
            else
                wmid = -1;

            foreach (MeshRenderer mr in portal.GetComponentsInChildren<MeshRenderer>())
            {
                if (mr.gameObject.name == "_Wall")
                {
                    material = mr.material.name.Replace(" (Instance)", "");
                    break;
                }
            }
        }
    }
}