using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class UpdateDecalOnStart : MonoBehaviour {

 // Use this for initialization
	void OnEnable ()
	{
	  Decal decal = GetComponent<Decal>();
    Debug.Log(decal.gameObject.name);
    BuildDecal(decal);
	}

  private void BuildDecal(Decal decal)
  {
    MeshFilter filter = decal.GetComponent<MeshFilter>();
    if (filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
    if (decal.renderer == null) decal.gameObject.AddComponent<MeshRenderer>();
    decal.renderer.material = decal.material;

    if (decal.material == null || decal.sprite == null)
    {
      filter.mesh = null;
      return;
    }

    var affectedObjects = GetAffectedObjects(decal.GetBounds(), decal.affectedLayers);
    foreach (GameObject go in affectedObjects)
    {
      DecalBuilder2.BuildDecalForObject(decal, go);
    }
    DecalBuilder2.Push(decal.pushDistance);
    
    Mesh mesh = DecalBuilder2.CreateMesh();
    Debug.Log(mesh);
    if (mesh != null)
    {
      mesh.name = "DecalMesh";
      filter.mesh = mesh;
    }
  }

  private static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers)
  {
    MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
    List<GameObject> objects = new List<GameObject>();
    foreach (Renderer r in renderers)
    {
      if (!r.enabled) continue;
      if (!IsLayerContains(affectedLayers, r.gameObject.layer)) continue;
      if (r.GetComponent<Decal>() != null) continue;

      if (bounds.Intersects(r.bounds))
      {
        objects.Add(r.gameObject);
      }
    }
    return objects.ToArray();
  }

  private static bool IsLayerContains(LayerMask mask, int layer)
  {
    return (mask.value & 1 << layer) != 0;
  }

	
	// Update is called once per frame
	void Update () {
	
	}
}
