using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000517 RID: 1303
public class LiftPlatform : DebugDrawColliderRuntimeAdder
{
	// Token: 0x06002E96 RID: 11926 RVA: 0x000CD2C4 File Offset: 0x000CB4C4
	private void OnValidate()
	{
		if (this.part1)
		{
			this.parts.Add(new LiftPlatform.LiftPart
			{
				Transform = this.part1.transform,
				Delay = new MinMaxFloat(0f, 0f),
				Magnitude = new MinMaxFloat(0.75f, 0.75f)
			});
			this.part1 = null;
		}
		if (this.part2)
		{
			this.parts.Add(new LiftPlatform.LiftPart
			{
				Transform = this.part2.transform,
				Delay = new MinMaxFloat(0f, 0f),
				Magnitude = new MinMaxFloat(0.75f, 0.75f)
			});
			this.part2 = null;
		}
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x000CD38F File Offset: 0x000CB58F
	private void Reset()
	{
		this.parts = new List<LiftPlatform.LiftPart>
		{
			new LiftPlatform.LiftPart()
		};
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x000CD3A7 File Offset: 0x000CB5A7
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000CD3B8 File Offset: 0x000CB5B8
	private void OnDisable()
	{
		if (this.runningTimers != null)
		{
			foreach (LiftPlatform.PartTimer partTimer in this.runningTimers)
			{
				partTimer.Part.SetLocalPositionY(partTimer.InitialYPos);
			}
			this.runningTimers.Clear();
		}
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000CD428 File Offset: 0x000CB628
	private void LateUpdate()
	{
		if (this.runningTimers != null)
		{
			int i = this.runningTimers.Count - 1;
			while (i >= 0)
			{
				LiftPlatform.PartTimer partTimer = this.runningTimers[i];
				if (partTimer.DelayLeft > 0f)
				{
					partTimer.DelayLeft -= Time.deltaTime;
					goto IL_132;
				}
				LiftPlatform.PartStates state = partTimer.State;
				if (state != LiftPlatform.PartStates.Down)
				{
					if (state != LiftPlatform.PartStates.Up)
					{
						goto IL_132;
					}
					if (partTimer.Timer > 0f)
					{
						partTimer.Part.SetLocalPositionY(partTimer.InitialYPos - partTimer.Timer * partTimer.Magnitude);
						partTimer.Timer -= Time.deltaTime;
						goto IL_132;
					}
					partTimer.Part.SetLocalPositionY(partTimer.InitialYPos);
					this.runningTimers.RemoveAt(i);
				}
				else
				{
					if (partTimer.Timer < 0.12f)
					{
						partTimer.Part.SetLocalPositionY(partTimer.InitialYPos - partTimer.Timer * partTimer.Magnitude);
						partTimer.Timer += Time.deltaTime;
						goto IL_132;
					}
					partTimer.Part.SetLocalPositionY(partTimer.InitialYPos - 0.12f * partTimer.Magnitude);
					partTimer.State = LiftPlatform.PartStates.Up;
					partTimer.Timer = 0.12f;
					goto IL_132;
				}
				IL_13F:
				i--;
				continue;
				IL_132:
				this.runningTimers[i] = partTimer;
				goto IL_13F;
			}
		}
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000CD580 File Offset: 0x000CB780
	private void OnCollisionEnter2D(Collision2D collision)
	{
		int layer = collision.collider.gameObject.layer;
		if ((1 << layer & this.collisionMask) == 0)
		{
			return;
		}
		if (layer == 16 || layer == 18 || collision.GetSafeContact().Normal.y >= 0f)
		{
			return;
		}
		if (layer != 9 && collision.relativeVelocity.magnitude < 6f)
		{
			return;
		}
		this.DoBob();
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000CD5F6 File Offset: 0x000CB7F6
	public void DoBob()
	{
		this.DoBobInternal(true, true);
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000CD600 File Offset: 0x000CB800
	public void DoBob(bool vibrate)
	{
		this.DoBobInternal(true, vibrate);
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x000CD60A File Offset: 0x000CB80A
	public void DoBobSilent()
	{
		this.DoBobInternal(false, false);
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x000CD614 File Offset: 0x000CB814
	private void DoBobInternal(bool playSound, bool vibrate)
	{
		if (!base.enabled || this.isDisabled)
		{
			return;
		}
		if (playSound)
		{
			if (this.audioSource)
			{
				this.audioSource.pitch = Random.Range(0.85f, 1.15f);
				this.audioSource.Play();
			}
			this.landAudio.SpawnAndPlayOneShot(base.transform.position, null);
			RandomAudioClipTable[] array = this.landAudioTables;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SpawnAndPlayOneShot(base.transform.position, false);
			}
			if (vibrate)
			{
				VibrationManager.PlayVibrationClipOneShot(this.bobVibration, null, false, "", false);
			}
		}
		if (this.dustParticle)
		{
			this.dustParticle.Play();
		}
		if (this.runningTimers == null)
		{
			this.runningTimers = new List<LiftPlatform.PartTimer>(this.parts.Count);
		}
		else
		{
			foreach (LiftPlatform.PartTimer partTimer in this.runningTimers)
			{
				partTimer.Part.SetLocalPositionY(partTimer.InitialYPos);
			}
			this.runningTimers.Clear();
		}
		foreach (LiftPlatform.LiftPart liftPart in this.parts)
		{
			if (liftPart.Transform)
			{
				this.runningTimers.Add(new LiftPlatform.PartTimer
				{
					DelayLeft = liftPart.Delay.GetRandomValue(),
					Part = liftPart.Transform,
					InitialYPos = liftPart.Transform.localPosition.y,
					Magnitude = liftPart.Magnitude.GetRandomValue()
				});
			}
		}
		this.OnBob.Invoke();
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000CD820 File Offset: 0x000CBA20
	public void SetActive(bool active)
	{
		this.isDisabled = !active;
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000CD82C File Offset: 0x000CBA2C
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider, false);
	}

	// Token: 0x0400312A RID: 12586
	private const float MIN_REACT_MAGNITUDE = 6f;

	// Token: 0x0400312B RID: 12587
	[SerializeField]
	private LayerMask collisionMask = -1;

	// Token: 0x0400312C RID: 12588
	[Space]
	[SerializeField]
	private List<LiftPlatform.LiftPart> parts;

	// Token: 0x0400312D RID: 12589
	[SerializeField]
	private ParticleSystem dustParticle;

	// Token: 0x0400312E RID: 12590
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400312F RID: 12591
	[SerializeField]
	private AudioEventRandom landAudio;

	// Token: 0x04003130 RID: 12592
	[SerializeField]
	private RandomAudioClipTable[] landAudioTables;

	// Token: 0x04003131 RID: 12593
	[Space]
	[SerializeField]
	private VibrationDataAsset bobVibration;

	// Token: 0x04003132 RID: 12594
	[Space]
	public UnityEvent OnBob;

	// Token: 0x04003133 RID: 12595
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject part1;

	// Token: 0x04003134 RID: 12596
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject part2;

	// Token: 0x04003135 RID: 12597
	private List<LiftPlatform.PartTimer> runningTimers;

	// Token: 0x04003136 RID: 12598
	private bool isDisabled;

	// Token: 0x02001818 RID: 6168
	private enum PartStates
	{
		// Token: 0x040090B1 RID: 37041
		Down,
		// Token: 0x040090B2 RID: 37042
		Up
	}

	// Token: 0x02001819 RID: 6169
	private struct PartTimer
	{
		// Token: 0x040090B3 RID: 37043
		public float DelayLeft;

		// Token: 0x040090B4 RID: 37044
		public LiftPlatform.PartStates State;

		// Token: 0x040090B5 RID: 37045
		public float Timer;

		// Token: 0x040090B6 RID: 37046
		public Transform Part;

		// Token: 0x040090B7 RID: 37047
		public float Magnitude;

		// Token: 0x040090B8 RID: 37048
		public float InitialYPos;
	}

	// Token: 0x0200181A RID: 6170
	[Serializable]
	private class LiftPart
	{
		// Token: 0x040090B9 RID: 37049
		public Transform Transform;

		// Token: 0x040090BA RID: 37050
		public MinMaxFloat Delay;

		// Token: 0x040090BB RID: 37051
		public MinMaxFloat Magnitude = new MinMaxFloat(0.75f, 0.75f);
	}
}
