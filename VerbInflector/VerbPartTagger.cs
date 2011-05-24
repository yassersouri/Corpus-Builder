using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class VerbPartTagger
	{
		private static VerbList verbDic = null;

		public static Dictionary<string, int> StaticDic = new Dictionary<string, int>();

		private static Dictionary<int, List<VerbInflection>> GetVerbParts(string[] sentence, string[] posSentence, string verbDicPath)
		{
			var dic = new Dictionary<int, List<VerbInflection>>();
			if (verbDic == null)
			{
				verbDic = new VerbList(verbDicPath);
			}
			for (int i = 0; i < sentence.Length; i++)
			{
				if (posSentence[i] == "V")
				{
					dic.Add(i, VerbList.VerbShapes.ContainsKey(sentence[i]) ? VerbList.VerbShapes[sentence[i]] : null);
					if (dic[i] == null)
					{
						bool find = false;
						if (sentence[i].StartsWith("می"))
						{
							string newSen = sentence[i].Remove(0, 2).Insert(0, "می‌");
							if (VerbList.VerbShapes.ContainsKey(newSen))
							{
								sentence[i] = newSen;
								find = true;
								dic[i] = VerbList.VerbShapes[newSen];
							}
						}
						else if (sentence[i].StartsWith("نمی"))
						{
							string newSen = sentence[i].Remove(0, 2).Insert(0, "نمی‌");
							if (VerbList.VerbShapes.ContainsKey(newSen))
							{
								sentence[i] = newSen;
								find = true;
								dic[i] = VerbList.VerbShapes[newSen];
							}
						}
						else if (sentence[i].Contains("ئی"))
						{
							string newSen = sentence[i].Replace("ئی", "یی");
							if (VerbList.VerbShapes.ContainsKey(newSen))
							{
								sentence[i] = newSen;
								find = true;
								dic[i] = VerbList.VerbShapes[newSen];
							}
						}

						if (!find)
						{
							if (!StaticDic.ContainsKey(sentence[i]))
							{
								StaticDic.Add(sentence[i], 1);
							}
							else
							{
								StaticDic[sentence[i]]++;
							}
						}
					}
				}
				else
				{
					dic.Add(i, null);

				}
			}
			return dic;
		}
		private static Dictionary<int, List<VerbInflection>> GetVerbParts(string[] sentence, string verbDicPath)
		{
			var dic = new Dictionary<int, List<VerbInflection>>();
			if (verbDic == null)
			{
				verbDic = new VerbList(verbDicPath);
			}
			for (int i = 0; i < sentence.Length; i++)
			{
				dic.Add(i, VerbList.VerbShapes.ContainsKey(sentence[i]) ? VerbList.VerbShapes[sentence[i]] : null);
			}
			return dic;
		}

		/// <summary>
		/// finds simple and prefix verbs (do not consider compound verbs)
		/// </summary>
		/// Arguments as in ManageConsiderCompoundVerbs
		/// <param name="sentence"></param>
		/// <param name="posSentence"></param>
		/// <param name="posTokens"></param>
		/// <param name="verbDicPath"></param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, object>> Manage(string[] sentence, string[] posSentence, out string[] posTokens, string verbDicPath)
		{
			var bestDic = new Dictionary<int, KeyValuePair<string, object>>();
			var initDic = GetVerbTokens(sentence, posSentence, out posTokens, verbDicPath);

			var mostamars = new Dictionary<int, int>();
			for (int i = 0; i < initDic.Count; i++)
			{
				if (initDic[i].Value != null)
				{
					var verbInflection = initDic[i].Value;

					if (verbInflection.VerbStem.Type == VerbType.SADEH &&
						verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
						verbInflection.Positivity == TensePositivity.POSITIVE &&
						(verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH || verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI) &&
						verbInflection.VerbStem.HastehMazi == "داشت")
					{
						int key = i;
						int value = -1;
						for (int j = i + 1; j < initDic.Count; j++)
						{
							if (initDic[j].Value != null)
							{
								var newinfl = initDic[j].Value;
								if (newinfl.Positivity == TensePositivity.POSITIVE &&
									newinfl.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									newinfl.VerbStem.HastehMazi != "داشت" && newinfl.VerbStem.HastehMozareh != "است" && newinfl.VerbStem.HastehMozareh != "هست")
								{
									value = j;
									break;
								}
							}
						}
						if (value > 0)
						{
							mostamars.Add(key, value);
						}
					}

					if (verbInflection.VerbStem.Type == VerbType.SADEH &&
						verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
						verbInflection.Positivity == TensePositivity.POSITIVE &&
						verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
						verbInflection.VerbStem.HastehMazi == "داشت")
					{
						int key = i;
						int value = -1;
						for (int j = i + 1; j < initDic.Count; j++)
						{
							if (initDic[j].Value != null)
							{
								var newinfl = initDic[j].Value;

								if (newinfl.Positivity == TensePositivity.POSITIVE &&
									newinfl.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									newinfl.VerbStem.HastehMazi != "داشت")
								{
									value = j;
									break;
								}
							}
						}
						if (value > 0)
						{
							mostamars.Add(key, value);
						}
					}
				}
			}
			for (int i = 0; i < initDic.Count; i++)
			{
				if (initDic[i].Value != null)
				{
					if (mostamars.ContainsKey(i))
					{
						var mostamarVal = new KeyValuePair<string, int>();
						if (initDic[i].Value.TenseForm == TenseFormationType.HAAL_SAADEH || initDic[i].Value.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
						{
							mostamarVal = new KeyValuePair<string, int>("MOSTAMAR_SAAZ_HALL", mostamars[i]);
						}
						if (initDic[i].Value.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
						{
							mostamarVal = new KeyValuePair<string, int>("MOSTAMAR_SAAZ_GOZASHTEH", mostamars[i]);

						}
						bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, mostamarVal));
					}
					else
					{
						bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, initDic[i].Value));

					}
				}
				else
				{
					bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, initDic[i].Value));
				}
			}
			return bestDic;
		}

		/// <summary>
		/// finds simple and prefix verbs (do not consider compound verbs)
		/// </summary>
		/// Arguments as in ManageConsiderCompoundVerbs 
		/// <param name="sentence"></param>
		/// <param name="verbDicPath"></param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, object>> Manage(string[] sentence, string verbDicPath)
		{
			var bestDic = new Dictionary<int, KeyValuePair<string, object>>();
			var initDic = GetVerbTokens(sentence, verbDicPath);

			var mostamars = new Dictionary<int, int>();
			for (int i = 0; i < initDic.Count; i++)
			{
				if (initDic[i].Value != null)
				{
					var verbInflection = initDic[i].Value;

					if (verbInflection.VerbStem.Type == VerbType.SADEH &&
						verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
						verbInflection.Positivity == TensePositivity.POSITIVE &&
						(verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH || verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI) &&
						verbInflection.VerbStem.HastehMazi == "داشت")
					{
						int key = i;
						int value = -1;
						for (int j = i + 1; j < initDic.Count; j++)
						{
							if (initDic[j].Value != null)
							{
								var newinfl = initDic[j].Value;
								if (newinfl.Positivity == TensePositivity.POSITIVE &&
									(newinfl.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI) &&
									newinfl.VerbStem.HastehMazi != "داشت" && newinfl.VerbStem.HastehMozareh != "است" && newinfl.VerbStem.HastehMozareh != "هست")
								{
									value = j;
									break;
								}
							}
						}
						if (value > 0)
						{
							mostamars.Add(key, value);
						}
					}

					if (verbInflection.VerbStem.Type == VerbType.SADEH &&
						verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
						verbInflection.Positivity == TensePositivity.POSITIVE &&
						verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
						verbInflection.VerbStem.HastehMazi == "داشت")
					{
						int key = i;
						int value = -1;
						for (int j = i + 1; j < initDic.Count; j++)
						{
							if (initDic[j].Value != null)
							{
								var newinfl = initDic[j].Value;

								if (newinfl.Positivity == TensePositivity.POSITIVE &&
									newinfl.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									newinfl.VerbStem.HastehMazi != "داشت")
								{
									value = j;
									break;
								}
							}
						}
						if (value > 0)
						{
							mostamars.Add(key, value);
						}
					}
				}
			}
			for (int i = 0; i < initDic.Count; i++)
			{
				if (initDic[i].Value != null)
				{
					if (mostamars.ContainsKey(i))
					{
						var mostamarVal = new KeyValuePair<string, int>();
						if (initDic[i].Value.TenseForm == TenseFormationType.HAAL_SAADEH || initDic[i].Value.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
						{
							mostamarVal = new KeyValuePair<string, int>("MOSTAMAR_SAAZ_HALL", mostamars[i]);
						}
						if (initDic[i].Value.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
						{
							mostamarVal = new KeyValuePair<string, int>("MOSTAMAR_SAAZ_GOZASHTEH", mostamars[i]);

						}
						bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, mostamarVal));
					}
					else
					{
						bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, initDic[i].Value));

					}
				}
				else
				{
					bestDic.Add(i, new KeyValuePair<string, object>(initDic[i].Key, initDic[i].Value));
				}
			}
			return bestDic;
		}

		/// <summary>
		/// creates an object set of words,verbs and their inflections
		/// </summary>
		/// <param name="sentence">sentence words</param>
		/// <param name="posSentence">sentence words part of speech set</param>
		/// <param name="posTokens">the new sentence words part of speech set (after finding the verbs)</param>
		/// <param name="verbDicPath">path to the verb dictionary</param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, object>> ManageConsiderCompoundVerbs(string[] sentence, string[] posSentence, out string[] posTokens, string verbDicPath)
		{
			var bestDic = Manage(sentence, posSentence, out posTokens, verbDicPath);
			for (int i = posTokens.Length - 1; i >= 0; i--)
			{
				if (bestDic[i].Value is VerbInflection)
				{
					var verbValue = ((VerbInflection)bestDic[i].Value).VerbStem;
					if (VerbList.CompoundVerbDic.ContainsKey(verbValue))
					{
						for (int j = i - 1; j >= 0; j--)
						{
							if (posTokens[j] == "N")
							{
								if (VerbList.CompoundVerbDic[verbValue].ContainsKey(bestDic[j].Key))
								{
									if (j > 0 &&
										VerbList.CompoundVerbDic[verbValue][bestDic[j].Key].ContainsKey(
											bestDic[j - 1].Key))
									{
										var item1 = new KeyValuePair<string, object>(bestDic[j].Key,
																						new KeyValuePair<string, int>(
																					 "NON-VERBAL-ELEMENT", i));
										bestDic[j] = item1;
										var item2 = new KeyValuePair<string, object>(bestDic[j - 1].Key,
																					 new KeyValuePair<string, int>(
																					 "VERBAL-PREPOSIOTION", i));
										bestDic[j - 1] = item2;
										i = j - 2;
										break;
									}
									else if (VerbList.CompoundVerbDic[verbValue][bestDic[j].Key].ContainsKey("") &&
											 posTokens[j] != "P")
									{
										var item1 = new KeyValuePair<string, object>(bestDic[j].Key,
																					   new KeyValuePair<string, int>(
																					 "NON-VERBAL-ELEMENT", i));
										bestDic[j] = item1;
										i = j - 1;
										break;
									}
								}
							}
							else if (posTokens[j] == "V" || posTokens[j] == "PUNC" || posTokens[j] == "ADV" ||
									 posTokens[j] == "POSTP")
							{
								i = j - 1;
								break;
							}
						}
					}
				}
			}
			return bestDic;
		}

		/// <summary>
		/// creates an object set of words, verbs and inflections of sentence words
		/// </summary>
		/// <param name="sentence">sentence words</param>
		/// <param name="verbDicPath">path of the verb dictionary</param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, object>> ManageConsiderCompoundVerbs(string[] sentence, string verbDicPath)
		{
			var bestDic = Manage(sentence, verbDicPath);
			for (int i = bestDic.Count - 1; i >= 0; i--)
			{
				if (bestDic[i].Value is VerbInflection)
				{
					var verbValue = ((VerbInflection)bestDic[i].Value).VerbStem;
					if (VerbList.CompoundVerbDic.ContainsKey(verbValue))
					{
						for (int j = i - 1; j >= 0; j--)
						{
							if (VerbList.CompoundVerbDic[verbValue].ContainsKey(bestDic[j].Key))
							{
								if (j > 0 &&
									VerbList.CompoundVerbDic[verbValue][bestDic[j].Key].ContainsKey(
										bestDic[j - 1].Key))
								{
									var item1 = new KeyValuePair<string, object>(bestDic[j].Key,
																				 new KeyValuePair<string, int>(
																					 "NON-VERBAL-ELEMENT", i));
									bestDic[j] = item1;
									var item2 = new KeyValuePair<string, object>(bestDic[j - 1].Key,
																				 new KeyValuePair<string, int>(
																					 "VERBAL-PREPOSIOTION", i));
									bestDic[j - 1] = item2;
									i = j - 2;
									break;
								}
								else if (VerbList.CompoundVerbDic[verbValue][bestDic[j].Key].ContainsKey(""))
								{
									var item1 = new KeyValuePair<string, object>(bestDic[j].Key,
																				 new KeyValuePair<string, int>(
																					 "NON-VERBAL-ELEMENT", i));
									bestDic[j] = item1;
									i = j - 1;
									break;
								}
							}
						}
					}
				}
			}
			return bestDic;
		}

		/// <summary>
		/// returns a partial  dependency tree
		/// </summary>
		/// <param name="sentence">sentence words</param>
		/// <param name="verbDicPath">path to the verb dictionary</param>
		/// <param name="posSentence">pos of the words</param>
		/// <param name="posTokens">pos of words after verb finding</param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, KeyValuePair<int, object>>> MakePartialTree(string[] sentence, string[] posSentence, out string[] posTokens, string verbDicPath)
		{
			var dic = ManageConsiderCompoundVerbs(sentence, posSentence, out posTokens, verbDicPath);
			var partialTree = new Dictionary<int, KeyValuePair<string, KeyValuePair<int, object>>>();
			foreach (int key in dic.Keys)
			{
				string value = dic[key].Key;
				if (dic[key].Value is VerbInflection)
				{
					partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(-1, dic[key].Value)));

				}
				else if (dic[key].Value is KeyValuePair<string, int>)
				{
					var newValue = (KeyValuePair<string, int>)dic[key].Value;
					if (key > 0 && dic[key - 1].Value is KeyValuePair<string, int>)
					{
						var prevValue = (KeyValuePair<string, int>)dic[key - 1].Value;
						if (prevValue.Key == "VERBAL-PREPOSIOTION")
							partialTree.Add(key,
											new KeyValuePair<string, KeyValuePair<int, object>>(value,
																								new KeyValuePair
																									<int, object>(
																									key - 1,
																								   "POSDEP")));
						else
						{

							partialTree.Add(key,
											new KeyValuePair<string, KeyValuePair<int, object>>(value,
																								new KeyValuePair
																									<int, object>(
																									newValue.Value - 1,
																									newValue.Key)));
						}

					}
					else
					{
						partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(newValue.Value, newValue.Key)));
					}
				}
				else
				{
					partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(-1, "")));

				}

			}
			return partialTree;
		}

		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			var dic = MakePartialTree(sentence, verbDicPath);
			foreach (KeyValuePair<int, KeyValuePair<string, KeyValuePair<int, object>>> keyValuePair in dic)
			{
				int position = keyValuePair.Key + 1;
				string wordForm = keyValuePair.Value.Key;
				int head = keyValuePair.Value.Value.Key;
				string deprel = "_";
				object obj = keyValuePair.Value.Value.Value;
				string lemma = "_";
				int wordCount = wordForm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
				ShakhsType person = ShakhsType.Shakhs_NONE;
				NumberType number = NumberType.INVALID;
				TenseFormationType tma = TenseFormationType.TenseFormationType_NONE;
				if (obj is VerbInflection)
				{
					var newObj = (VerbInflection)obj;
					tma = newObj.TenseForm;
					var personType = newObj.Shakhs;
					person = personType;
					number = NumberType.SINGULAR;
					if (personType == ShakhsType.AVALSHAKHS_JAM || personType == ShakhsType.DOVVOMSHAKHS_JAM || personType == ShakhsType.SEVVOMSHAKHS_JAM)
					{
						number = NumberType.PLURAL;
					}
					lemma = newObj.VerbStem.SimpleToString();
				}
				if (obj is string)
				{
					var newObj = (string)obj;
					if (newObj == "POSDEP")
					{
						deprel = newObj;
					}
					else if (newObj == "VERBAL-PREPOSIOTION")
					{
						deprel = "VPRT";
					}
					else if (newObj == "NON-VERBAL-ELEMENT")
					{
						deprel = "NVE";
					}
				}

				var mfeat = new MorphoSyntacticFeatures(number, person, tma);
				var dependencyToken = new DependencyBasedToken(position, wordForm, lemma, "_", "_", head, deprel, wordCount,
															   mfeat);
				tree.Add(dependencyToken);
			}
			return tree;
		}
		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string[] posSentence, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			string[] outpos;
			var dic = MakePartialTree(sentence, posSentence, out outpos, verbDicPath);
			foreach (KeyValuePair<int, KeyValuePair<string, KeyValuePair<int, object>>> keyValuePair in dic)
			{
				int position = keyValuePair.Key + 1;
				string wordForm = keyValuePair.Value.Key;
				int head = keyValuePair.Value.Value.Key;
				string deprel = "_";
				object obj = keyValuePair.Value.Value.Value;
				string lemma = "_";
				int wordCount = wordForm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
				ShakhsType person = ShakhsType.Shakhs_NONE;
				NumberType number = NumberType.INVALID;
				TenseFormationType tma = TenseFormationType.TenseFormationType_NONE;
				if (obj is VerbInflection)
				{
					var newObj = (VerbInflection)obj;
					tma = newObj.TenseForm;
					var personType = newObj.Shakhs;
					person = personType;
					number = NumberType.SINGULAR;
					if (personType == ShakhsType.AVALSHAKHS_JAM || personType == ShakhsType.DOVVOMSHAKHS_JAM || personType == ShakhsType.SEVVOMSHAKHS_JAM)
					{
						number = NumberType.PLURAL;
					}
					lemma = newObj.VerbStem.SimpleToString();
				}
				if (obj is string)
				{
					var newObj = (string)obj;
					if (newObj == "POSDEP")
					{
						deprel = newObj;
					}
					else if (newObj == "VERBAL-PREPOSIOTION")
					{
						deprel = "VPRT";
					}
					else if (newObj == "NON-VERBAL-ELEMENT")
					{
						deprel = "NVE";
					}
				}

				var mfeat = new MorphoSyntacticFeatures(number, person, tma);
				var dependencyToken = new DependencyBasedToken(position, wordForm, lemma, outpos[position - 1], "_", head, deprel, wordCount,
															   mfeat);
				tree.Add(dependencyToken);
			}
			return tree;
		}
		/// <summary>
		/// returns a partial  dependency tree
		/// </summary>
		/// <param name="sentence">sentence words</param>
		/// <param name="verbDicPath">path to the verb dictionary</param>
		/// <returns></returns>
		public static Dictionary<int, KeyValuePair<string, KeyValuePair<int, object>>> MakePartialTree(string[] sentence, string verbDicPath)
		{
			var dic = ManageConsiderCompoundVerbs(sentence, verbDicPath);
			var partialTree = new Dictionary<int, KeyValuePair<string, KeyValuePair<int, object>>>();
			foreach (int key in dic.Keys)
			{
				string value = dic[key].Key;
				if (dic[key].Value is VerbInflection)
				{
					partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(-1, dic[key].Value)));

				}
				else if (dic[key].Value is KeyValuePair<string, int>)
				{
					var newValue = (KeyValuePair<string, int>)dic[key].Value;
					if (key > 0 && dic[key - 1].Value is KeyValuePair<string, int>)
					{
						var prevValue = (KeyValuePair<string, int>)dic[key - 1].Value;
						if (prevValue.Key == "VERBAL-PREPOSIOTION")
							partialTree.Add(key,
											new KeyValuePair<string, KeyValuePair<int, object>>(value,
																								new KeyValuePair
																									<int, object>(
																									key - 1,
																									"POSDEP")));
						else
						{

							partialTree.Add(key,
											new KeyValuePair<string, KeyValuePair<int, object>>(value,
																								new KeyValuePair
																									<int, object>(
																									newValue.Value - 1,
																									newValue.Key)));
						}

					}
					else
					{
						partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(newValue.Value, newValue.Key)));
					}
				}
				else
				{
					partialTree.Add(key, new KeyValuePair<string, KeyValuePair<int, object>>(value, new KeyValuePair<int, object>(-1, "")));

				}

			}
			return partialTree;
		}

		private static Dictionary<int, List<int>> GetGoodResult(string[] sentence, string[] posSentence, string verbDicPath)
		{
			var inflectionList = GetVerbParts(sentence, posSentence, verbDicPath);

			var stateList = new Dictionary<int, List<int>>();
			stateList.Add(-1, new List<int>());
			stateList[-1].Add(0);
			string tempPishvand = "";
			for (int i = 0; i < inflectionList.Count; i++)
			{
				stateList.Add(i, new List<int>());

				var valuePair = inflectionList[i];

				if (valuePair == null)
				{
					if (stateList[i - 1].Contains(6))
					{
						if (!stateList[i - 1].Contains(38))
							stateList[i - 1].Add(38);
						stateList[i - 1].Remove(6);
					}
					if (stateList[i - 1].Contains(-1))
					{
						if (!stateList[i - 1].Contains(48))
							stateList[i - 1].Add(48);
						stateList[i - 1].Remove(-1);
					}
					if (stateList[i - 1].Contains(-3))
					{
						if (!stateList[i - 1].Contains(52))
							stateList[i - 1].Add(52);
						stateList[i - 1].Remove(-3);
					}
					if (stateList[i - 1].Contains(8))
					{
						if (!stateList[i - 1].Contains(35))
							stateList[i - 1].Add(35);
						stateList[i - 1].Remove(8);
					}
					if (stateList[i - 1].Contains(5))
					{
						if (!stateList[i - 1].Contains(35))
							stateList[i - 1].Add(35);
						stateList[i - 1].Remove(5);
					}
					if (stateList[i - 1].Contains(7))
					{
						if (!stateList[i - 1].Contains(40))
							stateList[i - 1].Add(40);
						stateList[i - 1].Remove(7);
					}
					if (stateList[i - 1].Contains(9))
					{
						if (!stateList[i - 1].Contains(45))
							stateList[i - 1].Add(45);
						stateList[i - 1].Remove(9);
					}
					if (stateList[i - 1].Contains(1))
					{
						if (!stateList[i - 1].Contains(27))
							stateList[i - 1].Add(27);
						stateList[i - 1].Remove(1);
					}
					if (stateList[i - 1].Contains(4))
					{
						if (!stateList[i - 1].Contains(34))
							stateList[i - 1].Add(34);
						stateList[i - 1].Remove(4);
					}
					stateList[i].Add(0);
				}
				else
				{
					int counter = 0;
					foreach (VerbInflection verbInflection in valuePair)
					{
						counter++;

						#region state 0

						if (stateList[i - 1].Contains(0) || (stateList[i - 1].Count == 0 && stateList[i].Count == 0) || (stateList[i - 1].Count > 0 && stateList[i - 1][0] > 9))
						{
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(10))
									stateList[i].Add(10);
							}

							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(14))
									stateList[i].Add(14);
							}
							if (verbInflection.TenseForm == TenseFormationType.AMR &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(15))
									stateList[i].Add(15);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(17))
									stateList[i].Add(17);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(18))
									stateList[i].Add(18);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(20))
									stateList[i].Add(20);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(46))
									stateList[i].Add(46);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(21))
									stateList[i].Add(21);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(6))
									stateList[i].Add(6);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(3) && !stateList[i].Contains(5) && !stateList[i].Contains(7))
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
								}

							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" && verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(47))
									stateList[i].Add(47);
							}

							if (verbInflection.TenseForm == TenseFormationType.AMR &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(16))
									stateList[i].Add(16);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(19))
									stateList[i].Add(19);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "خواست")
							{
								if (!stateList[i].Contains(2))
									stateList[i].Add(2);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.Type == VerbType.AYANDEH_PISHVANDI)
							{
								if (!stateList[i].Contains(-2))
									stateList[i].Add(-2);
								tempPishvand = verbInflection.VerbStem.Pishvand;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(11))
									stateList[i].Add(11);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(3) && !stateList[i].Contains(5) && !stateList[i].Contains(7))
								{
									if (!stateList[i].Contains(12))
										stateList[i].Add(12);
								}
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(13))
									stateList[i].Add(13);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(1))
									stateList[i].Add(1);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(9))
									stateList[i].Add(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE)
							{
								if (!stateList[i].Contains(4))
									stateList[i].Add(4);
							}
						}

						#endregion


						#region state 1

						if (stateList[i - 1].Contains(1))
						{
							bool find1 = false;
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(22))
									stateList[i].Add(22);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(23))
									stateList[i].Add(23);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(24))
									stateList[i].Add(24);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(25))
									stateList[i].Add(25);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(26))
									stateList[i].Add(26);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(29))
									stateList[i].Add(29);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(30))
									stateList[i].Add(30);
								find1 = true;
							}
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.Positivity == TensePositivity.POSITIVE &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(27))
									stateList[i].Add(27);
								find1 = true;
							}
							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(27))
									stateList[i].Add(27);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(5))
									stateList[i].Add(5);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(-3))
									stateList[i].Add(-3);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
							  verbInflection.VerbStem.HastehMazi == "بود" &&
							  verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(51))
									stateList[i].Add(51);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(7))
									stateList[i].Add(7);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(28))
									stateList[i].Add(28);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "خواست" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(3))
									stateList[i].Add(3);
								find1 = true;
							}
							if (!find1)
							{
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									(verbInflection.VerbStem.HastehMazi == "کرد" ||
									 verbInflection.VerbStem.HastehMazi == "گشت" ||
									 verbInflection.VerbStem.HastehMazi == "نمود" ||
									 verbInflection.VerbStem.HastehMazi == "ساخت") &&
									verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
										 (verbInflection.VerbStem.HastehMazi == "کرد" ||
										  verbInflection.VerbStem.HastehMazi == "گشت" ||
										  verbInflection.VerbStem.HastehMazi == "نمود" ||
										  verbInflection.VerbStem.HastehMazi == "ساخت") &&
										 verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										 (verbInflection.VerbStem.HastehMazi == "کرد" ||
										  verbInflection.VerbStem.HastehMazi == "گشت" ||
										  verbInflection.VerbStem.HastehMazi == "نمود" ||
										  verbInflection.VerbStem.HastehMazi == "ساخت") &&
										 verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(10))
										stateList[i].Add(10);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (counter == valuePair.Count)
								{
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
											  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
											  verbInflection.VerbStem.Type == VerbType.SADEH &&
											  verbInflection.Positivity == TensePositivity.POSITIVE))
										{
											if (!stateList[i].Contains(10))
												stateList[i].Add(10);
											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
											  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
											  verbInflection.VerbStem.Type == VerbType.SADEH &&
											  verbInflection.Positivity == TensePositivity.POSITIVE))
										{
											if (!stateList[i].Contains(14))
												stateList[i].Add(14);
											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.AMR &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(15))
											stateList[i].Add(15);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(17))
											stateList[i].Add(17);
										if (
											!(verbInflection.VerbStem.HastehMazi == "کرد" &&
											  verbInflection.VerbStem.Type == VerbType.SADEH))
										{

											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(18))
											stateList[i].Add(18);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(20))
											stateList[i].Add(20);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm ==
										TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
									{
										if (!stateList[i].Contains(21))
											stateList[i].Add(21);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm ==
										TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
										verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
									{
										if (!stateList[i].Contains(6))
											stateList[i].Add(6);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}

									if (verbInflection.TenseForm == TenseFormationType.AMR &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(16))
											stateList[i].Add(16);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(19))
											stateList[i].Add(19);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
										verbInflection.VerbStem.HastehMazi == "خواست")
									{
										if (!stateList[i].Contains(2))
											stateList[i].Add(2);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(11))
											stateList[i].Add(11);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(13))
											stateList[i].Add(13);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Positivity == TensePositivity.POSITIVE)
									{
										if (!stateList[i].Contains(1))
											stateList[i].Add(1);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(9))
											stateList[i].Add(9);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Positivity == TensePositivity.NEGATIVE)
									{
										if (!stateList[i].Contains(4))
											stateList[i].Add(4);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
								}
							}
							else
							{
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
							}
						}

						#endregion

						#region state 5

						if (stateList[i - 1].Contains(5))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(35))
									stateList[i].Add(35);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(35))
									stateList[i].Add(35);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(36))
									stateList[i].Add(36);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(37))
									stateList[i].Add(37);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
						}

						#endregion


						#region state 2

						if (stateList[i - 1].Contains(2))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(31))
									stateList[i].Add(31);
								stateList[i - 1].Clear();
							}
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(32))
									stateList[i].Add(32);
								stateList[i - 1].Clear();
							}
						}

						#endregion

						#region state -2

						if (stateList[i - 1].Contains(-2))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && VerbList.VerbPishvandiDic[tempPishvand].Contains(verbInflection.VerbStem.HastehMazi + "|" + verbInflection.VerbStem.HastehMozareh))
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(31))
									stateList[i].Add(31);
								stateList[i - 1].Clear();
							}
							else if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && VerbList.VerbPishvandiDic[tempPishvand].Contains(verbInflection.VerbStem.HastehMazi + "|" + verbInflection.VerbStem.HastehMozareh))
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(32))
									stateList[i].Add(32);
								stateList[i - 1].Clear();
							}
							else
							{
								stateList[i - 1].Clear();
								stateList[i].Clear();
								stateList[i - 1].Add(0);
								stateList[i].Add(0);
							}
						}

						#endregion

						#region state -3

						if (stateList[i - 1].Contains(-3))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
															   verbInflection.VerbStem.Type == VerbType.SADEH &&
															   verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(52))
									stateList[i].Add(52);
								stateList[i - 1].Remove(-3);
							}
						}
						#endregion

						#region state 3

						if (stateList[i - 1].Contains(3))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(33))
									stateList[i].Add(33);
								stateList[i - 1].Clear();
							}
						}

						#endregion

						#region state 4

						if (stateList[i - 1].Contains(4))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(34))
									stateList[i].Add(34);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(41))
									stateList[i].Add(41);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(42))
									stateList[i].Add(42);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(-3))
									stateList[i].Add(-3);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
							 verbInflection.VerbStem.HastehMazi == "بود" &&
							 verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(51))
									stateList[i].Add(51);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								if (!stateList[i].Contains(8))
									stateList[i].Add(8);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
									  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									  verbInflection.VerbStem.Type == VerbType.SADEH &&
									  verbInflection.Positivity == TensePositivity.POSITIVE))
								{
									if (!stateList[i].Contains(10))
										stateList[i].Add(10);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
							}

							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								(verbInflection.VerbStem.HastehMazi == "کرد" ||
								 verbInflection.VerbStem.HastehMazi == "گشت" ||
								 verbInflection.VerbStem.HastehMazi == "نمود" ||
								 verbInflection.VerbStem.HastehMazi == "ساخت") &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(17))
									stateList[i].Add(17);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									 (verbInflection.VerbStem.HastehMazi == "کرد" ||
									  verbInflection.VerbStem.HastehMazi == "گشت" ||
									  verbInflection.VerbStem.HastehMazi == "نمود" ||
									  verbInflection.VerbStem.HastehMazi == "ساخت") &&
									 verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(14))
									stateList[i].Add(14);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									 (verbInflection.VerbStem.HastehMazi == "کرد" ||
									  verbInflection.VerbStem.HastehMazi == "گشت" ||
									  verbInflection.VerbStem.HastehMazi == "نمود" ||
									  verbInflection.VerbStem.HastehMazi == "ساخت") &&
									 verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(10))
									stateList[i].Add(10);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (stateList[i - 1].Contains(4))
							{
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									if (
										!(verbInflection.VerbStem.HastehMazi == "کرد" &&
										  verbInflection.VerbStem.Type == VerbType.SADEH))
									{
										stateList[i - 1].Remove(4);
										stateList[i - 1].Add(34);

									}
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(6))
										stateList[i].Add(6);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(47))
										stateList[i].Add(47);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(9))
										stateList[i].Add(9);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

							}
						}

						#endregion

						#region state 6

						if (stateList[i - 1].Contains(6))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(39))
									stateList[i].Add(39);
								stateList[i - 1].Remove(6);
							}
						}

						#endregion

						#region state 7

						if (stateList[i - 1].Contains(7))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(40))
									stateList[i].Add(40);
								stateList[i - 1].Remove(7);
							}

							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(40))
									stateList[i].Add(40);
								stateList[i - 1].Remove(7);
							}
						}

						#endregion

						#region state 8

						if (stateList[i - 1].Contains(8))
						{
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(43))
									stateList[i].Add(43);
								stateList[i - 1].Remove(8);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(44))
									stateList[i].Add(44);
								stateList[i - 1].Remove(8);
							}
						}

						#endregion

						#region state 9

						if (stateList[i - 1].Contains(9))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(45))
									stateList[i].Add(45);
								stateList[i - 1].Remove(9);
							}
							else if (verbInflection.VerbStem.HastehMozareh == "باش" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(49))
									stateList[i].Add(49);
								stateList[i - 1].Remove(9);
							}
							else if (verbInflection.VerbStem.HastehMozareh == "باش" &&
						   verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
						   verbInflection.VerbStem.Type == VerbType.SADEH &&
						   verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(50))
									stateList[i].Add(50);
								stateList[i - 1].Remove(9);
							}
							else
							{

								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
										  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										  verbInflection.VerbStem.Type == VerbType.SADEH &&
										  verbInflection.Positivity == TensePositivity.POSITIVE))
									{
										if (!stateList[i].Contains(10))
											stateList[i].Add(10);
										stateList[i - 1].Remove(9);
										stateList[i - 1].Add(45);
									}
								}

								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									if (stateList[i - 1].Contains(9))
									{
										stateList[i - 1].Remove(9);
										stateList[i - 1].Add(45);
									}

								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(46))
										stateList[i].Add(46);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(9))
										stateList[i].Add(9);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (stateList[i - 1].Contains(9))
								{
									if (!stateList[i - 1].Contains(45))
										stateList[i - 1].Add(45);
									stateList[i - 1].Remove(9);
								}
							}
						}

						#endregion

						#region state -1

						if (stateList[i - 1].Contains(-1))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(48))
									stateList[i].Add(48);
								stateList[i - 1].Remove(-1);
							}
							else
							{
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
										  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										  verbInflection.VerbStem.Type == VerbType.SADEH &&
										  verbInflection.Positivity == TensePositivity.POSITIVE))
									{
										if (!stateList[i].Contains(10))
											stateList[i].Add(10);
										stateList[i - 1].Remove(-1);
										stateList[i - 1].Add(48);
									}
								}

								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(46))
										stateList[i].Add(46);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(6))
										stateList[i].Add(6);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(47))
										stateList[i].Add(47);

									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(12))
										stateList[i].Add(12);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								stateList[i - 1].Remove(-1);
							}
						}

						#endregion


					}
					if (stateList[i].Count == 0)
					{
						stateList[i].Add(0);
					}
					if (!stateList[i].Contains(-2))
						tempPishvand = "";
				}
			}
			return stateList;
		}

		private static Dictionary<int, List<int>> GetGoodResult(string[] sentence, string verbDicPath)
		{
			var inflectionList = GetVerbParts(sentence, verbDicPath);

			var stateList = new Dictionary<int, List<int>>();
			stateList.Add(-1, new List<int>());
			stateList[-1].Add(0);
			string tempPishvand = "";
			for (int i = 0; i < inflectionList.Count; i++)
			{
				stateList.Add(i, new List<int>());

				var valuePair = inflectionList[i];

				if (valuePair == null)
				{
					if (stateList[i - 1].Contains(6))
					{
						if (!stateList[i - 1].Contains(38))
							stateList[i - 1].Add(38);
						stateList[i - 1].Remove(6);
					}
					if (stateList[i - 1].Contains(-1))
					{
						if (!stateList[i - 1].Contains(48))
							stateList[i - 1].Add(48);
						stateList[i - 1].Remove(-1);
					}
					if (stateList[i - 1].Contains(-3))
					{
						if (!stateList[i - 1].Contains(52))
							stateList[i - 1].Add(52);
						stateList[i - 1].Remove(-3);
					}
					if (stateList[i - 1].Contains(8))
					{
						if (!stateList[i - 1].Contains(35))
							stateList[i - 1].Add(35);
						stateList[i - 1].Remove(8);
					}
					if (stateList[i - 1].Contains(5))
					{
						if (!stateList[i - 1].Contains(35))
							stateList[i - 1].Add(35);
						stateList[i - 1].Remove(5);
					}
					if (stateList[i - 1].Contains(7))
					{
						if (!stateList[i - 1].Contains(40))
							stateList[i - 1].Add(40);
						stateList[i - 1].Remove(7);
					}
					if (stateList[i - 1].Contains(9))
					{
						if (!stateList[i - 1].Contains(45))
							stateList[i - 1].Add(45);
						stateList[i - 1].Remove(9);
					}
					if (stateList[i - 1].Contains(1))
					{
						if (!stateList[i - 1].Contains(27))
							stateList[i - 1].Add(27);
						stateList[i - 1].Remove(1);
					}
					if (stateList[i - 1].Contains(4))
					{
						if (!stateList[i - 1].Contains(34))
							stateList[i - 1].Add(34);
						stateList[i - 1].Remove(4);
					}
					stateList[i].Add(0);
				}
				else
				{
					int counter = 0;
					foreach (VerbInflection verbInflection in valuePair)
					{
						counter++;

						#region state 0

						if (stateList[i - 1].Contains(0) || (stateList[i - 1].Count == 0 && stateList[i].Count == 0) || (stateList[i - 1].Count > 0 && stateList[i - 1][0] > 9))
						{
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(10))
									stateList[i].Add(10);
							}

							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(14))
									stateList[i].Add(14);
							}
							if (verbInflection.TenseForm == TenseFormationType.AMR &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(15))
									stateList[i].Add(15);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(17))
									stateList[i].Add(17);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(18))
									stateList[i].Add(18);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!stateList[i].Contains(20))
									stateList[i].Add(20);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(46))
									stateList[i].Add(46);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(21))
									stateList[i].Add(21);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(6))
									stateList[i].Add(6);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(3) && !stateList[i].Contains(5) && !stateList[i].Contains(7))
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
								}

							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi != "شد" && verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
							{
								if (!stateList[i].Contains(47))
									stateList[i].Add(47);
							}

							if (verbInflection.TenseForm == TenseFormationType.AMR &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(16))
									stateList[i].Add(16);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(19))
									stateList[i].Add(19);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "خواست")
							{
								if (!stateList[i].Contains(2))
									stateList[i].Add(2);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.Type == VerbType.AYANDEH_PISHVANDI)
							{
								if (!stateList[i].Contains(-2))
									stateList[i].Add(-2);
								tempPishvand = verbInflection.VerbStem.Pishvand;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(11))
									stateList[i].Add(11);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(3) && !stateList[i].Contains(5) && !stateList[i].Contains(7))
								{
									if (!stateList[i].Contains(12))
										stateList[i].Add(12);
								}
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(13))
									stateList[i].Add(13);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(1))
									stateList[i].Add(1);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "شد")
							{
								if (!stateList[i].Contains(9))
									stateList[i].Add(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE)
							{
								if (!stateList[i].Contains(4))
									stateList[i].Add(4);
							}
						}

						#endregion


						#region state 1

						if (stateList[i - 1].Contains(1))
						{
							bool find1 = false;
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(22))
									stateList[i].Add(22);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(23))
									stateList[i].Add(23);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(24))
									stateList[i].Add(24);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(25))
									stateList[i].Add(25);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(26))
									stateList[i].Add(26);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(29))
									stateList[i].Add(29);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(30))
									stateList[i].Add(30);
								find1 = true;
							}
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.Positivity == TensePositivity.POSITIVE &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(27))
									stateList[i].Add(27);
								find1 = true;
							}
							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(27))
									stateList[i].Add(27);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(5))
									stateList[i].Add(5);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(-3))
									stateList[i].Add(-3);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
							  verbInflection.VerbStem.HastehMazi == "بود" &&
							  verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(51))
									stateList[i].Add(51);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(7))
									stateList[i].Add(7);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
								verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(28))
									stateList[i].Add(28);
								find1 = true;
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "خواست" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(3))
									stateList[i].Add(3);
								find1 = true;
							}
							if (!find1)
							{
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									(verbInflection.VerbStem.HastehMazi == "کرد" ||
									 verbInflection.VerbStem.HastehMazi == "گشت" ||
									 verbInflection.VerbStem.HastehMazi == "نمود" ||
									 verbInflection.VerbStem.HastehMazi == "ساخت") &&
									verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
										 (verbInflection.VerbStem.HastehMazi == "کرد" ||
										  verbInflection.VerbStem.HastehMazi == "گشت" ||
										  verbInflection.VerbStem.HastehMazi == "نمود" ||
										  verbInflection.VerbStem.HastehMazi == "ساخت") &&
										 verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										 (verbInflection.VerbStem.HastehMazi == "کرد" ||
										  verbInflection.VerbStem.HastehMazi == "گشت" ||
										  verbInflection.VerbStem.HastehMazi == "نمود" ||
										  verbInflection.VerbStem.HastehMazi == "ساخت") &&
										 verbInflection.VerbStem.Type == VerbType.SADEH)
								{
									if (!stateList[i].Contains(10))
										stateList[i].Add(10);
									stateList[i - 1].Remove(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(0);
								}
								else if (counter == valuePair.Count)
								{
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
											  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
											  verbInflection.VerbStem.Type == VerbType.SADEH &&
											  verbInflection.Positivity == TensePositivity.POSITIVE))
										{
											if (!stateList[i].Contains(10))
												stateList[i].Add(10);
											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
											  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
											  verbInflection.VerbStem.Type == VerbType.SADEH &&
											  verbInflection.Positivity == TensePositivity.POSITIVE))
										{
											if (!stateList[i].Contains(14))
												stateList[i].Add(14);
											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.AMR &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(15))
											stateList[i].Add(15);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(17))
											stateList[i].Add(17);
										if (
											!(verbInflection.VerbStem.HastehMazi == "کرد" &&
											  verbInflection.VerbStem.Type == VerbType.SADEH))
										{

											stateList[i - 1].Remove(1);
											stateList[i - 1].Add(27);
										}
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(18))
											stateList[i].Add(18);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
										verbInflection.VerbStem.HastehMazi != "شد")
									{
										if (!stateList[i].Contains(20))
											stateList[i].Add(20);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm ==
										TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
									{
										if (!stateList[i].Contains(21))
											stateList[i].Add(21);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm ==
										TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
										verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
									{
										if (!stateList[i].Contains(6))
											stateList[i].Add(6);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}

									if (verbInflection.TenseForm == TenseFormationType.AMR &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(16))
											stateList[i].Add(16);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(19))
											stateList[i].Add(19);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
										verbInflection.VerbStem.HastehMazi == "خواست")
									{
										if (!stateList[i].Contains(2))
											stateList[i].Add(2);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(11))
											stateList[i].Add(11);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(13))
											stateList[i].Add(13);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Positivity == TensePositivity.POSITIVE)
									{
										if (!stateList[i].Contains(1))
											stateList[i].Add(1);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi == "شد")
									{
										if (!stateList[i].Contains(9))
											stateList[i].Add(9);
										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);

									}
									if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
										verbInflection.VerbStem.HastehMazi != "شد" &&
										verbInflection.Positivity == TensePositivity.NEGATIVE)
									{
										if (!stateList[i].Contains(4))
											stateList[i].Add(4);

										stateList[i - 1].Remove(1);
										stateList[i - 1].Add(27);
									}
								}
							}
							else
							{
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
							}
						}

						#endregion

						#region state 5

						if (stateList[i - 1].Contains(5))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(35))
									stateList[i].Add(35);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(35))
									stateList[i].Add(35);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(36))
									stateList[i].Add(36);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(37))
									stateList[i].Add(37);
								stateList[i - 1].Remove(5);
								stateList[i - 1].Remove(9);
							}
						}

						#endregion


						#region state 2

						if (stateList[i - 1].Contains(2))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(31))
									stateList[i].Add(31);
								stateList[i - 1].Clear();
							}
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(32))
									stateList[i].Add(32);
								stateList[i - 1].Clear();
							}
						}

						#endregion

						#region state -2

						if (stateList[i - 1].Contains(-2))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi != "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && VerbList.VerbPishvandiDic[tempPishvand].Contains(verbInflection.VerbStem.HastehMazi + "|" + verbInflection.VerbStem.HastehMozareh))
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(31))
									stateList[i].Add(31);
								stateList[i - 1].Clear();
							}
							else if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && VerbList.VerbPishvandiDic[tempPishvand].Contains(verbInflection.VerbStem.HastehMazi + "|" + verbInflection.VerbStem.HastehMozareh))
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(32))
									stateList[i].Add(32);
								stateList[i - 1].Clear();
							}
							else
							{
								stateList[i - 1].Clear();
								stateList[i].Clear();
								stateList[i - 1].Add(0);
								stateList[i].Add(0);
							}
						}

						#endregion

						#region state -3

						if (stateList[i - 1].Contains(-3))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
															   verbInflection.VerbStem.Type == VerbType.SADEH &&
															   verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(52))
									stateList[i].Add(52);
								stateList[i - 1].Remove(-3);
							}
						}
						#endregion

						#region state 3

						if (stateList[i - 1].Contains(3))
						{
							if (verbInflection.IsPayehFelMasdari() && verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								stateList[i].Clear();
								if (!stateList[i].Contains(33))
									stateList[i].Add(33);
								stateList[i - 1].Clear();
							}
						}

						#endregion

						#region state 4

						if (stateList[i - 1].Contains(4))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(34))
									stateList[i].Add(34);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(41))
									stateList[i].Add(41);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(42))
									stateList[i].Add(42);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
								verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(-3))
									stateList[i].Add(-3);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
							 verbInflection.VerbStem.HastehMazi == "بود" &&
							 verbInflection.VerbStem.Type == VerbType.SADEH && verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(51))
									stateList[i].Add(51);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.VerbStem.HastehMazi == "شد" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								if (!stateList[i].Contains(8))
									stateList[i].Add(8);
								stateList[i - 1].Remove(4);
							}
							if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.HastehMazi != "شد")
							{
								if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
									  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									  verbInflection.VerbStem.Type == VerbType.SADEH &&
									  verbInflection.Positivity == TensePositivity.POSITIVE))
								{
									if (!stateList[i].Contains(10))
										stateList[i].Add(10);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
							}

							if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								(verbInflection.VerbStem.HastehMazi == "کرد" ||
								 verbInflection.VerbStem.HastehMazi == "گشت" ||
								 verbInflection.VerbStem.HastehMazi == "نمود" ||
								 verbInflection.VerbStem.HastehMazi == "ساخت") &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(17))
									stateList[i].Add(17);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									 (verbInflection.VerbStem.HastehMazi == "کرد" ||
									  verbInflection.VerbStem.HastehMazi == "گشت" ||
									  verbInflection.VerbStem.HastehMazi == "نمود" ||
									  verbInflection.VerbStem.HastehMazi == "ساخت") &&
									 verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(14))
									stateList[i].Add(14);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									 (verbInflection.VerbStem.HastehMazi == "کرد" ||
									  verbInflection.VerbStem.HastehMazi == "گشت" ||
									  verbInflection.VerbStem.HastehMazi == "نمود" ||
									  verbInflection.VerbStem.HastehMazi == "ساخت") &&
									 verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(10))
									stateList[i].Add(10);
								stateList[i - 1].Remove(1);
								stateList[i - 1].Remove(4);
								stateList[i - 1].Add(0);
							}
							else if (stateList[i - 1].Contains(4))
							{
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);

									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									if (
										!(verbInflection.VerbStem.HastehMazi == "کرد" &&
										  verbInflection.VerbStem.Type == VerbType.SADEH))
									{
										stateList[i - 1].Remove(4);
										stateList[i - 1].Add(34);

									}
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(6))
										stateList[i].Add(6);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(47))
										stateList[i].Add(47);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(9))
										stateList[i].Add(9);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(4);
									stateList[i - 1].Add(34);
								}

							}
						}

						#endregion

						#region state 6

						if (stateList[i - 1].Contains(6))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(39))
									stateList[i].Add(39);
								stateList[i - 1].Remove(6);
							}
						}

						#endregion

						#region state 7

						if (stateList[i - 1].Contains(7))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(40))
									stateList[i].Add(40);
								stateList[i - 1].Remove(7);
							}

							if (verbInflection.VerbStem.HastehMozareh == "هست" &&
								verbInflection.Positivity == TensePositivity.NEGATIVE &&
								verbInflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE &&
								verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
								verbInflection.VerbStem.Type == VerbType.SADEH)
							{
								if (!stateList[i].Contains(40))
									stateList[i].Add(40);
								stateList[i - 1].Remove(7);
							}
						}

						#endregion

						#region state 8

						if (stateList[i - 1].Contains(8))
						{
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(43))
									stateList[i].Add(43);
								stateList[i - 1].Remove(8);
							}
							if (verbInflection.VerbStem.HastehMazi == "بود" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(44))
									stateList[i].Add(44);
								stateList[i - 1].Remove(8);
							}
						}

						#endregion

						#region state 9

						if (stateList[i - 1].Contains(9))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(45))
									stateList[i].Add(45);
								stateList[i - 1].Remove(9);
							}
							else if (verbInflection.VerbStem.HastehMozareh == "باش" &&
								verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(49))
									stateList[i].Add(49);
								stateList[i - 1].Remove(9);
							}
							else if (verbInflection.VerbStem.HastehMozareh == "باش" &&
						   verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
						   verbInflection.VerbStem.Type == VerbType.SADEH &&
						   verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(50))
									stateList[i].Add(50);
								stateList[i - 1].Remove(9);
							}
							else
							{

								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
										  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										  verbInflection.VerbStem.Type == VerbType.SADEH &&
										  verbInflection.Positivity == TensePositivity.POSITIVE))
									{
										if (!stateList[i].Contains(10))
											stateList[i].Add(10);
										stateList[i - 1].Remove(9);
										stateList[i - 1].Add(45);
									}
								}

								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									if (stateList[i - 1].Contains(9))
									{
										stateList[i - 1].Remove(9);
										stateList[i - 1].Add(45);
									}

								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(46))
										stateList[i].Add(46);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}

								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(9))
										stateList[i].Add(9);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(9);
									stateList[i - 1].Add(45);
								}
								if (stateList[i - 1].Contains(9))
								{
									if (!stateList[i - 1].Contains(45))
										stateList[i - 1].Add(45);
									stateList[i - 1].Remove(9);
								}
							}
						}

						#endregion

						#region state -1

						if (stateList[i - 1].Contains(-1))
						{
							if (verbInflection.VerbStem.HastehMozareh == "است" &&
								verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
								verbInflection.VerbStem.Type == VerbType.SADEH &&
								verbInflection.Positivity == TensePositivity.POSITIVE)
							{
								if (!stateList[i].Contains(48))
									stateList[i].Add(48);
								stateList[i - 1].Remove(-1);
							}
							else
							{
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!(verbInflection.VerbStem.HastehMozareh == "است" &&
										  verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
										  verbInflection.VerbStem.Type == VerbType.SADEH &&
										  verbInflection.Positivity == TensePositivity.POSITIVE))
									{
										if (!stateList[i].Contains(10))
											stateList[i].Add(10);
										stateList[i - 1].Remove(-1);
										stateList[i - 1].Add(48);
									}
								}

								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(14))
										stateList[i].Add(14);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(15))
										stateList[i].Add(15);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(17))
										stateList[i].Add(17);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(18))
										stateList[i].Add(18);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(20))
										stateList[i].Add(20);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(46))
										stateList[i].Add(46);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(21))
										stateList[i].Add(21);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs != ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(6))
										stateList[i].Add(6);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد" &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD &&
									verbInflection.Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
								{
									if (!stateList[i].Contains(-1))
										stateList[i].Add(-1);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);

								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi != "شد")
								{
									if (!stateList[i].Contains(47))
										stateList[i].Add(47);

									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.AMR &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(16))
										stateList[i].Add(16);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(19))
										stateList[i].Add(19);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH &&
									verbInflection.VerbStem.HastehMazi == "خواست")
								{
									if (!stateList[i].Contains(2))
										stateList[i].Add(2);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(11))
										stateList[i].Add(11);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.HAAL_ELTEZAMI &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(12))
										stateList[i].Add(12);
								}
								if (verbInflection.TenseForm == TenseFormationType.GOZASHTEH_SADEH &&
									verbInflection.VerbStem.HastehMazi == "شد")
								{
									if (!stateList[i].Contains(13))
										stateList[i].Add(13);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.POSITIVE)
								{
									if (!stateList[i].Contains(1))
										stateList[i].Add(1);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								if (verbInflection.TenseForm == TenseFormationType.PAYEH_MAFOOLI &&
									verbInflection.VerbStem.HastehMazi != "شد" &&
									verbInflection.Positivity == TensePositivity.NEGATIVE)
								{
									if (!stateList[i].Contains(4))
										stateList[i].Add(4);
									stateList[i - 1].Remove(-1);
									stateList[i - 1].Add(48);
								}
								stateList[i - 1].Remove(-1);
							}
						}

						#endregion


					}
					if (stateList[i].Count == 0)
					{
						stateList[i].Add(0);
					}
					if (!stateList[i].Contains(-2))
						tempPishvand = "";
				}
			}
			return stateList;
		}

		private static Dictionary<int, KeyValuePair<string, int>> GetOutputResult(string[] sentence, string[] posSentence, out string[] newPOSTokens, string verbDicPath)
		{
			var posTokens = new List<string>();
			var list = VerbPartTagger.GetGoodResult(sentence, posSentence, verbDicPath);
			var newList = new Dictionary<int, KeyValuePair<string, int>>();
			int counter = 0;
			var tempStr = new StringBuilder();
			for (int i = 0; i < list.Count - 1; i++)
			{
				if (list[i].Count() > 0)
				{
					int value = list[i][0];
					tempStr.Append(sentence[i]);
					newList.Add(counter++, new KeyValuePair<string, int>(tempStr.ToString(), value));
					if (value == 0)
						posTokens.Add(posSentence[i]);
					else
						posTokens.Add("V");
					tempStr = new StringBuilder();
				}
				else
				{
					tempStr.Append(sentence[i] + " ");
				}
			}
			newPOSTokens = posTokens.ToArray();
			return newList;
		}

		private static Dictionary<int, KeyValuePair<string, int>> GetOutputResult(string[] sentence, string verbDicPath)
		{
			var list = GetGoodResult(sentence, verbDicPath);
			var newList = new Dictionary<int, KeyValuePair<string, int>>();
			int counter = 0;
			var tempStr = new StringBuilder();
			for (int i = 0; i < list.Count - 1; i++)
			{
				if (list[i].Count() > 0)
				{
					int value = list[i][0];
					tempStr.Append(sentence[i]);
					newList.Add(counter++, new KeyValuePair<string, int>(tempStr.ToString(), value));
					tempStr = new StringBuilder();
				}
				else
				{
					tempStr.Append(sentence[i] + " ");
				}
			}
			return newList;
		}

		public static Dictionary<int, KeyValuePair<string, VerbInflection>> GetVerbTokens(string[] sentence, string[] posTokens, out string[] newPosTokens, string verbDicPath)
		{
			var outputResults = new Dictionary<int, KeyValuePair<string, VerbInflection>>();
			var output = GetOutputResult(sentence, posTokens, out newPosTokens, verbDicPath);
			for (int i = 0; i < output.Count; i++)
			{
				var values = output[i];
				VerbInflection inflection;
				TensePassivity passivity;
				TensePositivity positivity;
				Verb verb;
				ShakhsType shakhsType;
				TenseFormationType tenseFormationType;
				ZamirPeyvastehType zamirPeyvastehType;
				List<VerbInflection> tempInfleclist;
				VerbInflection tempInflec;
				string[] tokens = output[i].Key.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				switch (values.Value)
				{
					#region 10

					case 10:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 14

					case 14:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 15

					case 15:
						tenseFormationType = TenseFormationType.AMR;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.AMR)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 17

					case 17:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 18

					case 18:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 20

					case 20:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 21

					case 21:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 11

					case 11:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 16

					case 16:
						tenseFormationType = TenseFormationType.AMR;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.AMR)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 12

					case 12:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 13

					case 13:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 19

					case 19:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 22

					case 22:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 23

					case 23:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 24

					case 24:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 25

					case 25:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 26

					case 26:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 27

					case 27:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 2)
						{
							if (tokens[1] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 28

					case 28:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 29

					case 29:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 30

					case 30:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 31

					case 31:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb("", tempInflec.VerbStem.HastehMazi, tempInflec.VerbStem.HastehMozareh,
										tempInflec.VerbStem.Pishvand, "", VerbTransitivity.GOZARA,
										tempInflec.VerbStem.Type, true,
										tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem,
										tempInflec.VerbStem.HastehMaziVowelStart,
										tempInflec.VerbStem.HastehMozarehVowelStart);
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.IsPayehFelMasdari())
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb.HastehMazi = tempInflec.VerbStem.HastehMazi;
						verb.HastehMozareh = tempInflec.VerbStem.HastehMozareh;
						verb.Transitivity = tempInflec.VerbStem.Transitivity;
						verb.AmrShodani = tempInflec.VerbStem.AmrShodani;
						verb.HastehMozarehConsonantVowelEndStem = tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem;
						verb.HastehMaziVowelStart = tempInflec.VerbStem.HastehMaziVowelStart;
						verb.HastehMozarehVowelStart = tempInflec.VerbStem.HastehMozarehVowelStart;
						if (zamirPeyvastehType == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 32

					case 32:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb("", tempInflec.VerbStem.HastehMazi, tempInflec.VerbStem.HastehMozareh,
										tempInflec.VerbStem.Pishvand, "", VerbTransitivity.GOZARA,
										tempInflec.VerbStem.Type, true,
										tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem,
										tempInflec.VerbStem.HastehMaziVowelStart,
										tempInflec.VerbStem.HastehMozarehVowelStart);
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.IsPayehFelMasdari())
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						verb.Transitivity = VerbTransitivity.GOZARA;
						verb.AmrShodani = tempInflec.VerbStem.AmrShodani;
						verb.HastehMozarehConsonantVowelEndStem = tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem;
						verb.HastehMaziVowelStart = tempInflec.VerbStem.HastehMaziVowelStart;
						verb.HastehMozarehVowelStart = tempInflec.VerbStem.HastehMozarehVowelStart;
						if (zamirPeyvastehType == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 34

					case 34:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 38,39

					case 38:
					case 39:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 41

					case 41:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						positivity = tempInflec.Positivity;
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 42

					case 42:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						positivity = tempInflec.Positivity;
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 33

					case 33:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						if (shakhsType == ShakhsType.Shakhs_NONE)
							shakhsType = tempInflec.Shakhs;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 35

					case 35:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 3)
						{
							if (tokens[2] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 36

					case 36:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.POSITIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 37

					case 37:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.POSITIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 40

					case 40:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 3)
						{
							if (tokens[2] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 43

					case 43:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.NEGATIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 44

					case 44:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.NEGATIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 45
					case 45:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;
					#endregion

					#region 46

					case 46:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 47

					case 47:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 48

					case 48:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 49

					case 49:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 50

					case 50:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 51

					case 51:
						tenseFormationType = TenseFormationType.GOZASHTEH_ABAD;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;

						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}

						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 52

					case 52:
						tenseFormationType = TenseFormationType.GOZASHTEH_ABAD;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;

						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region default
					default:
						VerbInflection nullinflec = null;
						if (i == output.Count - 1 && output[i].Value == 1)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 5)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 4)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 7)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();

							tempInfleclist = VerbList.VerbShapes[tokens[1]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.VerbStem.HastehMazi == "شد")
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 6)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 9)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
							verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else
						{
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, nullinflec));
						}
						break;
					#endregion
				}
			}
			return outputResults;
		}

		public static Dictionary<int, KeyValuePair<string, VerbInflection>> GetVerbTokens(string[] sentence, string verbDicPath)
		{
			var outputResults = new Dictionary<int, KeyValuePair<string, VerbInflection>>();
			var output = GetOutputResult(sentence, verbDicPath);
			for (int i = 0; i < output.Count; i++)
			{
				var values = output[i];
				VerbInflection inflection;
				TensePassivity passivity;
				TensePositivity positivity;
				Verb verb;
				ShakhsType shakhsType;
				TenseFormationType tenseFormationType;
				ZamirPeyvastehType zamirPeyvastehType;
				List<VerbInflection> tempInfleclist;
				VerbInflection tempInflec;
				string[] tokens = output[i].Key.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				switch (values.Value)
				{
					#region 10

					case 10:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 14

					case 14:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 15

					case 15:
						tenseFormationType = TenseFormationType.AMR;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.AMR)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 17

					case 17:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 18

					case 18:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 20

					case 20:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 21

					case 21:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						shakhsType = tempInflec.Shakhs;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
													   positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 11

					case 11:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 16

					case 16:
						tenseFormationType = TenseFormationType.AMR;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.AMR)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 12

					case 12:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 13

					case 13:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 19

					case 19:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 22

					case 22:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 23

					case 23:
						tenseFormationType = TenseFormationType.GOZASHTEH_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 24

					case 24:
						tenseFormationType = TenseFormationType.HAAL_SAADEH_EKHBARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH_EKHBARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 25

					case 25:
						tenseFormationType = TenseFormationType.HAAL_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 26

					case 26:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 27

					case 27:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 2)
						{
							if (tokens[1] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 28

					case 28:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 29

					case 29:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 30

					case 30:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 31

					case 31:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb("", tempInflec.VerbStem.HastehMazi, tempInflec.VerbStem.HastehMozareh,
										tempInflec.VerbStem.Pishvand, "", VerbTransitivity.GOZARA,
										tempInflec.VerbStem.Type, true,
										tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem,
										tempInflec.VerbStem.HastehMaziVowelStart,
										tempInflec.VerbStem.HastehMozarehVowelStart);
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.IsPayehFelMasdari())
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb.HastehMazi = tempInflec.VerbStem.HastehMazi;
						verb.HastehMozareh = tempInflec.VerbStem.HastehMozareh;
						verb.Transitivity = tempInflec.VerbStem.Transitivity;
						verb.AmrShodani = tempInflec.VerbStem.AmrShodani;
						verb.HastehMozarehConsonantVowelEndStem = tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem;
						verb.HastehMaziVowelStart = tempInflec.VerbStem.HastehMaziVowelStart;
						verb.HastehMozarehVowelStart = tempInflec.VerbStem.HastehMozarehVowelStart;
						if (zamirPeyvastehType == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 32

					case 32:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb("", tempInflec.VerbStem.HastehMazi, tempInflec.VerbStem.HastehMozareh,
										tempInflec.VerbStem.Pishvand, "", VerbTransitivity.GOZARA,
										tempInflec.VerbStem.Type, true,
										tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem,
										tempInflec.VerbStem.HastehMaziVowelStart,
										tempInflec.VerbStem.HastehMozarehVowelStart);
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.IsPayehFelMasdari())
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb.HastehMazi = "کرد";
						verb.HastehMozareh = "کن";
						verb.Transitivity = VerbTransitivity.GOZARA;
						verb.AmrShodani = tempInflec.VerbStem.AmrShodani;
						verb.HastehMozarehConsonantVowelEndStem = tempInflec.VerbStem.HastehMozarehConsonantVowelEndStem;
						verb.HastehMaziVowelStart = tempInflec.VerbStem.HastehMaziVowelStart;
						verb.HastehMozarehVowelStart = tempInflec.VerbStem.HastehMozarehVowelStart;
						if (zamirPeyvastehType == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 34

					case 34:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 38,39

					case 38:
					case 39:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 41

					case 41:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						positivity = tempInflec.Positivity;
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 42

					case 42:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						positivity = tempInflec.Positivity;
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 33

					case 33:
						tenseFormationType = TenseFormationType.AAYANDEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_ELTEZAMI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						if (shakhsType == ShakhsType.Shakhs_NONE)
							shakhsType = tempInflec.Shakhs;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 35

					case 35:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 3)
						{
							if (tokens[2] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 36

					case 36:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.POSITIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 37

					case 37:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.POSITIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 40

					case 40:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						if (tokens.Length == 3)
						{
							if (tokens[2] == "نیست")
							{
								positivity = TensePositivity.NEGATIVE;
							}
							else
							{
								positivity = TensePositivity.POSITIVE;
							}
						}
						else
						{
							positivity = TensePositivity.POSITIVE;
						}
						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 43

					case 43:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.NEGATIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 44

					case 44:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						tempInfleclist = VerbList.VerbShapes[tokens[2]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.HAAL_SAADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						shakhsType = tempInflec.Shakhs;
						positivity = TensePositivity.NEGATIVE;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 45
					case 45:
						tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;
					#endregion

					#region 46

					case 46:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 47

					case 47:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 48

					case 48:
						tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 49

					case 49:
						tenseFormationType = TenseFormationType.GOZASHTEH_BAEED;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 50

					case 50:
						tenseFormationType = TenseFormationType.GOZASHTEH_ELTEZAMI;
						passivity = TensePassivity.PASSIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");
						shakhsType = tempInflec.Shakhs;
						positivity = tempInflec.Positivity;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 51

					case 51:
						tenseFormationType = TenseFormationType.GOZASHTEH_ABAD;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;

						tempInfleclist = VerbList.VerbShapes[tokens[1]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH)
							{
								tempInflec = inflectionIter;
								break;
							}
						}

						shakhsType = tempInflec.Shakhs;
						zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region 52

					case 52:
						tenseFormationType = TenseFormationType.GOZASHTEH_ABAD;
						passivity = TensePassivity.ACTIVE;
						tempInfleclist = VerbList.VerbShapes[tokens[0]];
						tempInflec = null;
						foreach (VerbInflection inflectionIter in tempInfleclist)
						{
							if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
							{
								tempInflec = inflectionIter;
								break;
							}
						}
						verb = tempInflec.VerbStem.Clone();
						positivity = tempInflec.Positivity;

						shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;

						zamirPeyvastehType = ZamirPeyvastehType.ZamirPeyvasteh_NONE;

						inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
														positivity, passivity);
						outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						break;

					#endregion

					#region default
					default:
						VerbInflection nullinflec = null;
						if (i == output.Count - 1 && output[i].Value == 1)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 5)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 4)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_SADEH;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 7)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();

							tempInfleclist = VerbList.VerbShapes[tokens[1]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.VerbStem.HastehMazi == "شد")
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 6)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI;
							passivity = TensePassivity.ACTIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else if (i == output.Count - 1 && output[i].Value == 9)
						{
							tenseFormationType = TenseFormationType.GOZASHTEH_SADEH;
							passivity = TensePassivity.PASSIVE;
							tempInfleclist = VerbList.VerbShapes[tokens[0]];
							tempInflec = null;
							foreach (VerbInflection inflectionIter in tempInfleclist)
							{
								if (inflectionIter.TenseForm == TenseFormationType.PAYEH_MAFOOLI)
								{
									tempInflec = inflectionIter;
									break;
								}
							}
							verb = tempInflec.VerbStem.Clone();
							positivity = tempInflec.Positivity;
							zamirPeyvastehType = tempInflec.ZamirPeyvasteh;
							verb = new Verb(verb.HarfeEzafeh, "کرد", "کن", verb.Pishvand, verb.Felyar, VerbTransitivity.GOZARA, verb.Type, true, "?", "@", "!");

							shakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
							inflection = new VerbInflection(verb, zamirPeyvastehType, shakhsType, tenseFormationType,
															positivity, passivity);
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, inflection));
						}
						else
						{
							outputResults.Add(i, new KeyValuePair<string, VerbInflection>(output[i].Key, nullinflec));
						}
						break;
					#endregion
				}
			}
			return outputResults;
		}
	}
}
