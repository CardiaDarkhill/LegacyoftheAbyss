using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class ColourDistanceSilhouette : MonoBehaviour
{
	// Token: 0x06000359 RID: 857 RVA: 0x00011A56 File Offset: 0x0000FC56
	private void Reset()
	{
		this.tk2dSprite = base.GetComponent<tk2dSprite>();
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00011A64 File Offset: 0x0000FC64
	private void OnValidate()
	{
		if (this.distanceRange.Start < 0f)
		{
			this.distanceRange.Start = 0f;
		}
		if (this.distanceRange.End < this.distanceRange.Start)
		{
			this.distanceRange.End = this.distanceRange.Start;
		}
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00011AC4 File Offset: 0x0000FCC4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 position = base.transform.position;
		Gizmos.DrawWireSphere(position, this.distanceRange.Start);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(position, this.distanceRange.End);
		if (!this.hero)
		{
			HeroController silentInstance = HeroController.SilentInstance;
			if (silentInstance)
			{
				this.hero = silentInstance.transform;
			}
		}
		if (this.hero)
		{
			float t = this.GetT();
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(position, this.distanceRange.GetLerpedValue(t));
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00011B6A File Offset: 0x0000FD6A
	private void OnEnable()
	{
		if (!this.tk2dSprite)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00011B80 File Offset: 0x0000FD80
	private void Start()
	{
		this.hero = HeroController.instance.transform;
		this.startColour = this.tk2dSprite.color;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00011BA4 File Offset: 0x0000FDA4
	private void Update()
	{
		if (this.isActive)
		{
			float t = this.distanceCurve.Evaluate(this.GetT());
			Color color = Color.Lerp(this.startColour, this.targetColour, t);
			this.tk2dSprite.color = color;
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00011BEC File Offset: 0x0000FDEC
	private float GetT()
	{
		Vector2 a = this.hero.position;
		Vector2 b = base.transform.position;
		return (Mathf.Clamp(Vector2.Distance(a, b), this.distanceRange.Start, this.distanceRange.End) - this.distanceRange.Start) / (this.distanceRange.End - this.distanceRange.Start);
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00011C5F File Offset: 0x0000FE5F
	public void SetActive(bool setActive)
	{
		this.isActive = setActive;
	}

	// Token: 0x04000306 RID: 774
	[SerializeField]
	private tk2dSprite tk2dSprite;

	// Token: 0x04000307 RID: 775
	[SerializeField]
	private MinMaxFloat distanceRange;

	// Token: 0x04000308 RID: 776
	[SerializeField]
	private AnimationCurve distanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000309 RID: 777
	[SerializeField]
	private Color targetColour = Color.black;

	// Token: 0x0400030A RID: 778
	[SerializeField]
	private bool isActive = true;

	// Token: 0x0400030B RID: 779
	private Color startColour;

	// Token: 0x0400030C RID: 780
	private Transform hero;
}
