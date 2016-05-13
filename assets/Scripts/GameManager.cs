
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	public GameObject player1Prefab;
	public GameObject player2Prefab;
	// on join room
	// spawn player over network
	// enable /disable appriorate scripts

	public static GameManager instance;

	public int player1lives = 5;
	public int player2lives = 5;
	public int maxLives = 5;

	public Text player1livesText;
	public Text player2livesText;

	public GameObject gameOverScreen;
	public Text gameWonLbl;

	public Text announcementLbl;

	public GameObject gameOptionScreen;
	public static bool allowGameplay = false;

	void Awake(){

		instance = this;

		if (!PhotonNetwork.connected)
			Application.LoadLevel (0);


	}
	void Start(){

		GameObject.Find("ChannelNameDisplay").GetComponent<Text>().text = TwitchChannel.instance.channelName;


		GameObject spawnedPlayer = null;
		Vector3 spawnPos = Vector3.zero;

		if(PhotonNetwork.player.ID == 1){
			spawnPos = GameObject.Find("Player1ResetPosition").transform.position;
			
			spawnedPlayer = (GameObject)PhotonNetwork.Instantiate(player1Prefab.name, GameObject.Find("Player1ResetPosition").transform.position, Quaternion.identity, 0);
			spawnedPlayer.name = "Player" + PhotonNetwork.player.ID;

			photonView.RPC ("SetChannel", PhotonTargets.AllBuffered,  TwitchChannel.instance.channelName);

		}

		else {
			spawnPos= GameObject.Find("Player2ResetPosition").transform.position;

			spawnedPlayer = (GameObject)PhotonNetwork.Instantiate(player2Prefab.name, GameObject.Find("Player1ResetPosition").transform.position, Quaternion.identity, 0);
			spawnedPlayer.name = "Player" + PhotonNetwork.player.ID;
		}

		spawnedPlayer.transform.position = spawnPos;


		photonView.RPC ("SetPlayerName", PhotonTargets.AllBuffered, spawnedPlayer.GetPhotonView().viewID, spawnedPlayer.name);

		
	}

	[RPC]
	void SetChannel(string name){

		GameObject.Find("ChannelNameDisplay").GetComponent<Text>().text = name;
	}


	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){

			if(gameOptionScreen.activeSelf)
				gameOptionScreen.SetActive(false);
			else {
				gameOptionScreen.SetActive(true);
			}
		}

	}


	[RPC]
	void SetPlayerName(int id, string name){

		
		announcementLbl.enabled = true;

		GameObject player = PhotonView.Find(id).gameObject;
		player.name = name;

		if(!player.GetPhotonView().isMine)
			announcementLbl.text = "New player joined!";

		// spawn players on both sides
		if(PhotonNetwork.room.playerCount == 2){
			GameObject.Find("PlayerCenterPosition").GetComponent<PlayMakerFSM>().SendEvent("SetCamera");
			StartGame();
		}


	}

	void StartGame(){

		StartCoroutine(Countdown());
	}

	IEnumerator Countdown(){


		for (int i = 4; i > 0; i--) {

			if(i == 4)
				announcementLbl.text = "Get ready!";

			else 
				announcementLbl.text = i.ToString() + "!";

			yield return new WaitForSeconds(1);
		}

		announcementLbl.enabled = false;

		allowGameplay = true;
		Camera.main.GetComponent<PlayMakerFSM>().SendEvent("AllowGameplay");

		if(PhotonNetwork.room.playerCount == 2){
			GameObject.Find("Player1").GetComponent<PlayMakerFSM>().SendEvent("AllowGameplay");
			GameObject.Find("Player2").GetComponent<PlayMakerFSM>().SendEvent("AllowGameplay");
			GameObject.Find("Player1").transform.Find("Bazooka").GetComponent<PlayMakerFSM>().SendEvent("AllowGameplay");
			GameObject.Find("Player2").transform.Find("Bazooka").GetComponent<PlayMakerFSM>().SendEvent("AllowGameplay");
		}

	}

	public void PlaySound(AudioClip clip){
		audio.pitch = Random.Range(0.75f, 1f);
		audio.PlayOneShot(clip);

	}

	public void WithdrawLifeLocal(int playerNR){
		photonView.RPC("WithdrawLife", PhotonTargets.All, playerNR);
	}

	[RPC]	
	public void WithdrawLife(int playerNR){
		if(playerNR == 1){
			player1lives--;
			player1livesText.GetComponent<Animator>().SetTrigger("Flash");
			player1livesText.text = "X " + player1lives;
			
			if(player1lives <= 0){
				player1lives = 0;
				ShowGameOver(1);
			}
			
			
		}
		else {
			player2lives--;
			player2livesText.GetComponent<Animator>().SetTrigger("Flash");
			player2livesText.text = "X " + player2lives;
			
			if(player2lives <= 0){
				player2lives = 0;
				ShowGameOver(2);
			}
		}
	}

	public void Resume(){
		gameOptionScreen.SetActive(false);
		//Time.timeScale = 1;
	}

	public void BackToMainMenu(){
		PhotonNetwork.Disconnect();
		Application.LoadLevel(0);
	}

	public void Rematch(){
		photonView.RPC("LoadLevelMaster", PhotonTargets.All);
	}

	[RPC]
	void LoadLevelMaster(){
		PhotonNetwork.LoadLevel(1);
	}

	void ShowGameOver(int id){

		if(gameOverScreen.activeSelf)
			return;

		if(id == 1){
			gameWonLbl.text = "Player 2 wins!";
		}
		else {
			gameWonLbl.text = "Player 1 wins!";
		}

		gameOverScreen.SetActive(true);

		if(PhotonNetwork.room.playerCount < 2)
			gameOverScreen.transform.Find("ButtonRematch").GetComponent<Button>().interactable = false;



	}

	public void SpawnCluster(Vector3 pos){
		GetComponent<Cluster>().SpawnCluster(pos);
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer player){

		if(player.ID == 1)
			ShowGameOver(1);

		else 
			ShowGameOver(2);

		PhotonNetwork.room.open = false;
	}


}
