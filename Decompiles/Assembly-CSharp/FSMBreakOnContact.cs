using System;
using UnityEngine;

// Token: 0x020003DC RID: 988
public class FSMBreakOnContact : MonoBehaviour, IBreakOnContact
{
	// Token: 0x060021D1 RID: 8657 RVA: 0x0009C069 File Offset: 0x0009A269
	private void OnValidate()
	{
		if (this.fsm == null)
		{
			this.fsm = base.GetComponent<PlayMakerFSM>();
		}
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x0009C085 File Offset: 0x0009A285
	public void Break()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent(this.breakEvent);
		}
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x0009C0A5 File Offset: 0x0009A2A5
	private bool? IsFsmEventValidRequired(string eventName)
	{
		return this.fsm.IsEventValid(eventName, true);
	}

	// Token: 0x04002091 RID: 8337
	[SerializeField]
	private PlayMakerFSM fsm;

	// Token: 0x04002092 RID: 8338
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string breakEvent = "BREAK";
}
