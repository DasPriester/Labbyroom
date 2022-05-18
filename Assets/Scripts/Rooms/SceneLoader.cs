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
        }
    }

    public static string SeriaizeGameData()
    {
        GameData gd = new GameData();

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInventory inv = GameObject.FindObjectOfType<PlayerInventory>();

        //Save

        //Player Transform
        gd.playerPosition = player.transform.position;
        gd.playerRotation = new Vector2(player.Pitch, player.Yaw);

        //Player Inventory
        gd.inventory = inv.Inventory;
        gd.currentSlot = inv.CurrentSlot;

        //Rooms and doors

        //Objects
        gd.objectData = new List<ObjectData>();
        foreach (PickUpInteractable go in FindObjectsOfType<PickUpInteractable>())
        {
            ObjectData od = new ObjectData();
            od.name = go.PrefabName;
            od.position = go.transform.position;
            od.rotation = go.transform.rotation;

            if (go is RecipeInteractable)
            {
                od.recipe = (go as RecipeInteractable).Recipe;
            }

            gd.objectData.Add(od);
        }

        //Settings

        //Recipies

        return JsonUtility.ToJson(gd);
    }

    public static void DeserializeGameData(string data)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Moveable"))
        {
            Destroy(go);
        }

        GameData gd = JsonUtility.FromJson<GameData>(data);

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInventory inv = FindObjectOfType<PlayerInventory>();

        //Load

        //Player Transform
        player.transform.position = gd.playerPosition;
        player.Pitch = gd.playerRotation.x;
        player.Yaw = gd.playerRotation.y;

        //Player Inventory
        inv.Inventory = gd.inventory;
        inv.CurrentSlot = gd.currentSlot;

        //Rooms and doors

        //Objects
        foreach (ObjectData od in gd.objectData)
        {
            if (!od.recipe)
                Instantiate(Resources.Load("Prefabs/" + od.name), od.position, od.rotation);
            else
            {
                RecipeInteractable rec = Instantiate(Resources.Load<RecipeInteractable>("Prefabs/" + od.name), od.position, od.rotation);
                rec.Recipe = od.recipe;
            }
        }

        //Settings

        //Recipies

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

        //Objects
        public List<ObjectData> objectData;

        //Settings

        //Recipies
    }

    [Serializable]
    private struct ObjectData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public Recipe recipe;
    }
}