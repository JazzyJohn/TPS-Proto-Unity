using UnityEngine;
using System.Collections;

public class AwakeSound : MonoBehaviour {
    public AudioClip sound;

    public void Start(){
        PlayerMainGui.instance.PlayAnonce(sound);
    }
}
