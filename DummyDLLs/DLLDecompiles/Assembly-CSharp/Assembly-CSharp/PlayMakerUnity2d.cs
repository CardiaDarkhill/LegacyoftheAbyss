using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200003F RID: 63
public class PlayMakerUnity2d : MonoBehaviour
{
	// Token: 0x060001BE RID: 446 RVA: 0x00009305 File Offset: 0x00007505
	public static void RecordLastRaycastHitInfo(Fsm fsm, RaycastHit2D info)
	{
		if (PlayMakerUnity2d.lastRaycastHit2DInfoLUT == null)
		{
			PlayMakerUnity2d.lastRaycastHit2DInfoLUT = new Dictionary<Fsm, RaycastHit2D>();
		}
		PlayMakerUnity2d.lastRaycastHit2DInfoLUT[fsm] = info;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00009324 File Offset: 0x00007524
	public static RaycastHit2D GetLastRaycastHitInfo(Fsm fsm)
	{
		if (PlayMakerUnity2d.lastRaycastHit2DInfoLUT == null)
		{
			PlayMakerUnity2d.lastRaycastHit2DInfoLUT[fsm] = default(RaycastHit2D);
			return PlayMakerUnity2d.lastRaycastHit2DInfoLUT[fsm];
		}
		return PlayMakerUnity2d.lastRaycastHit2DInfoLUT[fsm];
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00009364 File Offset: 0x00007564
	private void Awake()
	{
		PlayMakerUnity2d.fsmProxy = base.GetComponent<PlayMakerFSM>();
		if (PlayMakerUnity2d.fsmProxy == null)
		{
			Debug.LogError("'PlayMaker Unity 2D' is missing.", this);
		}
		PlayMakerUnity2d.goTarget = new FsmOwnerDefault();
		PlayMakerUnity2d.goTarget.GameObject = new FsmGameObject();
		PlayMakerUnity2d.goTarget.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.excludeSelf = false;
		fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
		fsmEventTarget.gameObject = PlayMakerUnity2d.goTarget;
		fsmEventTarget.sendToChildren = false;
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000093F7 File Offset: 0x000075F7
	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000940A File Offset: 0x0000760A
	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		if (PlayMakerUnity2d.lastRaycastHit2DInfoLUT != null)
		{
			PlayMakerUnity2d.lastRaycastHit2DInfoLUT.Clear();
		}
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000941D File Offset: 0x0000761D
	public static bool isAvailable()
	{
		return PlayMakerUnity2d.fsmProxy != null;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000942C File Offset: 0x0000762C
	public static void ForwardEventToGameObject(GameObject target, string eventName)
	{
		PlayMakerUnity2d.goTarget.GameObject.Value = target;
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
		fsmEventTarget.gameObject = PlayMakerUnity2d.goTarget;
		FsmEvent fsmEvent = new FsmEvent(eventName);
		PlayMakerUnity2d.fsmProxy.Fsm.Event(fsmEventTarget, fsmEvent.Name);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x00009480 File Offset: 0x00007680
	public static void ForwardCollisionToCurrentState(GameObject target, PlayMakerUnity2d.Collision2DType type, Collision2D CollisionInfo)
	{
		foreach (PlayMakerFSM playMakerFSM in target.GetComponents<PlayMakerFSM>())
		{
			FsmState fsmState = null;
			foreach (FsmState fsmState2 in playMakerFSM.FsmStates)
			{
				if (fsmState2.Name.Equals(playMakerFSM.ActiveStateName))
				{
					fsmState = fsmState2;
					break;
				}
			}
			if (fsmState != null)
			{
				foreach (IFsmCollider2DStateAction fsmCollider2DStateAction in fsmState.Actions)
				{
					if (type == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D)
					{
						fsmCollider2DStateAction.DoCollisionEnter2D(CollisionInfo);
					}
				}
			}
		}
	}

	// Token: 0x04000186 RID: 390
	private static PlayMakerFSM fsmProxy;

	// Token: 0x04000187 RID: 391
	public static string PlayMakerUnity2dProxyName = "PlayMaker Unity 2D";

	// Token: 0x04000188 RID: 392
	private static FsmOwnerDefault goTarget;

	// Token: 0x04000189 RID: 393
	public static string OnCollisionEnter2DEvent = "COLLISION ENTER 2D";

	// Token: 0x0400018A RID: 394
	public static string OnCollisionExit2DEvent = "COLLISION EXIT 2D";

	// Token: 0x0400018B RID: 395
	public static string OnCollisionStay2DEvent = "COLLISION STAY 2D";

	// Token: 0x0400018C RID: 396
	public static string OnTriggerEnter2DEvent = "TRIGGER ENTER 2D";

	// Token: 0x0400018D RID: 397
	public static string OnTriggerExit2DEvent = "TRIGGER EXIT 2D";

	// Token: 0x0400018E RID: 398
	public static string OnTriggerStay2DEvent = "TRIGGER STAY 2D";

	// Token: 0x0400018F RID: 399
	private static Dictionary<Fsm, RaycastHit2D> lastRaycastHit2DInfoLUT;

	// Token: 0x020013D1 RID: 5073
	public enum Collision2DType
	{
		// Token: 0x040080C4 RID: 32964
		OnCollisionEnter2D,
		// Token: 0x040080C5 RID: 32965
		OnCollisionStay2D,
		// Token: 0x040080C6 RID: 32966
		OnCollisionExit2D
	}

	// Token: 0x020013D2 RID: 5074
	public enum Trigger2DType
	{
		// Token: 0x040080C8 RID: 32968
		OnTriggerEnter2D,
		// Token: 0x040080C9 RID: 32969
		OnTriggerStay2D,
		// Token: 0x040080CA RID: 32970
		OnTriggerExit2D
	}
}
