using UnityEngine;
using SmartFoxClientAPI;

// Statics for holding the connection to the SFS server end
// Can then be queried from the entire game to get the connection

public static class SmartFox {
	private static SmartFoxClient smartFox;
	public static SmartFoxClient Connection {
		get { return smartFox; }
		set { smartFox = value; }
	}

	public static bool IsInitialized() {
		if ( smartFox != null ) {
			return true;
		}
		return false;
	}
}