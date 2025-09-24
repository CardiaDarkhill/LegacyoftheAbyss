using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000907 RID: 2311
	[Serializable]
	public struct InputDeviceMatcher
	{
		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06005160 RID: 20832 RVA: 0x001756C8 File Offset: 0x001738C8
		// (set) Token: 0x06005161 RID: 20833 RVA: 0x001756D0 File Offset: 0x001738D0
		public OptionalUInt16 VendorID
		{
			get
			{
				return this.vendorID;
			}
			set
			{
				this.vendorID = value;
			}
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06005162 RID: 20834 RVA: 0x001756D9 File Offset: 0x001738D9
		// (set) Token: 0x06005163 RID: 20835 RVA: 0x001756E1 File Offset: 0x001738E1
		public OptionalUInt16 ProductID
		{
			get
			{
				return this.productID;
			}
			set
			{
				this.productID = value;
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06005164 RID: 20836 RVA: 0x001756EA File Offset: 0x001738EA
		// (set) Token: 0x06005165 RID: 20837 RVA: 0x001756F2 File Offset: 0x001738F2
		public OptionalUInt32 VersionNumber
		{
			get
			{
				return this.versionNumber;
			}
			set
			{
				this.versionNumber = value;
			}
		}

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06005166 RID: 20838 RVA: 0x001756FB File Offset: 0x001738FB
		// (set) Token: 0x06005167 RID: 20839 RVA: 0x00175703 File Offset: 0x00173903
		public OptionalInputDeviceDriverType DriverType
		{
			get
			{
				return this.driverType;
			}
			set
			{
				this.driverType = value;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06005168 RID: 20840 RVA: 0x0017570C File Offset: 0x0017390C
		// (set) Token: 0x06005169 RID: 20841 RVA: 0x00175714 File Offset: 0x00173914
		public OptionalInputDeviceTransportType TransportType
		{
			get
			{
				return this.transportType;
			}
			set
			{
				this.transportType = value;
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x0600516A RID: 20842 RVA: 0x0017571D File Offset: 0x0017391D
		// (set) Token: 0x0600516B RID: 20843 RVA: 0x00175725 File Offset: 0x00173925
		public string NameLiteral
		{
			get
			{
				return this.nameLiteral;
			}
			set
			{
				this.nameLiteral = value;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x0600516C RID: 20844 RVA: 0x0017572E File Offset: 0x0017392E
		// (set) Token: 0x0600516D RID: 20845 RVA: 0x00175736 File Offset: 0x00173936
		public string NamePattern
		{
			get
			{
				return this.namePattern;
			}
			set
			{
				this.namePattern = value;
			}
		}

		// Token: 0x0600516E RID: 20846 RVA: 0x00175740 File Offset: 0x00173940
		internal bool Matches(InputDeviceInfo deviceInfo)
		{
			return (!this.VendorID.HasValue || this.VendorID.Value == deviceInfo.vendorID) && (!this.ProductID.HasValue || this.ProductID.Value == deviceInfo.productID) && (!this.VersionNumber.HasValue || this.VersionNumber.Value == deviceInfo.versionNumber) && (!this.DriverType.HasValue || this.DriverType.Value == deviceInfo.driverType) && (!this.TransportType.HasValue || this.TransportType.Value == deviceInfo.transportType) && (this.NameLiteral == null || string.Equals(deviceInfo.name, this.NameLiteral, StringComparison.OrdinalIgnoreCase)) && (this.NamePattern == null || Regex.IsMatch(deviceInfo.name, this.NamePattern, RegexOptions.IgnoreCase));
		}

		// Token: 0x0400520E RID: 21006
		[SerializeField]
		[Hexadecimal]
		private OptionalUInt16 vendorID;

		// Token: 0x0400520F RID: 21007
		[SerializeField]
		private OptionalUInt16 productID;

		// Token: 0x04005210 RID: 21008
		[SerializeField]
		[Hexadecimal]
		private OptionalUInt32 versionNumber;

		// Token: 0x04005211 RID: 21009
		[SerializeField]
		private OptionalInputDeviceDriverType driverType;

		// Token: 0x04005212 RID: 21010
		[SerializeField]
		private OptionalInputDeviceTransportType transportType;

		// Token: 0x04005213 RID: 21011
		[SerializeField]
		private string nameLiteral;

		// Token: 0x04005214 RID: 21012
		[SerializeField]
		private string namePattern;
	}
}
