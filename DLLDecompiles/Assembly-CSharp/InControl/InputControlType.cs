using System;

namespace InControl
{
	// Token: 0x020008F8 RID: 2296
	public enum InputControlType
	{
		// Token: 0x040050D7 RID: 20695
		None,
		// Token: 0x040050D8 RID: 20696
		LeftStickUp,
		// Token: 0x040050D9 RID: 20697
		LeftStickDown,
		// Token: 0x040050DA RID: 20698
		LeftStickLeft,
		// Token: 0x040050DB RID: 20699
		LeftStickRight,
		// Token: 0x040050DC RID: 20700
		LeftStickButton,
		// Token: 0x040050DD RID: 20701
		RightStickUp,
		// Token: 0x040050DE RID: 20702
		RightStickDown,
		// Token: 0x040050DF RID: 20703
		RightStickLeft,
		// Token: 0x040050E0 RID: 20704
		RightStickRight,
		// Token: 0x040050E1 RID: 20705
		RightStickButton,
		// Token: 0x040050E2 RID: 20706
		DPadUp,
		// Token: 0x040050E3 RID: 20707
		DPadDown,
		// Token: 0x040050E4 RID: 20708
		DPadLeft,
		// Token: 0x040050E5 RID: 20709
		DPadRight,
		// Token: 0x040050E6 RID: 20710
		LeftTrigger,
		// Token: 0x040050E7 RID: 20711
		RightTrigger,
		// Token: 0x040050E8 RID: 20712
		LeftBumper,
		// Token: 0x040050E9 RID: 20713
		RightBumper,
		// Token: 0x040050EA RID: 20714
		Action1,
		// Token: 0x040050EB RID: 20715
		Action2,
		// Token: 0x040050EC RID: 20716
		Action3,
		// Token: 0x040050ED RID: 20717
		Action4,
		// Token: 0x040050EE RID: 20718
		Action5,
		// Token: 0x040050EF RID: 20719
		Action6,
		// Token: 0x040050F0 RID: 20720
		Action7,
		// Token: 0x040050F1 RID: 20721
		Action8,
		// Token: 0x040050F2 RID: 20722
		Action9,
		// Token: 0x040050F3 RID: 20723
		Action10,
		// Token: 0x040050F4 RID: 20724
		Action11,
		// Token: 0x040050F5 RID: 20725
		Action12,
		// Token: 0x040050F6 RID: 20726
		Back = 100,
		// Token: 0x040050F7 RID: 20727
		Start,
		// Token: 0x040050F8 RID: 20728
		Select,
		// Token: 0x040050F9 RID: 20729
		System,
		// Token: 0x040050FA RID: 20730
		Options,
		// Token: 0x040050FB RID: 20731
		Pause,
		// Token: 0x040050FC RID: 20732
		Menu,
		// Token: 0x040050FD RID: 20733
		Share,
		// Token: 0x040050FE RID: 20734
		Home,
		// Token: 0x040050FF RID: 20735
		View,
		// Token: 0x04005100 RID: 20736
		Power,
		// Token: 0x04005101 RID: 20737
		Capture,
		// Token: 0x04005102 RID: 20738
		Assistant,
		// Token: 0x04005103 RID: 20739
		Plus,
		// Token: 0x04005104 RID: 20740
		Minus,
		// Token: 0x04005105 RID: 20741
		Create,
		// Token: 0x04005106 RID: 20742
		Mute,
		// Token: 0x04005107 RID: 20743
		Guide,
		// Token: 0x04005108 RID: 20744
		PedalLeft = 150,
		// Token: 0x04005109 RID: 20745
		PedalRight,
		// Token: 0x0400510A RID: 20746
		PedalMiddle,
		// Token: 0x0400510B RID: 20747
		GearUp,
		// Token: 0x0400510C RID: 20748
		GearDown,
		// Token: 0x0400510D RID: 20749
		Pitch = 200,
		// Token: 0x0400510E RID: 20750
		Roll,
		// Token: 0x0400510F RID: 20751
		Yaw,
		// Token: 0x04005110 RID: 20752
		PitchUp,
		// Token: 0x04005111 RID: 20753
		PitchDown,
		// Token: 0x04005112 RID: 20754
		RollLeft,
		// Token: 0x04005113 RID: 20755
		RollRight,
		// Token: 0x04005114 RID: 20756
		YawLeft,
		// Token: 0x04005115 RID: 20757
		YawRight,
		// Token: 0x04005116 RID: 20758
		ThrottleUp,
		// Token: 0x04005117 RID: 20759
		ThrottleDown,
		// Token: 0x04005118 RID: 20760
		ThrottleLeft,
		// Token: 0x04005119 RID: 20761
		ThrottleRight,
		// Token: 0x0400511A RID: 20762
		POVUp,
		// Token: 0x0400511B RID: 20763
		POVDown,
		// Token: 0x0400511C RID: 20764
		POVLeft,
		// Token: 0x0400511D RID: 20765
		POVRight,
		// Token: 0x0400511E RID: 20766
		TiltX = 250,
		// Token: 0x0400511F RID: 20767
		TiltY,
		// Token: 0x04005120 RID: 20768
		TiltZ,
		// Token: 0x04005121 RID: 20769
		GyroscopeX = 250,
		// Token: 0x04005122 RID: 20770
		GyroscopeY,
		// Token: 0x04005123 RID: 20771
		GyroscopeZ,
		// Token: 0x04005124 RID: 20772
		AccelerometerX,
		// Token: 0x04005125 RID: 20773
		AccelerometerY,
		// Token: 0x04005126 RID: 20774
		AccelerometerZ,
		// Token: 0x04005127 RID: 20775
		ScrollWheel,
		// Token: 0x04005128 RID: 20776
		[Obsolete("Use InputControlType.TouchPadButton instead.", true)]
		TouchPadTap,
		// Token: 0x04005129 RID: 20777
		TouchPadButton,
		// Token: 0x0400512A RID: 20778
		TouchPadXAxis,
		// Token: 0x0400512B RID: 20779
		TouchPadYAxis,
		// Token: 0x0400512C RID: 20780
		LeftSL,
		// Token: 0x0400512D RID: 20781
		LeftSR,
		// Token: 0x0400512E RID: 20782
		RightSL,
		// Token: 0x0400512F RID: 20783
		RightSR,
		// Token: 0x04005130 RID: 20784
		Paddle1,
		// Token: 0x04005131 RID: 20785
		Paddle2,
		// Token: 0x04005132 RID: 20786
		Paddle3,
		// Token: 0x04005133 RID: 20787
		Paddle4,
		// Token: 0x04005134 RID: 20788
		Command = 300,
		// Token: 0x04005135 RID: 20789
		LeftStickX,
		// Token: 0x04005136 RID: 20790
		LeftStickY,
		// Token: 0x04005137 RID: 20791
		RightStickX,
		// Token: 0x04005138 RID: 20792
		RightStickY,
		// Token: 0x04005139 RID: 20793
		DPadX,
		// Token: 0x0400513A RID: 20794
		DPadY,
		// Token: 0x0400513B RID: 20795
		LeftCommand,
		// Token: 0x0400513C RID: 20796
		RightCommand,
		// Token: 0x0400513D RID: 20797
		Analog0 = 400,
		// Token: 0x0400513E RID: 20798
		Analog1,
		// Token: 0x0400513F RID: 20799
		Analog2,
		// Token: 0x04005140 RID: 20800
		Analog3,
		// Token: 0x04005141 RID: 20801
		Analog4,
		// Token: 0x04005142 RID: 20802
		Analog5,
		// Token: 0x04005143 RID: 20803
		Analog6,
		// Token: 0x04005144 RID: 20804
		Analog7,
		// Token: 0x04005145 RID: 20805
		Analog8,
		// Token: 0x04005146 RID: 20806
		Analog9,
		// Token: 0x04005147 RID: 20807
		Analog10,
		// Token: 0x04005148 RID: 20808
		Analog11,
		// Token: 0x04005149 RID: 20809
		Analog12,
		// Token: 0x0400514A RID: 20810
		Analog13,
		// Token: 0x0400514B RID: 20811
		Analog14,
		// Token: 0x0400514C RID: 20812
		Analog15,
		// Token: 0x0400514D RID: 20813
		Analog16,
		// Token: 0x0400514E RID: 20814
		Analog17,
		// Token: 0x0400514F RID: 20815
		Analog18,
		// Token: 0x04005150 RID: 20816
		Analog19,
		// Token: 0x04005151 RID: 20817
		Button0 = 500,
		// Token: 0x04005152 RID: 20818
		Button1,
		// Token: 0x04005153 RID: 20819
		Button2,
		// Token: 0x04005154 RID: 20820
		Button3,
		// Token: 0x04005155 RID: 20821
		Button4,
		// Token: 0x04005156 RID: 20822
		Button5,
		// Token: 0x04005157 RID: 20823
		Button6,
		// Token: 0x04005158 RID: 20824
		Button7,
		// Token: 0x04005159 RID: 20825
		Button8,
		// Token: 0x0400515A RID: 20826
		Button9,
		// Token: 0x0400515B RID: 20827
		Button10,
		// Token: 0x0400515C RID: 20828
		Button11,
		// Token: 0x0400515D RID: 20829
		Button12,
		// Token: 0x0400515E RID: 20830
		Button13,
		// Token: 0x0400515F RID: 20831
		Button14,
		// Token: 0x04005160 RID: 20832
		Button15,
		// Token: 0x04005161 RID: 20833
		Button16,
		// Token: 0x04005162 RID: 20834
		Button17,
		// Token: 0x04005163 RID: 20835
		Button18,
		// Token: 0x04005164 RID: 20836
		Button19,
		// Token: 0x04005165 RID: 20837
		Button20,
		// Token: 0x04005166 RID: 20838
		Button21,
		// Token: 0x04005167 RID: 20839
		Button22,
		// Token: 0x04005168 RID: 20840
		Button23,
		// Token: 0x04005169 RID: 20841
		Button24,
		// Token: 0x0400516A RID: 20842
		Button25,
		// Token: 0x0400516B RID: 20843
		Button26,
		// Token: 0x0400516C RID: 20844
		Button27,
		// Token: 0x0400516D RID: 20845
		Button28,
		// Token: 0x0400516E RID: 20846
		Button29,
		// Token: 0x0400516F RID: 20847
		Count
	}
}
