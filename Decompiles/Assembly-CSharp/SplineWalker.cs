using System;
using GlobalEnums;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class SplineWalker : MonoBehaviour, IInteractableBlocker
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000574 RID: 1396 RVA: 0x0001C495 File Offset: 0x0001A695
	public bool IsBlocking
	{
		get
		{
			return this.currentMoveState == SplineWalker.MoveStates.Flying || this.isInCustomAnim;
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001C4A8 File Offset: 0x0001A6A8
	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.targetPos, 0.15f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(this.adjustedTargetPos, 0.2f);
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001C4E8 File Offset: 0x0001A6E8
	private void Awake()
	{
		this.box = base.GetComponent<BoxCollider2D>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.enviroListener = base.gameObject.AddComponent<EnviroRegionListener>();
		EnviroRegionListener enviroRegionListener = this.enviroListener;
		enviroRegionListener.CurrentEnvironmentTypeChanged = (Action<EnvironmentTypes>)Delegate.Combine(enviroRegionListener.CurrentEnvironmentTypeChanged, new Action<EnvironmentTypes>(this.OnCurrentEnvironmentTypeChanged));
		this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
		this.heroPerformanceSingReaction = base.GetComponent<HeroPerformanceSingReaction>();
		this.hasHeroPerformanceSingReaction = (this.heroPerformanceSingReaction != null);
		if (this.blockInteractable)
		{
			this.blockInteractable.AddBlocker(this);
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001C593 File Offset: 0x0001A793
	private void Update()
	{
		this.UpdateState();
		this.prevMoveState = this.currentMoveState;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001C5A8 File Offset: 0x0001A7A8
	private void UpdateState()
	{
		if (!this.isFollowingPath || this.isInCustomAnim)
		{
			return;
		}
		this.UpdateTarget();
		this.UpdateRunner();
		if (!this.isFollowingPath || this.isInCustomAnim)
		{
			return;
		}
		SplineWalker.MoveStates moveStates = this.currentMoveState;
		if (moveStates != SplineWalker.MoveStates.Walking)
		{
			if (moveStates != SplineWalker.MoveStates.Flying)
			{
				throw new ArgumentOutOfRangeException();
			}
			string name = (this.runnerVelocity.y > 0f) ? "Fly Up" : "Fly Down";
			if (!this.animator.IsPlaying(name))
			{
				this.animator.Play(name);
			}
			this.box.enabled = this.CanCollide(this.targetPos.z);
			float z = this.targetPos.z;
			if (z < this.walkingZ)
			{
				z = this.walkingZ;
			}
			base.transform.SetPositionZ(z);
			return;
		}
		else
		{
			if (this.prevMoveState != SplineWalker.MoveStates.Walking)
			{
				if (this.hasHeroPerformanceSingReaction)
				{
					this.heroPerformanceSingReaction.enabled = true;
				}
				if (!this.animator.IsPlaying("Walk") && !this.animator.IsPlaying("TurnToWalk"))
				{
					this.animator.Play("Walk");
				}
				if (this.loopSource)
				{
					this.loopSource.Play();
				}
				if (this.audioLoopFly)
				{
					this.audioLoopFly.Stop();
				}
				if (this.walkVoiceController)
				{
					this.walkVoiceController.SendEventSafe("WALK START");
				}
				base.transform.SetPositionZ(this.walkingZ);
				this.box.enabled = true;
				return;
			}
			return;
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001C73B File Offset: 0x0001A93B
	private bool CanCollide(float zPos)
	{
		return zPos.IsWithinTolerance(0.1f, this.walkingZ);
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001C750 File Offset: 0x0001A950
	private void UpdateTarget()
	{
		if (this.isTargetAtEnd)
		{
			return;
		}
		Vector2 b = base.transform.position;
		this.targetPos = this.path.GetPositionAlongSpline(this.splineDistance);
		while ((this.targetPos - b).magnitude < this.targetDistance)
		{
			this.splineDistance += 0.1f * this.splineDirection;
			this.targetPos = this.path.GetPositionAlongSpline(this.splineDistance);
			if (this.splineDirection > 0f)
			{
				if (this.splineDistance < this.path.TotalDistance)
				{
					continue;
				}
			}
			else if (this.splineDistance > 0f)
			{
				continue;
			}
			this.isTargetAtEnd = true;
			break;
		}
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001C818 File Offset: 0x0001AA18
	private void UpdateRunner()
	{
		this.TrySnapToGround(this.targetPos, out this.adjustedTargetPos, true);
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		Vector2 vector2 = this.adjustedTargetPos - vector;
		Vector2 vector3 = vector2;
		float d;
		if (this.currentMoveState == SplineWalker.MoveStates.Walking)
		{
			vector3.y = 0f;
			d = this.walkSpeed;
		}
		else
		{
			if (vector3.y > 0f)
			{
				this.currentFlySpeed += this.flyAcceleration * Time.deltaTime;
			}
			else
			{
				this.currentFlySpeed = this.flySpeed;
			}
			if (this.flyMaxSpeed > this.flySpeed && this.currentFlySpeed > this.flyMaxSpeed)
			{
				this.currentFlySpeed = this.flyMaxSpeed;
			}
			d = this.currentFlySpeed;
		}
		vector3 = ((vector3.magnitude > 0f) ? vector3.normalized : Vector2.zero);
		this.runnerVelocity = vector3 * d;
		if (this.isTargetAtEnd && this.ShouldTurn(this.runnerVelocity.x))
		{
			this.StopWalking();
			return;
		}
		vector += this.runnerVelocity.ToVector3(0f) * Time.deltaTime;
		Vector3 v = vector;
		Vector3 vector4;
		float num;
		if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
		{
			this.StartFlying(true);
		}
		else if (this.TrySnapToGround(vector, out vector4, false, out num))
		{
			if (this.wasGroundSnapped)
			{
				if (vector4.y + 0.2f < this.previousGroundSnappedY)
				{
					this.wasGroundSnapped = false;
				}
				else if (vector4.y - 0.2f > this.previousGroundSnappedY && this.animator.IsPlaying("Walk"))
				{
					this.animator.PlayFromFrame(0);
				}
			}
			if (num < 0.1f)
			{
				if (this.currentMoveState == SplineWalker.MoveStates.Flying && this.landAudioClipTable)
				{
					this.landAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
				}
				if (this.currentMoveState == SplineWalker.MoveStates.Flying && this.landVoiceAudioClipTable)
				{
					this.landVoiceAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
				}
				this.currentMoveState = SplineWalker.MoveStates.Walking;
				this.wasGroundSnapped = true;
				this.groundSnapT = 1f;
				this.groundSnapLerpMultiplier = 1f;
				v = vector4;
			}
			else
			{
				if (!this.wasGroundSnapped)
				{
					this.wasGroundSnapped = true;
					this.groundSnapLerpMultiplier = 1f;
					this.groundSnapT = 0f;
				}
				this.groundSnapLerpMultiplier += Time.deltaTime * Mathf.Abs(Physics2D.gravity.y);
				this.groundSnapT += Time.deltaTime * this.groundSnapLerpMultiplier;
				if (this.groundSnapT > 1f)
				{
					this.groundSnapT = 1f;
				}
				v = Vector2.Lerp(vector, vector4, this.groundSnapT);
			}
			this.previousGroundSnappedY = vector4.y;
		}
		else
		{
			this.StartFlying(vector2.y > 0f);
		}
		if (this.ShouldTurn(this.runnerVelocity.x))
		{
			this.animator.transform.FlipLocalScale(true, false, false);
			if (this.currentMoveState == SplineWalker.MoveStates.Walking)
			{
				this.animator.Play("TurnToWalk");
			}
		}
		transform.SetPosition2D(v);
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001CB7C File Offset: 0x0001AD7C
	private void StartFlying(bool doLaunch)
	{
		this.wasGroundSnapped = false;
		if (this.loopSource)
		{
			this.loopSource.Stop();
		}
		if (this.currentMoveState == SplineWalker.MoveStates.Flying)
		{
			return;
		}
		if (this.audioLoopFly)
		{
			this.audioLoopFly.Play();
		}
		if (this.takeoffVoiceAudioClipTable)
		{
			this.takeoffVoiceAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		if (this.walkVoiceController)
		{
			this.walkVoiceController.SendEventSafe("WALK STOP");
		}
		if (doLaunch)
		{
			this.animator.Play("Fly Antic");
			this.isInCustomAnim = true;
		}
		this.currentMoveState = SplineWalker.MoveStates.Flying;
		if (this.hasHeroPerformanceSingReaction)
		{
			this.heroPerformanceSingReaction.enabled = false;
		}
		this.currentFlySpeed = this.flySpeed;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001CC50 File Offset: 0x0001AE50
	private bool ShouldTurn(float moveDir)
	{
		float num = base.transform.localScale.x * (float)(this.spriteFacesRight ? 1 : -1);
		return (num < 0f && moveDir > 0f) || (num > 0f && moveDir < 0f);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001CCA0 File Offset: 0x0001AEA0
	private void TrySnapToGround(Vector3 initialPos, out Vector3 newPos, bool isTarget)
	{
		float num;
		this.TrySnapToGround(initialPos, out newPos, isTarget, out num);
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001CCBC File Offset: 0x0001AEBC
	private bool TrySnapToGround(Vector3 initialPos, out Vector3 newPos, bool isTarget, out float groundDistance)
	{
		Vector2 vector = initialPos + this.box.offset;
		Vector2 vector2 = this.box.size * 0.5f;
		Vector2 vector3 = vector - vector2;
		float num = vector.y - vector3.y;
		RaycastHit2D hit;
		if (this.CanCollide(initialPos.z))
		{
			float distance = num + this.groundDistanceThreshold;
			hit = Helper.Raycast2D(vector, Vector2.down, distance, 256);
		}
		else
		{
			hit = default(RaycastHit2D);
		}
		bool result;
		if (hit)
		{
			newPos = hit.point;
			newPos.y += vector2.y - this.box.offset.y;
			result = true;
			groundDistance = vector3.y - hit.point.y;
		}
		else
		{
			newPos = initialPos;
			if (isTarget)
			{
				newPos.y += num;
			}
			result = false;
			groundDistance = float.MaxValue;
		}
		return result;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001CDBC File Offset: 0x0001AFBC
	private void OnAnimationCompleted(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip)
	{
		if (!this.isFollowingPath)
		{
			return;
		}
		string name = clip.name;
		if (name == "Fly Antic" || name == "Land")
		{
			this.isInCustomAnim = false;
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001CDFA File Offset: 0x0001AFFA
	[ContextMenu("TEST FORWARD", true)]
	[ContextMenu("TEST BACKWARD", true)]
	public bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001CE01 File Offset: 0x0001B001
	[ContextMenu("TEST FORWARD")]
	public void TestForward()
	{
		this.StartWalking(0f, 1f);
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0001CE13 File Offset: 0x0001B013
	[ContextMenu("TEST BACKWARD")]
	public void TestBackward()
	{
		this.StartWalking(1f, -1f);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0001CE28 File Offset: 0x0001B028
	public void StartWalking(float percentage, float moveDirection)
	{
		this.StopWalking();
		this.isInCustomAnim = false;
		this.isTargetAtEnd = false;
		this.splineDirection = moveDirection;
		this.prevMoveState = SplineWalker.MoveStates.None;
		this.splineDistance = this.path.TotalDistance * percentage;
		Vector3 positionAlongSpline = this.path.GetPositionAlongSpline(this.splineDistance);
		positionAlongSpline.y += 1f;
		this.TrySnapToGround(positionAlongSpline, out positionAlongSpline, false);
		base.transform.position = positionAlongSpline;
		float moveDir = this.path.GetPositionAlongSpline(this.splineDistance + 0.1f * this.splineDirection).x - positionAlongSpline.x;
		if (this.ShouldTurn(moveDir))
		{
			base.transform.FlipLocalScale(true, false, false);
		}
		this.ResumeWalking();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001CEEC File Offset: 0x0001B0EC
	public void StopWalking()
	{
		this.isFollowingPath = false;
		if (this.loopSource)
		{
			this.loopSource.Stop();
		}
		if (this.audioLoopFly)
		{
			this.audioLoopFly.Stop();
		}
		if (this.walkVoiceController)
		{
			this.walkVoiceController.SendEventSafe("WALK STOP");
		}
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0001CF4D File Offset: 0x0001B14D
	public void ResumeWalking()
	{
		this.isFollowingPath = true;
		this.prevMoveState = SplineWalker.MoveStates.None;
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0001CF60 File Offset: 0x0001B160
	private void OnCurrentEnvironmentTypeChanged(EnvironmentTypes newEnviroType)
	{
		if (!this.loopSource)
		{
			return;
		}
		AudioClip audioClip = null;
		foreach (SplineWalker.WalkAudio walkAudio in this.walkAudios)
		{
			if (walkAudio.Environment == newEnviroType)
			{
				audioClip = walkAudio.Clip;
				break;
			}
		}
		if (!audioClip)
		{
			this.loopSource.clip = null;
			this.loopSource.Stop();
			Debug.LogWarning(string.Format("{0} does not have clip for {1} enviro type", base.gameObject.name, newEnviroType));
			return;
		}
		this.loopSource.clip = audioClip;
		if (this.currentMoveState == SplineWalker.MoveStates.Walking)
		{
			this.loopSource.Play();
		}
	}

	// Token: 0x04000578 RID: 1400
	private const string FLY_UP_ANIM = "Fly Up";

	// Token: 0x04000579 RID: 1401
	private const string FLY_DOWN_ANIM = "Fly Down";

	// Token: 0x0400057A RID: 1402
	private const string FLY_ANTIC_ANIM = "Fly Antic";

	// Token: 0x0400057B RID: 1403
	private const string FLY_LAND_ANIM = "Land";

	// Token: 0x0400057C RID: 1404
	private const string WALK_ANIM = "Walk";

	// Token: 0x0400057D RID: 1405
	private const string WALK_TURNED_ANIM = "TurnToWalk";

	// Token: 0x0400057E RID: 1406
	[SerializeField]
	private HermiteSplinePath path;

	// Token: 0x0400057F RID: 1407
	[SerializeField]
	private float walkingZ;

	// Token: 0x04000580 RID: 1408
	[SerializeField]
	private InteractableBase blockInteractable;

	// Token: 0x04000581 RID: 1409
	[Space]
	[SerializeField]
	private float walkSpeed;

	// Token: 0x04000582 RID: 1410
	[SerializeField]
	private float flySpeed;

	// Token: 0x04000583 RID: 1411
	[SerializeField]
	private float flyAcceleration;

	// Token: 0x04000584 RID: 1412
	[SerializeField]
	private float flyMaxSpeed;

	// Token: 0x04000585 RID: 1413
	[SerializeField]
	private bool spriteFacesRight;

	// Token: 0x04000586 RID: 1414
	[SerializeField]
	private float groundDistanceThreshold;

	// Token: 0x04000587 RID: 1415
	[SerializeField]
	private float targetDistance;

	// Token: 0x04000588 RID: 1416
	[Space]
	[SerializeField]
	private AudioSource loopSource;

	// Token: 0x04000589 RID: 1417
	[SerializeField]
	private SplineWalker.WalkAudio[] walkAudios;

	// Token: 0x0400058A RID: 1418
	[Space]
	[SerializeField]
	private AudioSource audioLoopFly;

	// Token: 0x0400058B RID: 1419
	[SerializeField]
	private PlayMakerFSM walkVoiceController;

	// Token: 0x0400058C RID: 1420
	[SerializeField]
	private RandomAudioClipTable landAudioClipTable;

	// Token: 0x0400058D RID: 1421
	[SerializeField]
	private RandomAudioClipTable takeoffVoiceAudioClipTable;

	// Token: 0x0400058E RID: 1422
	[SerializeField]
	private RandomAudioClipTable landVoiceAudioClipTable;

	// Token: 0x0400058F RID: 1423
	private Vector3 targetPos;

	// Token: 0x04000590 RID: 1424
	private Vector3 adjustedTargetPos;

	// Token: 0x04000591 RID: 1425
	private Vector2 runnerVelocity;

	// Token: 0x04000592 RID: 1426
	private float splineDirection;

	// Token: 0x04000593 RID: 1427
	private float currentFlySpeed;

	// Token: 0x04000594 RID: 1428
	private bool isFollowingPath;

	// Token: 0x04000595 RID: 1429
	private bool isInCustomAnim;

	// Token: 0x04000596 RID: 1430
	private float splineDistance;

	// Token: 0x04000597 RID: 1431
	private bool isTargetAtEnd;

	// Token: 0x04000598 RID: 1432
	private SplineWalker.MoveStates currentMoveState;

	// Token: 0x04000599 RID: 1433
	private SplineWalker.MoveStates prevMoveState;

	// Token: 0x0400059A RID: 1434
	private bool wasGroundSnapped;

	// Token: 0x0400059B RID: 1435
	private float groundSnapLerpMultiplier;

	// Token: 0x0400059C RID: 1436
	private float groundSnapT;

	// Token: 0x0400059D RID: 1437
	private float previousGroundSnappedY;

	// Token: 0x0400059E RID: 1438
	private BoxCollider2D box;

	// Token: 0x0400059F RID: 1439
	private tk2dSpriteAnimator animator;

	// Token: 0x040005A0 RID: 1440
	private EnviroRegionListener enviroListener;

	// Token: 0x040005A1 RID: 1441
	private HeroPerformanceSingReaction heroPerformanceSingReaction;

	// Token: 0x040005A2 RID: 1442
	private bool hasHeroPerformanceSingReaction;

	// Token: 0x02001420 RID: 5152
	private enum MoveStates
	{
		// Token: 0x04008205 RID: 33285
		None = -1,
		// Token: 0x04008206 RID: 33286
		Walking,
		// Token: 0x04008207 RID: 33287
		Flying
	}

	// Token: 0x02001421 RID: 5153
	[Serializable]
	private class WalkAudio
	{
		// Token: 0x04008208 RID: 33288
		public EnvironmentTypes Environment;

		// Token: 0x04008209 RID: 33289
		public AudioClip Clip;
	}
}
