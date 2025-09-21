using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029E RID: 670
public class BlackThreadCore : MonoBehaviour
{
	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001766 RID: 5990 RVA: 0x00069894 File Offset: 0x00067A94
	public static bool IsAnyActive
	{
		get
		{
			if (BlackThreadCore._activeCores == null)
			{
				return false;
			}
			bool result = false;
			using (List<BlackThreadCore>.Enumerator enumerator = BlackThreadCore._activeCores.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsActive)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001767 RID: 5991 RVA: 0x000698F8 File Offset: 0x00067AF8
	private bool IsActive
	{
		get
		{
			PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
			if (component)
			{
				component.PreSetup();
				if (component.GetCurrentValue())
				{
					return false;
				}
			}
			return !this.wasDeactivated && base.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x0006993C File Offset: 0x00067B3C
	private void Awake()
	{
		if (BlackThreadCore._activeCores == null)
		{
			BlackThreadCore._activeCores = new List<BlackThreadCore>();
		}
		BlackThreadCore._activeCores.Add(this);
		bool flag = this.core != null;
		if (!flag)
		{
			this.core = base.transform.Find("Core");
			flag = (this.core != null);
		}
		if (flag)
		{
			foreach (SpriteRenderer spriteRenderer in base.GetComponentsInChildren<SpriteRenderer>(false))
			{
				if (!spriteRenderer.CompareTag("HeroLight"))
				{
					this.tintTargets.Add(new BlackThreadCore.SpriteRendererTintHelper(spriteRenderer));
				}
			}
			foreach (tk2dSprite target in base.GetComponentsInChildren<tk2dSprite>())
			{
				this.tintTargets.Add(new BlackThreadCore.Tk2dSpriteTintHelper(target));
			}
		}
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x00069A02 File Offset: 0x00067C02
	private void OnEnable()
	{
		this.wasDeactivated = false;
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x00069A0B File Offset: 0x00067C0B
	private void OnDestroy()
	{
		if (BlackThreadCore._activeCores == null)
		{
			return;
		}
		BlackThreadCore._activeCores.Remove(this);
		if (BlackThreadCore._activeCores.Count == 0)
		{
			BlackThreadCore._activeCores = null;
		}
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x00069A33 File Offset: 0x00067C33
	public void Deactivate()
	{
		this.wasDeactivated = true;
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x00069A3C File Offset: 0x00067C3C
	public void LerpToColor(Color color, float duration)
	{
		if (this.lerpRoutine != null)
		{
			base.StopCoroutine(this.lerpRoutine);
			this.lerpRoutine = null;
		}
		if (duration <= 0f)
		{
			foreach (BlackThreadCore.TintHelper tintHelper in this.tintTargets)
			{
				tintHelper.Color = color;
			}
			return;
		}
		this.lerpRoutine = base.StartCoroutine(this.LerpToColorRoutine(color, duration));
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00069AC8 File Offset: 0x00067CC8
	public void LerpToOriginal(float duration)
	{
		if (this.lerpRoutine != null)
		{
			base.StopCoroutine(this.lerpRoutine);
			this.lerpRoutine = null;
		}
		if (duration <= 0f)
		{
			foreach (BlackThreadCore.TintHelper tintHelper in this.tintTargets)
			{
				tintHelper.RestoreOriginalColor();
			}
			return;
		}
		this.lerpRoutine = base.StartCoroutine(this.LerpToOriginalRoutine(duration));
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x00069B50 File Offset: 0x00067D50
	private IEnumerator LerpToColorRoutine(Color color, float duration)
	{
		if (duration > 0f)
		{
			float multi = 1f / duration;
			float t = 0f;
			using (List<BlackThreadCore.TintHelper>.Enumerator enumerator = this.tintTargets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BlackThreadCore.TintHelper tintHelper = enumerator.Current;
					tintHelper.BeginLerpTo(color);
				}
				goto IL_E4;
			}
			IL_87:
			t += Time.deltaTime * multi;
			for (int i = 0; i < this.tintTargets.Count; i++)
			{
				this.tintTargets[i].LerpToTarget(t);
			}
			yield return null;
			IL_E4:
			if (t < 1f)
			{
				goto IL_87;
			}
		}
		foreach (BlackThreadCore.TintHelper tintHelper2 in this.tintTargets)
		{
			tintHelper2.Color = color;
		}
		this.lerpRoutine = null;
		yield break;
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x00069B6D File Offset: 0x00067D6D
	private IEnumerator LerpToOriginalRoutine(float duration)
	{
		if (duration > 0f)
		{
			float multi = 1f / duration;
			float t = 0f;
			using (List<BlackThreadCore.TintHelper>.Enumerator enumerator = this.tintTargets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BlackThreadCore.TintHelper tintHelper = enumerator.Current;
					tintHelper.BeginLerpTo(tintHelper.OriginalColor);
				}
				goto IL_E4;
			}
			IL_87:
			t += Time.deltaTime * multi;
			for (int i = 0; i < this.tintTargets.Count; i++)
			{
				this.tintTargets[i].LerpToTarget(t);
			}
			yield return null;
			IL_E4:
			if (t < 1f)
			{
				goto IL_87;
			}
		}
		foreach (BlackThreadCore.TintHelper tintHelper2 in this.tintTargets)
		{
			tintHelper2.RestoreOriginalColor();
		}
		this.lerpRoutine = null;
		yield break;
	}

	// Token: 0x04001610 RID: 5648
	[SerializeField]
	private Transform core;

	// Token: 0x04001611 RID: 5649
	private bool wasDeactivated;

	// Token: 0x04001612 RID: 5650
	private static List<BlackThreadCore> _activeCores;

	// Token: 0x04001613 RID: 5651
	private List<BlackThreadCore.TintHelper> tintTargets = new List<BlackThreadCore.TintHelper>();

	// Token: 0x04001614 RID: 5652
	private Coroutine lerpRoutine;

	// Token: 0x0200156A RID: 5482
	public abstract class TintHelper
	{
		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x060086BD RID: 34493
		// (set) Token: 0x060086BE RID: 34494
		public abstract Color Color { get; set; }

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x060086BF RID: 34495 RVA: 0x00273D3F File Offset: 0x00271F3F
		public Color OriginalColor
		{
			get
			{
				return this.originalColor;
			}
		}

		// Token: 0x060086C0 RID: 34496 RVA: 0x00273D47 File Offset: 0x00271F47
		protected void RecordOriginalColor()
		{
			if (!this.hasRecordedOriginal)
			{
				this.originalColor = this.Color;
				this.hasRecordedOriginal = true;
			}
		}

		// Token: 0x060086C1 RID: 34497 RVA: 0x00273D64 File Offset: 0x00271F64
		public void RestoreOriginalColor()
		{
			if (this.hasRecordedOriginal)
			{
				this.Color = this.originalColor;
			}
		}

		// Token: 0x060086C2 RID: 34498 RVA: 0x00273D7A File Offset: 0x00271F7A
		private void RecordStartColor()
		{
			this.startColor = this.Color;
		}

		// Token: 0x060086C3 RID: 34499 RVA: 0x00273D88 File Offset: 0x00271F88
		public void BeginLerpTo(Color newTargetColor)
		{
			this.RecordStartColor();
			this.targetColor = newTargetColor;
		}

		// Token: 0x060086C4 RID: 34500 RVA: 0x00273D97 File Offset: 0x00271F97
		public void LerpToTarget(float t)
		{
			this.Color = Color.Lerp(this.startColor, this.targetColor, t);
		}

		// Token: 0x060086C5 RID: 34501 RVA: 0x00273DB1 File Offset: 0x00271FB1
		public void LerpFromOriginalTo(Color newTargetColor, float t)
		{
			if (this.hasRecordedOriginal)
			{
				this.Color = Color.Lerp(this.originalColor, newTargetColor, t);
			}
		}

		// Token: 0x04008713 RID: 34579
		protected Color originalColor;

		// Token: 0x04008714 RID: 34580
		protected Color startColor;

		// Token: 0x04008715 RID: 34581
		protected Color targetColor;

		// Token: 0x04008716 RID: 34582
		protected bool hasRecordedOriginal;
	}

	// Token: 0x0200156B RID: 5483
	public sealed class SpriteRendererTintHelper : BlackThreadCore.TintHelper
	{
		// Token: 0x060086C7 RID: 34503 RVA: 0x00273DD6 File Offset: 0x00271FD6
		public SpriteRendererTintHelper(SpriteRenderer target)
		{
			this.target = target;
			base.RecordOriginalColor();
		}

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x060086C8 RID: 34504 RVA: 0x00273DEB File Offset: 0x00271FEB
		// (set) Token: 0x060086C9 RID: 34505 RVA: 0x00273DF8 File Offset: 0x00271FF8
		public override Color Color
		{
			get
			{
				return this.target.color;
			}
			set
			{
				this.target.color = value;
			}
		}

		// Token: 0x04008717 RID: 34583
		private readonly SpriteRenderer target;
	}

	// Token: 0x0200156C RID: 5484
	public sealed class Tk2dSpriteTintHelper : BlackThreadCore.TintHelper
	{
		// Token: 0x060086CA RID: 34506 RVA: 0x00273E06 File Offset: 0x00272006
		public Tk2dSpriteTintHelper(tk2dSprite target)
		{
			this.target = target;
			base.RecordOriginalColor();
		}

		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x060086CB RID: 34507 RVA: 0x00273E1B File Offset: 0x0027201B
		// (set) Token: 0x060086CC RID: 34508 RVA: 0x00273E28 File Offset: 0x00272028
		public override Color Color
		{
			get
			{
				return this.target.color;
			}
			set
			{
				this.target.color = value;
			}
		}

		// Token: 0x04008718 RID: 34584
		private readonly tk2dSprite target;
	}
}
