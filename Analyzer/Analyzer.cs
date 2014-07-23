using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Analyzer : BaseWeapon{

	/// <summary>
    /// Hit logic for 
	protected override void HitEffect(RaycastHit hitInfo){
			AnalyzedObject target =hitInfo.collider.GetComponent<AnalyzedObject>();
			if(target!=null){
				if(target.Analyze()){
					foreach(AnalyzeEntry entry in target.analyzePoint){
                        LoreManager.instance.AddAnalyzePoint(entry.name, entry.point);					
					}
				}
			}
	}
}