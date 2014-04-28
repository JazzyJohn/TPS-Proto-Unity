using UnityEngine;
using System.Collections;

public class DummyTarget : DamagebleObject {
		public bool Critical;
		public HitCounter hitCounter;
		public override void Damage(BaseDamage damage,GameObject killer){
				float dmgflaot =damage.Damage;
				if(Critical){
					dmgflaot*=2;
				}
				hitCounter.ShootCnt(killer);
				PlayerMainGui.instance.AddMessage(dmgflaot.ToString(),transform.position,PlayerMainGui.MessageType.DMG_TEXT);
	

		}	
}