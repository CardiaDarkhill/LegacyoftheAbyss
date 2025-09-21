using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000908 RID: 2312
	[Preserve]
	[Serializable]
	public class InputDeviceProfile
	{
		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x0600516F RID: 20847 RVA: 0x00175852 File Offset: 0x00173A52
		// (set) Token: 0x06005170 RID: 20848 RVA: 0x0017585A File Offset: 0x00173A5A
		public InputDeviceProfileType ProfileType
		{
			get
			{
				return this.profileType;
			}
			protected set
			{
				this.profileType = value;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06005171 RID: 20849 RVA: 0x00175863 File Offset: 0x00173A63
		// (set) Token: 0x06005172 RID: 20850 RVA: 0x0017586B File Offset: 0x00173A6B
		public string DeviceName
		{
			get
			{
				return this.deviceName;
			}
			protected set
			{
				this.deviceName = value;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06005173 RID: 20851 RVA: 0x00175874 File Offset: 0x00173A74
		// (set) Token: 0x06005174 RID: 20852 RVA: 0x0017587C File Offset: 0x00173A7C
		public string DeviceNotes
		{
			get
			{
				return this.deviceNotes;
			}
			protected set
			{
				this.deviceNotes = value;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06005175 RID: 20853 RVA: 0x00175885 File Offset: 0x00173A85
		// (set) Token: 0x06005176 RID: 20854 RVA: 0x0017588D File Offset: 0x00173A8D
		public InputDeviceClass DeviceClass
		{
			get
			{
				return this.deviceClass;
			}
			protected set
			{
				this.deviceClass = value;
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06005177 RID: 20855 RVA: 0x00175896 File Offset: 0x00173A96
		// (set) Token: 0x06005178 RID: 20856 RVA: 0x0017589E File Offset: 0x00173A9E
		public InputDeviceStyle DeviceStyle
		{
			get
			{
				return this.deviceStyle;
			}
			protected set
			{
				this.deviceStyle = value;
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06005179 RID: 20857 RVA: 0x001758A7 File Offset: 0x00173AA7
		// (set) Token: 0x0600517A RID: 20858 RVA: 0x001758AF File Offset: 0x00173AAF
		public float Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			protected set
			{
				this.sensitivity = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x0600517B RID: 20859 RVA: 0x001758BD File Offset: 0x00173ABD
		// (set) Token: 0x0600517C RID: 20860 RVA: 0x001758C5 File Offset: 0x00173AC5
		public float LowerDeadZone
		{
			get
			{
				return this.lowerDeadZone;
			}
			protected set
			{
				this.lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x0600517D RID: 20861 RVA: 0x001758D3 File Offset: 0x00173AD3
		// (set) Token: 0x0600517E RID: 20862 RVA: 0x001758DB File Offset: 0x00173ADB
		public float UpperDeadZone
		{
			get
			{
				return this.upperDeadZone;
			}
			protected set
			{
				this.upperDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x0600517F RID: 20863 RVA: 0x001758E9 File Offset: 0x00173AE9
		// (set) Token: 0x06005180 RID: 20864 RVA: 0x001758F1 File Offset: 0x00173AF1
		public InputControlMapping[] AnalogMappings
		{
			get
			{
				return this.analogMappings;
			}
			protected set
			{
				this.analogMappings = value;
			}
		}

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06005181 RID: 20865 RVA: 0x001758FA File Offset: 0x00173AFA
		// (set) Token: 0x06005182 RID: 20866 RVA: 0x00175902 File Offset: 0x00173B02
		public InputControlMapping[] ButtonMappings
		{
			get
			{
				return this.buttonMappings;
			}
			protected set
			{
				this.buttonMappings = value;
			}
		}

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06005183 RID: 20867 RVA: 0x0017590B File Offset: 0x00173B0B
		// (set) Token: 0x06005184 RID: 20868 RVA: 0x00175913 File Offset: 0x00173B13
		public string[] IncludePlatforms
		{
			get
			{
				return this.includePlatforms;
			}
			protected set
			{
				this.includePlatforms = value;
			}
		}

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06005185 RID: 20869 RVA: 0x0017591C File Offset: 0x00173B1C
		// (set) Token: 0x06005186 RID: 20870 RVA: 0x00175924 File Offset: 0x00173B24
		public string[] ExcludePlatforms
		{
			get
			{
				return this.excludePlatforms;
			}
			protected set
			{
				this.excludePlatforms = value;
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06005187 RID: 20871 RVA: 0x0017592D File Offset: 0x00173B2D
		// (set) Token: 0x06005188 RID: 20872 RVA: 0x00175935 File Offset: 0x00173B35
		public int MinSystemBuildNumber
		{
			get
			{
				return this.minSystemBuildNumber;
			}
			protected set
			{
				this.minSystemBuildNumber = value;
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06005189 RID: 20873 RVA: 0x0017593E File Offset: 0x00173B3E
		// (set) Token: 0x0600518A RID: 20874 RVA: 0x00175946 File Offset: 0x00173B46
		public int MaxSystemBuildNumber
		{
			get
			{
				return this.maxSystemBuildNumber;
			}
			protected set
			{
				this.maxSystemBuildNumber = value;
			}
		}

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x0600518B RID: 20875 RVA: 0x0017594F File Offset: 0x00173B4F
		// (set) Token: 0x0600518C RID: 20876 RVA: 0x00175957 File Offset: 0x00173B57
		public VersionInfo MinUnityVersion
		{
			get
			{
				return this.minUnityVersion;
			}
			protected set
			{
				this.minUnityVersion = value;
			}
		}

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x0600518D RID: 20877 RVA: 0x00175960 File Offset: 0x00173B60
		// (set) Token: 0x0600518E RID: 20878 RVA: 0x00175968 File Offset: 0x00173B68
		public VersionInfo MaxUnityVersion
		{
			get
			{
				return this.maxUnityVersion;
			}
			protected set
			{
				this.maxUnityVersion = value;
			}
		}

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x0600518F RID: 20879 RVA: 0x00175971 File Offset: 0x00173B71
		// (set) Token: 0x06005190 RID: 20880 RVA: 0x00175979 File Offset: 0x00173B79
		public InputDeviceMatcher[] Matchers
		{
			get
			{
				return this.matchers;
			}
			protected set
			{
				this.matchers = value;
			}
		}

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06005191 RID: 20881 RVA: 0x00175982 File Offset: 0x00173B82
		// (set) Token: 0x06005192 RID: 20882 RVA: 0x0017598A File Offset: 0x00173B8A
		public InputDeviceMatcher[] LastResortMatchers
		{
			get
			{
				return this.lastResortMatchers;
			}
			protected set
			{
				this.lastResortMatchers = value;
			}
		}

		// Token: 0x06005193 RID: 20883 RVA: 0x00175993 File Offset: 0x00173B93
		public static InputDeviceProfile CreateInstanceOfType(Type type)
		{
			InputDeviceProfile inputDeviceProfile = (InputDeviceProfile)Activator.CreateInstance(type);
			inputDeviceProfile.Define();
			return inputDeviceProfile;
		}

		// Token: 0x06005194 RID: 20884 RVA: 0x001759A8 File Offset: 0x00173BA8
		public static InputDeviceProfile CreateInstanceOfType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				Logger.LogWarning("Cannot find type: " + typeName + "(is the IL2CPP stripping level too high?)");
				return null;
			}
			return InputDeviceProfile.CreateInstanceOfType(type);
		}

		// Token: 0x06005195 RID: 20885 RVA: 0x001759E4 File Offset: 0x00173BE4
		public virtual void Define()
		{
			this.profileType = ((base.GetType().GetCustomAttributes(typeof(NativeInputDeviceProfileAttribute), false).Length != 0) ? InputDeviceProfileType.Native : InputDeviceProfileType.Unity);
		}

		// Token: 0x06005196 RID: 20886 RVA: 0x00175A19 File Offset: 0x00173C19
		public bool Matches(InputDeviceInfo deviceInfo)
		{
			return this.Matches(deviceInfo, this.Matchers);
		}

		// Token: 0x06005197 RID: 20887 RVA: 0x00175A28 File Offset: 0x00173C28
		public bool LastResortMatches(InputDeviceInfo deviceInfo)
		{
			return this.Matches(deviceInfo, this.LastResortMatchers);
		}

		// Token: 0x06005198 RID: 20888 RVA: 0x00175A38 File Offset: 0x00173C38
		public bool Matches(InputDeviceInfo deviceInfo, InputDeviceMatcher[] matchers)
		{
			if (matchers != null)
			{
				int num = matchers.Length;
				for (int i = 0; i < num; i++)
				{
					if (matchers[i].Matches(deviceInfo))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06005199 RID: 20889 RVA: 0x00175A6C File Offset: 0x00173C6C
		public bool IsSupportedOnThisPlatform
		{
			get
			{
				VersionInfo a = VersionInfo.UnityVersion();
				if (a < this.MinUnityVersion || a > this.MaxUnityVersion)
				{
					return false;
				}
				int systemBuildNumber = Utility.GetSystemBuildNumber();
				if (this.MaxSystemBuildNumber > 0 && systemBuildNumber > this.MaxSystemBuildNumber)
				{
					return false;
				}
				if (this.MinSystemBuildNumber > 0 && systemBuildNumber < this.MinSystemBuildNumber)
				{
					return false;
				}
				if (this.ExcludePlatforms != null)
				{
					int num = this.ExcludePlatforms.Length;
					for (int i = 0; i < num; i++)
					{
						if (InputManager.Platform.Contains(this.ExcludePlatforms[i].ToUpper()))
						{
							return false;
						}
					}
				}
				if (this.IncludePlatforms == null || this.IncludePlatforms.Length == 0)
				{
					return true;
				}
				if (this.IncludePlatforms != null)
				{
					int num2 = this.IncludePlatforms.Length;
					for (int j = 0; j < num2; j++)
					{
						if (InputManager.Platform.Contains(this.IncludePlatforms[j].ToUpper()))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x0600519A RID: 20890 RVA: 0x00175B56 File Offset: 0x00173D56
		public static void Hide(Type type)
		{
			InputDeviceProfile.hiddenProfiles.Add(type);
		}

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x0600519B RID: 20891 RVA: 0x00175B64 File Offset: 0x00173D64
		public bool IsHidden
		{
			get
			{
				return InputDeviceProfile.hiddenProfiles.Contains(base.GetType());
			}
		}

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x0600519C RID: 20892 RVA: 0x00175B76 File Offset: 0x00173D76
		public bool IsNotHidden
		{
			get
			{
				return !this.IsHidden;
			}
		}

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x0600519D RID: 20893 RVA: 0x00175B81 File Offset: 0x00173D81
		public int AnalogCount
		{
			get
			{
				return this.AnalogMappings.Length;
			}
		}

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x0600519E RID: 20894 RVA: 0x00175B8B File Offset: 0x00173D8B
		public int ButtonCount
		{
			get
			{
				return this.ButtonMappings.Length;
			}
		}

		// Token: 0x0600519F RID: 20895 RVA: 0x00175B95 File Offset: 0x00173D95
		protected static InputControlSource Button(int index)
		{
			return new InputControlSource(InputControlSourceType.Button, index);
		}

		// Token: 0x060051A0 RID: 20896 RVA: 0x00175B9E File Offset: 0x00173D9E
		protected static InputControlSource Analog(int index)
		{
			return new InputControlSource(InputControlSourceType.Analog, index);
		}

		// Token: 0x060051A1 RID: 20897 RVA: 0x00175BA7 File Offset: 0x00173DA7
		protected static InputControlMapping LeftStickLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Left",
				Target = InputControlType.LeftStickLeft,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A2 RID: 20898 RVA: 0x00175BDA File Offset: 0x00173DDA
		protected static InputControlMapping LeftStickRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Right",
				Target = InputControlType.LeftStickRight,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A3 RID: 20899 RVA: 0x00175C0D File Offset: 0x00173E0D
		protected static InputControlMapping LeftStickUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A4 RID: 20900 RVA: 0x00175C40 File Offset: 0x00173E40
		protected static InputControlMapping LeftStickDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A5 RID: 20901 RVA: 0x00175C73 File Offset: 0x00173E73
		protected static InputControlMapping LeftStickUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A6 RID: 20902 RVA: 0x00175CA6 File Offset: 0x00173EA6
		protected static InputControlMapping LeftStickDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A7 RID: 20903 RVA: 0x00175CD9 File Offset: 0x00173ED9
		protected static InputControlMapping RightStickLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Left",
				Target = InputControlType.RightStickLeft,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A8 RID: 20904 RVA: 0x00175D0C File Offset: 0x00173F0C
		protected static InputControlMapping RightStickRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Right",
				Target = InputControlType.RightStickRight,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051A9 RID: 20905 RVA: 0x00175D40 File Offset: 0x00173F40
		protected static InputControlMapping RightStickUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051AA RID: 20906 RVA: 0x00175D73 File Offset: 0x00173F73
		protected static InputControlMapping RightStickDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051AB RID: 20907 RVA: 0x00175DA6 File Offset: 0x00173FA6
		protected static InputControlMapping RightStickUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051AC RID: 20908 RVA: 0x00175DD9 File Offset: 0x00173FD9
		protected static InputControlMapping RightStickDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051AD RID: 20909 RVA: 0x00175E0C File Offset: 0x0017400C
		protected static InputControlMapping LeftTriggerMapping(int analog, string name = "Left Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftTrigger,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.ZeroToOne,
				IgnoreInitialZeroValue = true
			};
		}

		// Token: 0x060051AE RID: 20910 RVA: 0x00175E43 File Offset: 0x00174043
		protected static InputControlMapping RightTriggerMapping(int analog, string name = "Right Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightTrigger,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.ZeroToOne,
				IgnoreInitialZeroValue = true
			};
		}

		// Token: 0x060051AF RID: 20911 RVA: 0x00175E7A File Offset: 0x0017407A
		protected static InputControlMapping DPadLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Left",
				Target = InputControlType.DPadLeft,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051B0 RID: 20912 RVA: 0x00175EAE File Offset: 0x001740AE
		protected static InputControlMapping DPadRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Right",
				Target = InputControlType.DPadRight,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051B1 RID: 20913 RVA: 0x00175EE2 File Offset: 0x001740E2
		protected static InputControlMapping DPadUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051B2 RID: 20914 RVA: 0x00175F16 File Offset: 0x00174116
		protected static InputControlMapping DPadDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051B3 RID: 20915 RVA: 0x00175F4A File Offset: 0x0017414A
		protected static InputControlMapping DPadUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060051B4 RID: 20916 RVA: 0x00175F7E File Offset: 0x0017417E
		protected static InputControlMapping DPadDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = InputDeviceProfile.Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x04005215 RID: 21013
		private static readonly HashSet<Type> hiddenProfiles = new HashSet<Type>();

		// Token: 0x04005216 RID: 21014
		[SerializeField]
		private InputDeviceProfileType profileType;

		// Token: 0x04005217 RID: 21015
		[SerializeField]
		private string deviceName = "";

		// Token: 0x04005218 RID: 21016
		[SerializeField]
		[TextArea]
		private string deviceNotes = "";

		// Token: 0x04005219 RID: 21017
		[SerializeField]
		private InputDeviceClass deviceClass;

		// Token: 0x0400521A RID: 21018
		[SerializeField]
		private InputDeviceStyle deviceStyle;

		// Token: 0x0400521B RID: 21019
		[SerializeField]
		private float sensitivity = 1f;

		// Token: 0x0400521C RID: 21020
		[SerializeField]
		private float lowerDeadZone = 0.2f;

		// Token: 0x0400521D RID: 21021
		[SerializeField]
		private float upperDeadZone = 0.9f;

		// Token: 0x0400521E RID: 21022
		[SerializeField]
		private string[] includePlatforms = new string[0];

		// Token: 0x0400521F RID: 21023
		[SerializeField]
		private string[] excludePlatforms = new string[0];

		// Token: 0x04005220 RID: 21024
		[SerializeField]
		private int minSystemBuildNumber;

		// Token: 0x04005221 RID: 21025
		[SerializeField]
		private int maxSystemBuildNumber;

		// Token: 0x04005222 RID: 21026
		[SerializeField]
		private VersionInfo minUnityVersion = VersionInfo.Min;

		// Token: 0x04005223 RID: 21027
		[SerializeField]
		private VersionInfo maxUnityVersion = VersionInfo.Max;

		// Token: 0x04005224 RID: 21028
		[SerializeField]
		private InputDeviceMatcher[] matchers = new InputDeviceMatcher[0];

		// Token: 0x04005225 RID: 21029
		[SerializeField]
		private InputDeviceMatcher[] lastResortMatchers = new InputDeviceMatcher[0];

		// Token: 0x04005226 RID: 21030
		[SerializeField]
		private InputControlMapping[] analogMappings = new InputControlMapping[0];

		// Token: 0x04005227 RID: 21031
		[SerializeField]
		private InputControlMapping[] buttonMappings = new InputControlMapping[0];

		// Token: 0x04005228 RID: 21032
		protected static readonly InputControlSource MenuKey = new InputControlSource(KeyCode.Menu);

		// Token: 0x04005229 RID: 21033
		protected static readonly InputControlSource EscapeKey = new InputControlSource(KeyCode.Escape);
	}
}
