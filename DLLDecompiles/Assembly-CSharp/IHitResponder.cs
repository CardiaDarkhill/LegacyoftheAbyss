using System;

// Token: 0x0200002F RID: 47
public interface IHitResponder
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600016D RID: 365 RVA: 0x0000836D File Offset: 0x0000656D
	bool HitRecurseUpwards
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600016E RID: 366
	IHitResponder.HitResponse Hit(HitInstance damageInstance);

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600016F RID: 367 RVA: 0x00008370 File Offset: 0x00006570
	int HitPriority
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x020013C7 RID: 5063
	public enum Response
	{
		// Token: 0x040080B2 RID: 32946
		None,
		// Token: 0x040080B3 RID: 32947
		GenericHit,
		// Token: 0x040080B4 RID: 32948
		DamageEnemy,
		// Token: 0x040080B5 RID: 32949
		Invincible
	}

	// Token: 0x020013C8 RID: 5064
	public struct HitResponse : IEquatable<IHitResponder.HitResponse>
	{
		// Token: 0x06008175 RID: 33141 RVA: 0x002617D1 File Offset: 0x0025F9D1
		public HitResponse(IHitResponder.Response response, bool consumeCharges = true)
		{
			this.response = response;
			this.consumeCharges = consumeCharges;
		}

		// Token: 0x06008176 RID: 33142 RVA: 0x002617E4 File Offset: 0x0025F9E4
		public static implicit operator IHitResponder.HitResponse(IHitResponder.Response response)
		{
			return new IHitResponder.HitResponse
			{
				response = response
			};
		}

		// Token: 0x06008177 RID: 33143 RVA: 0x00261802 File Offset: 0x0025FA02
		public static implicit operator IHitResponder.Response(IHitResponder.HitResponse hitResponse)
		{
			return hitResponse.response;
		}

		// Token: 0x06008178 RID: 33144 RVA: 0x0026180A File Offset: 0x0025FA0A
		public bool Equals(IHitResponder.HitResponse other)
		{
			return this.response == other.response && this.consumeCharges == other.consumeCharges;
		}

		// Token: 0x06008179 RID: 33145 RVA: 0x0026182C File Offset: 0x0025FA2C
		public override bool Equals(object obj)
		{
			if (obj is IHitResponder.HitResponse)
			{
				IHitResponder.HitResponse other = (IHitResponder.HitResponse)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x0600817A RID: 33146 RVA: 0x00261851 File Offset: 0x0025FA51
		public override int GetHashCode()
		{
			return HashCode.Combine<int, bool>((int)this.response, this.consumeCharges);
		}

		// Token: 0x040080B6 RID: 32950
		public IHitResponder.Response response;

		// Token: 0x040080B7 RID: 32951
		public bool consumeCharges;

		// Token: 0x040080B8 RID: 32952
		public static IHitResponder.HitResponse Default = new IHitResponder.HitResponse
		{
			response = IHitResponder.Response.None,
			consumeCharges = true
		};
	}
}
