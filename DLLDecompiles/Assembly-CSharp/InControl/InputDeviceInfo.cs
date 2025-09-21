using System;
using System.Runtime.InteropServices;

namespace InControl
{
	// Token: 0x02000904 RID: 2308
	public struct InputDeviceInfo
	{
		// Token: 0x06005157 RID: 20823 RVA: 0x00175637 File Offset: 0x00173837
		public bool HasSameVendorID(InputDeviceInfo deviceInfo)
		{
			return this.vendorID == deviceInfo.vendorID;
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x00175647 File Offset: 0x00173847
		public bool HasSameProductID(InputDeviceInfo deviceInfo)
		{
			return this.productID == deviceInfo.productID;
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x00175657 File Offset: 0x00173857
		public bool HasSameVersionNumber(InputDeviceInfo deviceInfo)
		{
			return this.versionNumber == deviceInfo.versionNumber;
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x00175667 File Offset: 0x00173867
		public bool HasSameLocation(InputDeviceInfo deviceInfo)
		{
			return !string.IsNullOrEmpty(this.location) && this.location == deviceInfo.location;
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x00175689 File Offset: 0x00173889
		public bool HasSameSerialNumber(InputDeviceInfo deviceInfo)
		{
			return !string.IsNullOrEmpty(this.serialNumber) && this.serialNumber == deviceInfo.serialNumber;
		}

		// Token: 0x04005203 RID: 20995
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string name;

		// Token: 0x04005204 RID: 20996
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string location;

		// Token: 0x04005205 RID: 20997
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string serialNumber;

		// Token: 0x04005206 RID: 20998
		public ushort vendorID;

		// Token: 0x04005207 RID: 20999
		public ushort productID;

		// Token: 0x04005208 RID: 21000
		public uint versionNumber;

		// Token: 0x04005209 RID: 21001
		public InputDeviceDriverType driverType;

		// Token: 0x0400520A RID: 21002
		public InputDeviceTransportType transportType;

		// Token: 0x0400520B RID: 21003
		public uint numButtons;

		// Token: 0x0400520C RID: 21004
		public uint numAnalogs;
	}
}
