using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DecalOnCollision : MonoBehaviour
{

  public GameObject[] Decals;
  public int ParticlesCollisionCount = 30;
  public AnimationCurve ScaleOverLifeTime;
  public float ScaleMinTime = 1;
  public float ScaleMaxTime = 2;

  private ParticleSystem.CollisionEvent[] collisionEvents;
  private List<GameObject> decalsInstances;
  private List<Transform> decalsInstancesTransforms;
  private int usedInstances;
  private List<float> decalsStartTime;
  private List<float> randomScaleTime; 

	// Use this for initialization
	void Start ()
	{
	  var go = new GameObject("Decals");
	  collisionEvents = new ParticleSystem.CollisionEvent[ParticlesCollisionCount];
    decalsInstances = new List<GameObject>(ParticlesCollisionCount);
    decalsInstancesTransforms = new List<Transform>(ParticlesCollisionCount);
    decalsStartTime = new List<float>(ParticlesCollisionCount);
    randomScaleTime = new List<float>(ParticlesCollisionCount); 
	  for (int i = 0; i < ParticlesCollisionCount; i++) {
	    var instance = Instantiate(Decals[Random.Range(0, Decals.Length)]) as GameObject;
      instance.transform.Rotate(0, Random.Range(0, 360), 0);
      instance.isStatic = true;
      instance.transform.parent = go.transform;
	    var scale = ScaleOverLifeTime.Evaluate(0);
      instance.transform.localScale = new Vector3(scale, scale, scale);
      instance.SetActive(false);
      decalsInstances.Add(instance);
      decalsInstancesTransforms.Add(instance.transform);
      randomScaleTime.Add(Random.Range(ScaleMinTime*1000, ScaleMaxTime*1000)/1000f);
	  }
	}

  void OnEnable()
  {
    usedInstances = 0;
    if (decalsStartTime != null)
    {
        decalsStartTime.Clear();
    }
    for (int i = 0; i < ParticlesCollisionCount; i++)
    {
      //instance.transform.Rotate(0, Random.Range(0, 360), 0);
      decalsInstances[i].transform.parent = transform;
      var scale = ScaleOverLifeTime.Evaluate(0);
      decalsInstances[i].transform.localScale = new Vector3(scale, scale, scale);
      decalsInstances[i].SetActive(false);
    }
  }

  void OnParticleCollision(GameObject other)
  {
    int safeLength = particleSystem.safeCollisionEventSize;
    if (collisionEvents.Length < safeLength)
      Debug.Log("Need to fix the particles count of effect");

    int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents); 
    for (int i = 0; i < numCollisionEvents; i++) {
      if (usedInstances + i >= ParticlesCollisionCount)
        return;
      decalsInstancesTransforms[usedInstances].position = collisionEvents[i].intersection;
      decalsInstancesTransforms[usedInstances].rotation = Quaternion.FromToRotation(Vector3.back, collisionEvents[i].normal);
      decalsInstances[usedInstances].SetActive(true);
      decalsStartTime.Add(Time.time);
      ++usedInstances;
    }
  }

  void Update()
  {
    if (ScaleOverLifeTime.length==0)
      return;
    for (int i = 0; i < usedInstances; i++) {
      var scale = ScaleOverLifeTime.Evaluate((Time.time - decalsStartTime[i])/ randomScaleTime[i]) ;
      decalsInstancesTransforms[i].localScale = new Vector3(scale, scale, scale);
    }
  }
}
