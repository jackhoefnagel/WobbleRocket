using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Connect : Photon.MonoBehaviour {

	public GameObject noRoomFound;

	public Text channelInput;

	private void Awake()
	{



		// Connect to the main photon server.
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
		
		
		//Load our name from PlayerPrefs
		PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			JoinGame();
		}
	}

	public void JoinGame(){


		PhotonNetwork.JoinRandomRoom();
	}


	public void CreateGame(){

		TwitchChannel.instance.channelName = channelInput.text;

		CreateNewRoom();
	}

	
	void CreateNewRoom(){

		PhotonNetwork.CreateRoom(null, true, true, 2);
		
	}

	public void DisableRoom(){
		noRoomFound.SetActive(false);
	}

	void OnPhotonRandomJoinFailed()
	{
		noRoomFound.SetActive(true);
	}

	private void OnJoinedRoom()
	{
		StartCoroutine(MoveToGameScene());
	}

	private IEnumerator MoveToGameScene()
	{
		//Wait for the 
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
	
		PhotonNetwork.LoadLevel(1);
		
		
	}
}
