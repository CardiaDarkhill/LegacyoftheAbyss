using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000523 RID: 1315
public class NoiseMaker : MonoBehaviour
{
	// Token: 0x14000093 RID: 147
	// (add) Token: 0x06002F42 RID: 12098 RVA: 0x000D0740 File Offset: 0x000CE940
	// (remove) Token: 0x06002F43 RID: 12099 RVA: 0x000D0774 File Offset: 0x000CE974
	public static event NoiseMaker.NoiseEvent NoiseCreated;

	// Token: 0x14000094 RID: 148
	// (add) Token: 0x06002F44 RID: 12100 RVA: 0x000D07A8 File Offset: 0x000CE9A8
	// (remove) Token: 0x06002F45 RID: 12101 RVA: 0x000D07DC File Offset: 0x000CE9DC
	public static event NoiseMaker.NoiseEvent NoiseCreatedInScene;

	// Token: 0x06002F46 RID: 12102 RVA: 0x000D080F File Offset: 0x000CEA0F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void RegisterReset()
	{
		SceneManager.sceneLoaded += delegate(Scene _, LoadSceneMode _)
		{
			NoiseMaker.NoiseCreatedInScene = null;
		};
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x000D0835 File Offset: 0x000CEA35
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.originOffset), this.radius);
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x000D0862 File Offset: 0x000CEA62
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (this.createNoiseOnEnable)
		{
			this.CreateNoise();
		}
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x000D087B File Offset: 0x000CEA7B
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x000D088A File Offset: 0x000CEA8A
	public void CreateNoise()
	{
		NoiseMaker.CreateNoise(base.transform.TransformPoint(this.originOffset), this.radius, this.intensity, this.allowOffScreen);
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x000D08C0 File Offset: 0x000CEAC0
	public static void CreateNoise(Vector2 worldPosition, float radius, NoiseMaker.Intensities intensity, bool allowOffScreen = false)
	{
		NoiseMaker.NoiseEvent noiseCreated = NoiseMaker.NoiseCreated;
		if (noiseCreated != null)
		{
			noiseCreated(worldPosition, (Vector2 objectPos) => Vector2.Distance(objectPos, worldPosition) <= radius, intensity, allowOffScreen);
		}
		NoiseMaker.NoiseEvent noiseCreatedInScene = NoiseMaker.NoiseCreatedInScene;
		if (noiseCreatedInScene == null)
		{
			return;
		}
		noiseCreatedInScene(worldPosition, (Vector2 objectPos) => Vector2.Distance(objectPos, worldPosition) <= radius, intensity, allowOffScreen);
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x000D0928 File Offset: 0x000CEB28
	public static void CreateNoise(Vector2 worldPosition, Vector2 size, NoiseMaker.Intensities intensity, bool allowOffScreen = false)
	{
		Vector2 b = size * 0.5f;
		Vector2 min = worldPosition - b;
		Vector2 max = worldPosition + b;
		NoiseMaker.NoiseEvent noiseCreated = NoiseMaker.NoiseCreated;
		if (noiseCreated != null)
		{
			noiseCreated(worldPosition, (Vector2 objectPos) => objectPos.x >= min.x && objectPos.x <= max.x && objectPos.y >= min.y && objectPos.y <= max.y, intensity, allowOffScreen);
		}
		NoiseMaker.NoiseEvent noiseCreatedInScene = NoiseMaker.NoiseCreatedInScene;
		if (noiseCreatedInScene == null)
		{
			return;
		}
		noiseCreatedInScene(worldPosition, (Vector2 objectPos) => objectPos.x >= min.x && objectPos.x <= max.x && objectPos.y >= min.y && objectPos.y <= max.y, intensity, allowOffScreen);
	}

	// Token: 0x0400320A RID: 12810
	[SerializeField]
	private Vector2 originOffset;

	// Token: 0x0400320B RID: 12811
	[SerializeField]
	private float radius;

	// Token: 0x0400320C RID: 12812
	[SerializeField]
	private NoiseMaker.Intensities intensity;

	// Token: 0x0400320D RID: 12813
	[SerializeField]
	private bool allowOffScreen;

	// Token: 0x0400320E RID: 12814
	[SerializeField]
	private bool createNoiseOnEnable;

	// Token: 0x0400320F RID: 12815
	private bool hasStarted;

	// Token: 0x02001833 RID: 6195
	public enum Intensities
	{
		// Token: 0x04009119 RID: 37145
		Normal,
		// Token: 0x0400911A RID: 37146
		Intense
	}

	// Token: 0x02001834 RID: 6196
	// (Invoke) Token: 0x06009052 RID: 36946
	public delegate bool NoiseEventCheck(Vector2 worldPosition);

	// Token: 0x02001835 RID: 6197
	// (Invoke) Token: 0x06009056 RID: 36950
	public delegate void NoiseEvent(Vector2 noiseSourcePos, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffScreen);
}
