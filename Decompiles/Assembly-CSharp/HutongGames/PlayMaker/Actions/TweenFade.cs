using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001100 RID: 4352
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Fades a GameObject with a Material, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.\n\nNote: The Material Shader must support transparency. For example, in URP set the Surface Type to Transparent.\n\nTip: When using the Standard shader, set Rendering Mode to Fade for best fading effect.")]
	public class TweenFade : TweenActionBase
	{
		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x060075AC RID: 30124 RVA: 0x0023F6AA File Offset: 0x0023D8AA
		public TweenFade.TargetType type
		{
			get
			{
				return this.targetType;
			}
		}

		// Token: 0x060075AD RID: 30125 RVA: 0x0023F6B2 File Offset: 0x0023D8B2
		public override void Reset()
		{
			base.Reset();
			this.tweenDirection = TweenDirection.To;
			this.value = null;
			this.gameObject = null;
			this.cachedGameObject = null;
			this.cachedComponent = null;
		}

		// Token: 0x060075AE RID: 30126 RVA: 0x0023F6E0 File Offset: 0x0023D8E0
		private void UpdateCache(GameObject go)
		{
			this.cachedGameObject = go;
			if (go == null)
			{
				this.cachedComponent = null;
				return;
			}
			this.FindComponent(new Type[]
			{
				typeof(MeshRenderer),
				typeof(Image),
				typeof(Text),
				typeof(Light),
				typeof(SpriteRenderer),
				typeof(AudioSource),
				typeof(CanvasGroup)
			});
		}

		// Token: 0x060075AF RID: 30127 RVA: 0x0023F76C File Offset: 0x0023D96C
		private void FindComponent(params Type[] components)
		{
			foreach (Type type in components)
			{
				this.cachedComponent = this.cachedGameObject.GetComponent(type);
				if (this.cachedComponent != null)
				{
					return;
				}
			}
		}

		// Token: 0x060075B0 RID: 30128 RVA: 0x0023F7B0 File Offset: 0x0023D9B0
		private void CheckCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.cachedGameObject != ownerDefaultTarget)
			{
				this.UpdateCache(ownerDefaultTarget);
			}
			this.InitTarget();
		}

		// Token: 0x060075B1 RID: 30129 RVA: 0x0023F7EC File Offset: 0x0023D9EC
		private void InitTarget()
		{
			this.targetType = TweenFade.TargetType.None;
			this.renderer = (this.cachedComponent as MeshRenderer);
			if (this.renderer != null)
			{
				this.targetType = TweenFade.TargetType.Material;
				return;
			}
			this.image = (this.cachedComponent as Image);
			if (this.image != null)
			{
				this.targetType = TweenFade.TargetType.Image;
				return;
			}
			this.spriteRenderer = (this.cachedComponent as SpriteRenderer);
			if (this.spriteRenderer != null)
			{
				this.targetType = TweenFade.TargetType.Sprite;
				return;
			}
			this.text = (this.cachedComponent as Text);
			if (this.text != null)
			{
				this.targetType = TweenFade.TargetType.Text;
				return;
			}
			this.light = (this.cachedComponent as Light);
			if (this.light != null)
			{
				this.targetType = TweenFade.TargetType.Light;
				return;
			}
			this.audioSource = (this.cachedComponent as AudioSource);
			if (this.audioSource != null)
			{
				this.targetType = TweenFade.TargetType.AudioSource;
				return;
			}
			this.canvasGroup = (this.cachedComponent as CanvasGroup);
			if (this.canvasGroup != null)
			{
				this.targetType = TweenFade.TargetType.CanvasGroup;
			}
		}

		// Token: 0x060075B2 RID: 30130 RVA: 0x0023F910 File Offset: 0x0023DB10
		public override void OnEnter()
		{
			this.CheckCache();
			if (this.targetType == TweenFade.TargetType.None)
			{
				base.LogWarning("GameObject needs a MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.");
				base.Enabled = false;
				return;
			}
			if (this.tweenDirection == TweenDirection.From)
			{
				this.startValue = this.value.Value;
				this.endValue = this.GetTargetFade();
			}
			else
			{
				this.startValue = this.GetTargetFade();
				this.endValue = this.value.Value;
			}
			base.OnEnter();
			this.DoTween();
		}

		// Token: 0x060075B3 RID: 30131 RVA: 0x0023F990 File Offset: 0x0023DB90
		private float GetTargetFade()
		{
			switch (this.targetType)
			{
			case TweenFade.TargetType.None:
				return 1f;
			case TweenFade.TargetType.Material:
				return this.renderer.material.color.a;
			case TweenFade.TargetType.Sprite:
				return this.spriteRenderer.color.a;
			case TweenFade.TargetType.Image:
				return this.image.color.a;
			case TweenFade.TargetType.Text:
				return this.text.color.a;
			case TweenFade.TargetType.Light:
				return this.light.intensity;
			case TweenFade.TargetType.AudioSource:
				return this.audioSource.volume;
			case TweenFade.TargetType.CanvasGroup:
				return this.canvasGroup.alpha;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075B4 RID: 30132 RVA: 0x0023FA44 File Offset: 0x0023DC44
		private void SetTargetFade(float fade)
		{
			switch (this.targetType)
			{
			case TweenFade.TargetType.None:
				return;
			case TweenFade.TargetType.Material:
			{
				Color color = this.renderer.material.color;
				color.a = fade;
				this.renderer.material.color = color;
				return;
			}
			case TweenFade.TargetType.Sprite:
			{
				Color color = this.spriteRenderer.color;
				color.a = fade;
				this.spriteRenderer.color = color;
				return;
			}
			case TweenFade.TargetType.Image:
			{
				Color color = this.image.color;
				color.a = fade;
				this.image.color = color;
				return;
			}
			case TweenFade.TargetType.Text:
			{
				Color color = this.text.color;
				color.a = fade;
				this.text.color = color;
				return;
			}
			case TweenFade.TargetType.Light:
				this.light.intensity = fade;
				return;
			case TweenFade.TargetType.AudioSource:
				this.audioSource.volume = fade;
				return;
			case TweenFade.TargetType.CanvasGroup:
				this.canvasGroup.alpha = fade;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075B5 RID: 30133 RVA: 0x0023FB40 File Offset: 0x0023DD40
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.SetTargetFade(Mathf.Lerp(this.startValue, this.endValue, t));
		}

		// Token: 0x04007619 RID: 30233
		private const string SupportedComponents = "MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.";

		// Token: 0x0400761A RID: 30234
		[Tooltip("A GameObject with a MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400761B RID: 30235
		[Tooltip("Fade To or From value.")]
		public TweenDirection tweenDirection;

		// Token: 0x0400761C RID: 30236
		[Tooltip("Value to fade to. E.g., alpha if fading an image, volume if fading audio...")]
		public FsmFloat value;

		// Token: 0x0400761D RID: 30237
		private GameObject cachedGameObject;

		// Token: 0x0400761E RID: 30238
		private Component cachedComponent;

		// Token: 0x0400761F RID: 30239
		private TweenFade.TargetType targetType;

		// Token: 0x04007620 RID: 30240
		private Renderer renderer;

		// Token: 0x04007621 RID: 30241
		private SpriteRenderer spriteRenderer;

		// Token: 0x04007622 RID: 30242
		private Text text;

		// Token: 0x04007623 RID: 30243
		private Image image;

		// Token: 0x04007624 RID: 30244
		private Light light;

		// Token: 0x04007625 RID: 30245
		private CanvasGroup canvasGroup;

		// Token: 0x04007626 RID: 30246
		private AudioSource audioSource;

		// Token: 0x04007627 RID: 30247
		private float startValue;

		// Token: 0x04007628 RID: 30248
		private float endValue;

		// Token: 0x02001BD0 RID: 7120
		public enum TargetType
		{
			// Token: 0x04009EDF RID: 40671
			None,
			// Token: 0x04009EE0 RID: 40672
			Material,
			// Token: 0x04009EE1 RID: 40673
			Sprite,
			// Token: 0x04009EE2 RID: 40674
			Image,
			// Token: 0x04009EE3 RID: 40675
			Text,
			// Token: 0x04009EE4 RID: 40676
			Light,
			// Token: 0x04009EE5 RID: 40677
			AudioSource,
			// Token: 0x04009EE6 RID: 40678
			CanvasGroup
		}
	}
}
