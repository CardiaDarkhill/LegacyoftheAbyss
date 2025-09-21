using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014D RID: 333
[CreateAssetMenu(menuName = "Camera/Camera Manager Reference")]
public class CameraManagerReference : ScriptableObject
{
	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000A22 RID: 2594 RVA: 0x0002DDD8 File Offset: 0x0002BFD8
	// (remove) Token: 0x06000A23 RID: 2595 RVA: 0x0002DE10 File Offset: 0x0002C010
	public event CameraManagerReference.CameraShakeWorldForceDelegate CameraShakedWorldForce;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000A24 RID: 2596 RVA: 0x0002DE48 File Offset: 0x0002C048
	// (remove) Token: 0x06000A25 RID: 2597 RVA: 0x0002DE80 File Offset: 0x0002C080
	public event CameraManagerReference.CameraShakingWorldForceDelegate CameraShakingWorldForce;

	// Token: 0x06000A26 RID: 2598 RVA: 0x0002DEB8 File Offset: 0x0002C0B8
	public void Register(CameraShakeManager manager)
	{
		if (!this.shakeManagers.Contains(manager))
		{
			this.shakeManagers.Add(manager);
			foreach (CameraManagerReference.LoopingShake loopingShake in this.loopingShakes)
			{
				manager.DoShake(loopingShake.Shake, loopingShake.Source, false, loopingShake.Vibrate, loopingShake.SendWorldForce);
			}
		}
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0002DF40 File Offset: 0x0002C140
	public void Deregister(CameraShakeManager manager)
	{
		if (this.shakeManagers.Contains(manager))
		{
			this.shakeManagers.Remove(manager);
		}
		if (this.shakeManagers.Count <= 0)
		{
			this.loopingShakes.Clear();
		}
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0002DF78 File Offset: 0x0002C178
	public void DoShake(ICameraShake shake, Object source, bool doFreeze = true, bool vibrate = true, bool sendWorldForce = true)
	{
		foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
		{
			cameraShakeManager.DoShake(shake, source, doFreeze, vibrate, sendWorldForce);
		}
		if (!shake.CanFinish)
		{
			this.loopingShakes.Add(new CameraManagerReference.LoopingShake
			{
				Shake = shake,
				Source = source,
				Vibrate = vibrate,
				SendWorldForce = sendWorldForce
			});
		}
		if (sendWorldForce)
		{
			this.OnDidShake(shake);
		}
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0002E010 File Offset: 0x0002C210
	public void ApplyOffsets()
	{
		foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
		{
			cameraShakeManager.ApplyOffset();
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0002E060 File Offset: 0x0002C260
	private void OnDidShake(ICameraShake shake)
	{
		this.SendWorldForce(shake.WorldForceOnStart);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0002E070 File Offset: 0x0002C270
	public void SendWorldForce(CameraShakeWorldForceIntensities worldForce)
	{
		if (worldForce != CameraShakeWorldForceIntensities.None)
		{
			if (this.CameraShakedWorldForce != null)
			{
				foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
				{
					this.CameraShakedWorldForce(cameraShakeManager.transform.position, worldForce);
				}
			}
			this.SendWorldShaking(worldForce);
		}
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0002E0EC File Offset: 0x0002C2EC
	public void SendWorldShaking(CameraShakeWorldForceIntensities worldForce)
	{
		if (worldForce == CameraShakeWorldForceIntensities.None)
		{
			return;
		}
		this.SendWorldShaking(worldForce.ToFlag());
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0002E100 File Offset: 0x0002C300
	public void SendWorldShaking(CameraShakeWorldForceFlag worldForce)
	{
		if (worldForce != CameraShakeWorldForceFlag.None && this.CameraShakingWorldForce != null)
		{
			foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
			{
				this.CameraShakingWorldForce(cameraShakeManager.transform.position, worldForce);
			}
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0002E174 File Offset: 0x0002C374
	public void CancelShake(ICameraShake shake)
	{
		if (shake == null)
		{
			foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
			{
				cameraShakeManager.CancelAllShakes();
			}
			this.loopingShakes.Clear();
			return;
		}
		foreach (CameraShakeManager cameraShakeManager2 in this.shakeManagers)
		{
			cameraShakeManager2.CancelShake(shake);
		}
		this.loopingShakes.RemoveAll((CameraManagerReference.LoopingShake s) => s.Shake == shake);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0002E244 File Offset: 0x0002C444
	public void DoShakeInRange(ICameraShake shake, Object source, Vector2 range, Vector2 sourcePos, bool doFreeze = true, bool vibrate = true)
	{
		range.x = Mathf.Abs(range.x);
		range.y = Mathf.Abs(range.y);
		bool flag = false;
		foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
		{
			Vector2 vector = cameraShakeManager.transform.position - sourcePos;
			if ((range.x <= 0f || Mathf.Abs(vector.x) <= range.x) && (range.y <= 0f || Mathf.Abs(vector.y) <= range.y))
			{
				cameraShakeManager.DoShake(shake, source, doFreeze, vibrate, true);
				flag = true;
			}
		}
		if (flag)
		{
			this.OnDidShake(shake);
		}
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0002E328 File Offset: 0x0002C528
	public IEnumerable<CameraShakeManager.CameraShakeTracker> EnumerateCurrentShakes()
	{
		foreach (CameraShakeManager cameraShakeManager in this.shakeManagers)
		{
			foreach (CameraShakeManager.CameraShakeTracker cameraShakeTracker in cameraShakeManager.EnumerateCurrentShakes())
			{
				yield return cameraShakeTracker;
			}
			IEnumerator<CameraShakeManager.CameraShakeTracker> enumerator2 = null;
		}
		List<CameraShakeManager>.Enumerator enumerator = default(List<CameraShakeManager>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x040009B8 RID: 2488
	[NonSerialized]
	private readonly List<CameraShakeManager> shakeManagers = new List<CameraShakeManager>();

	// Token: 0x040009B9 RID: 2489
	[NonSerialized]
	private readonly List<CameraManagerReference.LoopingShake> loopingShakes = new List<CameraManagerReference.LoopingShake>();

	// Token: 0x02001482 RID: 5250
	private class LoopingShake
	{
		// Token: 0x04008379 RID: 33657
		public ICameraShake Shake;

		// Token: 0x0400837A RID: 33658
		public Object Source;

		// Token: 0x0400837B RID: 33659
		public bool Vibrate;

		// Token: 0x0400837C RID: 33660
		public bool SendWorldForce;
	}

	// Token: 0x02001483 RID: 5251
	// (Invoke) Token: 0x060083C2 RID: 33730
	public delegate void CameraShakeWorldForceDelegate(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity);

	// Token: 0x02001484 RID: 5252
	// (Invoke) Token: 0x060083C6 RID: 33734
	public delegate void CameraShakingWorldForceDelegate(Vector2 cameraPosition, CameraShakeWorldForceFlag intensity);
}
