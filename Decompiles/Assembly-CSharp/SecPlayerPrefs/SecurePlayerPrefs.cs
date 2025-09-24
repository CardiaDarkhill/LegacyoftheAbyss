using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SecPlayerPrefs
{
	// Token: 0x02000834 RID: 2100
	public class SecurePlayerPrefs : ScriptableObject
	{
		// Token: 0x06004AB9 RID: 19129 RVA: 0x001625F0 File Offset: 0x001607F0
		private static string Encrypt(string toEncrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
			byte[] array = new RijndaelManaged
			{
				Key = SecurePlayerPrefs.keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x00162644 File Offset: 0x00160844
		private static string Decrypt(string toDecrypt)
		{
			byte[] array = Convert.FromBase64String(toDecrypt);
			byte[] bytes = new RijndaelManaged
			{
				Key = SecurePlayerPrefs.keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes);
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x00162691 File Offset: 0x00160891
		private static string UTF8ByteArrayToString(byte[] characters)
		{
			return new UTF8Encoding().GetString(characters);
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x0016269E File Offset: 0x0016089E
		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			return new UTF8Encoding().GetBytes(pXmlString);
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x001626AB File Offset: 0x001608AB
		public static void SetInt(string Key, int Value)
		{
			PlayerPrefs.SetString(SecurePlayerPrefs.Encrypt(Key), SecurePlayerPrefs.Encrypt(Value.ToString()));
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x001626C4 File Offset: 0x001608C4
		public static void SetString(string Key, string Value)
		{
			PlayerPrefs.SetString(SecurePlayerPrefs.Encrypt(Key), SecurePlayerPrefs.Encrypt(Value));
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x001626D7 File Offset: 0x001608D7
		public static void SetFloat(string Key, float Value)
		{
			PlayerPrefs.SetString(SecurePlayerPrefs.Encrypt(Key), SecurePlayerPrefs.Encrypt(Value.ToString(CultureInfo.InvariantCulture)));
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x001626F5 File Offset: 0x001608F5
		public static void SetBool(string Key, bool Value)
		{
			PlayerPrefs.SetString(SecurePlayerPrefs.Encrypt(Key), SecurePlayerPrefs.Encrypt(Value.ToString()));
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x00162710 File Offset: 0x00160910
		public static string GetString(string Key)
		{
			string @string = PlayerPrefs.GetString(SecurePlayerPrefs.Encrypt(Key));
			if (@string == "")
			{
				return "";
			}
			return SecurePlayerPrefs.Decrypt(@string);
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x00162744 File Offset: 0x00160944
		public static int GetInt(string Key)
		{
			string @string = PlayerPrefs.GetString(SecurePlayerPrefs.Encrypt(Key));
			if (@string == "")
			{
				return 0;
			}
			return int.Parse(SecurePlayerPrefs.Decrypt(@string));
		}

		// Token: 0x06004AC3 RID: 19139 RVA: 0x00162778 File Offset: 0x00160978
		public static float GetFloat(string Key)
		{
			string @string = PlayerPrefs.GetString(SecurePlayerPrefs.Encrypt(Key));
			if (@string == "")
			{
				return 0f;
			}
			return float.Parse(SecurePlayerPrefs.Decrypt(@string), NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		// Token: 0x06004AC4 RID: 19140 RVA: 0x001627BC File Offset: 0x001609BC
		public static bool GetBool(string Key)
		{
			string @string = PlayerPrefs.GetString(SecurePlayerPrefs.Encrypt(Key));
			return !(@string == "") && bool.Parse(SecurePlayerPrefs.Decrypt(@string));
		}

		// Token: 0x06004AC5 RID: 19141 RVA: 0x001627EF File Offset: 0x001609EF
		public static void DeleteKey(string Key)
		{
			PlayerPrefs.DeleteKey(SecurePlayerPrefs.Encrypt(Key));
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x001627FC File Offset: 0x001609FC
		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00162803 File Offset: 0x00160A03
		public static void Save()
		{
			PlayerPrefs.Save();
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x0016280A File Offset: 0x00160A0A
		public static bool HasKey(string Key)
		{
			return PlayerPrefs.HasKey(SecurePlayerPrefs.Encrypt(Key));
		}

		// Token: 0x04004A90 RID: 19088
		private static readonly byte[] Salt = new byte[]
		{
			10,
			20,
			30,
			40,
			50,
			60,
			70,
			80,
			90,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19
		};

		// Token: 0x04004A91 RID: 19089
		private static byte[] keyArray = Encoding.UTF8.GetBytes("UKu52ePUBwetZ9wNX88o54dnfKRu0T1l");
	}
}
