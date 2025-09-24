using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class AudioPlayParticleCollision : MonoBehaviour
{
	// Token: 0x06000870 RID: 2160 RVA: 0x00027BAC File Offset: 0x00025DAC
	private void Awake()
	{
		this.system = base.GetComponent<ParticleSystem>();
		if (this.system)
		{
			this.system.collision.sendCollisionMessages = true;
		}
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00027BE8 File Offset: 0x00025DE8
	public void OnParticleCollision(GameObject other)
	{
		this.particleCollisions.Clear();
		this.system.GetCollisionEvents(other, this.particleCollisions);
		if (this.particleCollisions.Count == 0)
		{
			return;
		}
		float num = 0f;
		ParticleCollisionEvent particleCollisionEvent = this.particleCollisions[0];
		bool flag = false;
		foreach (ParticleCollisionEvent particleCollisionEvent2 in this.particleCollisions)
		{
			float sqrMagnitude = particleCollisionEvent2.velocity.sqrMagnitude;
			if (!float.IsNaN(sqrMagnitude) && !float.IsNaN(particleCollisionEvent2.intersection.sqrMagnitude) && sqrMagnitude > num)
			{
				num = sqrMagnitude;
				particleCollisionEvent = particleCollisionEvent2;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		num = Mathf.Sqrt(num);
		float num2 = (Math.Abs(this.collisionSpeedRange.Start - this.collisionSpeedRange.End) <= Mathf.Epsilon) ? 1f : this.collisionSpeedRange.GetTBetween(num);
		if (num2 <= Mathf.Epsilon)
		{
			return;
		}
		float num3 = this.speedVolumeCurve.Evaluate(num2);
		if (num3 <= Mathf.Epsilon)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.alwaysPlayTimer)
		{
			if (timeAsDouble < this.limitTimer)
			{
				return;
			}
			if (Random.Range(0f, 1f) > this.playChance)
			{
				return;
			}
		}
		this.alwaysPlayTimer = timeAsDouble + (double)this.alwaysPlayTime;
		this.limitTimer = timeAsDouble + (double)this.frequencyLimit;
		Vector3 intersection = particleCollisionEvent.intersection;
		float? z = new float?(base.transform.position.z);
		Vector3 position = intersection.Where(null, null, z);
		AudioSource prefab = this.sourcePrefabOverride ? this.sourcePrefabOverride : Audio.DefaultAudioSourcePrefab;
		this.clips.SpawnAndPlayOneShot(prefab, position, num3);
		this.table.SpawnAndPlayOneShot(prefab, position, false, num3, null);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00027DEC File Offset: 0x00025FEC
	public Vector3 FixNaN(Vector3 v)
	{
		bool flag = float.IsNaN(v.x);
		bool flag2 = float.IsNaN(v.y);
		bool flag3 = float.IsNaN(v.z);
		bool flag4 = flag || flag2 || flag3;
		return new Vector3(flag ? 0f : v.x, flag2 ? 0f : v.y, flag3 ? 0f : v.z);
	}

	// Token: 0x0400080C RID: 2060
	[SerializeField]
	private RandomAudioClipTable table;

	// Token: 0x0400080D RID: 2061
	[SerializeField]
	private AudioEventRandom clips;

	// Token: 0x0400080E RID: 2062
	[SerializeField]
	private AudioSource sourcePrefabOverride;

	// Token: 0x0400080F RID: 2063
	[Space]
	[SerializeField]
	private float alwaysPlayTime;

	// Token: 0x04000810 RID: 2064
	[SerializeField]
	private float frequencyLimit;

	// Token: 0x04000811 RID: 2065
	[SerializeField]
	[Range(0f, 1f)]
	private float playChance = 1f;

	// Token: 0x04000812 RID: 2066
	[SerializeField]
	private MinMaxFloat collisionSpeedRange = new MinMaxFloat(2f, 10f);

	// Token: 0x04000813 RID: 2067
	[SerializeField]
	private AnimationCurve speedVolumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000814 RID: 2068
	private double alwaysPlayTimer;

	// Token: 0x04000815 RID: 2069
	private double limitTimer;

	// Token: 0x04000816 RID: 2070
	private ParticleSystem system;

	// Token: 0x04000817 RID: 2071
	private readonly List<ParticleCollisionEvent> particleCollisions = new List<ParticleCollisionEvent>();
}
