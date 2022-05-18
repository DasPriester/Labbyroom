using UnityEngine;
using System;

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

        return JsonUtility.ToJson(gd);
    }

    public static void DeserializeGameData(string data)
    {
        GameData gd = JsonUtility.FromJson<GameData>(data);

        //Variables
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInventory inv = GameObject.FindObjectOfType<PlayerInventory>();

        //Load

        //Player Transform
        player.transform.position = gd.playerPosition;
        player.Pitch = gd.playerRotation.x;
        player.Yaw = gd.playerRotation.y;

        //Player Inventory
        inv.Inventory = gd.inventory;
        inv.CurrentSlot = gd.currentSlot;
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
    }
}