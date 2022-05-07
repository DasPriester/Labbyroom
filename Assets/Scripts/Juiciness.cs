using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Juiciness : MonoBehaviour
{
    private enum levels { None, Basic, Full };
    private levels level = levels.Full;
    PlayerController player;
    ParticleSystem atmos;
    List<Light> spots = new List<Light>();
    List<MeshRenderer> dimSplits = new List<MeshRenderer>();
    List<MeshRenderer> grass = new List<MeshRenderer>();

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        Light[] lights = FindObjectsOfType<Light>();
        MeshRenderer[] meshes = FindObjectsOfType<MeshRenderer>();

        foreach (ParticleSystem ps in particles)
        {
            if (ps.name == "Atmospheric")
            {
                atmos = ps;
            }
        }

        foreach (Light l in lights)
        {
            if (l.type == LightType.Spot)
            {
                spots.Add(l);
            }
        }

        foreach (MeshRenderer mr in meshes)
        {
            if (mr.material.shader == Shader.Find("Shader Graphs/DimensionalSplit"))
            {
                dimSplits.Add(mr);
            }

            if (mr.material.shader == Shader.Find("Shader Graphs/GrassChunk"))
            {
                grass.Add(mr);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            level++;
            if (level > levels.Full)
                level = levels.None;

            Interactable[] interactables = FindObjectsOfType<Interactable>();

            if (level == levels.None)
            {
                player.useHeadbob = false;
                player.useFootsteps = false;

                atmos.Stop();

                foreach (MeshRenderer ds in dimSplits)
                {
                    ds.enabled = false;
                    ds.GetComponentInChildren<ParticleSystem>().Stop();
                }

                foreach (MeshRenderer g in grass)
                {
                    g.enabled = false;
                }

                foreach (Light s in spots)
                {
                    s.enabled = false;
                }

                foreach (Interactable i in interactables)
                {
                    i.UseAudio = false;
                    i.UseOutline = false;
                    i.UseParticle = false;
                    i.UseAnimation = false;

                }
            }
            else if (level == levels.Basic)
            {
                player.useHeadbob = true;
                player.useFootsteps = true;

                atmos.Play();

                foreach (MeshRenderer ds in dimSplits)
                {
                    ds.enabled = false;
                    ds.GetComponentInChildren<ParticleSystem>().Stop();
                }

                foreach (MeshRenderer g in grass)
                {
                    g.enabled = true;
                    g.material.SetFloat("_Speed", 0f);
                }

                foreach (Light s in spots)
                {
                    s.enabled = false;
                }

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

                atmos.Play();

                foreach (MeshRenderer ds in dimSplits)
                {
                    ds.enabled = true;
                    ds.GetComponentInChildren<ParticleSystem>().Play();
                }

                foreach (MeshRenderer g in grass)
                {
                    g.enabled = true;
                    g.material.SetFloat("_Speed", 1.0f);
                }

                foreach (Light s in spots)
                {
                    s.enabled = true;
                }

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
