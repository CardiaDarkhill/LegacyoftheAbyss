using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000027 RID: 39
[ActionCategory("Hollow Knight")]
public class GetNextPreSpawnedGameObject : FsmStateAction
{
	// Token: 0x06000153 RID: 339 RVA: 0x00007DDD File Offset: 0x00005FDD
	public override void Reset()
	{
		this.storedArray = null;
		this.spawnPosition = null;
		this.storeObject = null;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00007DF4 File Offset: 0x00005FF4
	public override void OnEnter()
	{
		if (!this.currentIndex.IsNone && this.currentIndex.Value < this.storedArray.Length)
		{
			GameObject gameObject = (GameObject)this.storedArray.Values[this.currentIndex.Value];
			if (!this.spawnPosition.IsNone)
			{
				gameObject.transform.position = this.spawnPosition.Value;
			}
			gameObject.SetActive(true);
			this.storeObject.Value = gameObject;
			FsmInt fsmInt = this.currentIndex;
			int value = fsmInt.Value;
			fsmInt.Value = value + 1;
		}
		base.Finish();
	}

	// Token: 0x040000F7 RID: 247
	[UIHint(UIHint.Variable)]
	[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
	public FsmArray storedArray;

	// Token: 0x040000F8 RID: 248
	public FsmVector3 spawnPosition;

	// Token: 0x040000F9 RID: 249
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeObject;

	// Token: 0x040000FA RID: 250
	[UIHint(UIHint.Variable)]
	public FsmInt currentIndex;
}
