using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface  LocalPlayerListener{
	void EventAppear(Player target);
	void EventPawnDeadByPlayer(Player target);
	void EventPawnDeadByAI(Player target);
	void EventPawnKillPlayer(Player target);
	void EventPawnKillAi(Player target);
	void EventPawnGround(Player target);
	void EventPawnDoubleJump(Player target);
	void EventStartWallRun(Player target,Vecto3 Position);
	void EventEndWallRun(Player target, Vector3 Position);
	void EventPawnReload(Player target);
} 
public interface  GameListener{
	void EventStart();
	void EventTeamWin(int teamNumber);
	void EventRestart();
} 
public interface HolderBase{
	void Bind(object listener);
	bool isValid(Type type);
	void FireEvent(MethodInfo theMethod,params object[] values);
}
public class ListnerHolder<T>:HolderBase{
	protected List<T> listenerList = new List<T>();
	public void Bind(T listener){
		listenerlist.Add(listener);
	}
	public void Bind(object listener){
		listenerlist.Add((T)listener);
	}
	public bool isValid(Type type){
		type.IsSubclassOf(typeof(T));
	}
	public void FireEvent(MethodInfo theMethod,params object[] values){
		foreach(T t in listenerList){
			theMethod.Invoke(t, values);
		}
	}

}
public class EventHolder : MonoBehaviour
{
		protected List<HolderBase> list=  null;
		public void  Awake(){
				InitList();
		}
		public void InitList(){
			list = new List<HolderBase>(); 
			list.add(ListnerHolder<LocalPlayerListener>());
			list.add(ListnerHolder<GameListener>());
		}
		public void Bind(object  listener){
			if(list==null){
				InitList();
			}
			foreach(HolderBase holder  in list){
				if(holder.isValid(listener. GetType()){
					holder. Bind(listener);
				}
			}
		}
		public void FireEvent(Type type,MethodInfo theMethod,params object[] values){
			if(list==null){
				InitList();
			}
			foreach(HolderBase holder  in list){
				if(holder.isValid(type){
					holder. FireEvent(theMethod,values);
				}
			}
		}
		
		private static EventHolder s_Instance = null;
	 
 	    public static EventHolder instance {
			get {
				if (s_Instance == null) {
					// This is where the magic happens.
					//  FindObjectOfType(...) returns the first AManager object in the scene.
					s_Instance =  FindObjectOfType(typeof (EventHolder)) as EventHolder;
				}
	 
				// If it is still null, create a new instance
				if (s_Instance == null) {
					GameObject obj = new GameObject("EventHolder");
					s_Instance = obj.AddComponent(typeof (EventHolder)) as EventHolder;
				   
				}
	 
				return s_Instance;
			}
		}

}

