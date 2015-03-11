using UnityEngine;
using System.Collections;

public enum MUSIC_STAGE{
    NONE,
    MAIN_MENU,
	MAIN_MENU_LOOP,
	BATLLE,
	EXPLORATION,
}
[System.Serializable]
public class MusizStage{

	public AudioClip[] clips;

    public bool withIntro=false;
	
}
public class MusicHolder : MonoBehaviour {
	public MusizStage[] MusicInStage;
	

	
	public float musicTimeOut;
	
	public AudioSource musicPlayer;

    public MusizStage curStage;

    public MUSIC_STAGE curStageEnum;

    public int curStageInt;
	
	AudioClip curMusic;
			
	private static float volume = 1.0f;

    public bool wasIntro;
	
	public static void  SetVolume(float newVolume){
	
		
		MusicHolder holder = FindObjectOfType(typeof (MusicHolder)) as MusicHolder;
		if(holder!=null){
            if (holder.musicPlayer.volume != newVolume)
            {
                volume = newVolume;
                holder.musicPlayer.volume = volume;
            }
		}
		
	
	}
	
	// Use this for initialization
	void Awake () {
		musicPlayer = GetComponent<AudioSource> ();
        musicPlayer.ignoreListenerVolume = true;
        curStage = MusicInStage[0];

	}
	
	
	// Update is called once per frame
	void Update () {
     
        if (!musicPlayer.isPlaying && MusicInStage.Length > 0)
        {
            switch (curStageEnum)
            {
                case MUSIC_STAGE.NONE:
                    SetStage(MUSIC_STAGE.MAIN_MENU);
                    break;
                case MUSIC_STAGE.MAIN_MENU:
                    SetStage(MUSIC_STAGE.MAIN_MENU_LOOP);
                    break;
            }
            
			StartSong();
		}

        if (GameRule.instance != null && curStageEnum != GameRule.instance.curStage)
        {
            
            SetStage(GameRule.instance.curStage);
           
			NextStage();
		}
	}

    public void SetStage(MUSIC_STAGE nextStage)
    {
        wasIntro = false;
        curStageEnum = nextStage;
        StartSong();
    }
	public void StartSong(){
        curStageInt = (int)curStageEnum;
        curStage = MusicInStage[curStageInt  ];
         int curMusicInt;
        if (curStage.withIntro)
        {
            if (wasIntro)
            {
                curMusicInt = (int)UnityEngine.Random.Range(1, curStage.clips.Length);
            }
            else
            {
                wasIntro = true;
                curMusicInt = 0;
            }
        }else{
            curMusicInt = (int)UnityEngine.Random.Range(0, curStage.clips.Length);
        }
		curMusic = curStage.clips[curMusicInt];
//        Debug.Log("NOW PLAY" + curMusic); 
        musicPlayer.clip = curMusic;
		musicPlayer.PlayDelayed (musicTimeOut);

	}
	
	public void NextStage(){
        Debug.Log("NEW STAGE " + MusicInStage.Length);
		if (curStageInt <MusicInStage.Length ) {
            SetStage((MUSIC_STAGE)curStageInt);
            
		}
	}
}
