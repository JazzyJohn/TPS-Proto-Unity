using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TopGUI : MonoBehaviour {

    public UIPanel topPanel;
    public bool isActive = false;
    public bool isPopulate = false;
    public MainMenuGUI MainMenu;

    public Transform topPrefab;

    public Transform tableTransform;

    public UITable table;

    public UIScrollView scrollview;

    public UILabel loading;

    public void ShowTop()
    {

        if (topPanel.alpha > 0f)
        {
            MainMenu.HideAllPanel();
            isActive = false;
            MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
        }
        else
        {
            MainMenu.HideAllPanel();
            topPanel.alpha = 1f;
            isActive = true;
           
         
        }

    }
    public void Update()
    {
        if (!isPopulate && isActive )
        {
               #if UNITY_EDITOR
                 Populate();

                #endif
            if (TournamentManager.instance.isLoaded)
            {
                Populate();
            }
        }
    }

  
    public void Populate()
    {
        isPopulate = true;
        List<Top> tops  =  TournamentManager.instance.GetAllTops();
      
        for (int i = 0; i < tops.Count;i++ )
        {
            Transform newTrans = Instantiate(topPrefab) as Transform;
            newTrans.parent = tableTransform;
            newTrans.localScale = new Vector3(1f, 1f, 1f);
            newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
            newTrans.localPosition = new Vector3(0f, 0f, 0f);

            OneTopGUI gui = newTrans.GetComponent<OneTopGUI>();
            gui.Populate(tops[i].winners, tops[i].name);
        }
        table.Reposition();
        scrollview.ResetPosition();
        loading.alpha = 0.0f;
    }
}
