using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x0200081A RID: 2074
	[Serializable]
	public class TMP_Style
	{
		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06004935 RID: 18741 RVA: 0x001564A2 File Offset: 0x001546A2
		// (set) Token: 0x06004936 RID: 18742 RVA: 0x001564AA File Offset: 0x001546AA
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (value != this.m_Name)
				{
					this.m_Name = value;
				}
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06004937 RID: 18743 RVA: 0x001564C1 File Offset: 0x001546C1
		// (set) Token: 0x06004938 RID: 18744 RVA: 0x001564C9 File Offset: 0x001546C9
		public int hashCode
		{
			get
			{
				return this.m_HashCode;
			}
			set
			{
				if (value != this.m_HashCode)
				{
					this.m_HashCode = value;
				}
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06004939 RID: 18745 RVA: 0x001564DB File Offset: 0x001546DB
		public string styleOpeningDefinition
		{
			get
			{
				return this.m_OpeningDefinition;
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x0600493A RID: 18746 RVA: 0x001564E3 File Offset: 0x001546E3
		public string styleClosingDefinition
		{
			get
			{
				return this.m_ClosingDefinition;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x0600493B RID: 18747 RVA: 0x001564EB File Offset: 0x001546EB
		public int[] styleOpeningTagArray
		{
			get
			{
				return this.m_OpeningTagArray;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x0600493C RID: 18748 RVA: 0x001564F3 File Offset: 0x001546F3
		public int[] styleClosingTagArray
		{
			get
			{
				return this.m_ClosingTagArray;
			}
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x001564FC File Offset: 0x001546FC
		public void RefreshStyle()
		{
			this.m_HashCode = TMP_TextUtilities.GetSimpleHashCode(this.m_Name);
			this.m_OpeningTagArray = new int[this.m_OpeningDefinition.Length];
			for (int i = 0; i < this.m_OpeningDefinition.Length; i++)
			{
				this.m_OpeningTagArray[i] = (int)this.m_OpeningDefinition[i];
			}
			this.m_ClosingTagArray = new int[this.m_ClosingDefinition.Length];
			for (int j = 0; j < this.m_ClosingDefinition.Length; j++)
			{
				this.m_ClosingTagArray[j] = (int)this.m_ClosingDefinition[j];
			}
		}

		// Token: 0x04004937 RID: 18743
		[SerializeField]
		private string m_Name;

		// Token: 0x04004938 RID: 18744
		[SerializeField]
		private int m_HashCode;

		// Token: 0x04004939 RID: 18745
		[SerializeField]
		private string m_OpeningDefinition;

		// Token: 0x0400493A RID: 18746
		[SerializeField]
		private string m_ClosingDefinition;

		// Token: 0x0400493B RID: 18747
		[SerializeField]
		private int[] m_OpeningTagArray;

		// Token: 0x0400493C RID: 18748
		[SerializeField]
		private int[] m_ClosingTagArray;
	}
}
