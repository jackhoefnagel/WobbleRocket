using UnityEngine;
using System.Collections;

public class ProjectileExplodeScript : MonoBehaviour {

	private PlayMakerFSM fsm;

	// Use this for initialization
	void Start () {
		fsm = GetComponent<PlayMakerFSM>();
	}

	public GameObject GetPlayersHitByProjectile()
	{
		GameObject GO = new GameObject();
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position,15);
		foreach(Collider coll in colliders)
		{
			if(coll.tag == "Player")
			{
				GO = coll.gameObject;
			}
			else
			{
				GO = null;
			}
		}

		return GO;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
