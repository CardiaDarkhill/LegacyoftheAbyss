using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class CameraShakeManager : MonoBehaviour
{
	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000A48 RID: 2632 RVA: 0x0002E5CA File Offset: 0x0002C7CA
	// (set) Token: 0x06000A49 RID: 2633 RVA: 0x0002E5D1 File Offset: 0x0002C7D1
	public static CameraShakeManager.ShakeSettings ShakeSetting { get; set; }

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000A4A RID: 2634 RVA: 0x0002E5DC File Offset: 0x0002C7DC
	public static float ShakeMultiplier
	{
		get
		{
			switch (CameraShakeManager.ShakeSetting)
			{
			case CameraShakeManager.ShakeSettings.On:
				return 1f;
			case CameraShakeManager.ShakeSettings.Reduced:
				return ConfigManager.ReducedCameraShake;
			case CameraShakeManager.ShakeSettings.Off:
				return 0f;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x0002E61A File Offset: 0x0002C81A
	private Transform CurrentTransform
	{
		get
		{
			if (!this.overrideTransform)
			{
				return base.transform;
			}
			return this.overrideTransform;
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0002E638 File Offset: 0x0002C838
	private void OnEnable()
	{
		this.evaluateShakesRoutine = base.StartCoroutine(this.EvaluateShakesTimed());
		if (this.cameraTypeReference)
		{
			this.cameraTypeReference.Register(this);
		}
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.NextSceneWillActivate += this.CancelAllShakes;
		}
		this.CancelAllShakes();
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0002E698 File Offset: 0x0002C898
	private void OnDisable()
	{
		if (this.cameraTypeReference)
		{
			this.cameraTypeReference.Deregister(this);
		}
		base.StopCoroutine(this.evaluateShakesRoutine);
		this.currentOffset = Vector3.zero;
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.NextSceneWillActivate -= this.CancelAllShakes;
		}
		this.CancelAllShakes();
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0002E6FB File Offset: 0x0002C8FB
	private IEnumerator EvaluateShakesTimed()
	{
		double lastEvalTime = this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
		WaitForSeconds wait = new WaitForSeconds(0.016666668f);
		WaitForSecondsRealtime realtimeWait = new WaitForSecondsRealtime(0.016666668f);
		for (;;)
		{
			double num = this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
			float timeSinceLastEvaluation = (float)(num - lastEvalTime);
			this.currentOffset = this.EvaluateShakes(timeSinceLastEvaluation);
			lastEvalTime = num;
			if (this.isRealtime)
			{
				yield return realtimeWait;
			}
			else
			{
				yield return wait;
			}
		}
		yield break;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0002E70A File Offset: 0x0002C90A
	private void OnPreCull()
	{
		if (!this.isRealtime && Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		if (this.wasOffsetApplied)
		{
			this.ClearOffset();
		}
		this.initialPosition = this.CurrentTransform.localPosition;
		this.ApplyOffset();
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0002E746 File Offset: 0x0002C946
	private void OnPostRender()
	{
		if (!this.isRealtime && Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		this.ClearOffset();
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0002E763 File Offset: 0x0002C963
	public void ApplyOffset()
	{
		if (this.freezeFrameRoutine == null)
		{
			this.CurrentTransform.localPosition += this.currentOffset;
		}
		this.wasOffsetApplied = true;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0002E790 File Offset: 0x0002C990
	private void ClearOffset()
	{
		this.CurrentTransform.localPosition = this.initialPosition;
		this.wasOffsetApplied = false;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0002E7AC File Offset: 0x0002C9AC
	public void DoShake(ICameraShake shake, Object source, bool doFreeze = true, bool vibrate = true, bool sendWorldForce = true)
	{
		if (shake == null)
		{
			return;
		}
		if ((this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < this.cameraShakeStartTime && shake.CanFinish)
		{
			return;
		}
		bool flag = false;
		using (List<CameraShakeManager.CameraShakeTracker>.Enumerator enumerator = this.currentShakes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Shake == shake)
				{
					flag = true;
					break;
				}
			}
		}
		if (!shake.CanFinish && flag)
		{
			return;
		}
		CameraShakeManager.CameraShakeTracker item = new CameraShakeManager.CameraShakeTracker
		{
			Shake = shake,
			Source = source,
			StartStackTrace = (CheatManager.IsStackTracesEnabled ? new StackTrace() : null),
			PersistThroughScenes = shake.PersistThroughScenes,
			SendWorldForce = sendWorldForce
		};
		if (vibrate)
		{
			item.StartVibration(this.isRealtime);
		}
		this.currentShakes.Add(item);
		if (doFreeze && shake.FreezeFrames > 0)
		{
			if (this.freezeFrameRoutine != null)
			{
				base.StopCoroutine(this.freezeFrameRoutine);
				if (this.onFreezeEnd != null)
				{
					this.onFreezeEnd();
				}
			}
			this.freezeFrameRoutine = base.StartCoroutine(this.FreezeFrames(shake.FreezeFrames));
		}
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0002E8EC File Offset: 0x0002CAEC
	public void CancelShake(ICameraShake profile)
	{
		for (int i = this.currentShakes.Count - 1; i >= 0; i--)
		{
			if (this.currentShakes[i].Shake == profile)
			{
				this.currentShakes[i].StopVibration();
				this.currentShakes.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0002E948 File Offset: 0x0002CB48
	public void CancelAllShakes()
	{
		for (int i = this.currentShakes.Count - 1; i >= 0; i--)
		{
			CameraShakeManager.CameraShakeTracker cameraShakeTracker = this.currentShakes[i];
			if (!cameraShakeTracker.PersistThroughScenes)
			{
				cameraShakeTracker.StopVibration();
				this.currentShakes.RemoveAt(i);
			}
		}
		double num = this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
		this.cameraShakeStartTime = num + 0.5;
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0002E9BC File Offset: 0x0002CBBC
	private Vector2 EvaluateShakes(float timeSinceLastEvaluation)
	{
		if ((this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < this.cameraShakeStartTime)
		{
			return Vector2.zero;
		}
		Vector2 a = Vector2.zero;
		CameraShakeWorldForceFlag cameraShakeWorldForceFlag = CameraShakeWorldForceFlag.None;
		float num = 0f;
		for (int i = 0; i < this.currentShakes.Count; i++)
		{
			CameraShakeManager.CameraShakeTracker cameraShakeTracker = this.currentShakes[i];
			cameraShakeTracker.ElapsedTime += timeSinceLastEvaluation;
			a += cameraShakeTracker.GetOffset();
			this.currentShakes[i] = cameraShakeTracker;
			if (cameraShakeTracker.SendWorldForce)
			{
				cameraShakeWorldForceFlag |= cameraShakeTracker.ShakeFlag;
			}
			float magnitude = cameraShakeTracker.Shake.Magnitude;
			if (magnitude > num)
			{
				num = magnitude;
			}
		}
		if (a.magnitude > num)
		{
			a = a.normalized * num;
		}
		if (cameraShakeWorldForceFlag != CameraShakeWorldForceFlag.None)
		{
			this.cameraTypeReference.SendWorldShaking(cameraShakeWorldForceFlag);
		}
		for (int j = this.currentShakes.Count - 1; j >= 0; j--)
		{
			CameraShakeManager.CameraShakeTracker cameraShakeTracker2 = this.currentShakes[j];
			if (cameraShakeTracker2.IsDone)
			{
				cameraShakeTracker2.StopLoop();
				this.currentShakes.RemoveAt(j);
			}
		}
		return a * CameraShakeManager.ShakeMultiplier;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0002EAE8 File Offset: 0x0002CCE8
	private IEnumerator FreezeFrames(int frameCount)
	{
		if (CameraShakeManager._freezesRunning <= 0)
		{
			TimeManager.CameraShakeTimeScale = 0f;
		}
		CameraShakeManager._freezesRunning++;
		this.onFreezeEnd = delegate()
		{
			CameraShakeManager._freezesRunning--;
			if (CameraShakeManager._freezesRunning <= 0)
			{
				TimeManager.CameraShakeTimeScale = 1f;
			}
		};
		yield return new WaitForSecondsRealtime(0.016666668f * (float)frameCount);
		this.onFreezeEnd();
		this.onFreezeEnd = null;
		this.freezeFrameRoutine = null;
		yield break;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0002EAFE File Offset: 0x0002CCFE
	public IEnumerable<CameraShakeManager.CameraShakeTracker> EnumerateCurrentShakes()
	{
		foreach (CameraShakeManager.CameraShakeTracker cameraShakeTracker in this.currentShakes)
		{
			yield return cameraShakeTracker;
		}
		List<CameraShakeManager.CameraShakeTracker>.Enumerator enumerator = default(List<CameraShakeManager.CameraShakeTracker>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0002EB10 File Offset: 0x0002CD10
	public void CopyTo(GameObject other)
	{
		CameraShakeManager cameraShakeManager = other.GetComponent<CameraShakeManager>();
		if (!cameraShakeManager)
		{
			cameraShakeManager = other.AddComponent<CameraShakeManager>();
		}
		if (cameraShakeManager.cameraTypeReference)
		{
			cameraShakeManager.cameraTypeReference.Deregister(cameraShakeManager);
		}
		cameraShakeManager.cameraTypeReference = this.cameraTypeReference;
		if (cameraShakeManager.cameraTypeReference)
		{
			cameraShakeManager.cameraTypeReference.Register(cameraShakeManager);
		}
	}

	// Token: 0x040009CF RID: 2511
	private const float SHAKE_DISABLED_TIME = 0.5f;

	// Token: 0x040009D0 RID: 2512
	[SerializeField]
	private CameraManagerReference cameraTypeReference;

	// Token: 0x040009D1 RID: 2513
	[SerializeField]
	private Transform overrideTransform;

	// Token: 0x040009D2 RID: 2514
	[SerializeField]
	private bool isRealtime;

	// Token: 0x040009D3 RID: 2515
	private Vector3 initialPosition;

	// Token: 0x040009D4 RID: 2516
	private bool wasOffsetApplied;

	// Token: 0x040009D5 RID: 2517
	private readonly List<CameraShakeManager.CameraShakeTracker> currentShakes = new List<CameraShakeManager.CameraShakeTracker>();

	// Token: 0x040009D6 RID: 2518
	private Coroutine evaluateShakesRoutine;

	// Token: 0x040009D7 RID: 2519
	private Vector3 currentOffset;

	// Token: 0x040009D8 RID: 2520
	private Coroutine freezeFrameRoutine;

	// Token: 0x040009D9 RID: 2521
	private Action onFreezeEnd;

	// Token: 0x040009DA RID: 2522
	private static int _freezesRunning;

	// Token: 0x040009DB RID: 2523
	private double cameraShakeStartTime;

	// Token: 0x02001487 RID: 5255
	[Serializable]
	public struct CameraShakeTracker
	{
		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x060083D5 RID: 33749 RVA: 0x00269DA7 File Offset: 0x00267FA7
		public bool IsDone
		{
			get
			{
				return this.Shake.IsDone(this.ElapsedTime);
			}
		}

		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x060083D6 RID: 33750 RVA: 0x00269DBA File Offset: 0x00267FBA
		public CameraShakeWorldForceFlag ShakeFlag
		{
			get
			{
				return this.Shake.WorldForceOnStart.ToFlag();
			}
		}

		// Token: 0x060083D7 RID: 33751 RVA: 0x00269DCC File Offset: 0x00267FCC
		public Vector2 GetOffset()
		{
			this.UpdateShake();
			return this.Shake.GetOffset(this.ElapsedTime);
		}

		// Token: 0x060083D8 RID: 33752 RVA: 0x00269DE5 File Offset: 0x00267FE5
		public void StartVibration(bool isRealtime)
		{
			if (this.Shake.CameraShakeVibration != null)
			{
				this.Emission = this.Shake.CameraShakeVibration.PlayVibration(isRealtime);
				this.IsVibrating = (this.Emission != null);
			}
		}

		// Token: 0x060083D9 RID: 33753 RVA: 0x00269E1A File Offset: 0x0026801A
		public void StopVibration()
		{
			if (this.IsVibrating)
			{
				this.IsVibrating = false;
				this.Emission.Stop();
			}
		}

		// Token: 0x060083DA RID: 33754 RVA: 0x00269E36 File Offset: 0x00268036
		public void StopLoop()
		{
			if (this.IsVibrating)
			{
				this.Emission.IsLooping = false;
			}
		}

		// Token: 0x060083DB RID: 33755 RVA: 0x00269E4C File Offset: 0x0026804C
		public void UpdateShake()
		{
			if (this.IsVibrating)
			{
				float vibrationStrength = this.Shake.CameraShakeVibration.GetVibrationStrength(this.ElapsedTime);
				this.Emission.SetStrength(vibrationStrength);
			}
		}

		// Token: 0x04008384 RID: 33668
		public ICameraShake Shake;

		// Token: 0x04008385 RID: 33669
		public float ElapsedTime;

		// Token: 0x04008386 RID: 33670
		public Object Source;

		// Token: 0x04008387 RID: 33671
		public StackTrace StartStackTrace;

		// Token: 0x04008388 RID: 33672
		public bool IsVibrating;

		// Token: 0x04008389 RID: 33673
		public VibrationEmission Emission;

		// Token: 0x0400838A RID: 33674
		public bool PersistThroughScenes;

		// Token: 0x0400838B RID: 33675
		public bool SendWorldForce;
	}

	// Token: 0x02001488 RID: 5256
	public enum ShakeSettings
	{
		// Token: 0x0400838D RID: 33677
		On,
		// Token: 0x0400838E RID: 33678
		Reduced,
		// Token: 0x0400838F RID: 33679
		Off
	}
}
