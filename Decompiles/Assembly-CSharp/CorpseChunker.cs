using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class CorpseChunker : Corpse
{
	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001861 RID: 6241 RVA: 0x000702B9 File Offset: 0x0006E4B9
	protected override bool DoLandEffectsInstantly
	{
		get
		{
			return this.instantChunker;
		}
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000702C4 File Offset: 0x0006E4C4
	protected override void LandEffects()
	{
		base.LandEffects();
		if (this.body)
		{
			this.body.linearVelocity = Vector2.zero;
		}
		this.splatAudioClipTable.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, false, 1f, null);
		BloodSpawner.SpawnBlood(base.transform.position, 30, 30, 5f, 30f, 60f, 120f, null, 0f);
		GameCameras instance = GameCameras.instance;
		if (instance)
		{
			instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if (this.effects)
		{
			this.effects.SetActive(true);
		}
		if (this.chunks)
		{
			this.chunks.SetActive(true);
			this.chunks.transform.SetParent(null, true);
			FlingUtils.FlingChildren(new FlingUtils.ChildrenConfig
			{
				Parent = this.chunks,
				SpeedMin = 15f,
				SpeedMax = 20f,
				AngleMin = 60f,
				AngleMax = 120f,
				OriginVariationX = 0f,
				OriginVariationY = 0f
			}, base.transform, Vector3.zero, new MinMaxFloat?(new MinMaxFloat(0f, 0.001f)));
		}
		if (this.meshRenderer && !this.keepMeshRendererActive)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x04001768 RID: 5992
	[Header("Chunker Variables")]
	[SerializeField]
	private bool instantChunker;

	// Token: 0x04001769 RID: 5993
	[Space]
	[SerializeField]
	private GameObject effects;

	// Token: 0x0400176A RID: 5994
	[SerializeField]
	private GameObject chunks;

	// Token: 0x0400176B RID: 5995
	[SerializeField]
	private bool keepMeshRendererActive;
}
