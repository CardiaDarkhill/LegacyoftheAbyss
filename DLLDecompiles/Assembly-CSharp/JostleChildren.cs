using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class JostleChildren : MonoBehaviour
{
	// Token: 0x0600049E RID: 1182 RVA: 0x00018C44 File Offset: 0x00016E44
	private void Start()
	{
		int childCount = base.transform.childCount;
		this.children = new List<Transform>(childCount);
		this.initialPositions = new List<Vector3>(childCount);
		this.targetPositions = new List<Vector3>(childCount);
		this.initialEulerAngles = new List<Vector3>(childCount);
		this.targetEulerAngles = new List<Vector3>(childCount);
		this.isReady = true;
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00018CA0 File Offset: 0x00016EA0
	[ContextMenu("Do Jostle")]
	public void DoJostle()
	{
		if (!this.isReady)
		{
			return;
		}
		if (this.jostleRoutine != null)
		{
			base.StopCoroutine(this.jostleRoutine);
			this.jostleRoutine = null;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].localPosition = this.initialPositions[i];
			}
		}
		this.children.Clear();
		this.initialPositions.Clear();
		this.targetPositions.Clear();
		this.initialEulerAngles.Clear();
		this.targetEulerAngles.Clear();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.children.Add(transform);
			this.initialPositions.Add(transform.localPosition);
			this.initialEulerAngles.Add(transform.localEulerAngles);
			Vector3 randomVector3InRange = Helper.GetRandomVector3InRange(this.offsetMin, this.offsetMax);
			this.targetPositions.Add(transform.localPosition + randomVector3InRange);
			Vector3 randomVector3InRange2 = Helper.GetRandomVector3InRange(this.rotationOffsetMin, this.rotationOffsetMax);
			this.targetEulerAngles.Add(transform.localEulerAngles + randomVector3InRange2);
		}
		this.cameraShake.DoShake(this, true);
		this.jostleRoutine = this.StartTimerRoutine(0f, this.jostleDuration, delegate(float time)
		{
			time = this.jostleCurve.Evaluate(time);
			int count = this.children.Count;
			for (int j = 0; j < count; j++)
			{
				Transform transform2 = this.children[j];
				transform2.localPosition = Vector3.LerpUnclamped(this.initialPositions[j], this.targetPositions[j], time);
				transform2.localEulerAngles = Vector3.LerpUnclamped(this.initialEulerAngles[j], this.targetEulerAngles[j], time);
			}
		}, null, delegate
		{
			this.jostleRoutine = null;
		}, false);
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00018E4C File Offset: 0x0001704C
	[ContextMenu("Do Jostle", true)]
	private bool CanTestJostle()
	{
		return Application.isPlaying;
	}

	// Token: 0x0400045E RID: 1118
	[SerializeField]
	private Vector3 offsetMin;

	// Token: 0x0400045F RID: 1119
	[SerializeField]
	private Vector3 offsetMax;

	// Token: 0x04000460 RID: 1120
	[Space]
	[SerializeField]
	private Vector3 rotationOffsetMin;

	// Token: 0x04000461 RID: 1121
	[SerializeField]
	private Vector3 rotationOffsetMax;

	// Token: 0x04000462 RID: 1122
	[Space]
	[SerializeField]
	private AnimationCurve jostleCurve;

	// Token: 0x04000463 RID: 1123
	[SerializeField]
	private float jostleDuration;

	// Token: 0x04000464 RID: 1124
	[Space]
	[SerializeField]
	private CameraShakeTarget cameraShake;

	// Token: 0x04000465 RID: 1125
	private List<Transform> children;

	// Token: 0x04000466 RID: 1126
	private List<Vector3> initialPositions;

	// Token: 0x04000467 RID: 1127
	private List<Vector3> targetPositions;

	// Token: 0x04000468 RID: 1128
	private List<Vector3> initialEulerAngles;

	// Token: 0x04000469 RID: 1129
	private List<Vector3> targetEulerAngles;

	// Token: 0x0400046A RID: 1130
	private bool isReady;

	// Token: 0x0400046B RID: 1131
	private Coroutine jostleRoutine;
}
