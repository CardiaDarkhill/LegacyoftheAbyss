using System;
using System.Collections.Generic;
using System.Linq;

namespace TMProOld
{
	// Token: 0x020007F5 RID: 2037
	[Serializable]
	public class KerningTable
	{
		// Token: 0x060047CF RID: 18383 RVA: 0x0014DCBB File Offset: 0x0014BEBB
		public KerningTable()
		{
			this.kerningPairs = new List<KerningPair>();
		}

		// Token: 0x060047D0 RID: 18384 RVA: 0x0014DCD0 File Offset: 0x0014BED0
		public void AddKerningPair()
		{
			if (this.kerningPairs.Count == 0)
			{
				this.kerningPairs.Add(new KerningPair(0, 0, 0f));
				return;
			}
			int ascII_Left = this.kerningPairs.Last<KerningPair>().AscII_Left;
			int ascII_Right = this.kerningPairs.Last<KerningPair>().AscII_Right;
			float xadvanceOffset = this.kerningPairs.Last<KerningPair>().XadvanceOffset;
			this.kerningPairs.Add(new KerningPair(ascII_Left, ascII_Right, xadvanceOffset));
		}

		// Token: 0x060047D1 RID: 18385 RVA: 0x0014DD48 File Offset: 0x0014BF48
		public int AddKerningPair(int left, int right, float offset)
		{
			if (this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right) == -1)
			{
				this.kerningPairs.Add(new KerningPair(left, right, offset));
				return 0;
			}
			return -1;
		}

		// Token: 0x060047D2 RID: 18386 RVA: 0x0014DDA4 File Offset: 0x0014BFA4
		public void RemoveKerningPair(int left, int right)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
			if (num != -1)
			{
				this.kerningPairs.RemoveAt(num);
			}
		}

		// Token: 0x060047D3 RID: 18387 RVA: 0x0014DDED File Offset: 0x0014BFED
		public void RemoveKerningPair(int index)
		{
			this.kerningPairs.RemoveAt(index);
		}

		// Token: 0x060047D4 RID: 18388 RVA: 0x0014DDFC File Offset: 0x0014BFFC
		public void SortKerningPairs()
		{
			if (this.kerningPairs.Count > 0)
			{
				this.kerningPairs = (from s in this.kerningPairs
				orderby s.AscII_Left, s.AscII_Right
				select s).ToList<KerningPair>();
			}
		}

		// Token: 0x040047C1 RID: 18369
		public List<KerningPair> kerningPairs;
	}
}
