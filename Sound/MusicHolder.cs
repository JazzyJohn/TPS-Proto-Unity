using UnityEngine;
using System.Collections;

public class MusicHolder : MonoBehaviour {

	public AudioClip[] allClip;

	public float musicTimeOut;

	public AudioSource musicPlayer;

	// Use this for initialization
	void Start () {
		musicPlayer = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!musicPlayer.isPlaying) {
			StartSong();
		}
	}
	public void StartSong(){
		musicPlayer.clip = allClip [(int)(UnityEngine.Random.value * allClip.Length)];
		musicPlayer.PlayDelayed (musicTimeOut);
	}
}
