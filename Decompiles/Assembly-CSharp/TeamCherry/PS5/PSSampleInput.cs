using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A7 RID: 2215
	public class PSSampleInput : PSInputBase
	{
		// Token: 0x06004CA6 RID: 19622 RVA: 0x00168484 File Offset: 0x00166684
		public override void Init(GamePad gamePad)
		{
			int num = gamePad.playerId + 1;
			this.optionsBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button7", true);
			this.leftStickHorizontalAxis = "leftstick" + num.ToString() + "horizontal";
			this.leftStickVerticalAxis = "leftstick" + num.ToString() + "vertical";
			this.rightStickHorizontalAxis = "rightstick" + num.ToString() + "horizontal";
			this.rightStickVerticalAxis = "rightstick" + num.ToString() + "vertical";
			this.CrossBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button0", true);
			this.CircleBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button1", true);
			this.SquareBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button2", true);
			this.TriangleBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button3", true);
			this.L1BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button4", true);
			this.R1BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button5", true);
			this.L3BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button8", true);
			this.R3BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num.ToString() + "Button9", true);
			this.DPadRightAxis = "dpad" + num.ToString() + "_horizontal";
			this.DPadLeftAxis = "dpad" + num.ToString() + "_horizontal";
			this.DPadUpAxis = "dpad" + num.ToString() + "_vertical";
			this.DPadDownAxis = "dpad" + num.ToString() + "_vertical";
			this.L2Axis = "joystick" + num.ToString() + "_left_trigger";
			this.R2Axis = "joystick" + num.ToString() + "_left_trigger";
		}

		// Token: 0x06004CA7 RID: 19623 RVA: 0x0016876B File Offset: 0x0016696B
		public override Vector2 GetThumbStickLeft()
		{
			return new Vector2(Input.GetAxis(this.leftStickHorizontalAxis), Input.GetAxis(this.leftStickVerticalAxis));
		}

		// Token: 0x06004CA8 RID: 19624 RVA: 0x00168788 File Offset: 0x00166988
		public override Vector2 GetThumbStickRight()
		{
			return new Vector2(Input.GetAxis(this.rightStickHorizontalAxis), Input.GetAxis(this.rightStickVerticalAxis));
		}

		// Token: 0x06004CA9 RID: 19625 RVA: 0x001687A5 File Offset: 0x001669A5
		public override bool GetL3()
		{
			return Input.GetKey(this.L3BtnKeyCode);
		}

		// Token: 0x06004CAA RID: 19626 RVA: 0x001687B2 File Offset: 0x001669B2
		public override bool GetR3()
		{
			return Input.GetKey(this.R3BtnKeyCode);
		}

		// Token: 0x06004CAB RID: 19627 RVA: 0x001687BF File Offset: 0x001669BF
		public override bool GetOptions()
		{
			return Input.GetKey(this.optionsBtnKeyCode);
		}

		// Token: 0x06004CAC RID: 19628 RVA: 0x001687CC File Offset: 0x001669CC
		public override bool GetCross()
		{
			return Input.GetKey(this.CrossBtnKeyCode);
		}

		// Token: 0x06004CAD RID: 19629 RVA: 0x001687D9 File Offset: 0x001669D9
		public override bool GetCircle()
		{
			return Input.GetKey(this.CircleBtnKeyCode);
		}

		// Token: 0x06004CAE RID: 19630 RVA: 0x001687E6 File Offset: 0x001669E6
		public override bool GetSquare()
		{
			return Input.GetKey(this.SquareBtnKeyCode);
		}

		// Token: 0x06004CAF RID: 19631 RVA: 0x001687F3 File Offset: 0x001669F3
		public override bool GetTriangle()
		{
			return Input.GetKey(this.TriangleBtnKeyCode);
		}

		// Token: 0x06004CB0 RID: 19632 RVA: 0x00168800 File Offset: 0x00166A00
		public override bool GetDpadRight()
		{
			return Input.GetAxis(this.DPadRightAxis) > 0f;
		}

		// Token: 0x06004CB1 RID: 19633 RVA: 0x00168814 File Offset: 0x00166A14
		public override bool GetDpadLeft()
		{
			return Input.GetAxis(this.DPadLeftAxis) < 0f;
		}

		// Token: 0x06004CB2 RID: 19634 RVA: 0x00168828 File Offset: 0x00166A28
		public override bool GetDpadUp()
		{
			return Input.GetAxis(this.DPadUpAxis) > 0f;
		}

		// Token: 0x06004CB3 RID: 19635 RVA: 0x0016883C File Offset: 0x00166A3C
		public override bool GetDpadDown()
		{
			return Input.GetAxis(this.DPadDownAxis) < 0f;
		}

		// Token: 0x06004CB4 RID: 19636 RVA: 0x00168850 File Offset: 0x00166A50
		public override bool GetR1()
		{
			return Input.GetKey(this.R1BtnKeyCode);
		}

		// Token: 0x06004CB5 RID: 19637 RVA: 0x0016885D File Offset: 0x00166A5D
		public override bool GetR2()
		{
			return Input.GetAxis(this.R2Axis) != 0f;
		}

		// Token: 0x06004CB6 RID: 19638 RVA: 0x00168874 File Offset: 0x00166A74
		public override bool GetL1()
		{
			return Input.GetKey(this.L1BtnKeyCode);
		}

		// Token: 0x06004CB7 RID: 19639 RVA: 0x00168881 File Offset: 0x00166A81
		public override bool GetL2()
		{
			return Input.GetAxis(this.L2Axis) != 0f;
		}

		// Token: 0x06004CB8 RID: 19640 RVA: 0x00168898 File Offset: 0x00166A98
		public override bool TouchPadButton()
		{
			return false;
		}

		// Token: 0x04004DC0 RID: 19904
		private KeyCode optionsBtnKeyCode;

		// Token: 0x04004DC1 RID: 19905
		private string leftStickHorizontalAxis;

		// Token: 0x04004DC2 RID: 19906
		private string leftStickVerticalAxis;

		// Token: 0x04004DC3 RID: 19907
		private string rightStickHorizontalAxis;

		// Token: 0x04004DC4 RID: 19908
		private string rightStickVerticalAxis;

		// Token: 0x04004DC5 RID: 19909
		private KeyCode L1BtnKeyCode;

		// Token: 0x04004DC6 RID: 19910
		private KeyCode R1BtnKeyCode;

		// Token: 0x04004DC7 RID: 19911
		private string L2Axis;

		// Token: 0x04004DC8 RID: 19912
		private string R2Axis;

		// Token: 0x04004DC9 RID: 19913
		private KeyCode L3BtnKeyCode;

		// Token: 0x04004DCA RID: 19914
		private KeyCode R3BtnKeyCode;

		// Token: 0x04004DCB RID: 19915
		private KeyCode CrossBtnKeyCode;

		// Token: 0x04004DCC RID: 19916
		private KeyCode CircleBtnKeyCode;

		// Token: 0x04004DCD RID: 19917
		private KeyCode SquareBtnKeyCode;

		// Token: 0x04004DCE RID: 19918
		private KeyCode TriangleBtnKeyCode;

		// Token: 0x04004DCF RID: 19919
		private string DPadRightAxis;

		// Token: 0x04004DD0 RID: 19920
		private string DPadLeftAxis;

		// Token: 0x04004DD1 RID: 19921
		private string DPadUpAxis;

		// Token: 0x04004DD2 RID: 19922
		private string DPadDownAxis;
	}
}
