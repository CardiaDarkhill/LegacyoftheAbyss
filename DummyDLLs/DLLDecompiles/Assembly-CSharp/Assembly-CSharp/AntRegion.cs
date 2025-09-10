using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class AntRegion : MonoBehaviour
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x0600069F RID: 1695 RVA: 0x00021798 File Offset: 0x0001F998
	private bool IsGroundType
	{
		get
		{
			return this.pickupRegion;
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x000217A8 File Offset: 0x0001F9A8
	private void OnDrawGizmos()
	{
		if (!this.antLeftPoint || !this.antRightPoint)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 localPosition = this.antLeftPoint.localPosition;
		Vector3 localPosition2 = this.antRightPoint.localPosition;
		Color gray = Color.gray;
		float? num = new float?(0.5f);
		Gizmos.color = gray.Where(null, null, null, num);
		Gizmos.DrawLine(localPosition, localPosition2);
		Gizmos.color = Color.gray;
		Vector2 vector = localPosition2 - localPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			Vector3 from = localPosition;
			Vector3 original = localPosition2;
			num = new float?(localPosition.y);
			Gizmos.DrawLine(from, original.Where(null, num, null));
			Vector3 from2 = localPosition2;
			Vector3 original2 = localPosition;
			num = new float?(localPosition2.y);
			Gizmos.DrawLine(from2, original2.Where(null, num, null));
			return;
		}
		Vector3 from3 = localPosition;
		Vector3 original3 = localPosition2;
		float? x = new float?(localPosition.x);
		num = null;
		float? y = num;
		num = null;
		Gizmos.DrawLine(from3, original3.Where(x, y, num));
		Vector3 from4 = localPosition2;
		Vector3 original4 = localPosition;
		float? x2 = new float?(localPosition2.x);
		num = null;
		float? y2 = num;
		num = null;
		Gizmos.DrawLine(from4, original4.Where(x2, y2, num));
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002191C File Offset: 0x0001FB1C
	private void Awake()
	{
		if (this.pickupRegion)
		{
			this.pickupRegion.OnTriggerEntered += delegate(Collider2D col, GameObject _)
			{
				this.OnPickupRegionEntered(col.gameObject, false);
			};
			this.pickupRegion.OnTriggerExited += delegate(Collider2D col, GameObject _)
			{
				this.OnPickupRegionExited(col.gameObject, false);
			};
		}
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0002195C File Offset: 0x0001FB5C
	private void Start()
	{
		Vector2 vector = this.antLeftPoint.position;
		Vector2 vector2 = this.antRightPoint.position;
		RaycastHit2D raycastHit2D;
		if (this.IsGroundType && global::Helper.IsRayHittingNoTriggers(Vector2.Lerp(vector, vector2, 0.5f) + new Vector2(0f, 1f), Vector2.down, 2f, 256, out raycastHit2D))
		{
			vector.y = (vector2.y = raycastHit2D.point.y);
			base.transform.SetPositionY(raycastHit2D.point.y);
			this.pickupAntParticleTemplate.transform.SetPositionY(raycastHit2D.point.y);
		}
		float num = Vector2.Distance(vector, vector2);
		ParticleSystem.MainModule main = this.antParticleTemplate.main;
		ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
		float constant = startLifetime.constant;
		startLifetime.constant = num / Mathf.Abs(this.antParticleTemplate.velocityOverLifetime.x.constantMax);
		main.startLifetime = startLifetime;
		ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule;
		this.antParticleTemplate.textureSheetAnimation.cycleCount = textureSheetAnimationModule.cycleCount * Mathf.RoundToInt(startLifetime.constant / constant);
		Transform parent = this.antParticleTemplate.transform.parent;
		this.leftParticle = Object.Instantiate<ParticleSystem>(this.antParticleTemplate, parent);
		this.rightParticle = Object.Instantiate<ParticleSystem>(this.antParticleTemplate, parent);
		this.antParticleTemplate.gameObject.SetActive(false);
		Transform transform = this.leftParticle.transform;
		transform.SetPosition2D(vector);
		AntRegion.ParticleFlipMode particleFlipMode = this.antParticleFlipMode;
		if (particleFlipMode != AntRegion.ParticleFlipMode.Scale)
		{
			if (particleFlipMode != AntRegion.ParticleFlipMode.Rotation)
			{
				throw new ArgumentOutOfRangeException();
			}
			transform.Rotate(new Vector3(0f, 0f, 180f), Space.Self);
		}
		else
		{
			transform.FlipLocalScale(true, false, false);
		}
		this.rightParticle.transform.SetPosition2D(vector2);
		if (!this.IsGroundType)
		{
			return;
		}
		BoxCollider2D component = this.pickupRegion.GetComponent<BoxCollider2D>();
		component.size += new Vector2(0f, 1f);
		component.transform.Translate(new Vector3(0f, -0.5f, 0f), Space.Self);
		while (this.pickupAnts.Count < 10)
		{
			ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.pickupAntParticleTemplate, this.pickupAntParticleTemplate.transform.parent);
			particleSystem.gameObject.SetActive(false);
			this.pickupAnts.Add(particleSystem);
		}
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00021BF2 File Offset: 0x0001FDF2
	private void OnDisable()
	{
		this.ReleaseAllTrackedObjects();
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00021BFC File Offset: 0x0001FDFC
	private void OnPickupRegionEntered(GameObject obj, bool breakRecursion = false)
	{
		if (this.hasStoppedEmitting)
		{
			return;
		}
		int i = obj.layer;
		if (i != 18 && i != 26 && !obj.GetComponent<AntRegionHandler>())
		{
			return;
		}
		using (List<AntRegion.CarryTracker>.Enumerator enumerator = this.carryTrackers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GameObject == obj)
				{
					return;
				}
			}
		}
		AntRegion.ICheck component = obj.GetComponent<AntRegion.ICheck>();
		Renderer rend = null;
		if (component != null)
		{
			if (!component.CanEnterAntRegion)
			{
				return;
			}
			component.TryGetRenderer(out rend);
		}
		ParticleSystem particleSystem = null;
		foreach (ParticleSystem particleSystem2 in this.pickupAnts)
		{
			if (!particleSystem2.gameObject.activeSelf)
			{
				particleSystem = particleSystem2;
				particleSystem.gameObject.SetActive(true);
				break;
			}
		}
		if (particleSystem == null)
		{
			particleSystem = Object.Instantiate<ParticleSystem>(this.pickupAntParticleTemplate, this.pickupAntParticleTemplate.transform.parent);
			particleSystem.gameObject.SetActive(true);
			this.pickupAnts.Add(particleSystem);
		}
		Rigidbody2D component2 = obj.GetComponent<Rigidbody2D>();
		bool flag = component2 != null;
		Transform transform = obj.transform;
		this.carryTrackers.Add(new AntRegion.CarryTracker
		{
			GameObject = obj,
			Body = component2,
			InitialBodyType = (flag ? component2.bodyType : RigidbodyType2D.Dynamic),
			InitialAngle = transform.localEulerAngles.z,
			Routine = base.StartCoroutine(this.PickupGameObject(obj, component2, particleSystem, rend)),
			CarryAnts = particleSystem
		});
		if (breakRecursion)
		{
			return;
		}
		AntRegion.INotify[] components = obj.GetComponents<AntRegion.INotify>();
		for (i = 0; i < components.Length; i++)
		{
			components[i].EnteredAntRegion(this);
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x00021DF0 File Offset: 0x0001FFF0
	private void OnPickupRegionExited(GameObject obj, bool breakRecursion = false)
	{
		for (int i = this.carryTrackers.Count - 1; i >= 0; i--)
		{
			AntRegion.CarryTracker carryTracker = this.carryTrackers[i];
			if (!(carryTracker.GameObject != obj))
			{
				base.StopCoroutine(carryTracker.Routine);
				this.carryTrackers.RemoveAt(i);
				this.DropGameObject(carryTracker);
			}
		}
		if (breakRecursion)
		{
			return;
		}
		AntRegion.INotify[] components = obj.GetComponents<AntRegion.INotify>();
		for (int j = 0; j < components.Length; j++)
		{
			components[j].ExitedAntRegion(this);
		}
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x00021E71 File Offset: 0x00020071
	public void ResetTracker(GameObject obj)
	{
		this.OnPickupRegionExited(obj, true);
		this.OnPickupRegionEntered(obj, true);
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x00021E84 File Offset: 0x00020084
	private void ReleaseAllTrackedObjects()
	{
		foreach (AntRegion.CarryTracker carryTracker in this.carryTrackers.ToArray())
		{
			this.OnPickupRegionExited(carryTracker.GameObject, false);
		}
		this.carryTrackers.Clear();
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00021ECB File Offset: 0x000200CB
	private IEnumerator PickupGameObject(GameObject obj, Rigidbody2D body, ParticleSystem carryAnts, Renderer rend)
	{
		WaitFrameAndPaused frameWait = new WaitFrameAndPaused();
		yield return new WaitForSeconds(this.pickupWaitTime.GetRandomValue());
		bool flag = body != null;
		if (flag)
		{
			body.bodyType = RigidbodyType2D.Kinematic;
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0f;
		}
		AntRegionHandler component = obj.GetComponent<AntRegionHandler>();
		if (component)
		{
			component.SetPickedUp(true);
		}
		AntRegion.IPickUpNotify[] pickNoNotifier = obj.GetComponents<AntRegion.IPickUpNotify>();
		AntRegion.IPickUpNotify[] array = pickNoNotifier;
		int i;
		for (i = 0; i < array.Length; i++)
		{
			array[i].PickUpStarted(this);
		}
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			yield break;
		}
		Transform root = obj.transform;
		if (flag && root.IsChildOf(body.transform))
		{
			root = body.transform;
		}
		Vector2 startPos = root.position;
		bool flag2 = rend != null;
		if (!flag2)
		{
			rend = obj.GetComponent<Renderer>();
			flag2 = (rend != null);
		}
		Bounds bounds = flag2 ? rend.bounds : new Bounds(root.position, new Vector3(0.5f, 0.5f, 1f));
		Vector3 size = bounds.size;
		Vector3 vector = size * 0.5f;
		Vector3 center = bounds.center;
		Vector2 boundsOffset = center - startPos;
		switch (this.carryDirection)
		{
		case AntRegion.CarryDirection.AwayFromHero:
			i = ((startPos.x < instance.transform.position.x) ? 1 : -1);
			break;
		case AntRegion.CarryDirection.Left:
			i = 1;
			break;
		case AntRegion.CarryDirection.Right:
			i = -1;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		float num = (float)i;
		float targetX = (num > 0f) ? (this.antLeftPoint.position.x + vector.x) : (this.antRightPoint.position.x - vector.x);
		float num2 = Mathf.Abs(targetX - startPos.x);
		float moveSpeed = this.pickupMoveSpeed.GetRandomValue();
		float moveDuration = num2 / moveSpeed;
		float height = this.pickupHeight.GetRandomValue();
		float raiseDuration = this.pickupDuration.GetRandomValue();
		float moveWaitTime = this.pickupMoveWaitTime.GetRandomValue();
		float dropWaitTime = this.sinkWaitTime.GetRandomValue();
		float num3 = raiseDuration + moveWaitTime + moveDuration + dropWaitTime + 0.5f;
		ParticleSystem.MainModule main = carryAnts.main;
		main.startLifetime = new ParticleSystem.MinMaxCurve(num3);
		main.duration = num3;
		float num4 = size.x * this.pickupAntsWidth;
		int num5 = Mathf.RoundToInt(this.pickupAntsPerUnit.GetRandomValue() * num4);
		carryAnts.emission.SetBurst(0, new ParticleSystem.Burst(0f, (float)num5));
		ParticleSystem.ShapeModule shape = carryAnts.shape;
		if (num5 > 1)
		{
			shape.radius = num4 * 0.5f;
			shape.randomPositionAmount = this.pickupAntParticleTemplate.shape.randomPositionAmount;
		}
		else
		{
			shape.radius = 0f;
			shape.randomPositionAmount = 0f;
		}
		ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = carryAnts.textureSheetAnimation;
		float constant = this.pickupAntParticleTemplate.main.startLifetime.constant;
		textureSheetAnimation.cycleCount = Mathf.RoundToInt((float)this.pickupAntParticleTemplate.textureSheetAnimation.cycleCount * (num3 / constant) * moveSpeed);
		carryAnts.Play(true);
		Transform carryAntsTrans = carryAnts.transform;
		carryAntsTrans.SetPosition2D(new Vector2(startPos.x + boundsOffset.x, this.pickupAntParticleTemplate.transform.position.y));
		float scaleX = carryAntsTrans.GetScaleX();
		carryAntsTrans.SetScaleX(Mathf.Abs(scaleX) * num);
		Vector2 raisePos = startPos + new Vector2(0f, height);
		float elapsed;
		for (elapsed = 0f; elapsed < raiseDuration; elapsed += Time.deltaTime)
		{
			float t = this.pickupCurve.Evaluate(elapsed / raiseDuration);
			Vector2 position = Vector2.Lerp(startPos, raisePos, t);
			root.SetPosition2D(position);
			yield return frameWait;
		}
		root.SetPosition2D(raisePos);
		raisePos = default(Vector2);
		yield return new WaitForSeconds(moveWaitTime);
		float wobbleMult = 1f / size.x;
		if (obj.GetComponent<Corpse>() || obj.GetComponent<ActiveCorpse>())
		{
			wobbleMult *= 0.15f;
		}
		raisePos = root.position;
		elapsed = root.localEulerAngles.z;
		float elapsed2;
		for (elapsed2 = 0f; elapsed2 < moveDuration; elapsed2 += Time.deltaTime)
		{
			float num6 = Mathf.Lerp(raisePos.x, targetX, elapsed2 / moveDuration);
			float time = elapsed2 * moveSpeed;
			float y = raisePos.y + this.moveBobCurve.Evaluate(time);
			float angle = this.moveWobbleCurve.Evaluate(time) * wobbleMult;
			Vector2 vector2 = new Vector2(num6, y);
			root.SetPosition2D(vector2);
			root.SetLocalRotation2D(elapsed);
			root.RotateAround(vector2, Vector3.forward, angle);
			carryAntsTrans.SetPositionX(num6 + boundsOffset.x);
			yield return frameWait;
		}
		raisePos = default(Vector2);
		yield return new WaitForSeconds(dropWaitTime);
		float num7 = size.y + height;
		elapsed2 = num7 / this.sinkSpeed.GetRandomValue();
		elapsed = root.position.y;
		float dropY = elapsed - num7;
		for (float elapsed3 = 0f; elapsed3 < elapsed2; elapsed3 += Time.deltaTime)
		{
			float t2 = this.sinkCurve.Evaluate(elapsed3 / elapsed2);
			float newY = Mathf.Lerp(elapsed, dropY, t2);
			root.SetPositionY(newY);
			yield return frameWait;
		}
		root.SetPositionY(dropY);
		array = pickNoNotifier;
		for (i = 0; i < array.Length; i++)
		{
			array[i].PickUpEnd(this);
		}
		if (ObjectPool.IsSpawned(obj))
		{
			obj.Recycle();
		}
		else
		{
			obj.SetActive(false);
		}
		yield break;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00021EF8 File Offset: 0x000200F8
	private void DropGameObject(AntRegion.CarryTracker tracker)
	{
		tracker.GameObject.transform.SetLocalRotation2D(tracker.InitialAngle);
		if (tracker.Body)
		{
			tracker.Body.bodyType = tracker.InitialBodyType;
		}
		tracker.CarryAnts.gameObject.SetActive(false);
		AntRegionHandler component = tracker.GameObject.GetComponent<AntRegionHandler>();
		if (component)
		{
			component.SetPickedUp(false);
		}
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00021F65 File Offset: 0x00020165
	public void StopEmitting()
	{
		this.hasStoppedEmitting = true;
		this.leftParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		this.rightParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	// Token: 0x04000675 RID: 1653
	[SerializeField]
	private ParticleSystem antParticleTemplate;

	// Token: 0x04000676 RID: 1654
	[SerializeField]
	private AntRegion.ParticleFlipMode antParticleFlipMode;

	// Token: 0x04000677 RID: 1655
	[SerializeField]
	private ParticleSystem pickupAntParticleTemplate;

	// Token: 0x04000678 RID: 1656
	[SerializeField]
	private MinMaxFloat pickupAntsPerUnit;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	private float pickupAntsWidth;

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	private Transform antLeftPoint;

	// Token: 0x0400067B RID: 1659
	[SerializeField]
	private Transform antRightPoint;

	// Token: 0x0400067C RID: 1660
	[SerializeField]
	private TriggerEnterEvent pickupRegion;

	// Token: 0x0400067D RID: 1661
	[SerializeField]
	private MinMaxFloat pickupWaitTime;

	// Token: 0x0400067E RID: 1662
	[SerializeField]
	private MinMaxFloat pickupHeight;

	// Token: 0x0400067F RID: 1663
	[SerializeField]
	private MinMaxFloat pickupDuration;

	// Token: 0x04000680 RID: 1664
	[SerializeField]
	private AnimationCurve pickupCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000681 RID: 1665
	[SerializeField]
	private MinMaxFloat pickupMoveWaitTime;

	// Token: 0x04000682 RID: 1666
	[SerializeField]
	private MinMaxFloat pickupMoveSpeed;

	// Token: 0x04000683 RID: 1667
	[SerializeField]
	private AntRegion.CarryDirection carryDirection;

	// Token: 0x04000684 RID: 1668
	[SerializeField]
	private AnimationCurve moveWobbleCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04000685 RID: 1669
	[SerializeField]
	private AnimationCurve moveBobCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04000686 RID: 1670
	[SerializeField]
	private MinMaxFloat sinkWaitTime;

	// Token: 0x04000687 RID: 1671
	[SerializeField]
	private MinMaxFloat sinkSpeed;

	// Token: 0x04000688 RID: 1672
	[SerializeField]
	private AnimationCurve sinkCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000689 RID: 1673
	private bool hasStoppedEmitting;

	// Token: 0x0400068A RID: 1674
	private ParticleSystem leftParticle;

	// Token: 0x0400068B RID: 1675
	private ParticleSystem rightParticle;

	// Token: 0x0400068C RID: 1676
	private readonly List<AntRegion.CarryTracker> carryTrackers = new List<AntRegion.CarryTracker>();

	// Token: 0x0400068D RID: 1677
	private readonly List<ParticleSystem> pickupAnts = new List<ParticleSystem>();

	// Token: 0x0200143B RID: 5179
	public interface ICheck
	{
		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x060082FA RID: 33530
		bool CanEnterAntRegion { get; }

		// Token: 0x060082FB RID: 33531 RVA: 0x00267540 File Offset: 0x00265740
		bool TryGetRenderer(out Renderer renderer)
		{
			renderer = null;
			return false;
		}
	}

	// Token: 0x0200143C RID: 5180
	public interface INotify
	{
		// Token: 0x060082FC RID: 33532
		void EnteredAntRegion(AntRegion antRegion);

		// Token: 0x060082FD RID: 33533
		void ExitedAntRegion(AntRegion antRegion);
	}

	// Token: 0x0200143D RID: 5181
	public interface IPickUpNotify
	{
		// Token: 0x060082FE RID: 33534
		void PickUpStarted(AntRegion antRegion);

		// Token: 0x060082FF RID: 33535
		void PickUpEnd(AntRegion antRegion);
	}

	// Token: 0x0200143E RID: 5182
	private struct CarryTracker
	{
		// Token: 0x04008274 RID: 33396
		public GameObject GameObject;

		// Token: 0x04008275 RID: 33397
		public Rigidbody2D Body;

		// Token: 0x04008276 RID: 33398
		public RigidbodyType2D InitialBodyType;

		// Token: 0x04008277 RID: 33399
		public float InitialAngle;

		// Token: 0x04008278 RID: 33400
		public Coroutine Routine;

		// Token: 0x04008279 RID: 33401
		public ParticleSystem CarryAnts;
	}

	// Token: 0x0200143F RID: 5183
	private enum ParticleFlipMode
	{
		// Token: 0x0400827B RID: 33403
		Scale,
		// Token: 0x0400827C RID: 33404
		Rotation
	}

	// Token: 0x02001440 RID: 5184
	private enum CarryDirection
	{
		// Token: 0x0400827E RID: 33406
		AwayFromHero,
		// Token: 0x0400827F RID: 33407
		Left,
		// Token: 0x04008280 RID: 33408
		Right
	}
}
