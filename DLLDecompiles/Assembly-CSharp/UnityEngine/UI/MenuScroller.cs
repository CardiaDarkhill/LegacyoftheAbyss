using System;
using System.Collections;
using PolyAndCode.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000877 RID: 2167
	public class MenuScroller : UIBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
	{
		// Token: 0x06004B6C RID: 19308 RVA: 0x0016467C File Offset: 0x0016287C
		protected override void Awake()
		{
			base.Awake();
			this.inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x00164694 File Offset: 0x00162894
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Deselect();
			this.previousScrollDir = 0;
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x001646AC File Offset: 0x001628AC
		private void Update()
		{
			if (!this.isSelected)
			{
				return;
			}
			int num = 0;
			if (this.inputActions.Up.IsPressed)
			{
				num--;
			}
			if (this.inputActions.Down.IsPressed)
			{
				num++;
			}
			bool isFast = false;
			if (this.inputActions.RsUp.IsPressed)
			{
				num--;
				isFast = true;
			}
			if (this.inputActions.RsDown.IsPressed)
			{
				num++;
				isFast = true;
			}
			this.DoInput(num, isFast);
			this.previousScrollDir = num;
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00164731 File Offset: 0x00162931
		public void OnSelect(BaseEventData eventData)
		{
			this.isSelected = true;
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x0016473A File Offset: 0x0016293A
		public void OnDeselect(BaseEventData eventData)
		{
			this.Deselect();
		}

		// Token: 0x06004B71 RID: 19313 RVA: 0x00164742 File Offset: 0x00162942
		private void Deselect()
		{
			this.isSelected = false;
		}

		// Token: 0x06004B72 RID: 19314 RVA: 0x0016474C File Offset: 0x0016294C
		private void DoInput(int scrollDir, bool isFast)
		{
			if (scrollDir == 0)
			{
				if (this.previousScrollDir != 0 && this.scrollRoutine == null)
				{
					this.scrollRoutine = base.StartCoroutine(this.ScrollLerped(this.previousScrollDir));
				}
				this.nextScrollTime = 0.0;
				return;
			}
			if (Time.timeAsDouble < this.nextScrollTime)
			{
				return;
			}
			if (this.scrollRoutine != null)
			{
				base.StopCoroutine(this.scrollRoutine);
				this.scrollRoutine = null;
			}
			if (!isFast && this.previousScrollDir == 0)
			{
				this.nextScrollTime = Time.timeAsDouble + (double)this.scrollFirstDelay;
				this.scrollRoutine = base.StartCoroutine(this.ScrollLerped(scrollDir));
				return;
			}
			float num = this.scrollRect.GetCellSize() / this.scrollRect.GetContentSize();
			float num2 = isFast ? this.scrollRepeatAmountFast : this.scrollRepeatAmount;
			float num3 = this.scrollRect.GetScrollPosition();
			num3 += num * num2 * Mathf.Sign((float)scrollDir) * Time.deltaTime;
			this.scrollRect.SetScrollPosition(num3);
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x00164845 File Offset: 0x00162A45
		private IEnumerator ScrollLerped(int direction)
		{
			float num = this.scrollRect.GetCellSize() / this.scrollRect.GetContentSize();
			float num2 = num * Mathf.Sign((float)direction);
			float startScrollPos = this.scrollRect.GetScrollPosition();
			float endScrollPos = startScrollPos + num2;
			endScrollPos = Mathf.Round(endScrollPos / num) * num;
			for (float elapsed = 0f; elapsed < this.scrollLerpTime; elapsed += Time.deltaTime)
			{
				float t = this.scrollLerpCurve.Evaluate(elapsed / this.scrollLerpTime);
				float scrollPosition = Mathf.Lerp(startScrollPos, endScrollPos, t);
				this.scrollRect.SetScrollPosition(scrollPosition);
				yield return null;
			}
			this.scrollRect.SetScrollPosition(endScrollPos);
			yield break;
		}

		// Token: 0x04004CD6 RID: 19670
		[SerializeField]
		private RecyclableScrollRect scrollRect;

		// Token: 0x04004CD7 RID: 19671
		[SerializeField]
		private float scrollFirstDelay;

		// Token: 0x04004CD8 RID: 19672
		[SerializeField]
		private float scrollLerpTime;

		// Token: 0x04004CD9 RID: 19673
		[SerializeField]
		private AnimationCurve scrollLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04004CDA RID: 19674
		[SerializeField]
		private float scrollRepeatAmount;

		// Token: 0x04004CDB RID: 19675
		[SerializeField]
		private float scrollRepeatAmountFast;

		// Token: 0x04004CDC RID: 19676
		private int previousScrollDir;

		// Token: 0x04004CDD RID: 19677
		private bool isSelected;

		// Token: 0x04004CDE RID: 19678
		private double nextScrollTime;

		// Token: 0x04004CDF RID: 19679
		private Coroutine scrollRoutine;

		// Token: 0x04004CE0 RID: 19680
		private HeroActions inputActions;
	}
}
