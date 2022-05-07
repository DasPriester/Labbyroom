using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{

    [SerializeField] private GameObject straw = null;
    [SerializeField] private Vector2 dimensions = new Vector2(1, 1);
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private float density = 2;

    void Awake()
    {
        //Quaternion rot = new Quaternion();
        float r = 1f / density / 2;

        for (float x = - dimensions.x / 2 + r; x < dimensions.x / 2f; x += 2 * r) {
            for (float z = -dimensions.y / 2 + r; z < dimensions.y / 2f; z += 2 * r) {
                float height = Random.Range(0.5f, 1f);
                straw.transform.localScale = new Vector3(1f, height, 1f);
                //rot = new Quaternion();
                //rot.eulerAngles = new Vector3(0, Random.Range(0f, 180f), 0);
                //straw.transform.rotation = rot;
                Instantiate(straw, new Vector3(
                    x + transform.position.x - Random.Range(-r, r) + offset.x,
                    transform.position.y + offset.y,
                    z + transform.position.z - Random.Range(-r, r) + offset.z),
                    transform.rotation);
            }
        }
        straw.transform.localScale = new Vector3(1f, 1f, 1f);
        //rot.eulerAngles = new Vector3(0, 0, 0);
        //straw.transform.rotation = rot;
    }
}
