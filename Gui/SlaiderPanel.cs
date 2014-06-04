using UnityEngine;
using System.Collections;

public class SlaiderPanel : MonoBehaviour {


	public GameObject oneNewPrefab;
	
	public UIPanel allNewsPanel;
	
	public Transform allNewsPivot;
		
	private int newsCount = 0;
	
	private int curItem= 0;
	
	public float slideTime = 2.0f;
	
	private bool IsSliding;
	
	private float slideTimer =0.0f
	
	public void ShowNews(){
		allNewsPanel.alpha = 1.0f;
	}
	public void HideNews(){
		allNewsPanel.alpha = 0.0f;
	}
	
	
	
	void Update(){
		if(newsCount==0&& NewsManager.instance. getNewsList().Count!=0){
				GenerateNewsBoxes();
		}	
		
		if(newsCount!=0){
			if(IsSliding){
				Vector3 target = new Vector3(curItem, 0, 0);
				allNewsPivot.localPosition = Vector3.Lerp(	allNewsPivot.localPosition, target,Time.deltaTime);
				if(Vecto3.Distance(allNewsPivot.localPosition-target)<0.1f){
					allNewsPivot.localPosition=target;
					IsSliding =false;
				}
				
			}else{
				slideTimer+=Tuime.deltaTime;
				if(slideTimer>slideTime){
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
	}
	void GenerateNewsBoxes(){
		float offset = oneNewPrefab.GetComponent<UITexture>().width;
		List <NewUpdate> news = NewsManager.instance. getNewsList();
		newCount = news.Count;
		for(i=0;i<news.Count;i++){
			NewUpdate oneNew= news[i];
			GameObject  objectnews =Instantiate(oneNewPrefab)as GameObject;
			objectnews.transform.parent = allNewsPivot;
			objectnews.transform.localPosition   = new Vector3(offset*i, 0, 0);
			objectnews.GetComponent<UITexture>().mainTexture = oneNew.img_tex;
			UILabel textData  =objectnews.GetComponentInChildren<UILabel>();
			textData.text  = "["+oneNew.color+"]"+oneNew.title;
			textData.fontSize  = oneNew.fontSize;
			float x = oneNew.title_x*objectnews.GetComponent<UITexture>().width,
					y= oneNew.title_y*objectnews.GetComponent<UITexture>().height,
			textData.localPosition   = new Vector3(x, y, 0);
		}
	
	}
	
	
	
	
	
	
	
	
}