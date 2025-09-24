using System;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class DoorTargetCondition : MonoBehaviour
{
	// Token: 0x0600139C RID: 5020 RVA: 0x000595AB File Offset: 0x000577AB
	private void Reset()
	{
		this.door = base.GetComponent<TransitionPoint>();
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x000595B9 File Offset: 0x000577B9
	private void Start()
	{
		this.door.SetTargetScene(this.condition.IsFulfilled ? this.targetIfTrue : this.targetIfFalse);
	}

	// Token: 0x040011FD RID: 4605
	[SerializeField]
	[InspectorValidation]
	private TransitionPoint door;

	// Token: 0x040011FE RID: 4606
	[SerializeField]
	private PlayerDataTest condition;

	// Token: 0x040011FF RID: 4607
	[SerializeField]
	private string targetIfTrue;

	// Token: 0x04001200 RID: 4608
	[SerializeField]
	private string targetIfFalse;
}
