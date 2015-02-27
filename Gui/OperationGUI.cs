using UnityEngine;
using System.Collections;

public class OperationGUI : MonoBehaviour {

    public UIWidget operPanel;
    public bool isActive = false;
    public bool isPopulate = false;
    public MainMenuGUI MainMenu;

    public UIPanel mainPanel;


    public Transform winnerPrefab;

    public OneOperationGUI currentOperation;

    public int panelindex;

    public void ShowOperation()
    {
        mainPanel.alpha = 1.0f;
        DrawOperations();
    }
    public void CloseOperation()
    {
        mainPanel.alpha = 0.0f;
    }

    /*public void Next()
    {
        HideAllPanels();
        panelindex++;
        if (panelindex >= panels.Length)
        {
            panelindex = 0;
        }
        panels[panelindex].alpha = 1.0f;
    }
    public void Prev()
    {
        HideAllPanels();
        panelindex--;
     
        if (panelindex <0)
        {
            panelindex = panels.Length-1;
        }
        panels[panelindex].alpha = 1.0f;
    }
    public void HideAllPanels()
    {
        foreach (UIPanel panel in panels)
        {
            panel.alpha = 0.0f;

        }


    }*/
    bool isDrawed = false;
    bool isWinnerDraw = false;
    public void DrawOperations()
    {
        if (!isDrawed)
        {
            isDrawed = true;
            currentOperation.Fill(TournamentManager.instance.currentOperation);
        }
       
    }

    public  void Update()
    {
          if (isDrawed&&!isWinnerDraw)
        {
           
            #if UNITY_EDITOR
            isWinnerDraw = true;
              currentOperation.Winners(TournamentManager.instance.currentOperation);

            #endif
            if(TournamentManager.instance.isLoaded){
                isWinnerDraw = true;
                 currentOperation.Winners(TournamentManager.instance.currentOperation);
            }
           
        }
    }
	
}


[System.Serializable]
public class GUIPrize{
    public UILabel label;
    public UIWidget box;
}
[System.Serializable]
public class OneOperationGUI{
    public OperationGUI main;

    public UILabel[] titles;

    public UILabel text;

    public UILabel goal;

    public UILabel myScore;

    public UILabel myPlace;

    public GUIPrize[] cashPrizes;

    public GUIPrize[] goldPrizes;

    public GUIPrize[] expPrizes;

    public Transform tableTransform;

    public UIGrid table;

    public WinnerGUI[] spawnedWinners;

    public void Fill(Operation oper)
    {
      
        table.Reposition();
        foreach (UILabel title in titles)
        {
            title.text = oper.name;
        }
      
        text.text = oper.desctiption;
        goal.text = TextGenerator.instance.GetSimpleText("Operation_" + oper.counterEvent);
        myScore.text = oper.myCounter.ToString();
        myPlace.text = oper.myPlace.ToString();
        for(int i=0; i<cashPrizes.Length;i++)
        {
            if(i>=oper.cashReward.Length){
                cashPrizes[i].box.alpha = 0.0f;
            }else{
                cashPrizes[i].label.text = TextGenerator.instance.GetMoneyText("oper_cash", oper.cashReward[i]);
            }

        }
        for (int i = 0; i < goldPrizes.Length; i++)
        {
            if (i >= oper.goldReward.Length)
            {
                goldPrizes[i].box.alpha = 0.0f;
            }
            else
            {
                goldPrizes[i].label.text = TextGenerator.instance.GetMoneyText("oper_gold", oper.goldReward[i]);
            }

        }
        for (int i = 0; i < expPrizes.Length; i++)
        {
            if (i >= oper.expReward.Length)
            {
                expPrizes[i].box.alpha = 0.0f;
            }
            else
            {
                expPrizes[i].label.text =  TextGenerator.instance.GetMoneyText("oper_exp",oper.expReward[i]);
            }

        }
    }

    public void Winners(Operation oper)
    {
        int count = Mathf.Min(3, oper.winners.Length);
        int i = 0;
        for (i = 0; i < count; i++)
        {
            Winner winner = oper.winners[i];
            if (winner.user.avatar == null)
            {
                #if! UNITY_EDITOR
                                continue;
                #endif
            }
            WinnerGUI gui = spawnedWinners[i];
            if (gui.avatar != null)
            {
                gui.avatar.mainTexture = winner.user.avatar;
            }
            gui.score.text = winner.score.ToString();
            gui.publicName.text = winner.user.name;

        }
        for(;i<oper.winners.Length;i++){

             Winner winner = oper.winners[i];
            if (winner.user.avatar == null)
            {
                  #if! UNITY_EDITOR
                continue;
                #endif
            }
            Transform newTrans = Object.Instantiate(main.winnerPrefab) as Transform;
            newTrans.parent = tableTransform;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);

            WinnerGUI gui = newTrans.GetComponent<WinnerGUI>();
            if (gui.avatar != null)
            {
                gui.avatar.mainTexture = winner.user.avatar;
            }
            gui.score.text = winner.score.ToString();
            gui.publicName.text = winner.user.name;
            if (gui.num!=null)
            {
                gui.num.text =i.ToString();
            }

        }
        table.Reposition();
    }

}
