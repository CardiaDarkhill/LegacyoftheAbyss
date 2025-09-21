using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC1 RID: 3521
	[Tooltip("Animate base action - DON'T USE IT!")]
	public abstract class CurveFsmAction : FsmStateAction
	{
		// Token: 0x06006601 RID: 26113 RVA: 0x00203C98 File Offset: 0x00201E98
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
			this.distances = new float[0];
			this.endTimes = new float[0];
			this.keyOffsets = new float[0];
			this.curves = new AnimationCurve[0];
			this.finishAction = false;
			this.start = false;
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x00203D60 File Offset: 0x00201F60
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

		// Token: 0x06006603 RID: 26115 RVA: 0x00203DEC File Offset: 0x00201FEC
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
			this.distances = new float[this.fromFloats.Length];
			for (int k = 0; k < this.fromFloats.Length; k++)
			{
				this.distances[k] = this.toFloats[k] - this.fromFloats[k];
			}
		}

		// Token: 0x06006604 RID: 26116 RVA: 0x00204094 File Offset: 0x00202294
		public override void OnUpdate()
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
			if (this.isRunning && !this.finishAction)
			{
				if (this.realTime)
				{
					this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
					this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
					if (!this.speed.IsNone)
					{
						this.currentTime += this.deltaTime * this.speed.Value;
					}
					else
					{
						this.currentTime += this.deltaTime;
					}
				}
				else if (!this.speed.IsNone)
				{
					this.currentTime += Time.deltaTime * this.speed.Value;
				}
				else
				{
					this.currentTime += Time.deltaTime;
				}
				for (int i = 0; i < this.curves.Length; i++)
				{
					if (this.curves[i] != null && this.curves[i].keys.Length != 0)
					{
						if (this.calculations[i] != CurveFsmAction.Calculation.None)
						{
							switch (this.calculations[i])
							{
							case CurveFsmAction.Calculation.AddToValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) + this.curves[i].Evaluate(this.currentTime));
								}
								break;
							case CurveFsmAction.Calculation.SubtractFromValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) - this.curves[i].Evaluate(this.currentTime));
								}
								break;
							case CurveFsmAction.Calculation.SubtractValueFromCurve:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
								}
								break;
							case CurveFsmAction.Calculation.MultiplyValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
								}
								break;
							case CurveFsmAction.Calculation.DivideValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) != 0f) ? (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value) / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)) : float.MaxValue);
								}
								else
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) != 0f) ? (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]) / this.curves[i].Evaluate(this.currentTime)) : float.MaxValue);
								}
								break;
							case CurveFsmAction.Calculation.DivideCurveByValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / (this.distances[i] * (this.currentTime / this.time.Value)) + this.fromFloats[i]) : float.MaxValue);
								}
								else
								{
									this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime) / (this.distances[i] * (this.currentTime / this.endTimes[i])) + this.fromFloats[i]) : float.MaxValue);
								}
								break;
							}
						}
						else if (!this.time.IsNone)
						{
							this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
						}
						else
						{
							this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]);
						}
					}
					else if (!this.time.IsNone)
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
					}
					else if (this.largestEndTime == 0f)
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / 1f);
					}
					else
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.largestEndTime);
					}
				}
				if (this.isRunning)
				{
					this.finishAction = true;
					for (int j = 0; j < this.endTimes.Length; j++)
					{
						if (this.currentTime < this.endTimes[j])
						{
							this.finishAction = false;
						}
					}
					this.isRunning = !this.finishAction;
				}
			}
		}

		// Token: 0x0400654B RID: 25931
		[Tooltip("Define animation time, scaling the curve to fit.")]
		public FsmFloat time;

		// Token: 0x0400654C RID: 25932
		[Tooltip("If you define speed, your animation will speed up or slow down.")]
		public FsmFloat speed;

		// Token: 0x0400654D RID: 25933
		[Tooltip("Delayed animation start.")]
		public FsmFloat delay;

		// Token: 0x0400654E RID: 25934
		[Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
		public FsmBool ignoreCurveOffset;

		// Token: 0x0400654F RID: 25935
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x04006550 RID: 25936
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x04006551 RID: 25937
		private float startTime;

		// Token: 0x04006552 RID: 25938
		private float currentTime;

		// Token: 0x04006553 RID: 25939
		private float[] endTimes;

		// Token: 0x04006554 RID: 25940
		private float lastTime;

		// Token: 0x04006555 RID: 25941
		private float deltaTime;

		// Token: 0x04006556 RID: 25942
		private float delayTime;

		// Token: 0x04006557 RID: 25943
		private float[] keyOffsets;

		// Token: 0x04006558 RID: 25944
		protected AnimationCurve[] curves;

		// Token: 0x04006559 RID: 25945
		protected CurveFsmAction.Calculation[] calculations;

		// Token: 0x0400655A RID: 25946
		protected float[] resultFloats;

		// Token: 0x0400655B RID: 25947
		protected float[] fromFloats;

		// Token: 0x0400655C RID: 25948
		protected float[] toFloats;

		// Token: 0x0400655D RID: 25949
		private float[] distances;

		// Token: 0x0400655E RID: 25950
		protected bool finishAction;

		// Token: 0x0400655F RID: 25951
		protected bool isRunning;

		// Token: 0x04006560 RID: 25952
		protected bool looping;

		// Token: 0x04006561 RID: 25953
		private bool start;

		// Token: 0x04006562 RID: 25954
		private float largestEndTime;

		// Token: 0x02001B97 RID: 7063
		public enum Calculation
		{
			// Token: 0x04009DB6 RID: 40374
			None,
			// Token: 0x04009DB7 RID: 40375
			AddToValue,
			// Token: 0x04009DB8 RID: 40376
			SubtractFromValue,
			// Token: 0x04009DB9 RID: 40377
			SubtractValueFromCurve,
			// Token: 0x04009DBA RID: 40378
			MultiplyValue,
			// Token: 0x04009DBB RID: 40379
			DivideValue,
			// Token: 0x04009DBC RID: 40380
			DivideCurveByValue
		}
	}
}
