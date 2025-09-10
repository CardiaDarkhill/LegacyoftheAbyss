using System;
using UnityEngine;

// Token: 0x020004E2 RID: 1250
public class FloorBells : MonoBehaviour
{
	// Token: 0x06002CD9 RID: 11481 RVA: 0x000C437F File Offset: 0x000C257F
	private void Awake()
	{
		this.tracker.InsideStateChanged += delegate(bool value)
		{
			if (value)
			{
				this.OnEntered();
				return;
			}
			this.OnExited();
		};
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000C4398 File Offset: 0x000C2598
	private void OnEntered()
	{
		this.animator.Play();
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000C43A5 File Offset: 0x000C25A5
	private void OnExited()
	{
		this.animator.QueueStop();
	}

	// Token: 0x04002E89 RID: 11913
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TrackTriggerObjects tracker;

	// Token: 0x04002E8A RID: 11914
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BasicSpriteAnimator animator;
}
