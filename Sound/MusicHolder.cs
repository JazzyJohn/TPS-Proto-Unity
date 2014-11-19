using UnityEngine;
using System.Collections;

public enum MUSIC_STAGE{
	MAIN_MENU,
	BATLLE,
	EXPLORATION,
}
[System.Serializable]
public class MusizStage{
	public enum MUSIC_STAGE;
	public AudioClip[] clips;
	
}
public class MusicHolder : MonoBehaviour {
	public MusizStage[] MusicInStage;
	

	
	public float musicTimeOut;
	
	public AudioSource musicPlayer;

    public MusizStage curStage;
	
	public int curStageInt;
	
	AudioClip curMusic;
			
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
		

	}
	void Start(){
        musicPlayer.volume = volume;
	}
	
	// Update is called once per frame
	void Update () {
        if (!musicPlayer.isPlaying && allClip.Length>0)
        {
			StartSong();
		}
        
        if ( GameRule.instance!=null&&curStage != (int)GameRule.instance.curStage)
        {
            curStageInt = (int)GameRule.instance.curStage;
			NextStage();
		}
	}
	
	public void StartSong(){
		
		int	curMusicInt = (int)UnityEngine.Random.Range (0, curStage.clips.Length);
		curMusic = curStage.clips[curMusicInt];
        Debug.Log("NOW PLAY" + curMusic); 
        musicPlayer.clip = curMusic;
		musicPlayer.PlayDelayed (musicTimeOut);

	}
	
	public void NextStage(){
        Debug.Log("NEW STAGE " + MusicInStage.Length);
		if (curStageInt <MusicInStage.Length ) {
			curStage = MusicInStage[curStageInt]
		    StartSong();
		}
	}
}
