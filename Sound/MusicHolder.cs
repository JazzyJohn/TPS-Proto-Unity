using UnityEngine;
using System.Collections;

public class MusicHolder : MonoBehaviour {
	public int[] MusicInStage;
	
	public AudioClip[] allClip;
	
	public float musicTimeOut;
	
	public AudioSource musicPlayer;
	
	int curStage;
	
	int curMusic;
	
	int[] MusicBefore;
	
	// Use this for initialization
	void Start () {
		musicPlayer = GetComponent<AudioSource> ();
		MusicBefore = new int[MusicInStage.Length];
		for (int i = 0; i < MusicBefore.Length; i++) {
			MusicBefore[i] = MusicBeforeInt(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!musicPlayer.isPlaying) {
			StartSong();
		}
	}
	
	public void StartSong(){
		if(curMusic >= MusicBefore[curStage + 1] - 1) curMusic = MusicBefore[curStage] - 1;
		musicPlayer.clip = allClip [curMusic];
		musicPlayer.PlayDelayed (musicTimeOut);
		curMusic++;
	}
	
	public void NextStage(){
		if(curStage < MusicInStage.Length) curStage++;
	}
	
	int MusicBeforeInt(int Stage){
		if(Stage == 0)return MusicInStage[0];
		return MusicInStage[Stage] + MusicBeforeInt(Stage - 1);
	}
}
