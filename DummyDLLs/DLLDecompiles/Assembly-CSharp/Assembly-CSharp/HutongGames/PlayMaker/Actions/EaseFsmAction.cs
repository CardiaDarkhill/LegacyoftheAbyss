using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC6 RID: 3526
	[Tooltip("Ease base action - don't use!")]
	public abstract class EaseFsmAction : FsmStateAction
	{
		// Token: 0x0600661A RID: 26138 RVA: 0x00205500 File Offset: 0x00203700
		public override void Reset()
		{
			this.easeType = EaseFsmAction.EaseType.linear;
			this.time = new FsmFloat
			{
				Value = 1f
			};
			this.delay = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.reverse = new FsmBool
			{
				Value = false
			};
			this.realTime = false;
			this.finishEvent = null;
			this.ease = null;
			this.runningTime = 0f;
			this.lastTime = 0f;
			this.percentage = 0f;
			this.fromFloats = new float[0];
			this.toFloats = new float[0];
			this.resultFloats = new float[0];
			this.finishAction = false;
			this.start = false;
			this.finished = false;
			this.isRunning = false;
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x002055D8 File Offset: 0x002037D8
		public override void OnEnter()
		{
			this.finished = false;
			this.isRunning = false;
			this.SetEasingFunction();
			this.runningTime = 0f;
			this.percentage = (this.reverse.IsNone ? 0f : (this.reverse.Value ? 1f : 0f));
			this.finishAction = false;
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
			this.delayTime = (this.delay.IsNone ? 0f : (this.delayTime = this.delay.Value));
			this.start = true;
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x00205690 File Offset: 0x00203890
		public override void OnExit()
		{
		}

		// Token: 0x0600661D RID: 26141 RVA: 0x00205694 File Offset: 0x00203894
		public override void OnUpdate()
		{
			if (this.start && !this.isRunning)
			{
				if (this.delayTime >= 0f)
				{
					if (this.realTime)
					{
						this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
						this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
						this.delayTime -= this.deltaTime;
					}
					else
					{
						this.delayTime -= Time.deltaTime;
					}
				}
				else
				{
					this.isRunning = true;
					this.start = false;
					this.startTime = FsmTime.RealtimeSinceStartup;
					this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				}
			}
			if (this.isRunning && !this.finished)
			{
				if (this.reverse.IsNone || !this.reverse.Value)
				{
					this.UpdatePercentage();
					if (this.percentage < 1f)
					{
						for (int i = 0; i < this.fromFloats.Length; i++)
						{
							this.resultFloats[i] = this.ease(this.fromFloats[i], this.toFloats[i], this.percentage);
						}
						return;
					}
					this.finishAction = true;
					this.finished = true;
					this.isRunning = false;
					return;
				}
				else
				{
					this.UpdatePercentage();
					if (this.percentage > 0f)
					{
						for (int j = 0; j < this.fromFloats.Length; j++)
						{
							this.resultFloats[j] = this.ease(this.fromFloats[j], this.toFloats[j], this.percentage);
						}
						return;
					}
					this.finishAction = true;
					this.finished = true;
					this.isRunning = false;
				}
			}
		}

		// Token: 0x0600661E RID: 26142 RVA: 0x0020584C File Offset: 0x00203A4C
		protected void UpdatePercentage()
		{
			if (this.realTime)
			{
				this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
				this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				if (!this.speed.IsNone)
				{
					this.runningTime += this.deltaTime * this.speed.Value;
				}
				else
				{
					this.runningTime += this.deltaTime;
				}
			}
			else if (!this.speed.IsNone)
			{
				this.runningTime += Time.deltaTime * this.speed.Value;
			}
			else
			{
				this.runningTime += Time.deltaTime;
			}
			if (!this.reverse.IsNone && this.reverse.Value)
			{
				this.percentage = 1f - Mathf.Clamp01(this.runningTime / this.time.Value);
				return;
			}
			this.percentage = Mathf.Clamp01(this.runningTime / this.time.Value);
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x0020596C File Offset: 0x00203B6C
		protected void SetEasingFunction()
		{
			switch (this.easeType)
			{
			case EaseFsmAction.EaseType.easeInQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuad);
				return;
			case EaseFsmAction.EaseType.easeOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuad);
				return;
			case EaseFsmAction.EaseType.easeInOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuad);
				return;
			case EaseFsmAction.EaseType.easeInCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCubic);
				return;
			case EaseFsmAction.EaseType.easeOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCubic);
				return;
			case EaseFsmAction.EaseType.easeInOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCubic);
				return;
			case EaseFsmAction.EaseType.easeInQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuart);
				return;
			case EaseFsmAction.EaseType.easeOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuart);
				return;
			case EaseFsmAction.EaseType.easeInOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuart);
				return;
			case EaseFsmAction.EaseType.easeInQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuint);
				return;
			case EaseFsmAction.EaseType.easeOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuint);
				return;
			case EaseFsmAction.EaseType.easeInOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuint);
				return;
			case EaseFsmAction.EaseType.easeInSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInSine);
				return;
			case EaseFsmAction.EaseType.easeOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutSine);
				return;
			case EaseFsmAction.EaseType.easeInOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutSine);
				return;
			case EaseFsmAction.EaseType.easeInExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInExpo);
				return;
			case EaseFsmAction.EaseType.easeOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutExpo);
				return;
			case EaseFsmAction.EaseType.easeInOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutExpo);
				return;
			case EaseFsmAction.EaseType.easeInCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCirc);
				return;
			case EaseFsmAction.EaseType.easeOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCirc);
				return;
			case EaseFsmAction.EaseType.easeInOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCirc);
				return;
			case EaseFsmAction.EaseType.linear:
				this.ease = new EaseFsmAction.EasingFunction(this.linear);
				return;
			case EaseFsmAction.EaseType.spring:
				this.ease = new EaseFsmAction.EasingFunction(this.spring);
				return;
			case EaseFsmAction.EaseType.bounce:
				this.ease = new EaseFsmAction.EasingFunction(this.bounce);
				return;
			case EaseFsmAction.EaseType.easeInBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInBack);
				return;
			case EaseFsmAction.EaseType.easeOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutBack);
				return;
			case EaseFsmAction.EaseType.easeInOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutBack);
				return;
			case EaseFsmAction.EaseType.elastic:
				this.ease = new EaseFsmAction.EasingFunction(this.elastic);
				return;
			case EaseFsmAction.EaseType.punch:
				this.ease = new EaseFsmAction.EasingFunction(this.elastic);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006620 RID: 26144 RVA: 0x00205C21 File Offset: 0x00203E21
		protected float linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x00205C2C File Offset: 0x00203E2C
		protected float clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((num2 - num) / 2f);
			float result;
			if (end - start < -num3)
			{
				float num4 = (num2 - start + end) * value;
				result = start + num4;
			}
			else if (end - start > num3)
			{
				float num4 = -(num2 - end + start) * value;
				result = start + num4;
			}
			else
			{
				result = start + (end - start) * value;
			}
			return result;
		}

		// Token: 0x06006622 RID: 26146 RVA: 0x00205C98 File Offset: 0x00203E98
		protected float spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		// Token: 0x06006623 RID: 26147 RVA: 0x00205CFC File Offset: 0x00203EFC
		protected float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x00205D0A File Offset: 0x00203F0A
		protected float easeOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2f) + start;
		}

		// Token: 0x06006625 RID: 26149 RVA: 0x00205D20 File Offset: 0x00203F20
		protected float easeInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value + start;
			}
			value -= 1f;
			return -end / 2f * (value * (value - 2f) - 1f) + start;
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x00205D74 File Offset: 0x00203F74
		protected float easeInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		// Token: 0x06006627 RID: 26151 RVA: 0x00205D84 File Offset: 0x00203F84
		protected float easeOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		// Token: 0x06006628 RID: 26152 RVA: 0x00205DA4 File Offset: 0x00203FA4
		protected float easeInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value + 2f) + start;
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x00205DF5 File Offset: 0x00203FF5
		protected float easeInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x00205E07 File Offset: 0x00204007
		protected float easeOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * (value * value * value * value - 1f) + start;
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x00205E2C File Offset: 0x0020402C
		protected float easeInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value + start;
			}
			value -= 2f;
			return -end / 2f * (value * value * value * value - 2f) + start;
		}

		// Token: 0x0600662C RID: 26156 RVA: 0x00205E82 File Offset: 0x00204082
		protected float easeInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		// Token: 0x0600662D RID: 26157 RVA: 0x00205E96 File Offset: 0x00204096
		protected float easeOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x00205EBC File Offset: 0x002040BC
		protected float easeInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value * value * value + 2f) + start;
		}

		// Token: 0x0600662F RID: 26159 RVA: 0x00205F15 File Offset: 0x00204115
		protected float easeInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value / 1f * 1.5707964f) + end + start;
		}

		// Token: 0x06006630 RID: 26160 RVA: 0x00205F35 File Offset: 0x00204135
		protected float easeOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value / 1f * 1.5707964f) + start;
		}

		// Token: 0x06006631 RID: 26161 RVA: 0x00205F52 File Offset: 0x00204152
		protected float easeInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end / 2f * (Mathf.Cos(3.1415927f * value / 1f) - 1f) + start;
		}

		// Token: 0x06006632 RID: 26162 RVA: 0x00205F7C File Offset: 0x0020417C
		protected float easeInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
		}

		// Token: 0x06006633 RID: 26163 RVA: 0x00205FA4 File Offset: 0x002041A4
		protected float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
		}

		// Token: 0x06006634 RID: 26164 RVA: 0x00205FD0 File Offset: 0x002041D0
		protected float easeInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		// Token: 0x06006635 RID: 26165 RVA: 0x00206040 File Offset: 0x00204240
		protected float easeInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		// Token: 0x06006636 RID: 26166 RVA: 0x00206060 File Offset: 0x00204260
		protected float easeOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x00206084 File Offset: 0x00204284
		protected float easeInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x002060F0 File Offset: 0x002042F0
		protected float bounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 0.95454544f;
			return end * (7.5625f * value * value + 0.984375f) + start;
		}

		// Token: 0x06006639 RID: 26169 RVA: 0x0020618C File Offset: 0x0020438C
		protected float easeInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		// Token: 0x0600663A RID: 26170 RVA: 0x002061C0 File Offset: 0x002043C0
		protected float easeOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value = value / 1f - 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		// Token: 0x0600663B RID: 26171 RVA: 0x00206200 File Offset: 0x00204400
		protected float easeInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		// Token: 0x0600663C RID: 26172 RVA: 0x0020627C File Offset: 0x0020447C
		protected float punch(float amplitude, float value)
		{
			if (value == 0f)
			{
				return 0f;
			}
			if (value == 1f)
			{
				return 0f;
			}
			float num = 0.3f;
			float num2 = num / 6.2831855f * Mathf.Asin(0f);
			return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.2831855f / num);
		}

		// Token: 0x0600663D RID: 26173 RVA: 0x002062F0 File Offset: 0x002044F0
		protected float elastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
		}

		// Token: 0x04006584 RID: 25988
		[RequiredField]
		[Tooltip("How long the ease should take in seconds")]
		public FsmFloat time;

		// Token: 0x04006585 RID: 25989
		[Tooltip("Optionally, use speed instead of time.")]
		public FsmFloat speed;

		// Token: 0x04006586 RID: 25990
		[Tooltip("Optional delay in seconds before starting to ease.")]
		public FsmFloat delay;

		// Token: 0x04006587 RID: 25991
		[Tooltip("The easing function to use.")]
		public EaseFsmAction.EaseType easeType = EaseFsmAction.EaseType.linear;

		// Token: 0x04006588 RID: 25992
		[Tooltip("Reverse the ease.")]
		public FsmBool reverse;

		// Token: 0x04006589 RID: 25993
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x0400658A RID: 25994
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x0400658B RID: 25995
		protected EaseFsmAction.EasingFunction ease;

		// Token: 0x0400658C RID: 25996
		protected float runningTime;

		// Token: 0x0400658D RID: 25997
		protected float lastTime;

		// Token: 0x0400658E RID: 25998
		protected float startTime;

		// Token: 0x0400658F RID: 25999
		protected float deltaTime;

		// Token: 0x04006590 RID: 26000
		protected float delayTime;

		// Token: 0x04006591 RID: 26001
		protected float percentage;

		// Token: 0x04006592 RID: 26002
		protected float[] fromFloats = new float[0];

		// Token: 0x04006593 RID: 26003
		protected float[] toFloats = new float[0];

		// Token: 0x04006594 RID: 26004
		protected float[] resultFloats = new float[0];

		// Token: 0x04006595 RID: 26005
		protected bool finishAction;

		// Token: 0x04006596 RID: 26006
		protected bool start;

		// Token: 0x04006597 RID: 26007
		protected bool finished;

		// Token: 0x04006598 RID: 26008
		protected bool isRunning;

		// Token: 0x02001B98 RID: 7064
		// (Invoke) Token: 0x06009A59 RID: 39513
		protected delegate float EasingFunction(float start, float end, float value);

		// Token: 0x02001B99 RID: 7065
		public enum EaseType
		{
			// Token: 0x04009DBE RID: 40382
			easeInQuad,
			// Token: 0x04009DBF RID: 40383
			easeOutQuad,
			// Token: 0x04009DC0 RID: 40384
			easeInOutQuad,
			// Token: 0x04009DC1 RID: 40385
			easeInCubic,
			// Token: 0x04009DC2 RID: 40386
			easeOutCubic,
			// Token: 0x04009DC3 RID: 40387
			easeInOutCubic,
			// Token: 0x04009DC4 RID: 40388
			easeInQuart,
			// Token: 0x04009DC5 RID: 40389
			easeOutQuart,
			// Token: 0x04009DC6 RID: 40390
			easeInOutQuart,
			// Token: 0x04009DC7 RID: 40391
			easeInQuint,
			// Token: 0x04009DC8 RID: 40392
			easeOutQuint,
			// Token: 0x04009DC9 RID: 40393
			easeInOutQuint,
			// Token: 0x04009DCA RID: 40394
			easeInSine,
			// Token: 0x04009DCB RID: 40395
			easeOutSine,
			// Token: 0x04009DCC RID: 40396
			easeInOutSine,
			// Token: 0x04009DCD RID: 40397
			easeInExpo,
			// Token: 0x04009DCE RID: 40398
			easeOutExpo,
			// Token: 0x04009DCF RID: 40399
			easeInOutExpo,
			// Token: 0x04009DD0 RID: 40400
			easeInCirc,
			// Token: 0x04009DD1 RID: 40401
			easeOutCirc,
			// Token: 0x04009DD2 RID: 40402
			easeInOutCirc,
			// Token: 0x04009DD3 RID: 40403
			linear,
			// Token: 0x04009DD4 RID: 40404
			spring,
			// Token: 0x04009DD5 RID: 40405
			bounce,
			// Token: 0x04009DD6 RID: 40406
			easeInBack,
			// Token: 0x04009DD7 RID: 40407
			easeOutBack,
			// Token: 0x04009DD8 RID: 40408
			easeInOutBack,
			// Token: 0x04009DD9 RID: 40409
			elastic,
			// Token: 0x04009DDA RID: 40410
			punch
		}
	}
}
