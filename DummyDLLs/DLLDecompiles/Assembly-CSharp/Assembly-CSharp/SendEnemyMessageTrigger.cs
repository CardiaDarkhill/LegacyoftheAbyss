using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class SendEnemyMessageTrigger : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x06001BE4 RID: 7140 RVA: 0x00081F70 File Offset: 0x00080170
	private void Awake()
	{
		this.OnSceneLintUpgrade(true);
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00081F7C File Offset: 0x0008017C
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "enemy_message");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "enemy_message FSM needs upgrading to SendEnemyMessageTrigger script";
		}
		FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Event");
		this.eventName = fsmString.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "enemy_message FSM was upgraded to SendEnemyMessageTrigger script";
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x00081FD5 File Offset: 0x000801D5
	private void FixedUpdate()
	{
		this.enteredEnemies.Clear();
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x00081FE4 File Offset: 0x000801E4
	private void OnTriggerStay2D(Collider2D collision)
	{
		Rigidbody2D attachedRigidbody = collision.attachedRigidbody;
		GameObject gameObject = collision.gameObject;
		GameObject gameObject2 = attachedRigidbody ? attachedRigidbody.gameObject : gameObject;
		if (this.enteredEnemies.Contains(gameObject2))
		{
			return;
		}
		this.enteredEnemies.Add(gameObject2);
		this.SendEvent(gameObject);
		GameObject gameObject3 = gameObject.transform.root.gameObject;
		if (gameObject3 != null && gameObject != gameObject3)
		{
			this.SendEvent(gameObject3);
		}
		if (gameObject2 != gameObject)
		{
			this.SendEvent(gameObject2);
		}
		IEnemyMessageReceiver component = gameObject2.GetComponent<IEnemyMessageReceiver>();
		if (component != null)
		{
			component.ReceiveEnemyMessage(this.eventName);
		}
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x00082088 File Offset: 0x00080288
	private void SendEvent(GameObject obj)
	{
		if (string.IsNullOrEmpty(this.eventName))
		{
			return;
		}
		FSMUtility.SendEventToGameObject(obj, this.eventName, false);
		if (string.IsNullOrEmpty(this.eventName))
		{
			return;
		}
		string a = this.eventName;
		if (a == "GO LEFT")
		{
			SendEnemyMessageTrigger.SendWalkerGoInDirection(obj, -1);
			return;
		}
		if (!(a == "GO RIGHT"))
		{
			return;
		}
		SendEnemyMessageTrigger.SendWalkerGoInDirection(obj, 1);
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000820F0 File Offset: 0x000802F0
	private static void SendWalkerGoInDirection(GameObject target, int facing)
	{
		Walker component = target.GetComponent<Walker>();
		if (component)
		{
			component.RecieveGoMessage(facing);
		}
		WalkerV2 component2 = target.GetComponent<WalkerV2>();
		if (component2)
		{
			component2.ForceDirection((float)facing);
		}
	}

	// Token: 0x04001AE7 RID: 6887
	[UnityEngine.Tooltip("If there is an enemy_message FSM on this gameobject, this value will be gotten from it.")]
	public string eventName = "";

	// Token: 0x04001AE8 RID: 6888
	private readonly List<GameObject> enteredEnemies = new List<GameObject>();
}
