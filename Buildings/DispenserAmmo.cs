using UnityEngine;
using System.Collections;

public class DispenserAmmo : Building
{
    public float ammoRate;
	// Use this for initialization
	 
    void OnTriggerStay(Collider other)
    {
      
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn!=null&&pawn.foxView.isMine&&!pawn.isAi)
        {
            
            pawn.AddAmmo(ammoRate * Time.fixedDeltaTime);
        }
    }
}
