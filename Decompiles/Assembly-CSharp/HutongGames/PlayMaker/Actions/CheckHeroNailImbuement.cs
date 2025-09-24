using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001270 RID: 4720
	public sealed class CheckHeroNailImbuement : FsmStateAction
	{
		// Token: 0x06007C6B RID: 31851 RVA: 0x002530A9 File Offset: 0x002512A9
		public override void Reset()
		{
			this.checkType = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.currentElementResult = null;
			this.storeResult = null;
			this.everyFrame = null;
		}

		// Token: 0x06007C6C RID: 31852 RVA: 0x002530D5 File Offset: 0x002512D5
		public override void OnEnter()
		{
			if (this.DoCheck() || !this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007C6D RID: 31853 RVA: 0x002530F2 File Offset: 0x002512F2
		public override void OnUpdate()
		{
			if (this.DoCheck())
			{
				base.Finish();
			}
		}

		// Token: 0x06007C6E RID: 31854 RVA: 0x00253104 File Offset: 0x00251304
		private bool DoCheck()
		{
			HeroController instance = HeroController.instance;
			NailElements nailElements = NailElements.None;
			bool flag = false;
			bool result = false;
			if (instance != null)
			{
				HeroNailImbuement nailImbuement = instance.NailImbuement;
				if (nailImbuement)
				{
					nailElements = nailImbuement.CurrentElement;
					switch ((CheckHeroNailImbuement.CheckType)this.checkType.Value)
					{
					case CheckHeroNailImbuement.CheckType.Any:
						flag = (nailElements > NailElements.None);
						break;
					case CheckHeroNailImbuement.CheckType.Fire:
						flag = (nailElements == NailElements.Fire);
						break;
					case CheckHeroNailImbuement.CheckType.Poison:
						flag = (nailElements == NailElements.Poison);
						break;
					default:
						this.storeResult.Value = false;
						result = true;
						break;
					}
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
			this.currentElementResult.Value = nailElements;
			this.storeResult.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.trueEvent);
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			return result;
		}

		// Token: 0x04007C71 RID: 31857
		[ObjectType(typeof(CheckHeroNailImbuement.CheckType))]
		public FsmEnum checkType;

		// Token: 0x04007C72 RID: 31858
		public FsmEvent trueEvent;

		// Token: 0x04007C73 RID: 31859
		public FsmEvent falseEvent;

		// Token: 0x04007C74 RID: 31860
		[ObjectType(typeof(NailElements))]
		[UIHint(UIHint.Variable)]
		public FsmEnum currentElementResult;

		// Token: 0x04007C75 RID: 31861
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x04007C76 RID: 31862
		public FsmBool everyFrame;

		// Token: 0x02001BE7 RID: 7143
		[Serializable]
		private enum CheckType
		{
			// Token: 0x04009F83 RID: 40835
			Any,
			// Token: 0x04009F84 RID: 40836
			Fire,
			// Token: 0x04009F85 RID: 40837
			Poison
		}
	}
}
