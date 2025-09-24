using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x020007A6 RID: 1958
public sealed class RainVibrationRegion : MonoBehaviour
{
	// Token: 0x170007B8 RID: 1976
	// (get) Token: 0x0600452E RID: 17710 RVA: 0x0012E65E File Offset: 0x0012C85E
	// (set) Token: 0x0600452F RID: 17711 RVA: 0x0012E666 File Offset: 0x0012C866
	public List<RainVibrationRegion.RainVibrationPlayer> RainVibrations
	{
		get
		{
			return this.rainVibrations;
		}
		set
		{
			this.rainVibrations = value;
		}
	}

	// Token: 0x170007B9 RID: 1977
	// (get) Token: 0x06004530 RID: 17712 RVA: 0x0012E66F File Offset: 0x0012C86F
	// (set) Token: 0x06004531 RID: 17713 RVA: 0x0012E677 File Offset: 0x0012C877
	public MinMaxFloat Frequency
	{
		get
		{
			return this.frequency;
		}
		set
		{
			this.frequency = value;
		}
	}

	// Token: 0x170007BA RID: 1978
	// (get) Token: 0x06004532 RID: 17714 RVA: 0x0012E680 File Offset: 0x0012C880
	// (set) Token: 0x06004533 RID: 17715 RVA: 0x0012E688 File Offset: 0x0012C888
	public bool UseStrengthMultiplier
	{
		get
		{
			return this.useStrengthMultiplier;
		}
		set
		{
			this.useStrengthMultiplier = value;
		}
	}

	// Token: 0x170007BB RID: 1979
	// (get) Token: 0x06004534 RID: 17716 RVA: 0x0012E691 File Offset: 0x0012C891
	// (set) Token: 0x06004535 RID: 17717 RVA: 0x0012E699 File Offset: 0x0012C899
	public MinMaxFloat StrengthMultiplier
	{
		get
		{
			return this.strengthMultiplier;
		}
		set
		{
			this.strengthMultiplier = value;
		}
	}

