using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controle_Trilha : MonoBehaviour
{
    public List<AudioSource> musicas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FazerControleVolume();
    }

    void FazerControleVolume()
    {
        float somatorio = 0;
        foreach(AudioSource musica in musicas) 
        {
            somatorio += musica.volume;
        }
        foreach(AudioSource musica in musicas)
        {
            musica.volume /= somatorio;
        }
    }
}
