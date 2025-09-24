using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x0200078E RID: 1934
public class StringEncrypt
{
	// Token: 0x06004485 RID: 17541 RVA: 0x0012C130 File Offset: 0x0012A330
	public static string EncryptData(string toEncrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
		byte[] array = new RijndaelManaged
		{
			Key = StringEncrypt.keyArray,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		}.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	// Token: 0x06004486 RID: 17542 RVA: 0x0012C184 File Offset: 0x0012A384
	public static string DecryptData(string toDecrypt)
	{
		byte[] array = Convert.FromBase64String(toDecrypt);
		byte[] bytes = new RijndaelManaged
		{
			Key = StringEncrypt.keyArray,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		}.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes);
	}

	// Token: 0x04004591 RID: 17809
	private static byte[] keyArray = Encoding.UTF8.GetBytes("UKu52ePUBwetZ9wNX88o54dnfKRu0T1l");
}
