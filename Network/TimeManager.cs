using UnityEngine;
using System;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;


// Synchronizes client time with the server time
public class TimeManager : MonoBehaviour {
	private readonly float period = 3.0f;
	
	private static TimeManager instance;
	public static TimeManager Instance {
		get {
			return instance;
		}
	}
		
	private float lastRequestTime = float.MaxValue;
	private float timeBeforeSync = 0;
	private bool synchronized = false;
		
	private long lastServerTime = 0;
	private double lastLocalTime = 0;
	
	private bool running = false;
	
	public long averagePing = 0;
	private int pingCount = 0;
	
	private readonly int averagePingCount = 10;
	private long[] pingValues;
	private int pingValueIndex;

	void Awake() {
		instance = this;
        
	}
	
	public void Init() {
		pingValues = new long[averagePingCount];
		pingCount = 0;
		pingValueIndex = 0;
        Debug.Log("timer");
		running = true;
	}
    /// <summary>
    /// Request the current server time. Used for time synchronization
    /// </summary>	
    public void TimeSyncRequest()
    {
        Sfs2X.Entities.Room room = NetworkController.smartFox.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
        NetworkController.smartFox.Send(request);
    }	
	public void Synchronize(long timeValue) {
		// Measure the ping in milliseconds
		long ping =(long)( (Time.time - timeBeforeSync)*1000.0f);
		CalculateAveragePing(ping);
				
		// Take the time passed between server sends response and we get it 
		// as half of the average ping value
		long timePassed = averagePing / 2;
		lastServerTime = timeValue + timePassed;
        lastLocalTime = Time.time;
		
		synchronized = true;	
	}
		
	void Update () {
		if (!running) return;
		
		if (lastRequestTime > period) {
			lastRequestTime = 0;
			timeBeforeSync = Time.time;
			NetworkController.Instance.TimeSyncRequest();
		}
		else {
			lastRequestTime += Time.deltaTime;
		}
	}
	
	/// <summary>
	/// Network time in msecs
	/// </summary>
	public long NetworkTime {
		get {
			// Taking server timestamp + time passed locally since the last server time received			
            return (long)((Time.time - lastLocalTime) * 1000) + lastServerTime;
		}
	}
			
	public long AveragePing {
		get {
			return averagePing;
		}
	}
	
	
	private void CalculateAveragePing(long ping) {
		pingValues[pingValueIndex] = ping;
		pingValueIndex++;
		if (pingValueIndex >= averagePingCount) pingValueIndex = 0;
		if (pingCount < averagePingCount) pingCount++;
					
		long pingSum = 0;
		for (int i=0; i<pingCount; i++) {
			pingSum += pingValues[i];
		}
		
		averagePing = pingSum / pingCount;
	}

		
}
