using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000921 RID: 2337
	public class TouchButtonControl : TouchControl
	{
		// Token: 0x060052B7 RID: 21175 RVA: 0x0017AE30 File Offset: 0x00179030
		public override void CreateControl()
		{
			this.button.Create("Button", base.transform, 1000);
		}

		// Token: 0x060052B8 RID: 21176 RVA: 0x0017AE4D File Offset: 0x0017904D
		public override void DestroyControl()
		{
			this.button.Delete();
			if (this.currentTouch != null)
			{
				this.TouchEnded(this.currentTouch);
				this.currentTouch = null;
			}
		}

		// Token: 0x060052B9 RID: 21177 RVA: 0x0017AE75 File Offset: 0x00179075
		public override void ConfigureControl()
		{
			base.transform.position = base.OffsetToWorldPosition(this.anchor, this.offset, this.offsetUnitType, this.lockAspectRatio);
			this.button.Update(true);
		}

		// Token: 0x060052BA RID: 21178 RVA: 0x0017AEAC File Offset: 0x001790AC
		public override void DrawGizmos()
		{
			this.button.DrawGizmos(this.ButtonPosition, Color.yellow);
		}

		// Token: 0x060052BB RID: 21179 RVA: 0x0017AEC4 File Offset: 0x001790C4
		private void Update()
		{
			if (this.dirty)
			{
				this.ConfigureControl();
				this.dirty = false;
				return;
			}
			this.button.Update();
		}

		// Token: 0x060052BC RID: 21180 RVA: 0x0017AEE8 File Offset: 0x001790E8
		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			if (this.pressureSensitive)
			{
				float num = 0f;
				if (this.currentTouch == null)
				{
					if (this.allowSlideToggle)
					{
						int touchCount = TouchManager.TouchCount;
						for (int i = 0; i < touchCount; i++)
						{
							Touch touch = TouchManager.GetTouch(i);
							if (this.button.Contains(touch))
							{
								num = Utility.Max(num, touch.NormalizedPressure);
							}
						}
					}
				}
				else
				{
					num = this.currentTouch.NormalizedPressure;
				}
				this.ButtonState = (num > 0f);
				base.SubmitButtonValue(this.target, num, updateTick, deltaTime);
				return;
			}
			if (this.currentTouch == null && this.allowSlideToggle)
			{
				this.ButtonState = false;
				int touchCount2 = TouchManager.TouchCount;
				for (int j = 0; j < touchCount2; j++)
				{
					this.ButtonState = (this.ButtonState || this.button.Contains(TouchManager.GetTouch(j)));
				}
			}
			base.SubmitButtonState(this.target, this.ButtonState, updateTick, deltaTime);
		}

		// Token: 0x060052BD RID: 21181 RVA: 0x0017AFD9 File Offset: 0x001791D9
		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			base.CommitButton(this.target);
		}

		// Token: 0x060052BE RID: 21182 RVA: 0x0017AFE7 File Offset: 0x001791E7
		public override void TouchBegan(Touch touch)
		{
			if (this.currentTouch != null)
			{
				return;
			}
			if (this.button.Contains(touch))
			{
				this.ButtonState = true;
				this.currentTouch = touch;
			}
		}

		// Token: 0x060052BF RID: 21183 RVA: 0x0017B00E File Offset: 0x0017920E
		public override void TouchMoved(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			if (this.toggleOnLeave && !this.button.Contains(touch))
			{
				this.ButtonState = false;
				this.currentTouch = null;
			}
		}

		// Token: 0x060052C0 RID: 21184 RVA: 0x0017B03E File Offset: 0x0017923E
		public override void TouchEnded(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			this.ButtonState = false;
			this.currentTouch = null;
		}

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x060052C1 RID: 21185 RVA: 0x0017B058 File Offset: 0x00179258
		// (set) Token: 0x060052C2 RID: 21186 RVA: 0x0017B060 File Offset: 0x00179260
		private bool ButtonState
		{
			get
			{
				return this.buttonState;
			}
			set
			{
				if (this.buttonState != value)
				{
					this.buttonState = value;
					this.button.State = value;
				}
			}
		}

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x060052C3 RID: 21187 RVA: 0x0017B07E File Offset: 0x0017927E
		// (set) Token: 0x060052C4 RID: 21188 RVA: 0x0017B0A4 File Offset: 0x001792A4
		public Vector3 ButtonPosition
		{
			get
			{
				if (!this.button.Ready)
				{
					return base.transform.position;
				}
				return this.button.Position;
			}
			set
			{
				if (this.button.Ready)
				{
					this.button.Position = value;
				}
			}
		}

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x060052C5 RID: 21189 RVA: 0x0017B0BF File Offset: 0x001792BF
		// (set) Token: 0x060052C6 RID: 21190 RVA: 0x0017B0C7 File Offset: 0x001792C7
		public TouchControlAnchor Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				if (this.anchor != value)
				{
					this.anchor = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x060052C7 RID: 21191 RVA: 0x0017B0E0 File Offset: 0x001792E0
		// (set) Token: 0x060052C8 RID: 21192 RVA: 0x0017B0E8 File Offset: 0x001792E8
		public Vector2 Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				if (this.offset != value)
				{
					this.offset = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x060052C9 RID: 21193 RVA: 0x0017B106 File Offset: 0x00179306
		// (set) Token: 0x060052CA RID: 21194 RVA: 0x0017B10E File Offset: 0x0017930E
		public TouchUnitType OffsetUnitType
		{
			get
			{
				return this.offsetUnitType;
			}
			set
			{
				if (this.offsetUnitType != value)
				{
					this.offsetUnitType = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x040052D7 RID: 21207
		[Header("Position")]
		[SerializeField]
		private TouchControlAnchor anchor = TouchControlAnchor.BottomRight;

		// Token: 0x040052D8 RID: 21208
		[SerializeField]
		private TouchUnitType offsetUnitType;

		// Token: 0x040052D9 RID: 21209
		[SerializeField]
		private Vector2 offset = new Vector2(-10f, 10f);

		// Token: 0x040052DA RID: 21210
		[SerializeField]
		private bool lockAspectRatio = true;

		// Token: 0x040052DB RID: 21211
		[Header("Options")]
		public TouchControl.ButtonTarget target = TouchControl.ButtonTarget.Action1;

		// Token: 0x040052DC RID: 21212
		public bool allowSlideToggle = true;

		// Token: 0x040052DD RID: 21213
		public bool toggleOnLeave;

		// Token: 0x040052DE RID: 21214
		public bool pressureSensitive;

		// Token: 0x040052DF RID: 21215
		[Header("Sprites")]
		public TouchSprite button = new TouchSprite(15f);

		// Token: 0x040052E0 RID: 21216
		private bool buttonState;

		// Token: 0x040052E1 RID: 21217
		private Touch currentTouch;

		// Token: 0x040052E2 RID: 21218
		private bool dirty;
	}
}
