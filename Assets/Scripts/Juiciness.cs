using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Juiciness : MonoBehaviour
{
    private enum levels { None, Basic, Full };
    private levels level = levels.Full;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { 
            level++;
            if(level > levels.Full) 
                level = levels.None;

            PlayerController player = FindObjectOfType<PlayerController>();
            Interactable[] interactables = FindObjectsOfType<Interactable>();

            if (level == levels.None) {
                player.useHeadbob = false;
                player.useFootsteps = false;

                foreach (Interactable i in interactables)
                {
                    i.UseAudio = false;
                    i.UseOutline = false;
                    i.UseParticle = false;
                    i.UseAnimation = false;

                }
            } else if (level == levels.Basic)
            {
                player.useHeadbob = true;
                player.useFootsteps = true;

                foreach (Interactable i in interactables)
                {
                    i.UseAudio = true;
                    i.UseOutline = true;
                    i.UseParticle = false;
                    i.UseAnimation = false;

                }
            }
            else if (level == levels.Full)
            {
                player.useHeadbob = true;
                player.useFootsteps = true;

                foreach (Interactable i in interactables)
                {
                    i.UseAudio = true;
                    i.UseOutline = true;
                    i.UseParticle = true;
                    i.UseAnimation = true;

                }
            }

            GetComponent<Text>().text = "Juiciness: " + level.ToString() + "\n Change: >Tab<";

        }
    }
}
