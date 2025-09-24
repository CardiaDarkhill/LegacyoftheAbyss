using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class CrossSceneWalker : MonoBehaviour
{
	// Token: 0x06002C21 RID: 11297 RVA: 0x000C14E0 File Offset: 0x000BF6E0
	private void OnValidate()
	{
		if (this.timerRange.End > 68.5f)
		{
			this.timerRange.End = 68.5f;
		}
		else if (this.timerRange.End < this.timerRange.Start)
		{
			this.timerRange.End = this.timerRange.Start;
		}
		if (this.timerRange.Start > this.timerRange.End)
		{
			this.timerRange.Start = this.timerRange.End;
		}
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x000C156D File Offset: 0x000BF76D
	private void OnEnable()
	{
		CrossSceneWalker._activeWalkers.AddIfNotPresent(this);
		if (!this.walker)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000C158F File Offset: 0x000BF78F
	private void OnDisable()
	{
		CrossSceneWalker._activeWalkers.Remove(this);
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000C159D File Offset: 0x000BF79D
	private void Start()
	{
		this.pd = PlayerData.instance;
		if (!this.activeCondition.IsFulfilled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x000C15C0 File Offset: 0x000BF7C0
	private void Update()
	{
		if (!this.hasTicked)
		{
			return;
		}
		if (this.isPaused)
		{
			return;
		}
		float fisherWalkerTimer = this.pd.FisherWalkerTimer;
		bool fisherWalkerDirection = this.pd.FisherWalkerDirection;
		float num = (fisherWalkerTimer - this.timerRange.Start) / (this.timerRange.End - this.timerRange.Start);
		this.wasInRange = this.isInRange;
		this.isInRange = (num >= 0f && num <= 1f);
		if (!this.isInRange)
		{
			return;
		}
		if (this.wasInRange)
		{
			return;
		}
		if (!fisherWalkerDirection)
		{
			this.walker.StartWalking(num, 1f);
			return;
		}
		this.walker.StartWalking(num, -1f);
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000C167B File Offset: 0x000BF87B
	public void PauseWalker()
	{
		this.walker.StopWalking();
		this.isPaused = true;
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x000C168F File Offset: 0x000BF88F
	public void ResumeWalker()
	{
		this.isPaused = false;
		this.walker.ResumeWalking();
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000C16A4 File Offset: 0x000BF8A4
	public static void Tick()
	{
		if (CrossSceneWalker._activeWalkers.Exists((CrossSceneWalker a) => a.isPaused))
		{
			return;
		}
		if (GameManager.instance.sceneNameHash == CrossSceneWalker._blockedScene)
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		foreach (CrossSceneWalker crossSceneWalker in CrossSceneWalker._activeWalkers)
		{
			crossSceneWalker.hasTicked = true;
		}
		float num = instance.FisherWalkerTimer;
		bool flag = instance.FisherWalkerDirection;
		if ((num >= 68.5f && !flag) || (num <= 0f && flag))
		{
			if (instance.FisherWalkerIdleTimeLeft <= 0f)
			{
				instance.FisherWalkerIdleTimeLeft = Random.Range(60f, 120f);
			}
			else
			{
				instance.FisherWalkerIdleTimeLeft -= Time.deltaTime;
				if (instance.FisherWalkerIdleTimeLeft > 0f)
				{
					return;
				}
				flag = !flag;
				instance.FisherWalkerDirection = flag;
				num = (flag ? 68.5f : 0f);
			}
		}
		if (flag)
		{
			num -= Time.deltaTime;
		}
		else
		{
			num += Time.deltaTime;
		}
		instance.FisherWalkerTimer = num;
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000C17E0 File Offset: 0x000BF9E0
	[UsedImplicitly]
	public static bool IsHome()
	{
		return PlayerData.instance.FisherWalkerTimer <= 0f;
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000C17F8 File Offset: 0x000BF9F8
	[UsedImplicitly]
	public static void ResetIdleTime()
	{
		PlayerData instance = PlayerData.instance;
		if (instance.FisherWalkerTimer > 0f || instance.FisherWalkerTimer < 120f)
		{
			return;
		}
		instance.FisherWalkerIdleTimeLeft = Random.Range(60f, 120f);
	}

	// Token: 0x04002D8A RID: 11658
	private const float MAX_WALK_TIME = 68.5f;

	// Token: 0x04002D8B RID: 11659
	private const float MIN_PAUSE_TIME = 60f;

	// Token: 0x04002D8C RID: 11660
	private const float MAX_PAUSE_TIME = 120f;

	// Token: 0x04002D8D RID: 11661
	private static readonly int _blockedScene = "Belltown_Room_Fisher".GetHashCode();

	// Token: 0x04002D8E RID: 11662
	[SerializeField]
	private PlayerDataTest activeCondition;

	// Token: 0x04002D8F RID: 11663
	[Space]
	[SerializeField]
	private MinMaxFloat timerRange;

	// Token: 0x04002D90 RID: 11664
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private SplineWalker walker;

	// Token: 0x04002D91 RID: 11665
	private bool isInRange;

	// Token: 0x04002D92 RID: 11666
	private bool wasInRange;

	// Token: 0x04002D93 RID: 11667
	private bool isPaused;

	// Token: 0x04002D94 RID: 11668
	private bool hasTicked;

	// Token: 0x04002D95 RID: 11669
	private PlayerData pd;

	// Token: 0x04002D96 RID: 11670
	private static readonly List<CrossSceneWalker> _activeWalkers = new List<CrossSceneWalker>();
}
