using UnityEngine;
using System;
using System.Collections.Generic;

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

    public static string SeriaizeGameData()
    {
        GameData gd = new GameData();

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Inventory inv = GameObject.FindObjectOfType<Inventory>();

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
                    rotation2 = portal.linkedPortal.transform.rotation
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
        foreach (PickUpInteractable go in FindObjectsOfType<PickUpInteractable>())
        {
            ObjectData od = new ObjectData
            {
                name = go.prefabName,
                position = go.transform.position,
                rotation = go.transform.rotation
            };

            if (go.GetComponent<RecipeInteractable>())
            {
                od.recipe = (go.GetComponent<RecipeInteractable>()).Recipe;
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

    public static void DeserializeGameData(string data)
    {
        foreach (PickUpInteractable go in GameObject.FindObjectsOfType<PickUpInteractable>())
        {
            Destroy(go.gameObject);
        }

        foreach (Room room in GameObject.FindObjectsOfType<Room>())
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

            foreach (PickUpInteractable pi in room.GetComponentsInChildren<PickUpInteractable>())
            {
                Destroy(pi.gameObject);
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

    [Serializable]
    private struct ObjectData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public Recipe recipe;
    }

    [Serializable]
    private struct RoomData
    {
        public Vector3 position;
        public string name;
        public List<WallManagerData> wmData;
    }

    [Serializable]
    private struct WallManagerData
    {
        public List<Vector2> doors;
        public int wmid;
    }

    [Serializable]
    private struct PortalPairData
    {
        public Vector3 position1;
        public Quaternion rotation1;
        public Vector3 position2;
        public Quaternion rotation2;
        public string material1;
        public string material2;
        public string name;
    }
}