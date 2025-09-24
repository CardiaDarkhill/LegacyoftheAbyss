using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
public class SetZ : MonoBehaviour
{
	// Token: 0x060034A0 RID: 13472 RVA: 0x000E9C23 File Offset: 0x000E7E23
	private void OnEnable()
	{
		if (this.started)
		{
			this.StartSetZ();
		}
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x000E9C33 File Offset: 0x000E7E33
	private void Start()
	{
		this.StartSetZ();
		this.started = true;
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x000E9C42 File Offset: 0x000E7E42
	private void OnDisable()
	{
		if (this.delayRoutine != null)
		{
			base.StopCoroutine(this.delayRoutine);
			this.delayRoutine = null;
		}
	}

	// Token: 0x060034A3 RID: 13475 RVA: 0x000E9C5F File Offset: 0x000E7E5F
	private void StartSetZ()
	{
		if (this.delayBeforeRandomizing <= 0f && this.waitFrames <= 0)
		{
			this.DoSetZ();
			return;
		}
		if (this.delayRoutine != null)
		{
			return;
		}
		this.delayRoutine = base.StartCoroutine(this.SetPosition());
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x000E9C99 File Offset: 0x000E7E99
	private IEnumerator SetPosition()
	{
		if (this.delayBeforeRandomizing > 0f)
		{
			if (this.waitForSeconds == null)
			{
				this.waitForSeconds = new WaitForSeconds(this.delayBeforeRandomizing);
			}
			yield return this.waitForSeconds;
		}
		if (this.waitFrames > 0)
		{
			int num;
			for (int i = 0; i < this.waitFrames; i = num + 1)
			{
				yield return null;
				num = i;
			}
		}
		if (!this.cancel)
		{
			this.DoSetZ();
		}
		this.delayRoutine = null;
		yield break;
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x000E9CA8 File Offset: 0x000E7EA8
	private void DoSetZ()
	{
		this.setZ = this.z;
		Vector3 position = base.transform.position;
		if (this.randomizeFromStartingValue)
		{
			this.setZ = Random.Range(position.z, position.z + 0.0009999f);
		}
		else if (!this.dontRandomize)
		{
			this.setZ = Random.Range(this.z, this.z + 0.0009999f);
		}
		if (this.deParent && base.transform.parent)
		{
			base.transform.SetParent(null, true);
		}
		base.transform.SetPositionZ(this.setZ);
	}

	// Token: 0x060034A6 RID: 13478 RVA: 0x000E9D51 File Offset: 0x000E7F51
	public void CancelSetZ()
	{
		this.cancel = true;
	}

	// Token: 0x0400380F RID: 14351
	[SerializeField]
	[ModifiableProperty]
	[Conditional("randomizeFromStartingValue", false, false, false)]
	private float z;

	// Token: 0x04003810 RID: 14352
	[SerializeField]
	[ModifiableProperty]
	[Conditional("randomizeFromStartingValue", false, false, false)]
	private bool dontRandomize;

	// Token: 0x04003811 RID: 14353
	[SerializeField]
	private bool randomizeFromStartingValue;

	// Token: 0x04003812 RID: 14354
	[SerializeField]
	private float delayBeforeRandomizing = 0.5f;

	// Token: 0x04003813 RID: 14355
	[SerializeField]
	private int waitFrames;

	// Token: 0x04003814 RID: 14356
	[SerializeField]
	private bool deParent;

	// Token: 0x04003815 RID: 14357
	private bool cancel;

	// Token: 0x04003816 RID: 14358
	private float setZ;

	// Token: 0x04003817 RID: 14359
	private Coroutine delayRoutine;

	// Token: 0x04003818 RID: 14360
	private WaitForSeconds waitForSeconds;

	// Token: 0x04003819 RID: 14361
	private bool started;
}
