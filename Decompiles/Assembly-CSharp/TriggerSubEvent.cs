using System;
using UnityEngine;

// Token: 0x02000602 RID: 1538
public abstract class TriggerSubEvent : MonoBehaviour
{
	// Token: 0x0200190C RID: 6412
	// (Invoke) Token: 0x060092FC RID: 37628
	public delegate void CollisionEvent(Collider2D collider);
}
