using UnityEngine;
using System.Collections;
using System.Collections.Generic;




class PathCache{
	public  Queue<KeyValuePair<Node, Node>> pathQueue;
	
	public  Dictionary<Node,Dictionary<Node,List<Vector3> > > allPathDictionary   = new Dictionary<Node,Dictionary<Node,List<Vector3> > >();

	public static int maxPath = 100;
	
	public void AddPath(Node startNode,Node endNode,List<Vector3>  path){
		CleanOld();
		if(!allPathDictionary.ContainsKey(startNode)){
			allPathDictionary[startNode] = new Dictionary<Node,List<Vector3> >();
		}
		allPathDictionary[startNode][endNode] = path;
		pathQueue(new KeyValuePair<Node, Node>(startNode,endNode));
	}

	public bool HasPath(Node startNode,Node endNode){
		return allPathDictionary.ContainsKey(startNode)&&allPathDictionary[startNode].ContainsKey(endNode);
	}
	public List<Vector3> GetPath(){
		if( allPathDictionary.ContainsKey(startNode)&&allPathDictionary[startNode].ContainsKey(endNode)){
			return allPathDictionary[startNode][endNode];
		}else{
			return new List<Vector3> ();
		}
	}
	public void CleanOld(){
		while(pathQueue.Count>maxPath){
			KeyValuePair<Node, Node> path = pathQueue.Dequeue();
			
			allPathDictionary[path.Key].Remove(path.Value);
		}
	
	}
}