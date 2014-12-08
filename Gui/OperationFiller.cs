using UnityEngine;
using System.Collections;

public class OperationFiller : MonoBehaviour {

    public bool loaded= false;

    public bool data = false;

    public Transform winnerPrefab;

    public Transform currentTableTransform;

    public UITable currentTable;

    public Transform lastTableTransform;

    public UITable lastTable;

    public UILabel currentOperationName;

    public UILabel lastOperartionName;

    public UIWidget lastOperation;
   
    void LoadTop()
    {
        if (loaded)
        {
            return;
        }
        loaded =true;
        if (TournamentManager.instance.currentOperation != null && currentTable!=null)
        {
            int count = Mathf.Min(3, TournamentManager.instance.currentOperation.winners.Length );
            for (int i = 0; i < count; i++)
            {
                Winner winner = TournamentManager.instance.currentOperation.winners[i];
                if (winner.user.avatar == null)
                {
#if! UNITY_EDITOR
                continue;
#endif
                }
                Transform newTrans = Instantiate(winnerPrefab) as Transform;
                newTrans.parent = currentTableTransform;
                newTrans.localScale = new Vector3(1f, 1f, 1f);
                newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
                newTrans.localPosition = new Vector3(0f, 0f, 0f);

                WinnerGUI gui = newTrans.GetComponent<WinnerGUI>();
                gui.avatar.mainTexture = winner.user.avatar;
                gui.score.text = winner.score.ToString();
                gui.publicName.text = winner.user.name;

            }
        

        }
        if (lastTable != null)
        {
            Debug.Log(TournamentManager.instance.lastOperation);
            if (TournamentManager.instance.lastOperation != null)
            {

                int count = Mathf.Min(3,TournamentManager.instance.lastOperation.winners.Length);
                for (int i = 0; i < count; i++)
                {
                    Winner winner = TournamentManager.instance.lastOperation.winners[i];
                    if (winner.user.avatar == null)
                    {
#if! UNITY_EDITOR
                continue;
#endif
                    }
                    Transform newTrans = Instantiate(winnerPrefab) as Transform;
                    newTrans.parent = lastTableTransform;
                    newTrans.localScale = new Vector3(1f, 1f, 1f);
                    newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
                    newTrans.localPosition = new Vector3(0f, 0f, 0f);

                    WinnerGUI gui = newTrans.GetComponent<WinnerGUI>();
                    gui.avatar.mainTexture = winner.user.avatar;
                    gui.score.text = winner.score.ToString();
                    gui.publicName.text = winner.user.name;

                }
                lastOperartionName.text = TournamentManager.instance.lastOperation.name;
            }
            else
            {
                lastOperation.alpha = 0.0f;
            }
        }

        currentTable.Reposition();
        lastTable.Reposition();
    }

    void Update()
    {
        //TODO:LoadMap
        if (TournamentManager.instance.dataLoaded)
        {
            LoadData();
            #if UNITY_EDITOR
                LoadTop();

            #endif
        }
        if (TournamentManager.instance.isLoaded)
        {
            LoadTop();
        }
       
    }
    void LoadData()
    {

        if (data)
        {
            return;
        }
        data = true;
        if (TournamentManager.instance.currentOperation != null && currentTable != null)
        {

            currentOperationName.text = TournamentManager.instance.currentOperation.name;
        }
        if (lastTable != null)
        {
       
            if (TournamentManager.instance.lastOperation != null)
            {
               
                lastOperartionName.text = TournamentManager.instance.lastOperation.name;
            }
            else
            {
                lastOperation.alpha = 0.0f;
            }
        }
    }
}
