using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SlaiderPanel : MonoBehaviour {

	public MainMenuGUI main;
	public GameObject oneNewPrefab;

	public GameObject onewNewButton;

	public UIPanel allNewsPanel;

	public UIWidget newsContainer;
	
	public Transform allNewsPivot;
	
	public Transform allBtnPivot;

    public Transform[] addnews;
		
	private int newsCount = 0;
	
	private int curItem= 0;
	
	public float slideTime = 2.0f;
	
	private bool IsSliding;
	
	private float slideTimer =0.0f;

	private float offset;

    public bool isActive = false;

	private List<UIToggle> newsbtns = new List<UIToggle> ();

	public void ShowNews(){
		allNewsPanel.alpha = 1.0f;
	}
	public void HideNews(){
		allNewsPanel.alpha = 0.0f;
	}
	
	
	
	void Update(){
		if(newsCount==0&& NewsManager.instance. finished){
				GenerateNewsBoxes();
				main.ShowNews();
		}	

		if(newsCount!=0&&isActive){
			if(IsSliding){
				Vector3 target = new Vector3(-curItem*offset, 0, 0);
				allNewsPivot.localPosition = Vector3.Lerp(	allNewsPivot.localPosition, target,Time.deltaTime);
				if((allNewsPivot.localPosition-target).sqrMagnitude<10f){
					allNewsPivot.localPosition=target;
					IsSliding =false;
				}
				
			}else{
				slideTimer+=Time.deltaTime;
				if(slideTimer>slideTime){
					slideTimer=0;
					NextNews();
				}
			}
		}
	}
	public void NextNews(){
		curItem++;
		IsSliding= true;
		if(curItem>=newsCount){
			curItem = 0;
		}
		SetButtons ();
	}
	public void SetNews(int i){
		curItem=i;
		IsSliding= true;
		if(curItem>=newsCount){
			curItem = 0;
		}
		slideTimer=0;
		SetButtons ();
	}
	public void SetButtons(){

		newsbtns [curItem].value = true;
	}
	public void ReSize(){
		offset = oneNewPrefab.GetComponent<UIWidget> ().width;
		//first is shablon

		for (int i=1; i<=newsCount; i++) {

            allNewsPivot.transform.GetChild(i).localPosition = new Vector3(offset * (i- 1), 0, 0);
		}
	}
	void GenerateNewsBoxes(){

		offset = oneNewPrefab.GetComponent<UIWidget> ().width;
		float btnoffset = onewNewButton.GetComponent<UISprite> ().width;
		List<NewUpdate> news = NewsManager.instance. getNewsList();
        newsCount = news.Count + addnews.Length;

        for (int i = 0; i < addnews.Length; i++)
        {
            GameObject objectnews = addnews[i].gameObject;
            objectnews.transform.parent = allNewsPivot;
            objectnews.transform.localScale = new Vector3(1f, 1f, 1f);
            objectnews.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            objectnews.transform.localPosition = new Vector3(offset * i, 0, 0);
         
            GameObject objectbtn = Instantiate(onewNewButton) as GameObject;

            objectbtn.transform.parent = allBtnPivot;
            objectbtn.transform.localScale = new Vector3(1f, 1f, 1f);
            objectbtn.transform.localPosition = new Vector3(btnoffset * (i - newsCount / 2), 0, 0);
            objectbtn.GetComponent<SliderBtn>().Init(i, this);
            newsbtns.Add(objectbtn.GetComponent<UIToggle>());
        }
        for (int i = addnews.Length; i < newsCount; i++)
        {
            NewUpdate oneNew = news[i - addnews.Length];
			GameObject  objectnews =Instantiate(oneNewPrefab)as GameObject;

			objectnews.transform.parent = allNewsPivot;
			objectnews.transform.localScale = new Vector3(1f,1f,1f);
            objectnews.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

			objectnews.transform.localPosition   = new Vector3(offset*i, 0, 0);
			//objectnews.GetComponent<UIWidget>().SetDimensions((int)allNewsPanel.baseClipRegion.w, (int)allNewsPanel.baseClipRegion.z);
			objectnews.GetComponent<UIWidget>().alpha = 1.0f;

			//SERVER DATA SET
			objectnews.GetComponentInChildren<UITexture>().mainTexture = oneNew.img_tex;
			UILabel textData  =objectnews.GetComponentInChildren<UILabel>();
			textData.text  = "["+oneNew.color+"]"+oneNew.title;
			textData.fontSize  = oneNew.fontSize;
			float x = oneNew.title_x*allNewsPanel.baseClipRegion.w/100f,
			y= oneNew.title_y*allNewsPanel.baseClipRegion.z/100f;
			textData.transform.localPosition   = new Vector3(x, y, 0);
			allNewsPanel.AddWidget(objectnews.GetComponent<UIWidget>());
			
			
			
			//Button creation 
			GameObject  objectbtn=Instantiate(onewNewButton)as GameObject;
				
			objectbtn.transform.parent = allBtnPivot;
			objectbtn.transform.localScale = new Vector3(1f,1f,1f);
			objectbtn.transform.localPosition   = new Vector3(btnoffset*(i-newsCount/2), 0, 0);
			objectbtn.GetComponent<SliderBtn>().Init(i,this);
			newsbtns.Add(objectbtn.GetComponent<UIToggle>());
		}
		
		SetButtons ();
	
	}
	
	
	
	
	
	
	
	
}