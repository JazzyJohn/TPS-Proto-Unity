using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(UIPopupList))]
public class SetConditionOnSelection : MonoBehaviour {

	UIPopupList mPopup;
	
	public void SetSpriteBySelection ()
	{
		if (UIPopupList.current == null) return;
		if (mPopup == null) mPopup = GetComponent<UIPopupList>();
		
		switch (UIPopupList.current.value) 
		{
		case "Время":	
			mPopup.items.Clear();
			mPopup.items.Add("истекло");
			mPopup.value="истекло";
		break;
		case "Приказ":
			mPopup.items.Clear();
			mPopup.items.Add("получен");
			mPopup.value="получен";
			break;
		case "Урон":
		case "Здоровье":
		case "Боеприпасы":
			mPopup.items.Clear();
			mPopup.items.Add("больше");
			mPopup.items.Add("равен");
			mPopup.items.Add("меньше");
			mPopup.value="равен";
			break;
		case "Оружие":
			mPopup.items.Clear();
			mPopup.items.Add("вышло из строя");
			mPopup.items.Add("починилось");
			mPopup.value="вышло из строя";
			break;
		}

			
				
				
				
				
	}
}
