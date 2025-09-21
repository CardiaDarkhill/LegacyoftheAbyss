using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class JitterEnemyInside : MonoBehaviour
{
	// Token: 0x06001AE0 RID: 6880 RVA: 0x0007D525 File Offset: 0x0007B725
	private void Awake()
	{
		if (this.frequency <= 0f)
		{
			this.frequency = 60f;
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x0007D53F File Offset: 0x0007B73F
	private void OnEnable()
	{
		CameraRenderHooks.CameraPreCull += this.OnCameraPreCull;
		CameraRenderHooks.CameraPostRender += this.OnCameraPostRender;
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x0007D563 File Offset: 0x0007B763
	private void OnDisable()
	{
		this.enemiesInside.Clear();
		CameraRenderHooks.CameraPreCull -= this.OnCameraPreCull;
		CameraRenderHooks.CameraPostRender -= this.OnCameraPostRender;
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0007D594 File Offset: 0x0007B794
	private void Update()
	{
		if (Time.timeAsDouble < this.nextJitterTime)
		{
			return;
		}
		this.nextJitterTime = Time.timeAsDouble + (double)(1f / this.frequency);
		for (int i = 0; i < this.enemiesInside.Count; i++)
		{
			JitterEnemyInside.EnemyInfo enemyInfo = this.enemiesInside[i];
			if (!enemyInfo.IsReady)
			{
				enemyInfo.IsReady = true;
			}
			if (enemyInfo.FreezePosition)
			{
				Vector3 initialPos = enemyInfo.InitialPos;
				if (this.ignoreZ)
				{
					initialPos.z = enemyInfo.Transform.position.z;
				}
				enemyInfo.Transform.position = initialPos;
			}
			enemyInfo.TargetPos = enemyInfo.Transform.position + this.amount.RandomInRange();
			this.enemiesInside[i] = enemyInfo;
		}
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x0007D66C File Offset: 0x0007B86C
	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < this.enemiesInside.Count; i++)
		{
			JitterEnemyInside.EnemyInfo enemyInfo = this.enemiesInside[i];
			if (enemyInfo.IsReady)
			{
				enemyInfo.PreCullPos = enemyInfo.Transform.position;
				enemyInfo.Transform.position = enemyInfo.TargetPos;
				this.enemiesInside[i] = enemyInfo;
			}
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x0007D6F0 File Offset: 0x0007B8F0
	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < this.enemiesInside.Count; i++)
		{
			JitterEnemyInside.EnemyInfo enemyInfo = this.enemiesInside[i];
			if (enemyInfo.IsReady)
			{
				enemyInfo.Transform.position = enemyInfo.PreCullPos;
				this.enemiesInside[i] = enemyInfo;
			}
		}
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x0007D760 File Offset: 0x0007B960
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Recoil component = collision.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		float recoilSpeedBase = component.RecoilSpeedBase;
		using (List<JitterEnemyInside.EnemyInfo>.Enumerator enumerator = this.enemiesInside.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Transform == component.transform)
				{
					return;
				}
			}
		}
		this.enemiesInside.Add(new JitterEnemyInside.EnemyInfo
		{
			Transform = component.transform,
			InitialPos = component.transform.position,
			FreezePosition = (recoilSpeedBase > 0f)
		});
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x0007D818 File Offset: 0x0007BA18
	private void OnTriggerExit2D(Collider2D collision)
	{
		Recoil component = collision.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		for (int i = this.enemiesInside.Count - 1; i >= 0; i--)
		{
			if (this.enemiesInside[i].Transform == component.transform)
			{
				this.enemiesInside.RemoveAt(i);
			}
		}
	}

	// Token: 0x040019FD RID: 6653
	[SerializeField]
	private float frequency;

	// Token: 0x040019FE RID: 6654
	[SerializeField]
	private Vector3 amount;

	// Token: 0x040019FF RID: 6655
	[SerializeField]
	private bool ignoreZ;

	// Token: 0x04001A00 RID: 6656
	private double nextJitterTime;

	// Token: 0x04001A01 RID: 6657
	private List<JitterEnemyInside.EnemyInfo> enemiesInside = new List<JitterEnemyInside.EnemyInfo>();

	// Token: 0x020015DB RID: 5595
	private struct EnemyInfo
	{
		// Token: 0x040088DB RID: 35035
		public Transform Transform;

		// Token: 0x040088DC RID: 35036
		public Vector3 InitialPos;

		// Token: 0x040088DD RID: 35037
		public bool FreezePosition;

		// Token: 0x040088DE RID: 35038
		public bool IsReady;

		// Token: 0x040088DF RID: 35039
		public Vector3 PreCullPos;

		// Token: 0x040088E0 RID: 35040
		public Vector3 TargetPos;
	}
}
