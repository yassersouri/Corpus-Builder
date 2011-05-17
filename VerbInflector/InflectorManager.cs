using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class InflectorManager
	{
		public static List<string> GetInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			switch (inflection.TenseForm)
			{
				case TenseFormationType.AMR:
					lstInflections = GetAmrInflections(inflection);
					break;
				case TenseFormationType.GOZASHTEH_ESTEMRAARI:
					lstInflections = GetGozashtehEstemrariInflections(inflection);
					break;
				case TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI:
					lstInflections = GetGozashtehNaghliEstemraiSadehInflections(inflection);
					break;
				case TenseFormationType.GOZASHTEH_NAGHLI_SADEH:
					lstInflections = GetGozashtehNaghliSadehInflections(inflection);
					break;
				case TenseFormationType.GOZASHTEH_SADEH:
					lstInflections = GetGozashtehSadehInflections(inflection);
					break;
				case TenseFormationType.HAAL_ELTEZAMI:
					lstInflections = GetHaalEltezamiInflections(inflection);
					break;
				case TenseFormationType.HAAL_SAADEH:
					lstInflections = GetHaalSaadehInflections(inflection);
					break;
				case TenseFormationType.HAAL_SAADEH_EKHBARI:
					lstInflections = GetHaalSaadehEkhbaariInflections(inflection);
					break;
				case TenseFormationType.PAYEH_MAFOOLI:
					lstInflections = GetPayehFelInflections(inflection);
					break;
			}
			return lstInflections;
		}
		private static List<string> GetGozashtehNaghliSadehInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbInflection = new VerbInflection(inflection.VerbStem, ZamirPeyvastehType.ZamirPeyvasteh_NONE,
													ShakhsType.Shakhs_NONE, TenseFormationType.PAYEH_MAFOOLI,
													inflection.Positivity);
			var tempLst = GetPayehFelInflections(verbInflection);
			string fel = tempLst[0];
			switch (inflection.Shakhs)
			{
				case ShakhsType.SEVVOMSHAKHS_JAM:
					fel += "‌اند";
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					fel += "‌ای";
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					fel += "‌اید";
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					fel += "‌ام";
					break;
				case ShakhsType.AVALSHAKHS_JAM:
					fel += "‌ایم";
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(fel, inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetGozashtehNaghliEstemraiSadehInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder = new StringBuilder();
			verbBuilder.Append(inflection.VerbStem.Pishvand);
			switch (inflection.Positivity)
			{
				case TensePositivity.POSITIVE:
					verbBuilder.Append("می‌");
					break;
				case TensePositivity.NEGATIVE:
					verbBuilder.Append("نمی‌");
					break;
			}
			var verb = new Verb("", inflection.VerbStem.HastehMazi,
								 inflection.VerbStem.HastehMozareh, "",
								 "", inflection.VerbStem.Transitivity, VerbType.SADEH,
								 inflection.VerbStem.AmrShodani, inflection.VerbStem.HastehMozarehConsonantVowelEndStem, inflection.VerbStem.HastehMaziVowelStart, inflection.VerbStem.HastehMozarehVowelStart);
			var verbInflection = new VerbInflection(verb, ZamirPeyvastehType.ZamirPeyvasteh_NONE,
													ShakhsType.Shakhs_NONE, TenseFormationType.PAYEH_MAFOOLI,
													TensePositivity.POSITIVE);
			var tempLst = GetPayehFelInflections(verbInflection);
			verbBuilder.Append(tempLst[0]);
			switch (inflection.Shakhs)
			{
				case ShakhsType.SEVVOMSHAKHS_JAM:
					verbBuilder.Append("‌اند");
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("‌ای");
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					verbBuilder.Append("‌اید");
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					verbBuilder.Append("‌ام");
					break;
				case ShakhsType.AVALSHAKHS_JAM:
					verbBuilder.Append("‌ایم");
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetGozashtehEstemrariInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder = new StringBuilder();
			verbBuilder.Append(inflection.VerbStem.Pishvand);
			switch (inflection.Positivity)
			{
				case TensePositivity.POSITIVE:
					verbBuilder.Append("می‌" + inflection.VerbStem.HastehMazi);
					break;
				case TensePositivity.NEGATIVE:
					verbBuilder.Append("نمی‌" + inflection.VerbStem.HastehMazi);
					break;
			}
			if (inflection.VerbStem.HastehMazi.EndsWith("آ"))
			{
				verbBuilder.Remove(verbBuilder.Length - 1, 1);
				verbBuilder.Append("ی");
			}
			else if (inflection.VerbStem.HastehMazi.EndsWith("ا") || inflection.VerbStem.HastehMazi.EndsWith("و"))
			{
				verbBuilder.Append("ی");
			}
			switch (inflection.Shakhs)
			{
				case ShakhsType.AVALSHAKHS_JAM:
					verbBuilder.Append("یم");
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					verbBuilder.Append("م");
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					verbBuilder.Append("ید");
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("ی");
					break;
				case ShakhsType.SEVVOMSHAKHS_JAM:
					verbBuilder.Append("ند");
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetGozashtehSadehInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder = new StringBuilder();
			verbBuilder.Append(inflection.VerbStem.Pishvand);
			if (inflection.Positivity == TensePositivity.NEGATIVE)
			{
				verbBuilder.Append("ن");
			}

			if (inflection.VerbStem.HastehMaziVowelStart == "A" && inflection.Positivity == TensePositivity.NEGATIVE)
			{
				if (!inflection.VerbStem.HastehMazi.StartsWith("آ"))
					verbBuilder.Append("ی");
				else
					verbBuilder.Append("یا");
				verbBuilder.Append(inflection.VerbStem.HastehMazi.Remove(0, 1));
			}

			else
			{
				verbBuilder.Append(inflection.VerbStem.HastehMazi);
			}

			if (inflection.VerbStem.HastehMazi.EndsWith("آ"))
			{
				verbBuilder.Remove(verbBuilder.Length - 1, 1);
				verbBuilder.Append("ی");
			}
			else if (inflection.VerbStem.HastehMazi.EndsWith("ا") || inflection.VerbStem.HastehMazi.EndsWith("و"))
			{
				verbBuilder.Append("ی");
			}
			switch (inflection.Shakhs)
			{
				case ShakhsType.AVALSHAKHS_JAM:
					verbBuilder.Append("یم");
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					verbBuilder.Append("م");
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					verbBuilder.Append("ید");
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("ی");
					break;
				case ShakhsType.SEVVOMSHAKHS_JAM:
					verbBuilder.Append("ند");
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetHaalSaadehEkhbaariInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder = new StringBuilder();
			verbBuilder.Append(inflection.VerbStem.Pishvand);
			if (inflection.VerbStem.HastehMozareh == "است")
			{
				verbBuilder.Append(inflection.VerbStem.HastehMozareh);
				lstInflections.Add(verbBuilder.ToString());
				return lstInflections;
			}
			if (inflection.VerbStem.HastehMozareh == "هست")
			{
				switch (inflection.Positivity)
				{
					case TensePositivity.POSITIVE:
						verbBuilder.Append(inflection.VerbStem.HastehMozareh);
						break;
					case TensePositivity.NEGATIVE:
						verbBuilder.Append("نیست");
						break;
				}
			}
			else
			{
				switch (inflection.Positivity)
				{
					case TensePositivity.POSITIVE:
						verbBuilder.Append("می‌" + inflection.VerbStem.HastehMozareh);
						break;
					case TensePositivity.NEGATIVE:
						verbBuilder.Append("نمی‌" + inflection.VerbStem.HastehMozareh);
						break;
				}
			}
			if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem == "A")
			{
				if (inflection.VerbStem.HastehMozareh.Length > 1)
				{
					verbBuilder.Remove(verbBuilder.Length - 1, 1);
					verbBuilder.Append("ای");
				}
				else
					verbBuilder.Append("ی");
			}
			else if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
			{
				verbBuilder.Append("ی");
			}
			switch (inflection.Shakhs)
			{
				case ShakhsType.AVALSHAKHS_JAM:
					verbBuilder.Append("یم");
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					verbBuilder.Append("م");
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					verbBuilder.Append("ید");
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("ی");
					break;
				case ShakhsType.SEVVOMSHAKHS_JAM:
					verbBuilder.Append("ند");
					break;
				case ShakhsType.SEVVOMSHAKHS_MOFRAD:
					if (inflection.VerbStem.HastehMozareh != "باید" && inflection.VerbStem.HastehMozareh != "هست")
						verbBuilder.Append("د");
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetHaalEltezamiInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder = new StringBuilder();
			var verbBuilder2 = new StringBuilder();
			verbBuilder.Append(inflection.VerbStem.Pishvand);
			verbBuilder2.Append(inflection.VerbStem.Pishvand);

			switch (inflection.Positivity)
			{
				case TensePositivity.POSITIVE:
					if (!(inflection.VerbStem.HastehMozareh == "باشد" || inflection.VerbStem.HastehMozareh == "باید"))
						verbBuilder.Append("ب");
					break;
				case TensePositivity.NEGATIVE:
					verbBuilder.Append("ن");
					break;
			}
			if (inflection.VerbStem.HastehMozarehVowelStart == "A")
			{
				if (inflection.VerbStem.HastehMozareh.StartsWith("آ"))
				{
					verbBuilder.Append("یا");
					verbBuilder.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
				}
				else
				{
					verbBuilder.Append("ی");
					verbBuilder.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
				}
			}
			else
			{
				verbBuilder.Append(inflection.VerbStem.HastehMozareh);
				verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
			}

			if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem == "A")
			{
				if (verbBuilder.Length > 1)
				{
					verbBuilder.Remove(verbBuilder.Length - 1, 1);
					verbBuilder.Append("ای");
					if (inflection.VerbStem.HastehMozareh.Length > 1)
					{
						verbBuilder2.Remove(verbBuilder2.Length - 1, 1);
						verbBuilder2.Append("ای");
					}
					else
					{
						verbBuilder2.Append("ی");
					}
				}
				else
				{
					verbBuilder.Append("ی");
					verbBuilder2.Append("ی");
				}
			}
			else if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
			{
				if (inflection.VerbStem.HastehMazi != "رفت" && inflection.VerbStem.HastehMazi != "شد")
				{
					verbBuilder.Append("ی");
					verbBuilder2.Append("ی");
				}
			}
			switch (inflection.Shakhs)
			{
				case ShakhsType.AVALSHAKHS_JAM:
					verbBuilder.Append("یم");
					verbBuilder2.Append("یم");
					break;
				case ShakhsType.AVALSHAKHS_MOFRAD:
					verbBuilder.Append("م");
					verbBuilder2.Append("م");
					break;
				case ShakhsType.DOVVOMSHAKHS_JAM:
					verbBuilder.Append("ید");
					verbBuilder2.Append("ید");
					break;
				case ShakhsType.DOVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("ی");
					verbBuilder2.Append("ی");
					break;
				case ShakhsType.SEVVOMSHAKHS_JAM:
					verbBuilder.Append("ند");
					verbBuilder2.Append("ند");
					break;
				case ShakhsType.SEVVOMSHAKHS_MOFRAD:
					verbBuilder.Append("د");
					verbBuilder2.Append("د");
					break;
			}
			lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			if (inflection.Positivity == TensePositivity.POSITIVE && (inflection.VerbStem.HastehMozareh.Length > 2 || inflection.VerbStem.Type == VerbType.PISHVANDI))
				lstInflections.Add(AddZamirPeyvasteh(verbBuilder2.ToString(), inflection.ZamirPeyvasteh));
			return lstInflections;
		}
		private static List<string> GetHaalSaadehInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			if (inflection.VerbStem.HastehMazi == "خواست" || inflection.VerbStem.HastehMazi == "خواست" || inflection.VerbStem.HastehMazi == "داشت" || inflection.VerbStem.HastehMazi == "بایست" || inflection.VerbStem.Type == VerbType.AYANDEH_PISHVANDI)
			{
				var verbBuilder = new StringBuilder();
				verbBuilder.Append(inflection.VerbStem.Pishvand);
				if (inflection.Positivity == TensePositivity.NEGATIVE)
				{
					verbBuilder.Append("ن");
				}
				verbBuilder.Append(inflection.VerbStem.HastehMozareh);

				if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem == "A")
				{
					if (verbBuilder.Length > 1)
					{
						verbBuilder.Remove(verbBuilder.Length - 1, 1);
						verbBuilder.Append("ای");
					}
					else
						verbBuilder.Append("ی");
				}
				else if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
				{
					verbBuilder.Append("ی");
				}
				switch (inflection.Shakhs)
				{
					case ShakhsType.AVALSHAKHS_JAM:
						verbBuilder.Append("یم");
						break;
					case ShakhsType.AVALSHAKHS_MOFRAD:
						verbBuilder.Append("م");
						break;
					case ShakhsType.DOVVOMSHAKHS_JAM:
						verbBuilder.Append("ید");
						break;
					case ShakhsType.DOVVOMSHAKHS_MOFRAD:
						verbBuilder.Append("ی");
						break;
					case ShakhsType.SEVVOMSHAKHS_JAM:
						verbBuilder.Append("ند");
						break;
					case ShakhsType.SEVVOMSHAKHS_MOFRAD:
						if (inflection.VerbStem.HastehMazi != "بایست")
							verbBuilder.Append("د");
						break;
				}
				lstInflections.Add(AddZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
			}
			return lstInflections;
		}
		private static List<string> GetAmrInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			var verbBuilder1 = new StringBuilder();
			var verbBuilder2 = new StringBuilder();
			var verbBuilder3 = new StringBuilder();
			if (inflection.VerbStem.Pishvand != "")
			{
				verbBuilder1.Append(inflection.VerbStem.Pishvand);
				if (inflection.Positivity == TensePositivity.NEGATIVE)
				{
					verbBuilder3.Append(inflection.VerbStem.Pishvand);
				}
				if (inflection.Positivity == TensePositivity.POSITIVE)
				{
					verbBuilder2.Append(inflection.VerbStem.Pishvand);
				}
			}
			switch (inflection.Positivity)
			{
				case TensePositivity.POSITIVE:
					if (!(inflection.VerbStem.HastehMozareh == "باش" || inflection.VerbStem.HastehMozareh == "باید"))
						verbBuilder1.Append("ب");
					break;
				case TensePositivity.NEGATIVE:
					verbBuilder1.Append("ن");
					verbBuilder3.Append("م");
					break;
			}
			if (inflection.VerbStem.HastehMozarehVowelStart == "A")
			{
				if (inflection.VerbStem.HastehMozareh.StartsWith("آ"))
				{
					verbBuilder1.Append("یا");
					verbBuilder1.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					if (inflection.Positivity == TensePositivity.NEGATIVE)
					{
						verbBuilder3.Append("یا");
						verbBuilder3.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					}
					if (inflection.Positivity == TensePositivity.POSITIVE)
					{
						verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
					}
				}
				else
				{
					verbBuilder1.Append("ی");
					verbBuilder1.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					if (inflection.Positivity == TensePositivity.NEGATIVE)
					{
						verbBuilder3.Append("ی");
						verbBuilder3.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
					}
					if (inflection.Positivity == TensePositivity.POSITIVE)
					{
						verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
					}
				}
			}
			else
			{
				verbBuilder1.Append(inflection.VerbStem.HastehMozareh);
				if (inflection.Positivity == TensePositivity.POSITIVE/* && inflection.VerbStem.Type==VerbType.PISHVANDI*/)
				{
					verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
				}
				if (inflection.Positivity == TensePositivity.NEGATIVE)
				{
					verbBuilder3.Append(inflection.VerbStem.HastehMozareh);
				}
			}

			switch (inflection.Shakhs)
			{
				case ShakhsType.DOVVOMSHAKHS_JAM:
					if (inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
					{
						verbBuilder1.Append("یید");
						if (inflection.Positivity == TensePositivity.NEGATIVE)
						{
							verbBuilder3.Append("یید");
						}
						if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Type == VerbType.PISHVANDI)
						{
							verbBuilder2.Append("یید");
						}
					}
					else
					{
						verbBuilder1.Append("ید");
						if (inflection.Positivity == TensePositivity.NEGATIVE)
						{
							verbBuilder3.Append("ید");
						}
						if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Type == VerbType.PISHVANDI)
						{
							verbBuilder2.Append("ید");
						}
					}
					break;
			}
			if (inflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
			{
				if (!(inflection.VerbStem.HastehMozareh == "نه" && inflection.Positivity == TensePositivity.NEGATIVE))
					lstInflections.Add(verbBuilder1.ToString());
				if (inflection.Positivity == TensePositivity.NEGATIVE)
					lstInflections.Add(verbBuilder3.ToString());
				if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Type == VerbType.PISHVANDI)
					lstInflections.Add(verbBuilder2.ToString());
				if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD && inflection.Positivity == TensePositivity.POSITIVE &&
					inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
				{
					lstInflections.Add(verbBuilder2.Append("ی").ToString());
				}
			}
			else
			{
				if (!(inflection.VerbStem.HastehMozareh == "نه" && inflection.Positivity == TensePositivity.NEGATIVE))
					lstInflections.Add(AddZamirPeyvasteh(verbBuilder1.ToString(), inflection.ZamirPeyvasteh));
				if (inflection.Positivity == TensePositivity.NEGATIVE)
					lstInflections.Add(AddZamirPeyvasteh(verbBuilder3.ToString(), inflection.ZamirPeyvasteh));
				if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Positivity == TensePositivity.POSITIVE)
					lstInflections.Add(AddZamirPeyvasteh(verbBuilder2.ToString(), inflection.ZamirPeyvasteh));
				if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD && inflection.Positivity == TensePositivity.POSITIVE &&
					inflection.VerbStem.HastehMozarehConsonantVowelEndStem != "?")
				{
					lstInflections.Add(AddZamirPeyvasteh(verbBuilder2.Append("ی").ToString(), inflection.ZamirPeyvasteh));
				}
			}
			return lstInflections;
		}
		private static List<string> GetPayehFelInflections(VerbInflection inflection)
		{
			var lstInflections = new List<string>();
			switch (inflection.Positivity)
			{
				case TensePositivity.POSITIVE:
					lstInflections.Add(inflection.VerbStem.Pishvand + inflection.VerbStem.HastehMazi + "ه");
					break;
				case TensePositivity.NEGATIVE:
					if (inflection.VerbStem.HastehMaziVowelStart == "A" && inflection.Positivity == TensePositivity.NEGATIVE)
					{
						var verbBuilder = new StringBuilder();
						verbBuilder.Append(inflection.VerbStem.Pishvand + "ن");
						if (!inflection.VerbStem.HastehMazi.StartsWith("آ"))
							verbBuilder.Append("ی");
						else
							verbBuilder.Append("یا");
						verbBuilder.Append(inflection.VerbStem.HastehMazi.Remove(0, 1));
						verbBuilder.Append("ه");
						lstInflections.Add(verbBuilder.ToString());

					}
					else
					{
						lstInflections.Add(inflection.VerbStem.Pishvand + "ن" + inflection.VerbStem.HastehMazi + "ه");
					}
					break;
			}
			return lstInflections;
		}
		private static string AddZamirPeyvasteh(string verb, ZamirPeyvastehType zamirPeyvastehType)
		{
			string inflectedVerb = verb;
			switch (zamirPeyvastehType)
			{
				case ZamirPeyvastehType.SEVVOMSHAKHS_MOFRAD:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یش";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌اش";
					}
					else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
					{
						inflectedVerb += "‌اش";
					}
					else if (verb.EndsWith("‌ای"))
					{
						inflectedVerb += "‌اش";
					}
					else
					{
						inflectedVerb += "ش";
					}
					break;
				case ZamirPeyvastehType.SEVVOMSHAKHS_JAM:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یشان";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌شان";
					}
					else
					{
						inflectedVerb += "شان";
					}
					break;
				case ZamirPeyvastehType.DOVVOMSHAKHS_JAM:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یتان";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌تان";
					}
					else
					{
						inflectedVerb += "تان";
					} break;
				case ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یت";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌ات";
					}
					else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
					{
						inflectedVerb += "‌ات";
					}
					else if (verb.EndsWith("‌ای"))
					{
						inflectedVerb += "‌ات";
					}
					else
					{
						inflectedVerb += "ت";
					}
					break;
				case ZamirPeyvastehType.AVALSHAKHS_JAM:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یمان";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌مان";
					}
					else
					{
						inflectedVerb += "مان";
					}
					break;
				case ZamirPeyvastehType.AVALSHAKHS_MOFRAD:
					if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
					{
						inflectedVerb += "یم";
					}
					else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
					{
						inflectedVerb += "‌ام";
					}
					else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
					{
						inflectedVerb += "‌ام";
					}
					else if (verb.EndsWith("‌ای"))
					{
						inflectedVerb += "‌ام";
					}
					else
					{
						inflectedVerb += "م";
					}
					break;
			}
			return inflectedVerb;
		}
	}
}
