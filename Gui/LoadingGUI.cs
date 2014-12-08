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
        List<Winner[]> killers = TournamentManager.instance.GetRandomTops(2,out indexs);
         int i = 0;
        int count = 0;
        while (count < killers[0].Length / 2 && i < killers[0].Length)
        {

            Winner winner = killers[0][i];
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
        while (count < killers[1].Length / 2 && i < killers[1].Length)
        {

            Winner winner = killers[1][i];
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

        firstLabel.text = TextGenerator.instance.GetSimpleText("topName" + indexs[0]);
        secondLablel.text = TextGenerator.instance.GetSimpleText("topName" + indexs[1]);
        aiKillerTable.Reposition();
        killerTable.Reposition();
    }

	void Update(){
		//TODO:LoadMap

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