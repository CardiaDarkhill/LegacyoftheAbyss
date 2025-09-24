using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBA RID: 3514
	public abstract class AnimateFsmAction : FsmStateAction
	{
		// Token: 0x060065DA RID: 26074 RVA: 0x002022D0 File Offset: 0x002004D0
		public override void Reset()
		{
			this.finishEvent = null;
			this.realTime = false;
			this.time = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.delay = new FsmFloat
			{
				UseVariable = true
			};
			this.ignoreCurveOffset = new FsmBool
			{
				Value = true
			};
			this.resultFloats = new float[0];
			this.fromFloats = new float[0];
			this.toFloats = new float[0];
			this.endTimes = new float[0];
			this.keyOffsets = new float[0];
			this.curves = new AnimationCurve[0];
			this.finishAction = false;
			this.start = false;
		}

		// Token: 0x060065DB RID: 26075 RVA: 0x0020238C File Offset: 0x0020058C
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
			this.deltaTime = 0f;
			this.currentTime = 0f;
			this.isRunning = false;
			this.finishAction = false;
			this.looping = false;
			this.delayTime = (this.delay.IsNone ? 0f : (this.delayTime = this.delay.Value));
			this.start = true;
		}

		// Token: 0x060065DC RID: 26076 RVA: 0x00202418 File Offset: 0x00200618
		protected void Init()
		{
			this.endTimes = new float[this.curves.Length];
			this.keyOffsets = new float[this.curves.Length];
			this.largestEndTime = 0f;
			for (int i = 0; i < this.curves.Length; i++)
			{
				if (this.curves[i] != null && this.curves[i].keys.Length != 0)
				{
					this.keyOffsets[i] = ((this.curves[i].keys.Length != 0) ? (this.time.IsNone ? this.curves[i].keys[0].time : (this.time.Value / this.curves[i].keys[this.curves[i].length - 1].time * this.curves[i].keys[0].time)) : 0f);
					this.currentTime = (this.ignoreCurveOffset.IsNone ? 0f : (this.ignoreCurveOffset.Value ? this.keyOffsets[i] : 0f));
					if (!this.time.IsNone)
					{
						this.endTimes[i] = this.time.Value;
					}
					else
					{
						this.endTimes[i] = this.curves[i].keys[this.curves[i].length - 1].time;
					}
					if (this.largestEndTime < this.endTimes[i])
					{
						this.largestEndTime = this.endTimes[i];
					}
					if (!this.looping)
					{
						this.looping = ActionHelpers.IsLoopingWrapMode(this.curves[i].postWrapMode);
					}
				}
				else
				{
					this.endTimes[i] = -1f;
				}
			}
			for (int j = 0; j < this.curves.Length; j++)
			{
				if (this.largestEndTime > 0f && this.endTimes[j] == -1f)
				{
					this.endTimes[j] = this.largestEndTime;
				}
				else if (this.largestEndTime == 0f && this.endTimes[j] == -1f)
				{
					if (this.time.IsNone)
					{
						this.endTimes[j] = 1f;
					}
					else
					{
						this.endTimes[j] = this.time.Value;
					}
				}
			}
			this.UpdateAnimation();
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x00202686 File Offset: 0x00200886
		public override void OnUpdate()
		{
			this.CheckStart();
			if (this.isRunning)
			{
				this.UpdateTime();
				this.UpdateAnimation();
				this.CheckFinished();
			}
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x002026A8 File Offset: 0x002008A8
		private void CheckStart()
		{
			if (!this.isRunning && this.start)
			{
				if (this.delayTime >= 0f)
				{
					if (this.realTime)
					{
						this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
						this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
						this.delayTime -= this.deltaTime;
						return;
					}
					this.delayTime -= Time.deltaTime;
					return;
				}
				else
				{
					this.isRunning = true;
					this.start = false;
				}
			}
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x0020273C File Offset: 0x0020093C
		private void UpdateTime()
		{
			if (this.realTime)
			{
				this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
				this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				if (!this.speed.IsNone)
				{
					this.currentTime += this.deltaTime * this.speed.Value;
					return;
				}
				this.currentTime += this.deltaTime;
				return;
			}
			else
			{
				if (!this.speed.IsNone)
				{
					this.currentTime += Time.deltaTime * this.speed.Value;
					return;
				}
				this.currentTime += Time.deltaTime;
				return;
			}
		}

		// Token: 0x060065E0 RID: 26080 RVA: 0x002027FC File Offset: 0x002009FC
		public void UpdateAnimation()
		{
			for (int i = 0; i < this.curves.Length; i++)
			{
				if (this.curves[i] != null && this.curves[i].keys.Length != 0)
				{
					if (this.calculations[i] != AnimateFsmAction.Calculation.None)
					{
						switch (this.calculations[i])
						{
						case AnimateFsmAction.Calculation.SetValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
							}
							else
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime);
							}
							break;
						case AnimateFsmAction.Calculation.AddToValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
							}
							else
							{
								this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime);
							}
							break;
						case AnimateFsmAction.Calculation.SubtractFromValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
							}
							else
							{
								this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime);
							}
							break;
						case AnimateFsmAction.Calculation.SubtractValueFromCurve:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.fromFloats[i];
							}
							else
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.fromFloats[i];
							}
							break;
						case AnimateFsmAction.Calculation.MultiplyValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.fromFloats[i];
							}
							else
							{
								this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.fromFloats[i];
							}
							break;
						case AnimateFsmAction.Calculation.DivideValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) != 0f) ? (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)) : float.MaxValue);
							}
							else
							{
								this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) != 0f) ? (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime)) : float.MaxValue);
							}
							break;
						case AnimateFsmAction.Calculation.DivideCurveByValue:
							if (!this.time.IsNone)
							{
								this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / this.fromFloats[i]) : float.MaxValue);
							}
							else
							{
								this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime) / this.fromFloats[i]) : float.MaxValue);
							}
							break;
						}
					}
					else
					{
						this.resultFloats[i] = this.fromFloats[i];
					}
				}
				else
				{
					this.resultFloats[i] = this.fromFloats[i];
				}
			}
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x00202D30 File Offset: 0x00200F30
		private void CheckFinished()
		{
			if (this.isRunning && !this.looping)
			{
				this.finishAction = true;
				for (int i = 0; i < this.endTimes.Length; i++)
				{
					if (this.currentTime < this.endTimes[i])
					{
						this.finishAction = false;
					}
				}
				this.isRunning = !this.finishAction;
			}
		}

		// Token: 0x04006502 RID: 25858
		[Tooltip("Define animation time,\u00a0scaling the curve to fit.")]
		public FsmFloat time;

		// Token: 0x04006503 RID: 25859
		[Tooltip("If you define speed, your animation will speed up or slow down.")]
		public FsmFloat speed;

		// Token: 0x04006504 RID: 25860
		[Tooltip("Delayed animation start.")]
		public FsmFloat delay;

		// Token: 0x04006505 RID: 25861
		[Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
		public FsmBool ignoreCurveOffset;

		// Token: 0x04006506 RID: 25862
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x04006507 RID: 25863
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x04006508 RID: 25864
		private float startTime;

		// Token: 0x04006509 RID: 25865
		private float currentTime;

		// Token: 0x0400650A RID: 25866
		private float[] endTimes;

		// Token: 0x0400650B RID: 25867
		private float lastTime;

		// Token: 0x0400650C RID: 25868
		private float deltaTime;

		// Token: 0x0400650D RID: 25869
		private float delayTime;

		// Token: 0x0400650E RID: 25870
		private float[] keyOffsets;

		// Token: 0x0400650F RID: 25871
		protected AnimationCurve[] curves;

		// Token: 0x04006510 RID: 25872
		protected AnimateFsmAction.Calculation[] calculations;

		// Token: 0x04006511 RID: 25873
		protected float[] resultFloats;

		// Token: 0x04006512 RID: 25874
		protected float[] fromFloats;

		// Token: 0x04006513 RID: 25875
		protected float[] toFloats;

		// Token: 0x04006514 RID: 25876
		protected bool finishAction;

		// Token: 0x04006515 RID: 25877
		protected bool isRunning;

		// Token: 0x04006516 RID: 25878
		protected bool looping;

		// Token: 0x04006517 RID: 25879
		private bool start;

		// Token: 0x04006518 RID: 25880
		private float largestEndTime;

		// Token: 0x02001B96 RID: 7062
		public enum Calculation
		{
			// Token: 0x04009DAD RID: 40365
			None,
			// Token: 0x04009DAE RID: 40366
			SetValue,
			// Token: 0x04009DAF RID: 40367
			AddToValue,
			// Token: 0x04009DB0 RID: 40368
			SubtractFromValue,
			// Token: 0x04009DB1 RID: 40369
			SubtractValueFromCurve,
			// Token: 0x04009DB2 RID: 40370
			MultiplyValue,
			// Token: 0x04009DB3 RID: 40371
			DivideValue,
			// Token: 0x04009DB4 RID: 40372
			DivideCurveByValue
		}
	}
}
