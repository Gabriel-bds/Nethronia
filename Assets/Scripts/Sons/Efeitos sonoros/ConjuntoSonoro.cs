using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConjuntoSonoro
{
    public string tag;
    public List<AudioClip> sons = new List<AudioClip>();
    public bool randomizado;
}
