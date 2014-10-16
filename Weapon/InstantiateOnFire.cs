using System;
using UnityEngine;
using System.Collections;

public class InstantiateOnFire : MonoBehaviour
{
  public BaseWeapon BaseWeapon;
  public GameObject Effect;
  public float DeactivateTimeDelay = 1;
  public bool UpdatePositionWithoutKinematic;
  public float LifeTimeAfterRootDestroy = 5;

  private bool isVisible;
  private GameObject effectInstance;
  private ParticleSystem[] particles;
  private FadeInOutLight[] lights;
  private Transform effectT, parentT;
  private Vector3 offsetPosition;
  private bool isFired;

	void Start () {
    BaseWeapon.FireStarted += BaseWeapon_FireStarted;
    BaseWeapon.FireStoped += BaseWeapon_FireStoped;
	  particles = Effect.GetComponentsInChildren<ParticleSystem>();
	  lights = Effect.GetComponentsInChildren<FadeInOutLight>();
    Effect.SetActive(false);
	  effectT = Effect.transform;
	  parentT = transform.parent.transform;
    if (UpdatePositionWithoutKinematic)
	    Effect.transform.parent = null;
	}

  void BaseWeapon_FireStarted(object sender, System.EventArgs e)
  {
    if (!isFired) isFired = true;
    else return;

    //Debug.Log("             FireStarted");
    if (DeactivateTimeDelay > 0.01f) {
      CancelInvoke("DeactivateEffect");
      Invoke("DeactivateEffect", DeactivateTimeDelay);
    }
    foreach (var particle in particles) {
      particle.Play();
    }
    foreach (var fadeInOutLight in lights) {
      fadeInOutLight.IsVisible = true;
    }
    if(!Effect.activeSelf) Effect.SetActive(true);
  }

  void BaseWeapon_FireStoped(object sender, System.EventArgs e)
  {
    if (isFired) isFired = false;
    else return;

    //Debug.Log("             FireStoped");
    foreach (var particle in particles) {
      particle.Stop();
    }
    foreach (var fadeInOutLight in lights) {
      fadeInOutLight.IsVisible = false;
    }
    if (DeactivateTimeDelay > 0.01f) {
      CancelInvoke("DeactivateEffect");
      Invoke("DeactivateEffect", DeactivateTimeDelay);
    }
  }

  void Update()
  {
    if (UpdatePositionWithoutKinematic) {
      effectT.position = parentT.position;
      effectT.rotation = parentT.rotation;
    }
  }

  void OnDestroy()
  {
    Destroy(Effect, LifeTimeAfterRootDestroy);
  }

  void DeactivateEffect()
  {
    Effect.SetActive(false);
    isFired = false;
  }
}
