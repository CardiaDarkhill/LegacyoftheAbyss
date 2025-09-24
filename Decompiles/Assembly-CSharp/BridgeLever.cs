using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class BridgeLever : MonoBehaviour
{
	// Token: 0x06000A15 RID: 2581 RVA: 0x0002DB92 File Offset: 0x0002BD92
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0002DBAC File Offset: 0x0002BDAC
	private void Start()
	{
		this.activated = GameManager.instance.playerData.GetBool(this.playerDataBool);
		if (this.activated)
		{
			this.bridgeCollider.enabled = true;
			BridgeSection[] array = this.sections;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Open(this, false);
			}
			this.anim.Play("Lever Activated");
			return;
		}
		this.bridgeCollider.enabled = false;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0002DC24 File Offset: 0x0002BE24
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.activated && collision.tag == "Nail Attack")
		{
			this.activated = true;
			base.StartCoroutine(this.OpenBridge());
		}
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0002DC54 File Offset: 0x0002BE54
	private IEnumerator OpenBridge()
	{
		GameManager.instance.playerData.SetBool(this.playerDataBool, true);
		this.switchSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		GameManager.instance.FreezeMoment(1);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		if (this.strikeNailPrefab)
		{
			this.strikeNailPrefab.Spawn(this.hitOrigin.position);
		}
		this.anim.Play("Lever Hit");
		this.bridgeCollider.enabled = true;
		yield return new WaitForSeconds(0.1f);
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", true);
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER", false);
		BridgeSection[] array = this.sections;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open(this, true);
		}
		this.source.Play();
		yield return new WaitForSeconds(3.25f);
		this.source.Stop();
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", false);
		GameCameras.instance.cameraShakeFSM.SendEvent("StopRumble");
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT", false);
		yield break;
	}

	// Token: 0x040009A4 RID: 2468
	public string playerDataBool = "cityBridge1";

	// Token: 0x040009A5 RID: 2469
	public Collider2D bridgeCollider;

	// Token: 0x040009A6 RID: 2470
	[Space]
	public BridgeSection[] sections;

	// Token: 0x040009A7 RID: 2471
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040009A8 RID: 2472
	public AudioEvent switchSound;

	// Token: 0x040009A9 RID: 2473
	public GameObject strikeNailPrefab;

	// Token: 0x040009AA RID: 2474
	public Transform hitOrigin;

	// Token: 0x040009AB RID: 2475
	private tk2dSpriteAnimator anim;

	// Token: 0x040009AC RID: 2476
	private AudioSource source;

	// Token: 0x040009AD RID: 2477
	private bool activated;
}
