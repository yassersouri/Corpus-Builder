using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class Verb : IComparable
	{
		public string HarfeEzafeh;
		public string Felyar;
		public string Pishvand;
		public string HastehMazi;
		public string HastehMozareh;
		public VerbTransitivity Transitivity;
		public VerbType Type;
		public bool AmrShodani;
		public string HastehMozarehConsonantVowelEndStem;
		public string HastehMaziVowelStart;
		public string HastehMozarehVowelStart;
		public Verb(string hz, string bonmazi, string bonmozareh, string psh, string flyar, VerbTransitivity trnst, VerbType type, bool amrshdn, string vowelEnd, string maziVowelStart, string mozarehVowelStart)
		{
			HarfeEzafeh = hz;
			Felyar = flyar;
			Pishvand = psh;
			HastehMazi = bonmazi;
			HastehMozareh = bonmozareh;
			Transitivity = trnst;
			Type = type;
			AmrShodani = amrshdn;
			HastehMozarehConsonantVowelEndStem = vowelEnd;
			HastehMaziVowelStart = maziVowelStart;
			HastehMozarehVowelStart = mozarehVowelStart;
		}
		public bool IsZamirPeyvastehValid()
		{
			return Transitivity != VerbTransitivity.NAGOZAR;
		}
		public override string ToString()
		{
			string verbStr;
			if (Pishvand != "")
				verbStr = HarfeEzafeh + " " + Felyar + " " + Pishvand + "#" + HastehMazi + "---" + HastehMozareh;
			else
				verbStr = HarfeEzafeh + " " + Felyar + " " + HastehMazi + "---" + HastehMozareh;

			verbStr = verbStr.Trim();
			verbStr += "\t" + Transitivity + "\t" + Type;
			return verbStr;
		}

		public string SimpleToString()
		{
			string verbStr;
			if (Pishvand != "")
				verbStr = HarfeEzafeh + " " + Felyar + " " + Pishvand + "#" + HastehMazi + "---" + HastehMozareh;
			else
				verbStr = HarfeEzafeh + " " + Felyar + " " + HastehMazi + "---" + HastehMozareh;
			verbStr = verbStr.Trim();
			return verbStr;
		}
		public Verb Clone()
		{
			var vrb = new Verb(HarfeEzafeh, HastehMazi, HastehMozareh, Pishvand, Felyar, Transitivity, Type, AmrShodani, HastehMozarehConsonantVowelEndStem, HastehMaziVowelStart, HastehMozarehVowelStart);
			return vrb;
		}
		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (this.Equals(obj))
				return 0;
			if (this.GetHashCode() > obj.GetHashCode())
				return 1;
			return -1;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Verb))
				return false;
			var verb = (Verb)obj;
			if (verb.HastehMazi == HastehMazi && verb.HastehMozareh == HastehMozareh && verb.Pishvand == Pishvand && verb.HarfeEzafeh == HarfeEzafeh && verb.Felyar == Felyar && verb.Transitivity == Transitivity && verb.AmrShodani == AmrShodani)
				return true;
			return false;
		}
		public override int GetHashCode()
		{
			return HarfeEzafeh.GetHashCode() + Felyar.GetHashCode() + Pishvand.GetHashCode() + HastehMazi.GetHashCode() +
				   HastehMozareh.GetHashCode() + Transitivity.GetHashCode() + Type.GetHashCode() +
				   AmrShodani.GetHashCode();
		}
		#endregion
	}
	[Flags]
	public enum VerbType
	{
		SADEH = 1,
		PISHVANDI = 2,
		MORAKKAB = 4,
		MORAKKABPISHVANDI = 8,
		MORAKKABHARFE_EZAFEH = 16,
		EBAARATFELI = 32,
		LAZEM_TAKFELI = 64,
		AYANDEH_PISHVANDI = 128
	}
	[Flags]
	public enum VerbTransitivity
	{
		GOZARA = 1,
		NAGOZAR = 2,
		DOVAJHI = 4
	}
}
