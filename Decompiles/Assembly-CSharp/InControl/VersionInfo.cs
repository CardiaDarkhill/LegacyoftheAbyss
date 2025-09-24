using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000945 RID: 2373
	[Serializable]
	public struct VersionInfo : IComparable<VersionInfo>
	{
		// Token: 0x06005483 RID: 21635 RVA: 0x00180CCB File Offset: 0x0017EECB
		public VersionInfo(int major, int minor, int patch, int build)
		{
			this.major = major;
			this.minor = minor;
			this.patch = patch;
			this.build = build;
		}

		// Token: 0x06005484 RID: 21636 RVA: 0x00180CEC File Offset: 0x0017EEEC
		public static VersionInfo InControlVersion()
		{
			return new VersionInfo
			{
				major = 1,
				minor = 8,
				patch = 11,
				build = 9379
			};
		}

		// Token: 0x06005485 RID: 21637 RVA: 0x00180D28 File Offset: 0x0017EF28
		public static VersionInfo UnityVersion()
		{
			Match match = Regex.Match(Application.unityVersion, "^(\\d+)\\.(\\d+)\\.(\\d+)[a-zA-Z](\\d+)");
			return new VersionInfo
			{
				major = Convert.ToInt32(match.Groups[1].Value),
				minor = Convert.ToInt32(match.Groups[2].Value),
				patch = Convert.ToInt32(match.Groups[3].Value),
				build = Convert.ToInt32(match.Groups[4].Value)
			};
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x06005486 RID: 21638 RVA: 0x00180DC2 File Offset: 0x0017EFC2
		public static VersionInfo Min
		{
			get
			{
				return new VersionInfo(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
			}
		}

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x06005487 RID: 21639 RVA: 0x00180DDD File Offset: 0x0017EFDD
		public static VersionInfo Max
		{
			get
			{
				return new VersionInfo(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x06005488 RID: 21640 RVA: 0x00180DF8 File Offset: 0x0017EFF8
		public VersionInfo Next
		{
			get
			{
				return new VersionInfo(this.major, this.minor, this.patch, this.build + 1);
			}
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x06005489 RID: 21641 RVA: 0x00180E19 File Offset: 0x0017F019
		public int Build
		{
			get
			{
				return this.build;
			}
		}

		// Token: 0x0600548A RID: 21642 RVA: 0x00180E24 File Offset: 0x0017F024
		public int CompareTo(VersionInfo other)
		{
			if (this.major < other.major)
			{
				return -1;
			}
			if (this.major > other.major)
			{
				return 1;
			}
			if (this.minor < other.minor)
			{
				return -1;
			}
			if (this.minor > other.minor)
			{
				return 1;
			}
			if (this.patch < other.patch)
			{
				return -1;
			}
			if (this.patch > other.patch)
			{
				return 1;
			}
			if (this.build < other.build)
			{
				return -1;
			}
			if (this.build > other.build)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0600548B RID: 21643 RVA: 0x00180EB2 File Offset: 0x0017F0B2
		public static bool operator ==(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) == 0;
		}

		// Token: 0x0600548C RID: 21644 RVA: 0x00180EBF File Offset: 0x0017F0BF
		public static bool operator !=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) != 0;
		}

		// Token: 0x0600548D RID: 21645 RVA: 0x00180ECC File Offset: 0x0017F0CC
		public static bool operator <=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) <= 0;
		}

		// Token: 0x0600548E RID: 21646 RVA: 0x00180EDC File Offset: 0x0017F0DC
		public static bool operator >=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) >= 0;
		}

		// Token: 0x0600548F RID: 21647 RVA: 0x00180EEC File Offset: 0x0017F0EC
		public static bool operator <(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) < 0;
		}

		// Token: 0x06005490 RID: 21648 RVA: 0x00180EF9 File Offset: 0x0017F0F9
		public static bool operator >(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) > 0;
		}

		// Token: 0x06005491 RID: 21649 RVA: 0x00180F06 File Offset: 0x0017F106
		public override bool Equals(object other)
		{
			return other is VersionInfo && this == (VersionInfo)other;
		}

		// Token: 0x06005492 RID: 21650 RVA: 0x00180F23 File Offset: 0x0017F123
		public override int GetHashCode()
		{
			return this.major.GetHashCode() ^ this.minor.GetHashCode() ^ this.patch.GetHashCode() ^ this.build.GetHashCode();
		}

		// Token: 0x06005493 RID: 21651 RVA: 0x00180F54 File Offset: 0x0017F154
		public override string ToString()
		{
			if (this.build == 0)
			{
				return string.Format("{0}.{1}.{2}", this.major, this.minor, this.patch);
			}
			return string.Format("{0}.{1}.{2} build {3}", new object[]
			{
				this.major,
				this.minor,
				this.patch,
				this.build
			});
		}

		// Token: 0x06005494 RID: 21652 RVA: 0x00180FE0 File Offset: 0x0017F1E0
		public string ToShortString()
		{
			if (this.build == 0)
			{
				return string.Format("{0}.{1}.{2}", this.major, this.minor, this.patch);
			}
			return string.Format("{0}.{1}.{2}b{3}", new object[]
			{
				this.major,
				this.minor,
				this.patch,
				this.build
			});
		}

		// Token: 0x040053A7 RID: 21415
		[SerializeField]
		private int major;

		// Token: 0x040053A8 RID: 21416
		[SerializeField]
		private int minor;

		// Token: 0x040053A9 RID: 21417
		[SerializeField]
		private int patch;

		// Token: 0x040053AA RID: 21418
		[SerializeField]
		private int build;
	}
}
