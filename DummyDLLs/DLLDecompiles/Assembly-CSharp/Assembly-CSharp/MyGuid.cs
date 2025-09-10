using System;
using UnityEngine;

// Token: 0x020007C3 RID: 1987
[ExecuteInEditMode]
public class MyGuid : MonoBehaviour
{
	// Token: 0x170007E4 RID: 2020
	// (get) Token: 0x060045FE RID: 17918 RVA: 0x001306EB File Offset: 0x0012E8EB
	public Guid guid
	{
		get
		{
			if (this._guid == Guid.Empty && !string.IsNullOrEmpty(this.guidAsString))
			{
				this._guid = new Guid(this.guidAsString);
			}
			return this._guid;
		}
	}

	// Token: 0x060045FF RID: 17919 RVA: 0x00130724 File Offset: 0x0012E924
	public void Generate()
	{
		this._guid = Guid.NewGuid();
		this.guidAsString = this.guid.ToString();
	}

	// Token: 0x06004600 RID: 17920 RVA: 0x00130756 File Offset: 0x0012E956
	public string GetGuid()
	{
		return this.guidAsString;
	}

	// Token: 0x04004697 RID: 18071
	[SerializeField]
	private string guidAsString;

	// Token: 0x04004698 RID: 18072
	private Guid _guid;
}
