using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AnalyzedObject : MonoBehaviour{
	
	private bool analyzed = false;
	
	public List<AnalyzeEntry> analyzePoint;
	
	public bool Analyze(){
		if(analyzed){
		 return false;
		}
		analyzed = true;
		return true;
	}

}