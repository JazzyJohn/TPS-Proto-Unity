using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OneTopGUI : MonoBehaviour {


    public Transform winnerPrefab;

    public Transform tableTransform;

    public UITable table;

    public UILabel title;

    public void Populate(Winner[] top,string name)
    {
        title.text = name;
        
        int i = 0;
        int count = 0;
        while (count<top.Length/2&&i<top.Length)
        {
     
            Winner winner = top[i];
            if (winner.user.avatar == null)
            {
                 #if !UNITY_EDITOR
                    i++;
                    continue;
                #endif
            }
            Transform newTrans = Instantiate(winnerPrefab) as Transform;
            newTrans.parent = tableTransform;
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
        table.Reposition();
    }
}
