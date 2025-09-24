using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020000FE RID: 254
public abstract class PlayAudioEventBase : FsmStateAction
{
	// Token: 0x060007FD RID: 2045 RVA: 0x00026148 File Offset: 0x00024348
	public override void Reset()
	{
		this.pitchMin = 1f;
		this.pitchMax = 1f;
		this.volume = 1f;
		this.audioPlayerPrefab = new FsmObject
		{
			UseVariable = true
		};
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00026198 File Offset: 0x00024398
	public override void OnEnter()
	{
		Vector3 vector = this.spawnPosition.Value;
		GameObject safe = this.spawnPoint.GetSafe(this);
		if (safe)
		{
			vector += safe.transform.position;
		}
		Action onRecycle = null;
		if (!this.SpawnedPlayerRef.IsNone)
		{
			onRecycle = delegate()
			{
				this.SpawnedPlayerRef.Value = null;
			};
		}
		this.spawnedAudioSource = this.SpawnAudioEvent(vector, onRecycle);
		if (this.spawnedAudioSource && !this.SpawnedPlayerRef.IsNone)
		{
			this.SpawnedPlayerRef.Value = this.spawnedAudioSource.gameObject;
		}
		base.Finish();
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00026238 File Offset: 0x00024438
	public override void OnExit()
	{
		base.OnExit();
		this.spawnedAudioSource = null;
	}

	// Token: 0x06000800 RID: 2048
	protected abstract AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle);

	// Token: 0x040007AA RID: 1962
	public FsmFloat pitchMin;

	// Token: 0x040007AB RID: 1963
	public FsmFloat pitchMax;

	// Token: 0x040007AC RID: 1964
	public FsmFloat volume;

	// Token: 0x040007AD RID: 1965
	[ObjectType(typeof(AudioSource))]
	public FsmObject audioPlayerPrefab;

	// Token: 0x040007AE RID: 1966
	public FsmOwnerDefault spawnPoint;

	// Token: 0x040007AF RID: 1967
	public FsmVector3 spawnPosition;

	// Token: 0x040007B0 RID: 1968
	[UIHint(UIHint.Variable)]
	public FsmGameObject SpawnedPlayerRef;

	// Token: 0x040007B1 RID: 1969
	protected AudioSource spawnedAudioSource;
}
