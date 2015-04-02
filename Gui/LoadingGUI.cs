using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingGUI : MonoBehaviour {

	public UIProgressBar LoadingProgress;
	public UILabel LoadingProcent;

    public UIWidget top;

    public Transform winnerPrefab;

    public Transform killerTableTransform;

    public UITable killerTable;

    public Transform aiKillerTableTransform;

    public UITable aiKillerTable;

    public UILabel firstLabel;

    public UILabel secondLablel;

    public UIWidget main;

    public ServerHolder Server;
    void Awake()
    {
        Server = FindObjectOfType<ServerHolder>();
        DontDestroyOnLoad(gameObject);
    }
    void LoadTop(){
        if (top.alpha == 1.0f)
        {
            return;
        }
        top.alpha = 1.0f;
        List<int> indexs = new List<int>();
        List<Top> killers = TournamentManager.instance.GetRandomTops(2,out indexs);
         int i = 0;
        int count = 0;
        while (count < killers[0].winners.Length / 2 && i < killers[0].winners.Length)
        {

            Winner winner = killers[0].winners[i];
            if (winner.user.avatar == null)
            {
                 #if !UNITY_EDITOR
                    i++;
                    continue;
                #endif
            }
           
            Transform newTrans = Instantiate(winnerPrefab) as Transform;
            newTrans.parent = killerTableTransform;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);

            WinnerGUI gui = newTrans.GetComponent<WinnerGUI>();
            gui.avatar.mainTexture = winner.user.avatar;
            gui.score.text = winner.score.ToString();
            gui.publicName.text = winner.user.name;
            i++;
            count++;

        }

         i = 0;
         count = 0;
         while (count < killers[1].winners.Length / 2 && i < killers[1].winners.Length)
        {

            Winner winner = killers[1].winners[i];
            if (winner.user.avatar == null)
            {
                #if !UNITY_EDITOR
                        i++;
                        continue;
                #endif
            }
            
            Transform newTrans = Instantiate(winnerPrefab) as Transform;
            newTrans.parent = aiKillerTableTransform;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);

            WinnerGUI gui = newTrans.GetComponent<WinnerGUI>();
            gui.avatar.mainTexture = winner.user.avatar;
            gui.score.text = winner.score.ToString();
            gui.publicName.text = winner.user.name;
            i++;
            count++;

        }

        firstLabel.text = killers[0].name;
        secondLablel.text = killers[1].name;
        aiKillerTable.Reposition();
        killerTable.Reposition();
    }

	void Update(){
		//TODO:LoadMap
        if (main.alpha == 0)
        {
            return;
        }
        if (TournamentManager.instance.isLoaded)
        {
            LoadTop();
        }
        if (Server.connectingToRoom)
        {
				float percent =Server.LoadProcent();
			LoadingProgress.value = percent/100f;
			LoadingProcent.text = percent.ToString("f0") + "%";
          
		}
	}
		
		
		

}