using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class TerainWalkPathGenerator : MonoBehaviour
{
    public bool reCalc;
    public float stepSize;
    public int h;
    public int w;
    [System.Serializable]
    public struct OneSplain
    {
        public Vector3 pos_0_0;
        public Vector3 pos_1_0;
        public Vector3 pos_0_1;
        public Vector3 pos_1_1;
    }
   public  List<OneSplain> splains = new List<OneSplain>();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
         #if UNITY_EDITOR
        if (reCalc)
        {
            Terrain ter = Terrain.activeTerrain;
            Vector3 startPosition = transform.position;
            splains.Clear();
            reCalc = false;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    OneSplain splain = new OneSplain();
                    splain.pos_0_0 = (Vector3.right * j + Vector3.forward * i) * stepSize;
                    splain.pos_0_0.y = ter.SampleHeight(startPosition + splain.pos_0_0) + ter.transform.position.y - startPosition.y;

                    splain.pos_1_0 =  (Vector3.right * (1 + j) + Vector3.forward * i) * stepSize;
                    splain.pos_1_0.y = ter.SampleHeight(startPosition + splain.pos_1_0) + ter.transform.position.y - startPosition.y;

                    splain.pos_1_1 = (Vector3.right * (1 + j) + Vector3.forward * (1 + i)) * stepSize;
                    splain.pos_1_1.y = ter.SampleHeight(startPosition + splain.pos_1_1) + ter.transform.position.y - startPosition.y;

                    splain.pos_0_1 = (Vector3.right * j + Vector3.forward * (1 + i)) * stepSize;
                    splain.pos_0_1.y = ter.SampleHeight(startPosition + splain.pos_0_1) + ter.transform.position.y - startPosition.y;
                    splains.Add(splain);
                }
            }
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = mesh;
            }
           
            mesh.Clear();
            Vector3[] vertices = new Vector3[splains.Count * 4];
            int[] triangles = new int[splains.Count * 6];
            Vector2[] uv = new Vector2[mesh.vertices.Length];
            int cnt = 0,trisCnt=0;
            foreach (OneSplain splain in splains)
            {
                // Debug.Log(splain.pos_0_0);
                vertices[cnt]= splain.pos_0_0;
              
                vertices[cnt+1] = splain.pos_0_1;
               
                vertices[cnt+2] = splain.pos_1_1;
               
                vertices[cnt+3] = splain.pos_1_0;

                triangles[trisCnt] = cnt;
                trisCnt++;
                triangles[trisCnt] = cnt + 1;
                trisCnt++;
                triangles[trisCnt] = cnt + 2;
                trisCnt++;
                triangles[trisCnt] = cnt;
                trisCnt++;
                triangles[trisCnt] = cnt + 2;
                trisCnt++;
                triangles[trisCnt] = cnt + 3;
                trisCnt++;
                cnt += 4;
            }
            int k = 0;
            while (k < uv.Length)
            {
                uv[k] = new Vector2(vertices[k].x, vertices[k].z);
                k++;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
          
        }

                    #endif

    }
    public Material lineMaterial;

    public void OnRenderObject()
    {
       
        // set the current material
        lineMaterial.SetPass(0);
       /* GL.Begin(GL.QUADS);
        GL.Color(new Color(1, 1, 1, 1));
        foreach(OneSplain splain in splains)
        {
           // Debug.Log(splain.pos_0_0);
            GL.Vertex(splain.pos_0_0);
            GL.Vertex(splain.pos_0_1);
            GL.Vertex(splain.pos_1_1);
            GL.Vertex(splain.pos_1_0);
           
        }

     
        GL.End();*/
    }
}