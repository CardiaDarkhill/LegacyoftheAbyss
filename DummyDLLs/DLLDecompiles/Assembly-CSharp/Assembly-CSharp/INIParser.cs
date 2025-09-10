using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class INIParser
{
	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000021 RID: 33 RVA: 0x00002641 File Offset: 0x00000841
	public string FileName
	{
		get
		{
			return this.m_FileName;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000022 RID: 34 RVA: 0x00002649 File Offset: 0x00000849
	public string iniString
	{
		get
		{
			return this.m_iniString;
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002654 File Offset: 0x00000854
	public void Open(string path)
	{
		this.m_FileName = path;
		if (File.Exists(this.m_FileName))
		{
			this.m_iniString = File.ReadAllText(this.m_FileName);
		}
		else
		{
			File.Create(this.m_FileName).Close();
			this.m_iniString = "";
		}
		this.Initialize(this.m_iniString, false);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x000026B0 File Offset: 0x000008B0
	public void Open(TextAsset name)
	{
		if (name == null)
		{
			this.error = 1;
			this.m_iniString = "";
			this.m_FileName = null;
			this.Initialize(this.m_iniString, false);
			return;
		}
		this.m_FileName = Application.persistentDataPath + name.name;
		if (File.Exists(this.m_FileName))
		{
			this.m_iniString = File.ReadAllText(this.m_FileName);
		}
		else
		{
			this.m_iniString = name.text;
		}
		this.Initialize(this.m_iniString, false);
	}

	// Token: 0x06000025 RID: 37 RVA: 0x0000273C File Offset: 0x0000093C
	public void OpenFromString(string str)
	{
		this.m_FileName = null;
		this.Initialize(str, false);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x0000274D File Offset: 0x0000094D
	public override string ToString()
	{
		return this.m_iniString;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002755 File Offset: 0x00000955
	private void Initialize(string iniString, bool AutoFlush)
	{
		this.m_iniString = iniString;
		this.m_AutoFlush = AutoFlush;
		this.Refresh();
	}

	// Token: 0x06000028 RID: 40 RVA: 0x0000276C File Offset: 0x0000096C
	public void Close()
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			this.PerformFlush();
			this.m_FileName = null;
			this.m_iniString = null;
		}
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000027BC File Offset: 0x000009BC
	private string ParseSectionName(string Line)
	{
		if (!Line.StartsWith("["))
		{
			return null;
		}
		if (!Line.EndsWith("]"))
		{
			return null;
		}
		if (Line.Length < 3)
		{
			return null;
		}
		return Line.Substring(1, Line.Length - 2);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000027F8 File Offset: 0x000009F8
	private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
	{
		int num;
		if ((num = Line.IndexOf('=')) <= 0)
		{
			return false;
		}
		int num2 = Line.Length - num - 1;
		Key = Line.Substring(0, num).Trim();
		if (Key.Length <= 0)
		{
			return false;
		}
		Value = ((num2 > 0) ? Line.Substring(num + 1, num2).Trim() : "");
		return true;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002858 File Offset: 0x00000A58
	private bool isComment(string Line)
	{
		string text = null;
		string text2 = null;
		return this.ParseSectionName(Line) == null && !this.ParseKeyValuePair(Line, ref text, ref text2);
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002884 File Offset: 0x00000A84
	private void Refresh()
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			StringReader stringReader = null;
			try
			{
				this.m_Sections.Clear();
				this.m_Modified.Clear();
				stringReader = new StringReader(this.m_iniString);
				Dictionary<string, string> dictionary = null;
				string key = null;
				string value = null;
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					text = text.Trim();
					string text2 = this.ParseSectionName(text);
					if (text2 != null)
					{
						if (this.m_Sections.ContainsKey(text2))
						{
							dictionary = null;
						}
						else
						{
							dictionary = new Dictionary<string, string>();
							this.m_Sections.Add(text2, dictionary);
						}
					}
					else if (dictionary != null && this.ParseKeyValuePair(text, ref key, ref value) && !dictionary.ContainsKey(key))
					{
						dictionary.Add(key, value);
					}
				}
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Close();
				}
				stringReader = null;
			}
		}
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002974 File Offset: 0x00000B74
	private void PerformFlush()
	{
		if (!this.m_CacheModified)
		{
			return;
		}
		this.m_CacheModified = false;
		StringWriter stringWriter = new StringWriter();
		try
		{
			Dictionary<string, string> dictionary = null;
			Dictionary<string, string> dictionary2 = null;
			StringReader stringReader = null;
			try
			{
				stringReader = new StringReader(this.m_iniString);
				string text = null;
				string value = null;
				bool flag = true;
				bool flag2 = false;
				string key = null;
				string text2 = null;
				while (flag)
				{
					string text3 = stringReader.ReadLine();
					flag = (text3 != null);
					bool flag3;
					string text4;
					if (flag)
					{
						flag3 = true;
						text3 = text3.Trim();
						text4 = this.ParseSectionName(text3);
					}
					else
					{
						flag3 = false;
						text4 = null;
					}
					if (text4 != null || !flag)
					{
						if (dictionary != null && dictionary.Count > 0)
						{
							StringBuilder stringBuilder = stringWriter.GetStringBuilder();
							while (stringBuilder[stringBuilder.Length - 1] == '\n' || stringBuilder[stringBuilder.Length - 1] == '\r')
							{
								stringBuilder.Length--;
							}
							stringWriter.WriteLine();
							foreach (string text5 in dictionary.Keys)
							{
								if (dictionary.TryGetValue(text5, out value))
								{
									stringWriter.Write(text5);
									stringWriter.Write('=');
									stringWriter.WriteLine(value);
								}
							}
							stringWriter.WriteLine();
							stringWriter.WriteLine();
							dictionary.Clear();
						}
						if (flag && !this.m_Modified.TryGetValue(text4, out dictionary))
						{
							dictionary = null;
						}
					}
					else if (dictionary != null && this.ParseKeyValuePair(text3, ref text, ref value) && dictionary.TryGetValue(text, out value))
					{
						flag3 = false;
						dictionary.Remove(text);
						stringWriter.Write(text);
						stringWriter.Write('=');
						stringWriter.WriteLine(value);
					}
					if (flag3)
					{
						if (text4 != null)
						{
							if (!this.m_Sections.ContainsKey(text4))
							{
								flag2 = true;
								dictionary2 = null;
							}
							else
							{
								flag2 = false;
								this.m_Sections.TryGetValue(text4, out dictionary2);
							}
						}
						else if (dictionary2 != null && this.ParseKeyValuePair(text3, ref key, ref text2))
						{
							flag2 = !dictionary2.ContainsKey(key);
						}
					}
					if (flag3 && !this.isComment(text3) && !flag2)
					{
						stringWriter.WriteLine(text3);
					}
				}
				stringReader.Close();
				stringReader = null;
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Close();
				}
				stringReader = null;
			}
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.m_Modified)
			{
				dictionary = keyValuePair.Value;
				if (dictionary.Count > 0)
				{
					stringWriter.WriteLine();
					stringWriter.Write('[');
					stringWriter.Write(keyValuePair.Key);
					stringWriter.WriteLine(']');
					foreach (KeyValuePair<string, string> keyValuePair2 in dictionary)
					{
						stringWriter.Write(keyValuePair2.Key);
						stringWriter.Write('=');
						stringWriter.WriteLine(keyValuePair2.Value);
					}
					dictionary.Clear();
				}
			}
			this.m_Modified.Clear();
			this.m_iniString = stringWriter.ToString();
			stringWriter.Close();
			stringWriter = null;
			if (this.m_FileName != null)
			{
				File.WriteAllText(this.m_FileName, this.m_iniString);
			}
		}
		finally
		{
			if (stringWriter != null)
			{
				stringWriter.Close();
			}
			stringWriter = null;
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002D28 File Offset: 0x00000F28
	public bool IsSectionExists(string SectionName)
	{
		return this.m_Sections.ContainsKey(SectionName);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002D38 File Offset: 0x00000F38
	public bool IsKeyExists(string SectionName, string Key)
	{
		if (this.m_Sections.ContainsKey(SectionName))
		{
			Dictionary<string, string> dictionary;
			this.m_Sections.TryGetValue(SectionName, out dictionary);
			return dictionary.ContainsKey(Key);
		}
		return false;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002D6C File Offset: 0x00000F6C
	public void SectionDelete(string SectionName)
	{
		if (this.IsSectionExists(SectionName))
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_CacheModified = true;
				this.m_Sections.Remove(SectionName);
				this.m_Modified.Remove(SectionName);
				if (this.m_AutoFlush)
				{
					this.PerformFlush();
				}
			}
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002DE0 File Offset: 0x00000FE0
	public void KeyDelete(string SectionName, string Key)
	{
		if (this.IsKeyExists(SectionName, Key))
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_CacheModified = true;
				Dictionary<string, string> dictionary;
				this.m_Sections.TryGetValue(SectionName, out dictionary);
				dictionary.Remove(Key);
				if (this.m_Modified.TryGetValue(SectionName, out dictionary))
				{
					dictionary.Remove(SectionName);
				}
				if (this.m_AutoFlush)
				{
					this.PerformFlush();
				}
			}
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002E68 File Offset: 0x00001068
	public string ReadValue(string SectionName, string Key, string DefaultValue)
	{
		object @lock = this.m_Lock;
		string result;
		lock (@lock)
		{
			Dictionary<string, string> dictionary;
			string text;
			if (!this.m_Sections.TryGetValue(SectionName, out dictionary))
			{
				result = DefaultValue;
			}
			else if (!dictionary.TryGetValue(Key, out text))
			{
				result = DefaultValue;
			}
			else
			{
				result = text;
			}
		}
		return result;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002ECC File Offset: 0x000010CC
	public void WriteValue(string SectionName, string Key, string Value)
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			this.m_CacheModified = true;
			Dictionary<string, string> dictionary;
			if (!this.m_Sections.TryGetValue(SectionName, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				this.m_Sections.Add(SectionName, dictionary);
			}
			if (dictionary.ContainsKey(Key))
			{
				dictionary.Remove(Key);
			}
			dictionary.Add(Key, Value);
			if (!this.m_Modified.TryGetValue(SectionName, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				this.m_Modified.Add(SectionName, dictionary);
			}
			if (dictionary.ContainsKey(Key))
			{
				dictionary.Remove(Key);
			}
			dictionary.Add(Key, Value);
			if (this.m_AutoFlush)
			{
				this.PerformFlush();
			}
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002F94 File Offset: 0x00001194
	private string EncodeByteArray(byte[] Value)
	{
		if (Value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Value.Length; i++)
		{
			string text = Convert.ToString(Value[i], 16);
			int length = text.Length;
			if (length > 2)
			{
				stringBuilder.Append(text.Substring(length - 2, 2));
			}
			else
			{
				if (length < 2)
				{
					stringBuilder.Append("0");
				}
				stringBuilder.Append(text);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00003008 File Offset: 0x00001208
	private byte[] DecodeByteArray(string Value)
	{
		if (Value == null)
		{
			return null;
		}
		int num = Value.Length;
		if (num < 2)
		{
			return new byte[0];
		}
		num /= 2;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
		}
		return array;
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003058 File Offset: 0x00001258
	public bool ReadValue(string SectionName, string Key, bool DefaultValue)
	{
		int num;
		if (int.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), out num))
		{
			return num != 0;
		}
		return DefaultValue;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00003088 File Offset: 0x00001288
	public int ReadValue(string SectionName, string Key, int DefaultValue)
	{
		int result;
		if (int.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x000030C0 File Offset: 0x000012C0
	public long ReadValue(string SectionName, string Key, long DefaultValue)
	{
		long result;
		if (long.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x000030F8 File Offset: 0x000012F8
	public double ReadValue(string SectionName, string Key, double DefaultValue)
	{
		double result;
		if (double.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00003130 File Offset: 0x00001330
	public float ReadValue(string SectionName, string Key, float DefaultValue)
	{
		float result;
		if (float.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003168 File Offset: 0x00001368
	public byte[] ReadValue(string SectionName, string Key, byte[] DefaultValue)
	{
		string value = this.ReadValue(SectionName, Key, this.EncodeByteArray(DefaultValue));
		byte[] result;
		try
		{
			result = this.DecodeByteArray(value);
		}
		catch (FormatException)
		{
			result = DefaultValue;
		}
		return result;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000031A8 File Offset: 0x000013A8
	public DateTime ReadValue(string SectionName, string Key, DateTime DefaultValue)
	{
		DateTime result;
		if (DateTime.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x000031DC File Offset: 0x000013DC
	public void WriteValue(string SectionName, string Key, bool Value)
	{
		this.WriteValue(SectionName, Key, Value ? "1" : "0");
	}

	// Token: 0x0600003E RID: 62 RVA: 0x000031F5 File Offset: 0x000013F5
	public void WriteValue(string SectionName, string Key, int Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x0600003F RID: 63 RVA: 0x0000320B File Offset: 0x0000140B
	public void WriteValue(string SectionName, string Key, long Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003221 File Offset: 0x00001421
	public void WriteValue(string SectionName, string Key, double Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00003237 File Offset: 0x00001437
	public void WriteValue(string SectionName, string Key, float Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000042 RID: 66 RVA: 0x0000324D File Offset: 0x0000144D
	public void WriteValue(string SectionName, string Key, byte[] Value)
	{
		this.WriteValue(SectionName, Key, this.EncodeByteArray(Value));
	}

	// Token: 0x06000043 RID: 67 RVA: 0x0000325E File Offset: 0x0000145E
	public void WriteValue(string SectionName, string Key, DateTime Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x0400000F RID: 15
	public int error;

	// Token: 0x04000010 RID: 16
	private object m_Lock = new object();

	// Token: 0x04000011 RID: 17
	private string m_FileName;

	// Token: 0x04000012 RID: 18
	private string m_iniString;

	// Token: 0x04000013 RID: 19
	private bool m_AutoFlush;

	// Token: 0x04000014 RID: 20
	private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();

	// Token: 0x04000015 RID: 21
	private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();

	// Token: 0x04000016 RID: 22
	private bool m_CacheModified;
}
