using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyUpdraftRegion : MonoBehaviour
{
	// Token: 0x06002C91 RID: 11409 RVA: 0x000C3135 File Offset: 0x000C1335
	private void Awake()
	{
		this.collider = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000C3144 File Offset: 0x000C1344
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Transform transform = collision.transform;
		PlayMakerFSM component = collision.GetComponent<PlayMakerFSM>();
		if (!component)
		{
			return;
		}
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		FsmGameObject fsmGameObject = component.FsmVariables.FindFsmGameObject("Top Point");
		FsmGameObject fsmGameObject2 = component.FsmVariables.FindFsmGameObject("Start Point");
		if (fsmGameObject != null)
		{
			gameObject = fsmGameObject.Value;
		}
		if (fsmGameObject2 != null)
		{
			gameObject2 = fsmGameObject2.Value;
		}
		Vector2 vector = this.collider.size / 2f;
		Vector2 position = base.transform.TransformPoint(new Vector2(this.collider.offset.x, this.collider.offset.y + vector.y));
		Vector2 position2 = base.transform.TransformPoint(new Vector2(this.collider.offset.x, this.collider.offset.y - vector.y));
		if (gameObject)
		{
			gameObject.transform.SetPosition2D(position);
		}
		if (gameObject2)
		{
			gameObject2.transform.SetPosition2D(position2);
		}
		FsmBool fsmBool = component.FsmVariables.FindFsmBool("Is In Updraft");
		if (fsmBool != null)
		{
			fsmBool.Value = true;
		}
		FsmGameObject fsmGameObject3 = component.FsmVariables.FindFsmGameObject("Updraft Obj");
		if (fsmGameObject3 != null)
		{
			fsmGameObject3.Value = base.gameObject;
		}
		component.SendEvent("ENTERED UPDRAFT");
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x000C32BC File Offset: 0x000C14BC
	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayMakerFSM component = collision.GetComponent<PlayMakerFSM>();
		if (!component)
		{
			return;
		}
		FsmBool fsmBool = component.FsmVariables.FindFsmBool("Is In Updraft");
		FsmGameObject fsmGameObject = component.FsmVariables.FindFsmGameObject("Updraft Obj");
		if (fsmBool != null && fsmGameObject != null && fsmGameObject.Value == base.gameObject)
		{
			fsmBool.Value = false;
			fsmGameObject.Value = null;
			component.SendEvent("EXITED UPDRAFT");
		}
	}

	// Token: 0x04002E38 RID: 11832
	private BoxCollider2D collider;
}
