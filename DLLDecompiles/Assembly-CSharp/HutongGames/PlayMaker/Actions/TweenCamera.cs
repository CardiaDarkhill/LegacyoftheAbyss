using System;
using HutongGames.Extensions;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FE RID: 4350
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(Camera), "", false)]
	[Tooltip("Tween common Camera properties.")]
	public class TweenCamera : TweenComponentBase<Camera>
	{
		// Token: 0x0600759B RID: 30107 RVA: 0x0023EE3A File Offset: 0x0023D03A
		public override void Reset()
		{
			base.Reset();
			this.property = TweenCamera.CameraProperty.FieldOfView;
			this.tweenDirection = TweenDirection.To;
			this.targetColor = null;
			this.targetFloat = null;
		}

		// Token: 0x0600759C RID: 30108 RVA: 0x0023EE60 File Offset: 0x0023D060
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			this.camera = this.cachedComponent;
			if (this.tweenDirection == TweenDirection.From)
			{
				switch (this.property)
				{
				case TweenCamera.CameraProperty.Aspect:
					this.fromFloat = this.targetFloat.Value;
					this.toFloat = this.camera.aspect;
					return;
				case TweenCamera.CameraProperty.BackgroundColor:
					this.fromColor = this.targetColor.Value;
					this.toColor = this.camera.backgroundColor;
					return;
				case TweenCamera.CameraProperty.FieldOfView:
					this.fromFloat = this.targetFloat.Value;
					this.toFloat = this.camera.fieldOfView;
					return;
				case TweenCamera.CameraProperty.OrthoSize:
					this.fromFloat = this.targetFloat.Value;
					this.toFloat = this.camera.orthographicSize;
					return;
				case TweenCamera.CameraProperty.PixelRect:
					this.fromRect = this.targetRect.Value;
					this.toRect = this.camera.pixelRect;
					return;
				case TweenCamera.CameraProperty.ViewportRect:
					this.fromRect = this.targetRect.Value;
					this.toRect = this.camera.rect;
					return;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (this.property)
				{
				case TweenCamera.CameraProperty.Aspect:
					this.fromFloat = this.camera.aspect;
					this.toFloat = this.targetFloat.Value;
					return;
				case TweenCamera.CameraProperty.BackgroundColor:
					this.fromColor = this.camera.backgroundColor;
					this.toColor = this.targetColor.Value;
					return;
				case TweenCamera.CameraProperty.FieldOfView:
					this.fromFloat = this.camera.fieldOfView;
					this.toFloat = this.targetFloat.Value;
					return;
				case TweenCamera.CameraProperty.OrthoSize:
					this.fromFloat = this.camera.orthographicSize;
					this.toFloat = this.targetFloat.Value;
					return;
				case TweenCamera.CameraProperty.PixelRect:
					this.fromRect = this.camera.pixelRect;
					this.toRect = this.targetRect.Value;
					return;
				case TweenCamera.CameraProperty.ViewportRect:
					this.fromRect = this.camera.rect;
					this.toRect = this.targetRect.Value;
					return;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Token: 0x0600759D RID: 30109 RVA: 0x0023F098 File Offset: 0x0023D298
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			switch (this.property)
			{
			case TweenCamera.CameraProperty.Aspect:
				this.camera.aspect = Mathf.Lerp(this.fromFloat, this.toFloat, t);
				return;
			case TweenCamera.CameraProperty.BackgroundColor:
				this.camera.backgroundColor = Color.Lerp(this.fromColor, this.toColor, t);
				return;
			case TweenCamera.CameraProperty.FieldOfView:
				this.camera.fieldOfView = Mathf.Lerp(this.fromFloat, this.toFloat, t);
				return;
			case TweenCamera.CameraProperty.OrthoSize:
				this.camera.orthographicSize = Mathf.Lerp(this.fromFloat, this.toFloat, t);
				return;
			case TweenCamera.CameraProperty.PixelRect:
				this.camera.pixelRect = this.camera.pixelRect.Lerp(this.fromRect, this.toRect, t);
				return;
			case TweenCamera.CameraProperty.ViewportRect:
				this.camera.rect = this.camera.rect.Lerp(this.fromRect, this.toRect, t);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x040075FE RID: 30206
		[Tooltip("Camera property to tween.")]
		public TweenCamera.CameraProperty property;

		// Token: 0x040075FF RID: 30207
		[Tooltip("Tween To/From values set below.")]
		public TweenDirection tweenDirection;

		// Token: 0x04007600 RID: 30208
		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmColor targetColor;

		// Token: 0x04007601 RID: 30209
		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmFloat targetFloat;

		// Token: 0x04007602 RID: 30210
		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmRect targetRect;

		// Token: 0x04007603 RID: 30211
		private Camera camera;

		// Token: 0x04007604 RID: 30212
		private Color fromColor;

		// Token: 0x04007605 RID: 30213
		private Color toColor;

		// Token: 0x04007606 RID: 30214
		private float fromFloat;

		// Token: 0x04007607 RID: 30215
		private float toFloat;

		// Token: 0x04007608 RID: 30216
		private Rect fromRect;

		// Token: 0x04007609 RID: 30217
		private Rect toRect;

		// Token: 0x02001BCD RID: 7117
		public enum CameraProperty
		{
			// Token: 0x04009ECE RID: 40654
			Aspect,
			// Token: 0x04009ECF RID: 40655
			BackgroundColor,
			// Token: 0x04009ED0 RID: 40656
			FieldOfView,
			// Token: 0x04009ED1 RID: 40657
			OrthoSize,
			// Token: 0x04009ED2 RID: 40658
			PixelRect,
			// Token: 0x04009ED3 RID: 40659
			ViewportRect
		}
	}
}
