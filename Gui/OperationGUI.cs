using UnityEngine;
using System.Collections;

public class OperationGUI : MonoBehaviour {

    public UIPanel operPanel;
    public bool isActive = false;
    public bool isPopulate = false;
    public MainMenuGUI MainMenu;

    public UIPanel[] panels;


    public Transform winnerPrefab;

    public OneOperationGUI currentOperation;

    public int panelindex;

    public void ShowTop()
    {
     
        if (operPanel.alpha > 0f)
        {
            MainMenu.HideAllPanel();
            isActive = false;
            MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
        }
        else
        {
            HideAllPanels();
            panels[0].alpha = 1.0f;
            panelindex = 0;
            MainMenu.HideAllPanel();
            isActive = true;
            operPanel.alpha = 1f;
            DrawOperations();

        }

    }
    public void ShowOperHelp()
    {

        if (operPanel.alpha > 0f)
        {
            MainMenu.HideAllPanel();
            isActive = false;
            MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
        }
        else
        {
            HideAllPanels();
            panels[panels.Length-1].alpha = 1.0f;
            panelindex = panels.Length - 1;
            MainMenu.HideAllPanel();
            isActive = true;
            operPanel.alpha = 1f;
            DrawOperations();

        }

    }

    public void Next()
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


    }
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
public class OneOperationGUI{
    public OperationGUI main;

    public UILabel[] titles;

    public UILabel text;

    public UILabel goal;

    public UILabel myScore;

    public UILabel[] cashPrizes;

    public UILabel[] goldPrizes;

    public Transform tableTransform;

    public UITable table;

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
        for(int i=0; i<cashPrizes.Length;i++)
        {
            if(i>=oper.cashReward.Length){
                cashPrizes[i].text= "0";
            }else{
                cashPrizes[i].text = oper.cashReward[i].ToString();
            }

        }
        for (int i = 0; i < goldPrizes.Length; i++)
        {
            if (i >= oper.goldReward.Length)
            {
                goldPrizes[i].text = "0";
            }
            else
            {
                goldPrizes[i].text = oper.goldReward[i].ToString();
            }

        }
    }

    public void Winners(Operation oper)
    {
        int count = Mathf.Min(3, oper.winners.Length);
        for (int i = 0; i < count; i++)
        {
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
            gui.avatar.mainTexture = winner.user.avatar;
            gui.score.text = winner.score.ToString();
            gui.publicName.text = winner.user.name;

        }
        table.Reposition();
    }

}
