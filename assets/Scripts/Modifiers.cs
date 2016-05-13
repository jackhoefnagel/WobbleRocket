using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class modifier {

	public string conditionName;
	public Modifier connectedModifier;
	public int amountTriggered;
	public Sprite modifierIcon;
	public string modifierText;
	
}

public class Modifiers : Photon.MonoBehaviour {

	public List<modifier> modifiersList = new List<modifier>();
	public float currentModifierTimer = 0;
	float modifierTimer = 10;

	public float currentCooldown = 0;
	float cooldown = 15;

	modifier currentMod;

	public Text announcementLbl;
	public Image modifierIcon;
	public Image modifierVote1;
	public Image modifierVote2;
	public Image modifierVote3;
	public Text voteTimeLbl;


	public static Modifiers instance;

	bool modifierActive = false;

	void Awake(){

		instance = this;
		currentModifierTimer = modifierTimer;
		currentCooldown = cooldown;

		announcementLbl.enabled = false;
		modifierIcon.enabled = false;

		if(PhotonNetwork.isMasterClient){
			InvokeRepeating("UpdateModifierVotesLocal", 0, 1f);
			InvokeRepeating("RandomVote", 0, 5f);
		}

	}

	void RandomVote(){
		modifiersList[Random.Range(0, modifiersList.Count)].amountTriggered++;
	}

	void Update(){

		if(PhotonNetwork.isMasterClient && GameManager.allowGameplay){


			if(currentCooldown > 0){
				currentCooldown -= Time.deltaTime;

				photonView.RPC ("SendTime", PhotonTargets.All, currentCooldown);

				if(currentCooldown <= 0){
					modifierActive = true;
					photonView.RPC("ToggleModifier", PhotonTargets.All, true);
					CheckWhatModifierToTrigger();
				}
			}

			if(modifierActive && currentModifierTimer > 0){
				currentModifierTimer -= Time.deltaTime;
				if(currentModifierTimer <= 0){
					modifierActive = false;
					currentCooldown = cooldown;

					photonView.RPC("ToggleModifier",PhotonTargets.All, false);
				}
			}
		}

	}

	[RPC]
	void SendTime(float time){

		string seconds = (time % 60).ToString("00");

		voteTimeLbl.text =  "0:" + seconds;
		
	}

	[RPC]
	void ToggleModifier(bool isActive){

		if(isActive){
			announcementLbl.enabled = true;
			modifierIcon.enabled = true;
		}
		else {
			announcementLbl.enabled = false;
			modifierIcon.enabled = false;
		}

	}


	void UpdateModifierVotesLocal(){

		List<modifier> temp = modifiersList;
		
		List<modifier> SortedList = temp.OrderBy(o=>o.amountTriggered).ToList();


		photonView.RPC("UpdateModifierVotes", PhotonTargets.All, SortedList[SortedList.Count-1].conditionName,SortedList[SortedList.Count-2].conditionName,SortedList[SortedList.Count-3].conditionName);


	}

	[RPC]
	void UpdateModifierVotes(string vote1, string vote2, string vote3){

		foreach (modifier mod in modifiersList) {
			if(mod.conditionName == vote1){
				modifierVote1.sprite = mod.modifierIcon;
			}
			if(mod.conditionName == vote2){
				modifierVote2.sprite = mod.modifierIcon;
			}
			if(mod.conditionName == vote3){
				modifierVote3.sprite = mod.modifierIcon;
			}
		}

	}


	public void CheckIfModifierTrigger(string msg){

		foreach (modifier mod in modifiersList) {
			
			if(msg == mod.conditionName){
				mod.amountTriggered++;
				Debug.Log("Modifier: " + mod.conditionName + ": " + mod.amountTriggered);
			}
		}

	}

	void CheckWhatModifierToTrigger(){

		modifier highestMod = new modifier();
		highestMod.amountTriggered = 0;


		foreach (modifier mod in modifiersList) {
			if(mod.amountTriggered > highestMod.amountTriggered)
				highestMod = mod;
		}

		// if no votes trigger random
		if(highestMod.amountTriggered == 0){
			TriggerModifierMaster(modifiersList[Random.Range(0, modifiersList.Count)]);
			Debug.Log("triggering random mod");
		}

		// trigger voted modifer
		else {
			TriggerModifierMaster(highestMod);
			Debug.Log("triggering voted mod");
		}

	}

	void TriggerModifierMaster(modifier mod){

		photonView.RPC ("TriggerModifier", PhotonTargets.All, mod.conditionName);

	}

	[RPC]
	void TriggerModifier(string modName){

		modifier mod = null;
		Debug.Log(modName);

		//search correct mod by name
		foreach (modifier _mod in modifiersList) {
			if(modName == _mod.conditionName)
				mod = _mod;
		}

		if(currentMod != null){
			currentMod.connectedModifier.Reset ();
		}

		currentMod = mod;

		// TRIGGER MOD
		Debug.Log("Trigger: " + mod.conditionName);
		mod.connectedModifier.Trigger();

		modifierIcon.sprite = currentMod.modifierIcon;
		announcementLbl.text = mod.modifierText;

		//reset all data
		foreach (modifier _mod in modifiersList) {
			_mod.amountTriggered =0;
		}
		currentModifierTimer = modifierTimer;

	}


}
