using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface  LocalPlayerListener{
	void EventAppear(Player target);
	void EventPawnDeadByPlayer(Player target,KillInfo killinfo);
	void EventPawnDeadByAI(Player target);
    void EventPawnKillPlayer(Player target, KillInfo killinfo);
    void EventPawnKillAI(Player target, KillInfo killinfo);
    void EventJuggerKill(Player target, KillInfo killinfo);
	void EventJuggerTake(Player target);	
	void EventPawnGround(Player target);
	void EventPawnDoubleJump(Player target);
	void EventStartSprintRun(Player target,Vector3 Position);
	void EventEndSprintRun(Player target,Vector3 Position);
	void EventStartWallRun(Player target,Vector3 Position);
	void EventEndWallRun(Player target, Vector3 Position);
	void EventPawnReload(Player target);
	void EventKilledByFriend(Player target,Player friend);
    void EventKilledAFriend(Player target, Player friend, KillInfo killinfo);
} 
public interface  GameListener{
	void EventStart();
	void EventTeamWin(int teamNumber);
	void EventRestart();
	void EventRoomFinished();
} 
public interface HolderBase{
	void Bind(object listener);
	bool isValid(Type type);
	void FireEvent(MethodInfo theMethod,params object[] values);
}
public class ListnerHolder<T>:HolderBase{
	protected List<T> listenerList = new List<T>();
	public void Bind(T listener){
		listenerList.Add(listener);
	}
	public void Bind(object listener){
		Debug.Log (listener);
		listenerList.Add((T)listener);
	}
	public bool isValid(Type type){
		return type.GetInterface(typeof(T).Name)!=null||object.ReferenceEquals(type,typeof(T));
	}
	public void FireEvent(MethodInfo theMethod,params object[] values){

		foreach(T t in listenerList){

			if(t!=null){
				theMethod.Invoke(t, values);
			}
		}
	}

}
public class EventHolder : MonoBehaviour
{
		protected List<HolderBase> list=  null;
		
		public void InitList(){
			list = new List<HolderBase>(); 
			list.Add(new ListnerHolder<LocalPlayerListener>());
			list.Add(new  ListnerHolder<GameListener>());
		}
		public void Bind(object  listener){
			if(list==null){
				InitList();
			}

			foreach(HolderBase holder  in list){
		
				if(holder.isValid(listener. GetType())){
					holder. Bind(listener);
				}
			}
		}
		public void FireEvent(Type type,string methodName,params object[] values){
			if(list==null){
				InitList();
			}

			MethodInfo theMethod = type.GetMethod (methodName);
			if (theMethod == null) {
				
				return;
			}
			foreach(HolderBase holder  in list){
		
				if(holder.isValid(type)){

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

