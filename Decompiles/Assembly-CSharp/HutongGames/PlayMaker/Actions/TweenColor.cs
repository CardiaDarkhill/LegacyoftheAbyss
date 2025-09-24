using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FF RID: 4351
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween the color of a GameObject or a Color variable. The GameObject needs a Material, Sprite, Image, Text, or Light component.")]
	public class TweenColor : TweenPropertyBase<FsmColor>
	{
		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x0600759F RID: 30111 RVA: 0x0023F1C2 File Offset: 0x0023D3C2
		public TweenColor.TargetType type
		{
			get
			{
				return this.targetType;
			}
		}

		// Token: 0x060075A0 RID: 30112 RVA: 0x0023F1CA File Offset: 0x0023D3CA
		public override void Reset()
		{
			base.Reset();
			this.fromOffsetBlendMode = ColorBlendMode.Normal;
			this.toOffsetBlendMode = ColorBlendMode.Normal;
			this.gameObject = null;
			this.cachedGameObject = null;
			this.cachedComponent = null;
		}

		// Token: 0x060075A1 RID: 30113 RVA: 0x0023F1F8 File Offset: 0x0023D3F8
		private void UpdateCache(GameObject go)
		{
			this.cachedGameObject = go;
			if (go == null)
			{
				this.cachedComponent = null;
				return;
			}
			this.cachedComponent = go.GetComponent<MeshRenderer>();
			if (this.cachedComponent != null)
			{
				return;
			}
			this.cachedComponent = go.GetComponent<Image>();
			if (this.cachedComponent != null)
			{
				return;
			}
			this.cachedComponent = go.GetComponent<Text>();
			if (this.cachedComponent != null)
			{
				return;
			}
			this.cachedComponent = go.GetComponent<Light>();
			if (this.cachedComponent != null)
			{
				return;
			}
			this.cachedComponent = go.GetComponent<SpriteRenderer>();
		}

		// Token: 0x060075A2 RID: 30114 RVA: 0x0023F298 File Offset: 0x0023D498
		private void CheckCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.cachedGameObject != ownerDefaultTarget)
			{
				this.UpdateCache(ownerDefaultTarget);
			}
			this.Init();
		}

		// Token: 0x060075A3 RID: 30115 RVA: 0x0023F2D4 File Offset: 0x0023D4D4
		private void Init()
		{
			this.targetType = TweenColor.TargetType.None;
			MeshRenderer meshRenderer = this.cachedComponent as MeshRenderer;
			if (meshRenderer != null)
			{
				this.targetType = TweenColor.TargetType.Material;
				this.material = meshRenderer.material;
				return;
			}
			this.image = (this.cachedComponent as Image);
			if (this.image != null)
			{
				this.targetType = TweenColor.TargetType.Image;
				return;
			}
			this.spriteRenderer = (this.cachedComponent as SpriteRenderer);
			if (this.spriteRenderer != null)
			{
				this.targetType = TweenColor.TargetType.Sprite;
				return;
			}
			this.text = (this.cachedComponent as Text);
			if (this.text != null)
			{
				this.targetType = TweenColor.TargetType.Text;
				return;
			}
			this.light = (this.cachedComponent as Light);
			if (this.light != null)
			{
				this.targetType = TweenColor.TargetType.Light;
			}
		}

		// Token: 0x060075A4 RID: 30116 RVA: 0x0023F3AC File Offset: 0x0023D5AC
		public override void OnEnter()
		{
			if (this.target == TweenColor.Target.GameObject)
			{
				this.CheckCache();
			}
			base.OnEnter();
			this.InitOffsets();
			this.DoTween();
		}

		// Token: 0x060075A5 RID: 30117 RVA: 0x0023F3D0 File Offset: 0x0023D5D0
		protected override void InitTargets()
		{
			switch (this.fromOption)
			{
			case TargetValueOptions.CurrentValue:
				base.StartValue = this.GetTargetColor();
				break;
			case TargetValueOptions.Offset:
				base.StartValue = this.GetOffsetValue(this.variable.RawValue, this.fromValue.RawValue);
				break;
			case TargetValueOptions.Value:
				base.StartValue = this.fromValue.Value;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			switch (this.toOption)
			{
			case TargetValueOptions.CurrentValue:
				base.EndValue = this.GetTargetColor();
				return;
			case TargetValueOptions.Offset:
				base.EndValue = this.GetOffsetValue(this.variable.RawValue, this.toValue.RawValue);
				return;
			case TargetValueOptions.Value:
				base.EndValue = this.toValue.Value;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075A6 RID: 30118 RVA: 0x0023F4BC File Offset: 0x0023D6BC
		private Color GetTargetColor()
		{
			if (this.target == TweenColor.Target.Variable)
			{
				return this.variable.Value;
			}
			switch (this.targetType)
			{
			case TweenColor.TargetType.None:
				return Color.white;
			case TweenColor.TargetType.Material:
				return this.material.color;
			case TweenColor.TargetType.Sprite:
				return this.spriteRenderer.color;
			case TweenColor.TargetType.Image:
				return this.image.color;
			case TweenColor.TargetType.Text:
				return this.text.color;
			case TweenColor.TargetType.Light:
				return this.light.color;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075A7 RID: 30119 RVA: 0x0023F54C File Offset: 0x0023D74C
		private void SetTargetColor(Color color)
		{
			if (this.target == TweenColor.Target.Variable)
			{
				this.variable.Value = color;
				return;
			}
			switch (this.targetType)
			{
			case TweenColor.TargetType.None:
				return;
			case TweenColor.TargetType.Material:
				this.material.color = color;
				return;
			case TweenColor.TargetType.Sprite:
				this.spriteRenderer.color = color;
				return;
			case TweenColor.TargetType.Image:
				this.image.color = color;
				return;
			case TweenColor.TargetType.Text:
				this.text.color = color;
				return;
			case TweenColor.TargetType.Light:
				this.light.color = color;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075A8 RID: 30120 RVA: 0x0023F5E0 File Offset: 0x0023D7E0
		private void InitOffsets()
		{
			if (this.fromOption == TargetValueOptions.Offset)
			{
				base.StartValue = ActionHelpers.BlendColor(this.fromOffsetBlendMode, this.GetTargetColor(), this.fromValue.Value);
			}
			if (this.toOption == TargetValueOptions.Offset)
			{
				base.EndValue = ActionHelpers.BlendColor(this.toOffsetBlendMode, this.GetTargetColor(), this.toValue.Value);
			}
		}

		// Token: 0x060075A9 RID: 30121 RVA: 0x0023F64D File Offset: 0x0023D84D
		protected override object GetOffsetValue(object value, object offset)
		{
			return value;
		}

		// Token: 0x060075AA RID: 30122 RVA: 0x0023F650 File Offset: 0x0023D850
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.SetTargetColor(Color.Lerp((Color)base.StartValue, (Color)base.EndValue, t));
		}

		// Token: 0x0400760A RID: 30218
		private const string SupportedComponents = "MeshRenderer, Sprite, Image, Text, Light.";

		// Token: 0x0400760B RID: 30219
		private const string OffsetTooltip = "How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.";

		// Token: 0x0400760C RID: 30220
		[Tooltip("What to tween.")]
		public TweenColor.Target target = TweenColor.Target.Variable;

		// Token: 0x0400760D RID: 30221
		[Tooltip("A GameObject with a Material, Sprite, Image, Text, or Light component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400760E RID: 30222
		[Tooltip("The variable to tween.")]
		[UIHint(UIHint.Variable)]
		public FsmColor variable;

		// Token: 0x0400760F RID: 30223
		[Tooltip("How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.")]
		public ColorBlendMode fromOffsetBlendMode;

		// Token: 0x04007610 RID: 30224
		[Tooltip("How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.")]
		public ColorBlendMode toOffsetBlendMode;

		// Token: 0x04007611 RID: 30225
		private GameObject cachedGameObject;

		// Token: 0x04007612 RID: 30226
		private Component cachedComponent;

		// Token: 0x04007613 RID: 30227
		private TweenColor.TargetType targetType;

		// Token: 0x04007614 RID: 30228
		private Material material;

		// Token: 0x04007615 RID: 30229
		private SpriteRenderer spriteRenderer;

		// Token: 0x04007616 RID: 30230
		private Text text;

		// Token: 0x04007617 RID: 30231
		private Image image;

		// Token: 0x04007618 RID: 30232
		private Light light;

		// Token: 0x02001BCE RID: 7118
		public enum Target
		{
			// Token: 0x04009ED5 RID: 40661
			GameObject,
			// Token: 0x04009ED6 RID: 40662
			Variable
		}

		// Token: 0x02001BCF RID: 7119
		public enum TargetType
		{
			// Token: 0x04009ED8 RID: 40664
			None,
			// Token: 0x04009ED9 RID: 40665
			Material,
			// Token: 0x04009EDA RID: 40666
			Sprite,
			// Token: 0x04009EDB RID: 40667
			Image,
			// Token: 0x04009EDC RID: 40668
			Text,
			// Token: 0x04009EDD RID: 40669
			Light
		}
	}
}
