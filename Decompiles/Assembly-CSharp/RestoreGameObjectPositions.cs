using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000029 RID: 41
[ActionCategory("Hollow Knight")]
public class RestoreGameObjectPositions : FsmStateAction
{
	// Token: 0x0600015B RID: 347 RVA: 0x00007FAE File Offset: 0x000061AE
	public override void Reset()
	{
		base.Reset();
		this.positions = null;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00007FC0 File Offset: 0x000061C0
	public override void OnEnter()
	{
		base.OnEnter();
		if (this.positions == null)
		{
			this.positions = new Dictionary<GameObject, Vector3>(base.Owner.transform.childCount);
			using (IEnumerator enumerator = base.Owner.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					this.positions.Add(transform.gameObject, transform.localPosition);
				}
				goto IL_C2;
			}
		}
		foreach (KeyValuePair<GameObject, Vector3> keyValuePair in this.positions)
		{
			keyValuePair.Key.transform.localPosition = keyValuePair.Value;
		}
		IL_C2:
		base.Finish();
	}

	// Token: 0x04000101 RID: 257
	private Dictionary<GameObject, Vector3> positions;
}
