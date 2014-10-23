using UnityEngine;
using System.Collections;

public enum MUSIC_STAGE{
	MAIN_MENU,
	BATLLE,
	EXPLORATION,
}

public class MusicHolder : MonoBehaviour {
	public int[] MusicInStage;
	
	public AudioClip[] allClip;
	
	public float musicTimeOut;
	
	public AudioSource musicPlayer;

    public int curStage;
	
	int curMusic;
	
	int[] MusicBefore;
	
	private static float volume = 1.0f;
	
	public static void  SetVolume(float newVolume){
		if(volume!=newVolume){
			volume = newVolume;
			MusicHolder holder = FindObjectOfType(typeof (MusicHolder)) as MusicHolder;
			if(holder!=null){
                holder.musicPlayer.volume = volume;
			}
		}
	
	}
	
	// Use this for initialization
	void Awake () {
		musicPlayer = GetComponent<AudioSource> ();
        musicPlayer.ignoreListenerVolume = true;
		MusicBefore = new int[MusicInStage.Length];
		int tempMusic = 0;
		for (int i = 0; i < MusicBefore.Length; i++) {
			MusicBefore[i] = MusicInStage[i] + tempMusic;
			tempMusic += MusicInStage[i];
			Debug.Log(MusicBefore[i]);
		}

	}
	void Start(){
        musicPlayer.volume = volume;
	}
	
	// Update is called once per frame
	void Update () {
		if (!musicPlayer.isPlaying) {
			StartSong();
		}
        
        if ( GameRule.instance!=null&&curStage != (int)GameRule.instance.curStage)
        {
            curStage = (int)GameRule.instance.curStage;
			NextStage();
		}
	}
	
	public void StartSong(){
		if (curStage == 0) {
			curMusic = (int)UnityEngine.Random.Range (0, MusicBefore [curStage]);
		}
		else {
            
			curMusic = (int)UnityEngine.Random.Range (MusicBefore [curStage - 1], MusicBefore [curStage]);
         
			if(curMusic>allClip.Length) curMusic = MusicBefore[curStage-1] - 1;
		}
        Debug.Log("NOW PLAY" + curMusic); 
        musicPlayer.clip = allClip[curMusic];
		musicPlayer.PlayDelayed (musicTimeOut);

	}
	
	public void NextStage(){
        Debug.Log("NEW STAGE " + MusicInStage.Length);
		if (curStage <= MusicInStage.Length - 1) {
				//curStage++;
				//curMusic = MusicBefore[curStage] - 1;
            Debug.Log("NEW STAGE " + curStage);
				musicPlayer.Stop ();
				
		}
	}
}