	// Token: 0x06004536 RID: 17718 RVA: 0x0012E6A2 File Offset: 0x0012C8A2
	private void Start()
	{
		RainVibrationRegion.rainVibrationRegions.Add(this);
		if (!this.isInside)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06004537 RID: 17719 RVA: 0x0012E6C0 File Offset: 0x0012C8C0
	private void OnDestroy()
	{
		foreach (RainVibrationRegion.RainVibrationPool.PooledRain pooledRain in this.activeEmissions)
		{
			VibrationEmission emission = pooledRain.emission;
			if (emission != null)
			{
				emission.Stop();
			}
		}
		this.activeEmissions.Clear();
		RainVibrationRegion.rainVibrationRegions.Remove(this);
	}

	// Token: 0x06004538 RID: 17720 RVA: 0x0012E734 File Offset: 0x0012C934
	private void Update()
	{
		this.timer -= Time.deltaTime;
		int count = this.activeEmissions.Count;
		while (count-- > 0)
		{
			LinkedListNode<RainVibrationRegion.RainVibrationPool.PooledRain> first = this.activeEmissions.First;
			RainVibrationRegion.RainVibrationPool.PooledRain value = first.Value;
			VibrationEmission emission = value.emission;
			if (emission != null && emission.IsPlaying)
			{
				break;
			}
			value.Release();
			this.activeEmissions.Remove(first);
		}
		if (this.timer > 0f)
		{
			return;
		}
		this.timer = this.frequency.GetRandomValue();
		this.PlayRainVibration();
	}

	// Token: 0x06004539 RID: 17721 RVA: 0x0012E7C5 File Offset: 0x0012C9C5
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			this.Enter();
		}
	}

	// Token: 0x0600453A RID: 17722 RVA: 0x0012E7DA File Offset: 0x0012C9DA
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			this.Exit();
		}
	}

	// Token: 0x0600453B RID: 17723 RVA: 0x0012E7EF File Offset: 0x0012C9EF
	private void Enter()
	{
		if (this.isInside)
		{
			return;
		}
		this.isInside = true;
		base.enabled = true;
	}

	// Token: 0x0600453C RID: 17724 RVA: 0x0012E808 File Offset: 0x0012CA08
	private void Exit()
	{
		if (!this.isInside)
		{
			return;
		}
		this.isInside = false;
		base.enabled = false;
	}

	// Token: 0x0600453D RID: 17725 RVA: 0x0012E824 File Offset: 0x0012CA24
	private void PlayRainVibration()
	{
		float num = this.useStrengthMultiplier ? this.strengthMultiplier.GetRandomValue() : 1f;
		RainVibrationRegion.RainVibrationPlayer rainVibrationPlayer = this.GetRainVibrationPlayer();
		RainVibrationRegion.RainVibrationPool.PooledRain pooledRain = (rainVibrationPlayer != null) ? rainVibrationPlayer.PlayVibration(num) : null;
		if (pooledRain != null)
		{
			this.activeEmissions.AddLast(pooledRain);
		}
	}

	// Token: 0x0600453E RID: 17726 RVA: 0x0012E870 File Offset: 0x0012CA70
	private RainVibrationRegion.RainVibrationPlayer GetRainVibrationPlayer()
	{
		if (this.rainVibrations.Count == 0)
		{
			return null;
		}
		return this.rainVibrations[Random.Range(0, this.rainVibrations.Count)];
	}

	// Token: 0x0600453F RID: 17727 RVA: 0x0012E89D File Offset: 0x0012CA9D
	public string GetNameString()
	{
		return string.Format("{0} : {1}", this, this.isInside ? "Active" : "Inactive");
	}

	// Token: 0x04004609 RID: 17929
	[SerializeField]
	private List<RainVibrationRegion.RainVibrationPlayer> rainVibrations = new List<RainVibrationRegion.RainVibrationPlayer>();

	// Token: 0x0400460A RID: 17930
	[Space]
	[SerializeField]
	private MinMaxFloat frequency = new MinMaxFloat(0.1f, 1.5f);

	// Token: 0x0400460B RID: 17931
	[SerializeField]
	private bool useStrengthMultiplier;

	// Token: 0x0400460C RID: 17932
	[SerializeField]
	private MinMaxFloat strengthMultiplier = new MinMaxFloat(0.9f, 1.1f);

	// Token: 0x0400460D RID: 17933
	private LinkedList<RainVibrationRegion.RainVibrationPool.PooledRain> activeEmissions = new LinkedList<RainVibrationRegion.RainVibrationPool.PooledRain>();

	// Token: 0x0400460E RID: 17934
	private float timer;

	// Token: 0x0400460F RID: 17935
	private bool isInside;

	// Token: 0x04004610 RID: 17936
	public static readonly List<RainVibrationRegion> rainVibrationRegions = new List<RainVibrationRegion>();

	// Token: 0x02001A82 RID: 6786
	public sealed class RainVibrationPool : IDisposable
	{
		// Token: 0x0600971C RID: 38684 RVA: 0x002A983C File Offset: 0x002A7A3C
		public RainVibrationPool(VibrationDataAsset source)
		{
			this.source = source;
			this.pool = new LinkedPool<RainVibrationRegion.RainVibrationPool.PooledRain>(new Func<RainVibrationRegion.RainVibrationPool.PooledRain>(this.CreateFunc), new Action<RainVibrationRegion.RainVibrationPool.PooledRain>(this.ActionOnGet), new Action<RainVibrationRegion.RainVibrationPool.PooledRain>(this.ActionOnRelease), new Action<RainVibrationRegion.RainVibrationPool.PooledRain>(this.ActionOnDestroy), false, 20);
		}

		// Token: 0x0600971D RID: 38685 RVA: 0x002A9894 File Offset: 0x002A7A94
		private RainVibrationRegion.RainVibrationPool.PooledRain CreateFunc()
		{
			VibrationData vibrationData = this.source;
			bool isLooping = this.loop;
			bool isRealtime = this.isRealTime;
			string text = this.tag;
			return new RainVibrationRegion.RainVibrationPool.PooledRain(this, VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime));
		}

		// Token: 0x0600971E RID: 38686 RVA: 0x002A98D8 File Offset: 0x002A7AD8
		private void ActionOnGet(RainVibrationRegion.RainVibrationPool.PooledRain obj)
		{
		}

		// Token: 0x0600971F RID: 38687 RVA: 0x002A98DA File Offset: 0x002A7ADA
		private void ActionOnRelease(RainVibrationRegion.RainVibrationPool.PooledRain obj)
		{
			VibrationEmission emission = obj.emission;
			if (emission == null)
			{
				return;
			}
			emission.Stop();
		}

		// Token: 0x06009720 RID: 38688 RVA: 0x002A98EC File Offset: 0x002A7AEC
		private void ActionOnDestroy(RainVibrationRegion.RainVibrationPool.PooledRain obj)
		{
			VibrationEmission emission = obj.emission;
			if (emission == null)
			{
				return;
			}
			emission.Stop();
		}

		// Token: 0x06009721 RID: 38689 RVA: 0x002A98FE File Offset: 0x002A7AFE
		public RainVibrationRegion.RainVibrationPool.PooledRain Get()
		{
			if (this.disposed)
			{
				return null;
			}
			return this.pool.Get();
		}

		// Token: 0x06009722 RID: 38690 RVA: 0x002A9915 File Offset: 0x002A7B15
		public void Release(RainVibrationRegion.RainVibrationPool.PooledRain vibrationEmission)
		{
			if (vibrationEmission == null)
			{
				return;
			}
			this.pool.Release(vibrationEmission);
		}

		// Token: 0x06009723 RID: 38691 RVA: 0x002A9927 File Offset: 0x002A7B27
		private void ReleaseUnmanagedResources()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			this.pool.Clear();
			this.pool = null;
		}

		// Token: 0x06009724 RID: 38692 RVA: 0x002A994B File Offset: 0x002A7B4B
		public void Dispose()
		{
			this.ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009725 RID: 38693 RVA: 0x002A995C File Offset: 0x002A7B5C
		~RainVibrationPool()
		{
			this.ReleaseUnmanagedResources();
		}

		// Token: 0x040099B5 RID: 39349
		private IObjectPool<RainVibrationRegion.RainVibrationPool.PooledRain> pool;

		// Token: 0x040099B6 RID: 39350
		private VibrationDataAsset source;

		// Token: 0x040099B7 RID: 39351
		private bool loop;

		// Token: 0x040099B8 RID: 39352
		private bool isRealTime;

		// Token: 0x040099B9 RID: 39353
		private bool updating;

		// Token: 0x040099BA RID: 39354
		private bool disposed;

		// Token: 0x040099BB RID: 39355
		private string tag;

		// Token: 0x02001C29 RID: 7209
		public sealed class PooledRain
		{
			// Token: 0x06009AF6 RID: 39670 RVA: 0x002B4C05 File Offset: 0x002B2E05
			public PooledRain(RainVibrationRegion.RainVibrationPool pool, VibrationEmission emission)
			{
				this.pool = pool;
				this.emission = emission;
			}

			// Token: 0x06009AF7 RID: 39671 RVA: 0x002B4C1B File Offset: 0x002B2E1B
			public void Release()
			{
				RainVibrationRegion.RainVibrationPool rainVibrationPool = this.pool;
				if (rainVibrationPool == null)
				{
					return;
				}
				rainVibrationPool.Release(this);
			}

			// Token: 0x0400A031 RID: 41009
			private RainVibrationRegion.RainVibrationPool pool;

			// Token: 0x0400A032 RID: 41010
			public VibrationEmission emission;
		}
	}

	// Token: 0x02001A83 RID: 6787
	[Serializable]
	public sealed class RainVibrationPlayer
	{
		// Token: 0x06009726 RID: 38694 RVA: 0x002A9988 File Offset: 0x002A7B88
		protected override void Finalize()
		{
			try
			{
				RainVibrationRegion.RainVibrationPool rainVibrationPool = this.pool;
				if (rainVibrationPool != null)
				{
					rainVibrationPool.Dispose();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		// Token: 0x06009727 RID: 38695 RVA: 0x002A99C0 File Offset: 0x002A7BC0
		private void Init()
		{
			if (this.init)
			{
				return;
			}
			this.init = true;
			this.pool = new RainVibrationRegion.RainVibrationPool(this.vibration);
		}

		// Token: 0x06009728 RID: 38696 RVA: 0x002A99E4 File Offset: 0x002A7BE4
		public RainVibrationRegion.RainVibrationPool.PooledRain PlayVibration(float strengthMultiplier)
		{
			this.Init();
			RainVibrationRegion.RainVibrationPool.PooledRain pooledRain = this.pool.Get();
			VibrationEmission emission = pooledRain.emission;
			if (emission != null)
			{
				emission.SetStrength(this.strength * strengthMultiplier);
				VibrationManager.PlayVibrationClipOneShot(emission);
			}
			return pooledRain;
		}

		// Token: 0x040099BC RID: 39356
		public VibrationDataAsset vibration;

		// Token: 0x040099BD RID: 39357
		public float strength = 1f;

		// Token: 0x040099BE RID: 39358
		private bool init;

		// Token: 0x040099BF RID: 39359
		private RainVibrationRegion.RainVibrationPool pool;
	}
}
