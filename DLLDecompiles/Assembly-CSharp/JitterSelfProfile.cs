using System;
using UnityEngine;

// Token: 0x020003F6 RID: 1014
[CreateAssetMenu(fileName = "New Jitter Profile", menuName = "Profiles/Jitter Config")]
public class JitterSelfProfile : ScriptableObject
{
	// Token: 0x17000397 RID: 919
	// (get) Token: 0x060022A1 RID: 8865 RVA: 0x0009F43E File Offset: 0x0009D63E
	public JitterSelfConfig Config
	{
		get
		{
			return this.config;
		}
	}

	// Token: 0x04002176 RID: 8566
	[SerializeField]
	private JitterSelfConfig config;
}
