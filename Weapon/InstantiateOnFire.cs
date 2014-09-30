using UnityEngine;
using System.Collections;

public class InstantiateOnFire : MonoBehaviour
{

  public BaseWeapon BaseWeapon;
  public GameObject Effect;

  private GameObject effectInstance;
	// Use this for initialization
	void Start () {
    BaseWeapon.IsFired += BaseWeapon_IsFired;
    Effect.SetActive(false);
	}

  void BaseWeapon_IsFired(object sender, System.EventArgs e)
  {
    Effect.SetActive(true);
  }
}
