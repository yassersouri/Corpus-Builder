using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VerbInflector
{
	public class VerbList
	{
		public static Dictionary<string, List<VerbInflection>> VerbShapes { private set; get; }

		public static Dictionary<string, List<string>> VerbPishvandiDic { private set; get; }

		public static Dictionary<Verb, Dictionary<string, Dictionary<string, bool>>> CompoundVerbDic { private set; get; }

		public VerbList(string verbDicPath)
		{
			if (VerbShapes == null)
			{
				VerbPishvandiDic = new Dictionary<string, List<string>>();
				VerbShapes = new Dictionary<string, List<VerbInflection>>();
				CompoundVerbDic = new Dictionary<Verb, Dictionary<string, Dictionary<string, bool>>>();
				var verbs = new List<Verb>();
				string[] records = File.ReadAllText(verbDicPath).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				foreach (var record in records)
				{

					string[] fields = record.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					int vtype = int.Parse(fields[0]);

					if (vtype == 1 || vtype == 2)
					{
						var verbType = VerbType.SADEH;
						if (vtype == 2)
							verbType = VerbType.PISHVANDI;
						int trans = int.Parse(fields[1]);
						var transitivity = VerbTransitivity.GOZARA;
						if (trans == 0)
							transitivity = VerbTransitivity.NAGOZAR;
						else if (trans == 2)
							transitivity = VerbTransitivity.DOVAJHI;
						string pishvand = "";
						if (fields[5] != "-")
						{
							pishvand = fields[5];
						}
						Verb verb;
						bool amrShodani = true;
						if (fields[7] == "*")
							amrShodani = false;
						string vowelEnd = fields[8];
						string maziVowel = fields[9];
						string mozarehVowel = fields[10];
						if (fields[3] == "-")
							verb = new Verb("", fields[2], "", pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else if (fields[2] == "-")
							verb = new Verb("", "", fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else
							verb = new Verb("", fields[2], fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);

						verbs.Add(verb);
						if (verb.Type == VerbType.PISHVANDI)
						{
							verbs.Add(new Verb("", "", "خواه", pishvand, "", VerbTransitivity.NAGOZAR,
											   VerbType.AYANDEH_PISHVANDI, false, "?", "@", "!"));
							if (VerbPishvandiDic.ContainsKey(pishvand))
							{
								VerbPishvandiDic[pishvand].Add(verb.HastehMazi + "|" + verb.HastehMozareh);
							}
							else
							{
								var lst = new List<string>();
								lst.Add(verb.HastehMazi + "|" + verb.HastehMozareh);
								VerbPishvandiDic.Add(pishvand, lst);
							}
						}
					}
					else if (vtype == 3)
					{
						var verbType = VerbType.SADEH;
						int trans = int.Parse(fields[1]);
						VerbTransitivity transitivity = VerbTransitivity.GOZARA;
						if (trans == 0)
							transitivity = VerbTransitivity.NAGOZAR;
						else if (trans == 2)
							transitivity = VerbTransitivity.DOVAJHI;
						Verb verb;
						bool amrShodani = true;
						string vowelEnd = fields[8];
						string maziVowel = fields[9];
						string mozarehVowel = fields[10];
						string nonVerbalElemant = fields[4];
						if (fields[3] == "-")
							verb = new Verb("", fields[2], "", "", "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else if (fields[2] == "-")
							verb = new Verb("", "", fields[3], "", "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else
							verb = new Verb("", fields[2], fields[3], "", "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						if (fields[7] == "*")
							amrShodani = false;
						if (!CompoundVerbDic.ContainsKey(verb))
							CompoundVerbDic.Add(verb, new Dictionary<string, Dictionary<string, bool>>());
						if (!CompoundVerbDic[verb].ContainsKey(nonVerbalElemant))
						{
							CompoundVerbDic[verb].Add(nonVerbalElemant, new Dictionary<string, bool>());
						}
						if (!CompoundVerbDic[verb][nonVerbalElemant].ContainsKey(""))
							CompoundVerbDic[verb][nonVerbalElemant].Add("", amrShodani);

					}
					else if (vtype == 4)
					{
						var verbType = VerbType.PISHVANDI;
						int trans = int.Parse(fields[1]);
						VerbTransitivity transitivity = VerbTransitivity.GOZARA;
						if (trans == 0)
							transitivity = VerbTransitivity.NAGOZAR;
						else if (trans == 2)
							transitivity = VerbTransitivity.DOVAJHI;
						Verb verb;
						string pishvand = "";
						if (fields[5] != "-")
						{
							pishvand = fields[5];
						}
						bool amrShodani = true;
						string vowelEnd = fields[8];
						string maziVowel = fields[9];
						string mozarehVowel = fields[10];
						string nonVerbalElemant = fields[4];
						if (fields[3] == "-")
							verb = new Verb("", fields[2], "", pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else if (fields[2] == "-")
							verb = new Verb("", "", fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else
							verb = new Verb("", fields[2], fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						if (fields[7] == "*")
							amrShodani = false;
						if (!CompoundVerbDic.ContainsKey(verb))
							CompoundVerbDic.Add(verb, new Dictionary<string, Dictionary<string, bool>>());
						if (!CompoundVerbDic[verb].ContainsKey(nonVerbalElemant))
						{
							CompoundVerbDic[verb].Add(nonVerbalElemant, new Dictionary<string, bool>());
						}
						if (!CompoundVerbDic[verb][nonVerbalElemant].ContainsKey(""))
							CompoundVerbDic[verb][nonVerbalElemant].Add("", amrShodani);
					}
					else if (vtype == 5 || vtype == 7)
					{
						var verbType = VerbType.SADEH;
						int trans = int.Parse(fields[1]);
						VerbTransitivity transitivity = VerbTransitivity.GOZARA;
						if (trans == 0)
							transitivity = VerbTransitivity.NAGOZAR;
						else if (trans == 2)
							transitivity = VerbTransitivity.DOVAJHI;
						Verb verb;
						string pishvand = "";
						if (fields[5] != "-")
						{
							pishvand = fields[5];
						}
						if (pishvand != "")
						{
							verbType = VerbType.PISHVANDI;
						}
						bool amrShodani = true;
						string vowelEnd = fields[8];
						string maziVowel = fields[9];
						string mozarehVowel = fields[10];
						string nonVerbalElemant = fields[4];
						string harfeEazafeh = fields[6];
						if (fields[3] == "-")
							verb = new Verb("", fields[2], "", pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else if (fields[2] == "-")
							verb = new Verb("", "", fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						else
							verb = new Verb("", fields[2], fields[3], pishvand, "", transitivity, verbType, amrShodani,
											vowelEnd, maziVowel, mozarehVowel);
						if (fields[7] == "*")
							amrShodani = false;
						if (!CompoundVerbDic.ContainsKey(verb))
							CompoundVerbDic.Add(verb, new Dictionary<string, Dictionary<string, bool>>());
						if (!CompoundVerbDic[verb].ContainsKey(nonVerbalElemant))
						{
							CompoundVerbDic[verb].Add(nonVerbalElemant, new Dictionary<string, bool>());
						}
						if (!CompoundVerbDic[verb][nonVerbalElemant].ContainsKey(harfeEazafeh))

							CompoundVerbDic[verb][nonVerbalElemant].Add(harfeEazafeh, amrShodani);
					}
				}
				var verbtext = new StringBuilder();
				var mitavanInflection = new VerbInflection(new Verb("", "", "می‌توان", "", "", VerbTransitivity.NAGOZAR, VerbType.SADEH, false, "?", "@", "!"), ZamirPeyvastehType.ZamirPeyvasteh_NONE,
																		 ShakhsType.Shakhs_NONE,
																		 TenseFormationType.HAAL_SAADEH, TensePositivity.POSITIVE);
				VerbShapes.Add("می‌توان", new List<VerbInflection>());
				VerbShapes["می‌توان"].Add(mitavanInflection);
				var nemitavanInflection = new VerbInflection(new Verb("", "", "می‌توان", "", "", VerbTransitivity.NAGOZAR, VerbType.SADEH, false, "?", "@", "!"), ZamirPeyvastehType.ZamirPeyvasteh_NONE,
																		 ShakhsType.Shakhs_NONE,
																		 TenseFormationType.HAAL_SAADEH, TensePositivity.POSITIVE);
				VerbShapes.Add("نمی‌توان", new List<VerbInflection>());
				VerbShapes["نمی‌توان"].Add(nemitavanInflection);

				var betavanInflection = new VerbInflection(new Verb("", "", "بتوان", "", "", VerbTransitivity.NAGOZAR, VerbType.SADEH, false, "?", "@", "!"), ZamirPeyvastehType.ZamirPeyvasteh_NONE,
																	   ShakhsType.Shakhs_NONE,
																	   TenseFormationType.HAAL_ELTEZAMI, TensePositivity.POSITIVE);
				VerbShapes.Add("بتوان", new List<VerbInflection>());
				VerbShapes["بتوان"].Add(betavanInflection);

				var naitavanInflection = new VerbInflection(new Verb("", "", "نتوان", "", "", VerbTransitivity.NAGOZAR, VerbType.SADEH, false, "?", "@", "!"), ZamirPeyvastehType.ZamirPeyvasteh_NONE,
																		 ShakhsType.Shakhs_NONE,
																		 TenseFormationType.HAAL_ELTEZAMI, TensePositivity.POSITIVE);
				VerbShapes.Add("نتوان", new List<VerbInflection>());
				VerbShapes["نتوان"].Add(naitavanInflection);


				foreach (Verb verb in verbs)
				{
					if (verb.Type == VerbType.SADEH || verb.Type == VerbType.PISHVANDI || verb.Type == VerbType.AYANDEH_PISHVANDI)
					{
						foreach (TensePositivity positivity in Enum.GetValues(typeof(TensePositivity)))
						{
							foreach (ShakhsType shakhsType in Enum.GetValues(typeof(ShakhsType)))
							{
								foreach (
									TenseFormationType tenseFormationType in
										Enum.GetValues(typeof(TenseFormationType)))
								{
									foreach (
										ZamirPeyvastehType zamirPeyvastehType in
											Enum.GetValues(typeof(ZamirPeyvastehType)))
									{

										var inflection = new VerbInflection(verb, zamirPeyvastehType,
																			shakhsType,
																			tenseFormationType, positivity);
										if (inflection.IsValid())
										{
											var output = InflectorManager.GetInflections(inflection);
											foreach (string list in output)
											{
												if (!(VerbShapes.ContainsKey(list)))
												{
													var verbInflections = new List<VerbInflection> { inflection };
													VerbShapes.Add(list, verbInflections);
												}
												else
												{
													bool contains = false;
													foreach (VerbInflection inf in VerbShapes[list])
													{
														if (inflection.Equals(inf))
														{
															contains = true;
															break;
														}
													}
													if (!contains)
														VerbShapes[list].Add(inflection);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
