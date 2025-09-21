using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200053C RID: 1340
[DisallowMultipleComponent]
public class ResetDynamicHierarchy : MonoBehaviour, AntRegion.INotify
{
	// Token: 0x06003016 RID: 12310 RVA: 0x000D4278 File Offset: 0x000D2478
	private void Awake()
	{
		ResetDynamicHierarchy.CaptureStatesRecursive(base.transform, this.initialStates);
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000D428B File Offset: 0x000D248B
	private void OnDestroy()
	{
		ResetDynamicHierarchy.activeResets.Remove(this);
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x000D429C File Offset: 0x000D249C
	public static void ForceReconnectAll()
	{
		ResetDynamicHierarchy.activeResets.ReserveListUsage();
		foreach (ResetDynamicHierarchy resetDynamicHierarchy in ResetDynamicHierarchy.activeResets.List)
		{
			resetDynamicHierarchy.ReconnectAll();
			resetDynamicHierarchy.DoReset(true);
		}
		ResetDynamicHierarchy.activeResets.ReleaseListUsage();
		ResetDynamicHierarchy.activeResets.Clear();
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000D4318 File Offset: 0x000D2518
	private void ReconnectAll()
	{
		this.disconnectedTransforms.ReserveListUsage();
		foreach (Transform transform in this.disconnectedTransforms.List)
		{
			if (!(transform == null))
			{
				this.Reconnect(transform, false, true);
			}
		}
		this.disconnectedTransforms.ReleaseListUsage();
		this.disconnectedTransforms.Clear();
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x000D439C File Offset: 0x000D259C
	public void DoReset(bool alsoRoot = false)
	{
		foreach (ResetDynamicHierarchy.State state in this.initialStates)
		{
			state.Apply(alsoRoot);
		}
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x000D43F0 File Offset: 0x000D25F0
	private void CreateLookup()
	{
		if (this.lookup.Count == 0 && this.initialStates.Count > 0)
		{
			foreach (ResetDynamicHierarchy.State state in this.initialStates)
			{
				this.lookup[state.Self] = state;
			}
		}
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x000D446C File Offset: 0x000D266C
	public void Disconnect(Transform target, bool recursive)
	{
		this.CreateLookup();
		ResetDynamicHierarchy.State state;
		if (!this.lookup.TryGetValue(target, out state))
		{
			return;
		}
		state.Disconnected = true;
		this.disconnectedTransforms.Add(target);
		ResetDynamicHierarchy.activeResets.Add(this);
		if (recursive)
		{
			foreach (object obj in target)
			{
				Transform target2 = (Transform)obj;
				this.Disconnect(target2, true);
			}
		}
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x000D44FC File Offset: 0x000D26FC
	public void Reconnect(Transform target, bool applyRoot, bool recursive)
	{
		ResetDynamicHierarchy.State state;
		if (!this.lookup.TryGetValue(target, out state))
		{
			return;
		}
		if (!state.Disconnected)
		{
			return;
		}
		state.Disconnected = false;
		this.disconnectedTransforms.Remove(target);
		if (this.disconnectedTransforms.Count == 0)
		{
			ResetDynamicHierarchy.activeResets.Remove(this);
		}
		if (!base.gameObject.activeInHierarchy)
		{
			state.Apply(applyRoot);
		}
		if (recursive)
		{
			foreach (object obj in target)
			{
				Transform target2 = (Transform)obj;
				this.Reconnect(target2, applyRoot, true);
			}
		}
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x000D45B4 File Offset: 0x000D27B4
	private static void CaptureStatesRecursive(Transform target, List<ResetDynamicHierarchy.State> addState)
	{
		Rigidbody2D component = target.GetComponent<Rigidbody2D>();
		ResetDynamicHierarchy.State state = new ResetDynamicHierarchy.State();
		state.Self = target;
		state.ActiveSelf = target.gameObject.activeSelf;
		state.Body = component;
		state.WasBodyKinematic = (component ? component.isKinematic : component);
		state.Parent = target.parent;
		state.Position = target.localPosition;
		state.Rotation = target.localRotation;
		state.Scale = target.localScale;
		state.BehaviourEnabledStates = (from b in target.GetComponents<MonoBehaviour>()
		select new ResetDynamicHierarchy.BehaviourActivation
		{
			Behaviour = b,
			Enabled = b.enabled
		}).ToArray<ResetDynamicHierarchy.BehaviourActivation>();
		state.ColliderEnabledStates = (from c in target.GetComponents<Collider2D>()
		select new ResetDynamicHierarchy.ColliderActivation
		{
			Collider = c,
			Enabled = c.enabled
		}).ToArray<ResetDynamicHierarchy.ColliderActivation>();
		addState.Add(state);
		foreach (object obj in target)
		{
			ResetDynamicHierarchy.CaptureStatesRecursive((Transform)obj, addState);
		}
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x000D46F0 File Offset: 0x000D28F0
	public void EnteredAntRegion(AntRegion antRegion)
	{
		if (!this.checkedInitialStates)
		{
			foreach (ResetDynamicHierarchy.State state in this.initialStates)
			{
				if (state.Self.GetComponent<FlingChildrenOnStart>())
				{
					state.Self.gameObject.AddComponentIfNotPresent<AntRegionFlingChildrenNotifier>();
				}
			}
			this.checkedInitialStates = true;
		}
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x000D4770 File Offset: 0x000D2970
	public void ExitedAntRegion(AntRegion antRegion)
	{
	}

	// Token: 0x040032F4 RID: 13044
	private readonly List<ResetDynamicHierarchy.State> initialStates = new List<ResetDynamicHierarchy.State>();

	// Token: 0x040032F5 RID: 13045
	private readonly Dictionary<Transform, ResetDynamicHierarchy.State> lookup = new Dictionary<Transform, ResetDynamicHierarchy.State>();

	// Token: 0x040032F6 RID: 13046
	private readonly UniqueList<Transform> disconnectedTransforms = new UniqueList<Transform>();

	// Token: 0x040032F7 RID: 13047
	private static UniqueList<ResetDynamicHierarchy> activeResets = new UniqueList<ResetDynamicHierarchy>();

	// Token: 0x040032F8 RID: 13048
	private bool checkedInitialStates;

	// Token: 0x02001844 RID: 6212
	private struct BehaviourActivation
	{
		// Token: 0x04009164 RID: 37220
		public MonoBehaviour Behaviour;

		// Token: 0x04009165 RID: 37221
		public bool Enabled;
	}

	// Token: 0x02001845 RID: 6213
	private struct ColliderActivation
	{
		// Token: 0x04009166 RID: 37222
		public Collider2D Collider;

		// Token: 0x04009167 RID: 37223
		public bool Enabled;
	}

	// Token: 0x02001846 RID: 6214
	private class State
	{
		// Token: 0x06009097 RID: 37015 RVA: 0x00295AF8 File Offset: 0x00293CF8
		public void Apply(bool alsoRoot)
		{
			if (this.Disconnected)
			{
				return;
			}
			this.Self.SetParent(this.Parent);
			if (alsoRoot || this.Parent)
			{
				if (this.Self.gameObject.activeSelf != this.ActiveSelf)
				{
					this.Self.gameObject.SetActive(this.ActiveSelf);
				}
				this.Self.localPosition = this.Position;
			}
			this.Self.localRotation = this.Rotation;
			this.Self.localScale = this.Scale;
			if (this.Body)
			{
				this.Body.linearVelocity = Vector2.zero;
				this.Body.angularVelocity = 0f;
				this.Body.isKinematic = this.WasBodyKinematic;
			}
			foreach (ResetDynamicHierarchy.BehaviourActivation behaviourActivation in this.BehaviourEnabledStates)
			{
				behaviourActivation.Behaviour.enabled = behaviourActivation.Enabled;
			}
			foreach (ResetDynamicHierarchy.ColliderActivation colliderActivation in this.ColliderEnabledStates)
			{
				colliderActivation.Collider.enabled = colliderActivation.Enabled;
			}
		}

		// Token: 0x04009168 RID: 37224
		public Transform Self;

		// Token: 0x04009169 RID: 37225
		public bool ActiveSelf;

		// Token: 0x0400916A RID: 37226
		public Transform Parent;

		// Token: 0x0400916B RID: 37227
		public Vector3 Position;

		// Token: 0x0400916C RID: 37228
		public Quaternion Rotation;

		// Token: 0x0400916D RID: 37229
		public Vector3 Scale;

		// Token: 0x0400916E RID: 37230
		public Rigidbody2D Body;

		// Token: 0x0400916F RID: 37231
		public bool WasBodyKinematic;

		// Token: 0x04009170 RID: 37232
		public ResetDynamicHierarchy.BehaviourActivation[] BehaviourEnabledStates;

		// Token: 0x04009171 RID: 37233
		public ResetDynamicHierarchy.ColliderActivation[] ColliderEnabledStates;

		// Token: 0x04009172 RID: 37234
		public bool Disconnected;
	}
}
