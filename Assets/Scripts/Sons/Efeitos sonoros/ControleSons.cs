using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleSons : MonoBehaviour
{
    private AudioSource audioSource; 
    public List<ConjuntoSonoro> conjuntoSons;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TocarSom(string tag)
    {
        foreach (var conjunto in conjuntoSons)
        {
            if (conjunto.tag == tag)
            {
                AudioClip clip;

                // Escolher clip
                if (conjunto.randomizado)
                    clip = conjunto.sons[Random.Range(0, conjunto.sons.Count)];
                else
                    clip = conjunto.sons[0];

                // Tocar
                audioSource.PlayOneShot(clip);
                return;
            }
        }

        Debug.LogError($"Nenhum conjunto sonoro encontrado para a tag: {tag}");
    }
}
