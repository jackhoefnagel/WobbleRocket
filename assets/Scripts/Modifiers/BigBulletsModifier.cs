using UnityEngine;
using System.Collections;

public class BigBulletsModifier : Modifier {

	
	bool bigBulletsActive = false;
	
	public override void Trigger ()
	{
		base.Trigger ();
		bigBulletsActive = true;
		// fsm state event
		GameObject.Find("_PMModifierManager").GetComponent<PlayMakerFSM>().Fsm.Event("bigBulletsActive");
		
	}
	
	public override void Reset ()
	{
		bigBulletsActive = false;
		GameObject.Find("_PMModifierManager").GetComponent<PlayMakerFSM>().Fsm.Event("bigBulletsInActive");
		base.Reset ();
		
	}
}
