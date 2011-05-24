using System;

namespace VerbInflector
{
	public class VerbInflection : IComparable
	{
		public Verb VerbStem;
		public ZamirPeyvastehType ZamirPeyvasteh;
		public ShakhsType Shakhs;
		public TenseFormationType TenseForm;
		public TensePositivity Positivity;
		public TensePassivity Passivity;

		public VerbInflection(Verb vrb, ZamirPeyvastehType zamir, ShakhsType shakhstype, TenseFormationType tenseFormationType, TensePositivity positivity)
		{
			VerbStem = vrb;
			ZamirPeyvasteh = zamir;
			Shakhs = shakhstype;
			TenseForm = tenseFormationType;
			Positivity = positivity;
			Passivity = TensePassivity.ACTIVE;
		}
		public VerbInflection(Verb vrb, ZamirPeyvastehType zamir, ShakhsType shakhstype, TenseFormationType tenseFormationType, TensePositivity positivity, TensePassivity passivity)
		{
			VerbStem = vrb;
			ZamirPeyvasteh = zamir;
			Shakhs = shakhstype;
			TenseForm = tenseFormationType;
			Positivity = positivity;
			Passivity = passivity;
		}
		public bool IsPayehFelMasdari()
		{
			if (Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD && TenseForm == TenseFormationType.GOZASHTEH_SADEH)//TODO 
				return true;
			return false;
		}
		private bool IsNegativeValid()
		{
			return true;
		}
		private bool IsShakhsValid()
		{
			if (TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI && VerbStem.HastehMozareh == "است" && Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
				return false;
			if (TenseForm == TenseFormationType.PAYEH_MAFOOLI && Shakhs != ShakhsType.Shakhs_NONE)
				return false;
			if (TenseForm != TenseFormationType.PAYEH_MAFOOLI && Shakhs == ShakhsType.Shakhs_NONE)
				return false;
			if (TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH && Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
				return false;
			if (TenseForm == TenseFormationType.AMR &&
				!(Shakhs == ShakhsType.DOVVOMSHAKHS_JAM || Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD))
				return false;
			return true;
		}
		private bool IsZamirPeyvastehValid()
		{
			if (TenseForm == TenseFormationType.AMR && (ZamirPeyvasteh == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD || ZamirPeyvasteh == ZamirPeyvastehType.DOVVOMSHAKHS_JAM || ZamirPeyvasteh == ZamirPeyvastehType.AVALSHAKHS_JAM || ZamirPeyvasteh == ZamirPeyvastehType.AVALSHAKHS_MOFRAD))
				return false;
			if (isEqualShaksZamir(ZamirPeyvasteh, Shakhs))
				return false;
			if (ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE && TenseForm == TenseFormationType.PAYEH_MAFOOLI)
				return true;
			if (ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
				return true;
			return VerbStem.IsZamirPeyvastehValid() && TenseForm != TenseFormationType.PAYEH_MAFOOLI;
		}
		private bool isEqualShaksZamir(ZamirPeyvastehType zamirPeyvastehType, ShakhsType shakhsType)
		{
			if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.DOVVOMSHAKHS_MOFRAD)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_JAM && shakhsType == ShakhsType.DOVVOMSHAKHS_JAM)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_MOFRAD && shakhsType == ShakhsType.AVALSHAKHS_MOFRAD)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_JAM && shakhsType == ShakhsType.AVALSHAKHS_JAM)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_JAM && shakhsType == ShakhsType.DOVVOMSHAKHS_MOFRAD)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.DOVVOMSHAKHS_JAM)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_JAM && shakhsType == ShakhsType.AVALSHAKHS_MOFRAD)
				return true;
			if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_MOFRAD && shakhsType == ShakhsType.AVALSHAKHS_JAM)
				return true;
			return false;
		}
		public override string ToString()
		{
			return VerbStem.ToString() + "\t" + TenseForm + "\t" + Shakhs + "\t" + ZamirPeyvasteh + "\t" + Positivity;
		}
		public string AbstarctString()
		{
			return TenseForm + "\t" + ZamirPeyvasteh;
		}
		public bool IsValid()
		{
			if ((TenseForm == TenseFormationType.HAAL_ELTEZAMI || TenseForm == TenseFormationType.AMR || TenseForm == TenseFormationType.PAYEH_MAFOOLI) && (VerbStem.HastehMozareh == "هست" || VerbStem.HastehMozareh == "است"))
				return false;
			if (Positivity == TensePositivity.NEGATIVE && VerbStem.HastehMozareh == "است")
				return false;
			if (TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI || TenseForm == TenseFormationType.HAAL_ELTEZAMI || TenseForm == TenseFormationType.AMR)
				if (string.IsNullOrEmpty(VerbStem.HastehMozareh))
					return false;
			if (TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI || TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI || TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH || TenseForm == TenseFormationType.GOZASHTEH_SADEH || TenseForm == TenseFormationType.PAYEH_MAFOOLI)
				if (string.IsNullOrEmpty(VerbStem.HastehMazi))
					return false;
			if (TenseForm == TenseFormationType.AMR && VerbStem.AmrShodani == false)
				return false;
			if (TenseForm != TenseFormationType.HAAL_SAADEH && VerbStem.Type == VerbType.AYANDEH_PISHVANDI)
				return false;
			if (VerbStem.HastehMazi == "بایست")
			{
				if (TenseForm == TenseFormationType.HAAL_SAADEH && Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
					return true;
				if (TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI && Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
					return true;
				if (TenseForm == TenseFormationType.GOZASHTEH_SADEH && (Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD || Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD))
					return true;
				if (TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI && (Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD || Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD))
					return true;
				return false;
			}
			return (IsZamirPeyvastehValid() && IsShakhsValid() && IsNegativeValid());
		}
		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (this.Equals(obj))
				return 0;
			else
				return this.GetHashCode().CompareTo(obj.GetHashCode());
		}
		public override bool Equals(object obj)
		{
			if (!(obj is VerbInflection))
				return false;
			var inflection = (VerbInflection)obj;
			if (inflection.VerbStem.Equals(VerbStem) && inflection.ZamirPeyvasteh == ZamirPeyvasteh && inflection.Shakhs == Shakhs && inflection.TenseForm == TenseForm && inflection.Positivity == Positivity)
				return true;
			return false;
		}
		public override int GetHashCode()
		{
			return VerbStem.GetHashCode() + ZamirPeyvasteh.GetHashCode() + Shakhs.GetHashCode() +
				   TenseForm.GetHashCode() + Positivity.GetHashCode();
		}
		#endregion
	}

	[Flags]
	public enum ZamirPeyvastehType
	{
		ZamirPeyvasteh_NONE = 1,
		AVALSHAKHS_MOFRAD = 2,
		DOVVOMSHAKHS_MOFRAD = 4,
		SEVVOMSHAKHS_MOFRAD = 8,
		AVALSHAKHS_JAM = 16,
		DOVVOMSHAKHS_JAM = 32,
		SEVVOMSHAKHS_JAM = 64
	}
	[Flags]
	public enum ShakhsType
	{
		Shakhs_NONE = 1,
		AVALSHAKHS_MOFRAD = 2,
		DOVVOMSHAKHS_MOFRAD = 4,
		SEVVOMSHAKHS_MOFRAD = 8,
		AVALSHAKHS_JAM = 16,
		DOVVOMSHAKHS_JAM = 32,
		SEVVOMSHAKHS_JAM = 64
	}
	[Flags]
	public enum TenseFormationType
	{
		TenseFormationType_NONE = 0,
		HAAL_SAADEH_EKHBARI = 1,
		HAAL_ELTEZAMI = 2,
		HAAL_SAADEH = 4,
		AMR = 8,
		GOZASHTEH_SADEH = 32,
		GOZASHTEH_ESTEMRAARI = 64,
		GOZASHTEH_NAGHLI_SADEH = 128,
		GOZASHTEH_NAGHLI_ESTEMRAARI = 256,
		GOZASHTEH_BAEED = 512,
		GOZASHTEH_ELTEZAMI = 1024,
		PAYEH_MAFOOLI = 2048,
		AAYANDEH = 4096,
		GOZASHTEH_ABAD = 8192
	}
	[Flags]
	public enum TensePositivity
	{
		POSITIVE = 1,
		NEGATIVE = 2
	}
	[Flags]
	public enum TensePassivity
	{
		ACTIVE = 1,
		PASSIVE = 2
	}

	[Flags]
	public enum NumberType
	{
		INVALID = 0,
		SINGULAR = 1,
		PLURAL = 2
	}
}
