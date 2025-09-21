using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FD RID: 4349
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(AudioSource), "", false)]
	[Tooltip("Tween common AudioSource properties.")]
	public class TweenAudio : TweenComponentBase<AudioSource>
	{
		// Token: 0x06007597 RID: 30103 RVA: 0x0023ECB3 File Offset: 0x0023CEB3
		public override void Reset()
		{
			base.Reset();
			this.property = TweenAudio.AudioProperty.Volume;
			this.tweenDirection = TweenDirection.To;
			this.value = null;
		}

		// Token: 0x06007598 RID: 30104 RVA: 0x0023ECD0 File Offset: 0x0023CED0
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			this.audio = this.cachedComponent;
			if (this.tweenDirection == TweenDirection.From)
			{
				TweenAudio.AudioProperty audioProperty = this.property;
				if (audioProperty == TweenAudio.AudioProperty.Volume)
				{
					this.fromFloat = this.value.Value;
					this.toFloat = this.audio.volume;
					return;
				}
				if (audioProperty != TweenAudio.AudioProperty.Pitch)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.fromFloat = this.value.Value;
				this.toFloat = this.audio.pitch;
				return;
			}
			else
			{
				TweenAudio.AudioProperty audioProperty = this.property;
				if (audioProperty == TweenAudio.AudioProperty.Volume)
				{
					this.fromFloat = this.audio.volume;
					this.toFloat = this.value.Value;
					return;
				}
				if (audioProperty != TweenAudio.AudioProperty.Pitch)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.fromFloat = this.audio.pitch;
				this.toFloat = this.value.Value;
				return;
			}
		}

		// Token: 0x06007599 RID: 30105 RVA: 0x0023EDB8 File Offset: 0x0023CFB8
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			TweenAudio.AudioProperty audioProperty = this.property;
			if (audioProperty == TweenAudio.AudioProperty.Volume)
			{
				this.audio.volume = Mathf.Lerp(this.fromFloat, this.toFloat, t);
				return;
			}
			if (audioProperty != TweenAudio.AudioProperty.Pitch)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.audio.pitch = Mathf.Lerp(this.fromFloat, this.toFloat, t);
		}

		// Token: 0x040075F8 RID: 30200
		[Tooltip("Audio property to tween.")]
		public TweenAudio.AudioProperty property;

		// Token: 0x040075F9 RID: 30201
		[Tooltip("Tween To/From values set below.")]
		public TweenDirection tweenDirection;

		// Token: 0x040075FA RID: 30202
		[Tooltip("Value for the selected property.")]
		public FsmFloat value;

		// Token: 0x040075FB RID: 30203
		private AudioSource audio;

		// Token: 0x040075FC RID: 30204
		private float fromFloat;

		// Token: 0x040075FD RID: 30205
		private float toFloat;

		// Token: 0x02001BCC RID: 7116
		public enum AudioProperty
		{
			// Token: 0x04009ECB RID: 40651
			Volume,
			// Token: 0x04009ECC RID: 40652
			Pitch
		}
	}
}
