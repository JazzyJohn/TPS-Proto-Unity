using UnityEngine;
using System.Collections;

public class InstantiateOnFire : MonoBehaviour
{

  public BaseWeapon BaseWeapon;
  public GameObject Effect;
  public float DeactivateTimeDelay = 1;

  private bool isVisible;
  private GameObject effectInstance;
  private ParticleSystem[] particles;

	void Start () {
    BaseWeapon.FireStarted += BaseWeapon_FireStarted;
    BaseWeapon.FireStoped += BaseWeapon_FireStoped;
	  particles = Effect.GetComponentsInChildren<ParticleSystem>();
    Effect.SetActive(false);
	}

  void BaseWeapon_FireStarted(object sender, System.EventArgs e)
  {
    if (DeactivateTimeDelay > 0.01f) {
      CancelInvoke("DeactivateEffect");
      Invoke("DeactivateEffect", DeactivateTimeDelay);
    }
    Effect.SetActive(true);
  }

  void BaseWeapon_FireStoped(object sender, System.EventArgs e)
  {
    foreach (var particle in particles) {
      particle.Stop();
    }
    if (DeactivateTimeDelay > 0.01f) {
      CancelInvoke("DeactivateEffect");
      Invoke("DeactivateEffect", DeactivateTimeDelay);
    }
  }

  void DeactivateEffect()
  {
    Effect.SetActive(false);
  }
}
