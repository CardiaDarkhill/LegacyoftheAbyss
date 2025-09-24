using System;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class AntRegionHandler : MonoBehaviour
{
	// Token: 0x060006AE RID: 1710 RVA: 0x00022044 File Offset: 0x00020244
	[UsedImplicitly]
	private bool? IsPickedUpBoolValid(string boolName)
	{
		if (!this.pickedUpBoolFsm)
		{
			return null;
		}
		return new bool?(this.pickedUpBoolFsm.FsmVariables.FindFsmBool(boolName) != null);
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00022084 File Offset: 0x00020284
	public void SetPickedUp(bool value)
	{
		if (!this.pickedUpBoolFsm)
		{
			return;
		}
		FsmBool fsmBool = this.pickedUpBoolFsm.FsmVariables.FindFsmBool(this.pickedUpBoolName);
		if (fsmBool != null)
		{
			fsmBool.Value = value;
		}
	}

	// Token: 0x0400068E RID: 1678
	[SerializeField]
	private PlayMakerFSM pickedUpBoolFsm;

	// Token: 0x0400068F RID: 1679
	[SerializeField]
	[ModifiableProperty]
	[Conditional("pickedUpBoolFsm", true, false, false)]
	[InspectorValidation("IsPickedUpBoolValid")]
	private string pickedUpBoolName;
}
