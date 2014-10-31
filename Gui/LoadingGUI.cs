using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingGUI : MonoBehaviour {

	public UIProgressBar LoadingProgress;
	public UILabel LoadingProcent;

    public ServerHolder Server;
    void Awake()
    {
        Server = FindObjectOfType<ServerHolder>();
        DontDestroyOnLoad(gameObject);

    }

	void Update(){
		//TODO:LoadMap
        if (Server.connectingToRoom)
        {
				float percent =Server.LoadProcent();
			LoadingProgress.value = percent/100f;
			LoadingProcent.text = percent.ToString("f0") + "%";
		}
	}
		
		
		

}