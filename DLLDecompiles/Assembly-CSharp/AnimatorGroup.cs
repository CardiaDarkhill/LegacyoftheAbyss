using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class AnimatorGroup : MonoBehaviour
{
	// Token: 0x0600027D RID: 637 RVA: 0x0000E99B File Offset: 0x0000CB9B
	private void Awake()
	{
		if (this.useChildren)
		{
			this.animators.AddRange(base.GetComponentsInChildren<Animator>());
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x0000E9B6 File Offset: 0x0000CBB6
	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(this.startAnim))
		{
			this.Play(this.startAnim);
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000E9D4 File Offset: 0x0000CBD4
	private void Update()
	{
		if (this.delayedPlays == null)
		{
			return;
		}
		foreach (KeyValuePair<string, List<AnimatorGroup.DelayedPlay>> keyValuePair in this.delayedPlays)
		{
			List<AnimatorGroup.DelayedPlay> value = keyValuePair.Value;
			for (int i = value.Count - 1; i >= 0; i--)
			{
				AnimatorGroup.DelayedPlay delayedPlay = value[i];
				delayedPlay.PlayDelayLeft -= Time.deltaTime;
				if (delayedPlay.PlayDelayLeft > 0f)
				{
					value[i] = delayedPlay;
				}
				else
				{
					delayedPlay.Action(delayedPlay.Animator);
					value.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000EA90 File Offset: 0x0000CC90
	private void DoAction(string key, Action<Animator> action)
	{
		if (this.delayVariance <= 0f)
		{
			using (List<Animator>.Enumerator enumerator = this.animators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Animator obj = enumerator.Current;
					action(obj);
				}
				return;
			}
		}
		if (this.delayedPlays == null)
		{
			this.delayedPlays = new Dictionary<string, List<AnimatorGroup.DelayedPlay>>();
		}
		List<AnimatorGroup.DelayedPlay> list;
		if (this.delayedPlays.TryGetValue(key, out list))
		{
			list.Clear();
		}
		else
		{
			list = new List<AnimatorGroup.DelayedPlay>(this.animators.Count);
			this.delayedPlays[key] = list;
		}
		float num = 0f;
		foreach (Animator animator in this.animators)
		{
			float playDelayLeft = Random.Range(0f, this.delayVariance) + num;
			list.Add(new AnimatorGroup.DelayedPlay
			{
				PlayDelayLeft = playDelayLeft,
				Animator = animator,
				Action = action
			});
			num += this.delayInSequence;
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000EBC8 File Offset: 0x0000CDC8
	public void Play(string stateName)
	{
		int animHash = Animator.StringToHash(stateName);
		this.DoAction(stateName, delegate(Animator animator)
		{
			animator.Play(animHash, 0, 0f);
		});
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000EBFC File Offset: 0x0000CDFC
	public void SetBool(string boolName, bool value)
	{
		int boolHash = Animator.StringToHash(boolName);
		this.DoAction(boolName, delegate(Animator animator)
		{
			animator.SetBool(boolHash, value);
		});
	}

	// Token: 0x04000224 RID: 548
	[SerializeField]
	private List<Animator> animators;

	// Token: 0x04000225 RID: 549
	[SerializeField]
	private bool useChildren;

	// Token: 0x04000226 RID: 550
	[Space]
	[SerializeField]
	private string startAnim;

	// Token: 0x04000227 RID: 551
	[SerializeField]
	private float delayVariance;

	// Token: 0x04000228 RID: 552
	[SerializeField]
	private float delayInSequence;

	// Token: 0x04000229 RID: 553
	private Dictionary<string, List<AnimatorGroup.DelayedPlay>> delayedPlays;

	// Token: 0x020013DB RID: 5083
	private struct DelayedPlay
	{
		// Token: 0x040080ED RID: 33005
		public float PlayDelayLeft;

		// Token: 0x040080EE RID: 33006
		public Animator Animator;

		// Token: 0x040080EF RID: 33007
		public Action<Animator> Action;
	}
}
