using System;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class SprintRaceController : MonoBehaviour
{
	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06000FFF RID: 4095 RVA: 0x0004D788 File Offset: 0x0004B988
	// (remove) Token: 0x06001000 RID: 4096 RVA: 0x0004D7C0 File Offset: 0x0004B9C0
	public event SprintRaceController.RaceCompletedDelegate RaceCompleted;

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06001001 RID: 4097 RVA: 0x0004D7F8 File Offset: 0x0004B9F8
	// (remove) Token: 0x06001002 RID: 4098 RVA: 0x0004D830 File Offset: 0x0004BA30
	public event Action RaceDisqualified;

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06001003 RID: 4099 RVA: 0x0004D865 File Offset: 0x0004BA65
	public float DashDownDuration
	{
		get
		{
			return this.dashDownDuration;
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06001004 RID: 4100 RVA: 0x0004D86D File Offset: 0x0004BA6D
	// (set) Token: 0x06001005 RID: 4101 RVA: 0x0004D875 File Offset: 0x0004BA75
	public bool IsTracking { get; private set; }

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06001006 RID: 4102 RVA: 0x0004D87E File Offset: 0x0004BA7E
	public bool IsNextLapMarkerEnd
	{
		get
		{
			return this.nextLapMarkerIndex == this.lapMarkers.Length - 1;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001007 RID: 4103 RVA: 0x0004D892 File Offset: 0x0004BA92
	public float TotalPathDistance
	{
		get
		{
			if (!this.path)
			{
				return 0f;
			}
			return this.path.TotalDistance;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001008 RID: 4104 RVA: 0x0004D8B2 File Offset: 0x0004BAB2
	public int LapCount
	{
		get
		{
			return this.lapCount;
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001009 RID: 4105 RVA: 0x0004D8BA File Offset: 0x0004BABA
	public SavedItem Reward
	{
		get
		{
			return this.reward;
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x0004D8C2 File Offset: 0x0004BAC2
	private void OnValidate()
	{
		if (this.lapCount < 1)
		{
			this.lapCount = 1;
		}
		this.SyncLapCount();
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x0004D8DC File Offset: 0x0004BADC
	private void SyncLapCount()
	{
		if (this.lapBaseSpeeds != null && this.lapBaseSpeeds.Length == this.lapCount)
		{
			return;
		}
		float[] array = this.lapBaseSpeeds;
		this.lapBaseSpeeds = new float[this.lapCount];
		if (array == null)
		{
			return;
		}
		int i = 0;
		while (i < this.lapBaseSpeeds.Length)
		{
			int num = i;
			if (num < array.Length)
			{
				goto IL_4B;
			}
			num = array.Length - 1;
			if (num >= 0)
			{
				goto IL_4B;
			}
			IL_56:
			i++;
			continue;
			IL_4B:
			this.lapBaseSpeeds[i] = array[num];
			goto IL_56;
		}
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x0004D950 File Offset: 0x0004BB50
	private void Awake()
	{
		this.OnValidate();
		if (this.lapMarkers == null)
		{
			return;
		}
		for (int i = 0; i < this.lapMarkers.Length; i++)
		{
			int capturedIndex = i;
			this.lapMarkers[i].RegisterController(this, delegate(bool canDisqualify)
			{
				bool result;
				this.ReportHeroLapMarkerHit(capturedIndex, canDisqualify, out result);
				return result;
			});
		}
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x0004D9AD File Offset: 0x0004BBAD
	private void OnDisable()
	{
		if (this.IsTracking)
		{
			this.StopTracking();
		}
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0004D9C0 File Offset: 0x0004BBC0
	private void OnDrawGizmosSelected()
	{
		if (this.lapMarkers == null)
		{
			return;
		}
		for (int i = 0; i < this.lapMarkers.Length; i++)
		{
			SprintRaceLapMarker sprintRaceLapMarker = this.lapMarkers[i];
			if (sprintRaceLapMarker)
			{
				Transform transform = sprintRaceLapMarker.transform;
				BoxCollider2D component = sprintRaceLapMarker.GetComponent<BoxCollider2D>();
				if (component)
				{
					Matrix4x4 matrix = Gizmos.matrix;
					Gizmos.matrix = transform.localToWorldMatrix;
					Gizmos.DrawWireCube(component.offset, component.size);
					Gizmos.matrix = matrix;
				}
				if (i != 0)
				{
					SprintRaceLapMarker sprintRaceLapMarker2 = this.lapMarkers[i - 1];
					if (sprintRaceLapMarker2)
					{
						Vector3 position = transform.position;
						Gizmos.DrawLine(sprintRaceLapMarker2.transform.position, position);
					}
				}
			}
		}
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0004DA79 File Offset: 0x0004BC79
	public Vector2 GetPositionAlongSpline(float splineDistance)
	{
		if (!this.path)
		{
			return Vector2.zero;
		}
		return this.path.GetPositionAlongSpline(splineDistance);
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x0004DA9F File Offset: 0x0004BC9F
	public float GetDistanceAlongSpline(Vector2 position, bool getNext = false)
	{
		if (!this.path)
		{
			return 0f;
		}
		return this.path.GetDistanceAlongSpline(position, getNext);
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x0004DAC6 File Offset: 0x0004BCC6
	public void BeginInRace()
	{
		this.counter.SetCap(this.lapCount);
		this.counter.SetCurrent(0);
		this.counter.Appear();
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x0004DAF0 File Offset: 0x0004BCF0
	public void EndInRace()
	{
		this.counter.Disappear();
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x0004DAFD File Offset: 0x0004BCFD
	public void StartTracking()
	{
		this.runnerLapsCompleted = 0;
		this.heroLapsCompleted = 0;
		this.nextLapMarkerIndex = 0;
		this.IsTracking = true;
		this.isCompleted = false;
		HazardRespawnTrigger.IsSuppressed = true;
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x0004DB28 File Offset: 0x0004BD28
	public void StopTracking()
	{
		this.IsTracking = false;
		HazardRespawnTrigger.IsSuppressed = false;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x0004DB37 File Offset: 0x0004BD37
	public void ReportRunnerLapCompleted(out bool isRaceComplete)
	{
		if (this.IsTracking)
		{
			this.runnerLapsCompleted++;
			this.CheckCompletion(false);
		}
		isRaceComplete = !this.IsTracking;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x0004DB64 File Offset: 0x0004BD64
	private void ReportHeroLapMarkerHit(int index, bool canDisqualify, out bool wasCorrect)
	{
		wasCorrect = false;
		if (!this.IsTracking)
		{
			return;
		}
		if (index == this.nextLapMarkerIndex - 1)
		{
			return;
		}
		if (index != this.nextLapMarkerIndex)
		{
			if (!canDisqualify)
			{
				return;
			}
			this.StopTracking();
			Action raceDisqualified = this.RaceDisqualified;
			if (raceDisqualified != null)
			{
				raceDisqualified();
			}
			if (!string.IsNullOrEmpty(this.raceEndEvent))
			{
				EventRegister.SendEvent(this.raceEndEvent, null);
			}
			return;
		}
		else
		{
			this.nextLapMarkerIndex++;
			wasCorrect = true;
			if (this.nextLapMarkerIndex < this.lapMarkers.Length)
			{
				return;
			}
			this.heroLapsCompleted++;
			this.nextLapMarkerIndex = 0;
			this.counter.SetCurrent(this.heroLapsCompleted);
			this.CheckCompletion(true);
			return;
		}
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x0004DC18 File Offset: 0x0004BE18
	private void CheckCompletion(bool isHero)
	{
		if (isHero)
		{
			if (this.heroLapsCompleted < this.lapCount)
			{
				return;
			}
			SprintRaceController.RaceCompletedDelegate raceCompleted = this.RaceCompleted;
			if (raceCompleted != null)
			{
				raceCompleted(true);
			}
		}
		else
		{
			if (this.runnerLapsCompleted != this.lapCount)
			{
				return;
			}
			SprintRaceController.RaceCompletedDelegate raceCompleted2 = this.RaceCompleted;
			if (raceCompleted2 != null)
			{
				raceCompleted2(false);
			}
		}
		if (this.isCompleted)
		{
			return;
		}
		this.isCompleted = true;
		if (!string.IsNullOrEmpty(this.raceEndCompleteEvent))
		{
			EventRegister.SendEvent(this.raceEndCompleteEvent, null);
		}
		if (!string.IsNullOrEmpty(this.raceEndEvent))
		{
			EventRegister.SendEvent(this.raceEndEvent, null);
		}
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x0004DCB7 File Offset: 0x0004BEB7
	public void GetRaceInfo(out int runnerLaps, out int heroLaps, out float currentBaseSpeed)
	{
		runnerLaps = this.runnerLapsCompleted;
		heroLaps = this.heroLapsCompleted;
		currentBaseSpeed = this.lapBaseSpeeds[Mathf.Clamp(heroLaps, 0, this.lapBaseSpeeds.Length - 1)];
	}

	// Token: 0x04000FA1 RID: 4001
	[SerializeField]
	private HermiteSplinePath path;

	// Token: 0x04000FA2 RID: 4002
	[SerializeField]
	private SprintRaceLapMarker[] lapMarkers;

	// Token: 0x04000FA3 RID: 4003
	[SerializeField]
	private SimpleCounter counter;

	// Token: 0x04000FA4 RID: 4004
	[Space]
	[SerializeField]
	private string raceEndEvent;

	// Token: 0x04000FA5 RID: 4005
	[SerializeField]
	private string raceEndCompleteEvent;

	// Token: 0x04000FA6 RID: 4006
	[SerializeField]
	private int lapCount;

	// Token: 0x04000FA7 RID: 4007
	[SerializeField]
	private float[] lapBaseSpeeds;

	// Token: 0x04000FA8 RID: 4008
	[SerializeField]
	private float dashDownDuration;

	// Token: 0x04000FA9 RID: 4009
	[Space]
	[SerializeField]
	private SavedItem reward;

	// Token: 0x04000FAA RID: 4010
	private int runnerLapsCompleted;

	// Token: 0x04000FAB RID: 4011
	private int heroLapsCompleted;

	// Token: 0x04000FAC RID: 4012
	private int nextLapMarkerIndex;

	// Token: 0x04000FAD RID: 4013
	private bool isCompleted;

	// Token: 0x020014DB RID: 5339
	// (Invoke) Token: 0x06008511 RID: 34065
	public delegate void RaceCompletedDelegate(bool didHeroWin);
}
