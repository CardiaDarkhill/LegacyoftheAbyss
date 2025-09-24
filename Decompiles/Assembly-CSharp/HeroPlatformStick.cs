using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class HeroPlatformStick : MonoBehaviour
{
	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000078 RID: 120 RVA: 0x000042FC File Offset: 0x000024FC
	public Transform SetParent
	{
		get
		{
			if (!this.setParent)
			{
				return base.transform;
			}
			return this.setParent;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000079 RID: 121 RVA: 0x00004318 File Offset: 0x00002518
	// (set) Token: 0x0600007A RID: 122 RVA: 0x00004323 File Offset: 0x00002523
	public bool IsActive
	{
		get
		{
			return !this.isInactive;
		}
		set
		{
			this.isInactive = !value;
			this.Refresh();
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004338 File Offset: 0x00002538
	private void Awake()
	{
		Transform transform = base.transform;
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		Vector3 lossyScale = transform.lossyScale;
		transform.localScale = Vector3.one;
		Vector3 lossyScale2 = transform.lossyScale;
		Vector3 v = lossyScale.DivideElements(lossyScale2);
		component.size = component.size.MultiplyElements(v);
		component.offset = component.offset.MultiplyElements(v);
		HeroPlatformStick.IMoveHooks componentInParent = base.GetComponentInParent<HeroPlatformStick.IMoveHooks>();
		if (componentInParent != null)
		{
			this.isInactive = true;
			componentInParent.AddMoveHooks(delegate
			{
				this.isInactive = false;
				this.Refresh();
			}, delegate
			{
				this.isInactive = true;
				this.Refresh();
			});
		}
		HeroPlatformStick.ITouchHooks componentInParent2 = base.GetComponentInParent<HeroPlatformStick.ITouchHooks>();
		if (componentInParent2 != null)
		{
			this.isInactive = true;
			componentInParent2.AddTouchHooks(delegate
			{
				this.isForcedTouching = true;
				this.Refresh();
			}, delegate
			{
				this.isForcedTouching = false;
				this.Refresh();
			});
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004404 File Offset: 0x00002604
	private void OnEnable()
	{
		if (this.insideTracker)
		{
			this.insideTracker.InsideStateChanged += this.OnInsideStateChanged;
		}
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000442A File Offset: 0x0000262A
	private void OnDisable()
	{
		if (this.insideTracker)
		{
			this.insideTracker.InsideStateChanged -= this.OnInsideStateChanged;
		}
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00004450 File Offset: 0x00002650
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.insideTracker)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 9)
		{
			return;
		}
		HeroController component = gameObject.GetComponent<HeroController>();
		if (!component)
		{
			return;
		}
		if (collision.GetSafeContact().Normal.y >= 0f)
		{
			return;
		}
		if (HeroPlatformStick._waitRoutine != null)
		{
			base.StopCoroutine(HeroPlatformStick._waitRoutine);
			HeroPlatformStick._waitRoutine = null;
		}
		if (component.cState.onGround)
		{
			this.wasInside = true;
			this.Refresh();
			return;
		}
		HeroPlatformStick._waitRoutine = base.StartCoroutine(this.WaitForGrounded(component));
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000044EC File Offset: 0x000026EC
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (this.insideTracker)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 9)
		{
			return;
		}
		if (!gameObject.GetComponent<HeroController>())
		{
			return;
		}
		if (HeroPlatformStick._waitRoutine != null)
		{
			base.StopCoroutine(HeroPlatformStick._waitRoutine);
			HeroPlatformStick._waitRoutine = null;
		}
		this.wasInside = false;
		this.Refresh();
	}

	// Token: 0x06000080 RID: 128 RVA: 0x0000454C File Offset: 0x0000274C
	private IEnumerator WaitForGrounded(HeroController heroController)
	{
		while (!heroController.cState.onGround)
		{
			yield return null;
		}
		this.wasInside = true;
		this.Refresh();
		yield break;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004564 File Offset: 0x00002764
	private void DoParent(HeroController heroController)
	{
		heroController.SetHeroParent(this.SetParent);
		Rigidbody2D body = heroController.Body;
		if (body != null)
		{
			body.interpolation = RigidbodyInterpolation2D.None;
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004594 File Offset: 0x00002794
	private void DoDeparent(HeroController heroController)
	{
		heroController.SetHeroParent(null);
		Rigidbody2D component = heroController.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			component.interpolation = RigidbodyInterpolation2D.Interpolate;
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000045BF File Offset: 0x000027BF
	private void OnInsideStateChanged(bool isInside)
	{
		this.wasInside = isInside;
		this.Refresh();
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000045D0 File Offset: 0x000027D0
	private void Refresh()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		if ((this.wasInside || this.isForcedTouching) && !this.isInactive)
		{
			this.DoParent(silentInstance);
			return;
		}
		if (silentInstance.transform.parent != this.SetParent)
		{
			return;
		}
		this.DoDeparent(silentInstance);
	}

	// Token: 0x0400005C RID: 92
	[Header("Optional")]
	[SerializeField]
	private TrackTriggerObjects insideTracker;

	// Token: 0x0400005D RID: 93
	[SerializeField]
	private Transform setParent;

	// Token: 0x0400005E RID: 94
	private bool isInactive;

	// Token: 0x0400005F RID: 95
	private bool wasInside;

	// Token: 0x04000060 RID: 96
	private bool isForcedTouching;

	// Token: 0x04000061 RID: 97
	private static Coroutine _waitRoutine;

	// Token: 0x020013B9 RID: 5049
	public interface IMoveHooks
	{
		// Token: 0x0600814A RID: 33098
		void AddMoveHooks(Action onStartMove, Action onStopMove);
	}

	// Token: 0x020013BA RID: 5050
	public interface ITouchHooks
	{
		// Token: 0x0600814B RID: 33099
		void AddTouchHooks(Action onStartTouching, Action onStopTouching);
	}
}
