using UnityEngine;
using System.Collections;

public class TwitchChannel : MonoBehaviour {

	public static TwitchChannel instance;
	static bool created;

	public string channelName;


	void Awake(){
		
		if (!created) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			created = true;
		} else {
			Destroy(this.gameObject);
		} 
	}
}
