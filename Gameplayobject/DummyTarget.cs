using UnityEngine;
using System.Collections;

public class DummyTarget : DamagebleObject {
		public bool Critical;
		public HitCounter hitCounter;

        public ShowOnGuiComponent gui;
		public override void Damage(BaseDamage damage,GameObject killer){
				float dmgflaot =damage.Damage;
				if(Critical){
                    dmgflaot *= Pawn.HEAD_SHOOT_MULTIPLIER;
				}
				//Debug.Log (killer.ToString()+ damage);
				hitCounter.ShootCnt(dmgflaot);
				PlayerMainGui.instance.AddMessage(dmgflaot.ToString("0"),transform.position,PlayerMainGui.MessageType.DMG_TEXT);
	
                
		}

        void Start()
        {
            gui = GetComponent<ShowOnGuiComponent>();
        }

        void Update()
        {
            gui.SetTitle("Dist:" + (Player.localPlayer.GetActivePawn().myTransform.position - transform.position).magnitude);
        }
}