using UnityEngine;
using System.Collections;

public class ClusterModifier : Modifier {

	public static bool clusterBombsActive = false;

	public override void Trigger ()
	{
		base.Trigger ();
		clusterBombsActive = true;
		// fsm state event
		GameObject.Find("_PMModifierManager").GetComponent<PlayMakerFSM>().Fsm.Event("clusterBombsActive");

	}

	public override void Reset ()
	{
		clusterBombsActive = false;
		GameObject.Find("_PMModifierManager").GetComponent<PlayMakerFSM>().Fsm.Event("clusterBombsInActive");
		base.Reset ();

	}
}
