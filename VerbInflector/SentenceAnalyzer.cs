﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerbInflector;

namespace VerbInflector
{
	public class SentenceAnalyzer
	{
		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			var dic = VerbPartTagger.MakePartialTree(sentence, verbDicPath);
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
															   mfeat, Chasbidegi.TANHA);
				tree.Add(dependencyToken);
			}
			return tree;
		}

		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string[] posSentence, string[] lemmas, MorphoSyntacticFeatures[] morphoSyntacticFeatureses, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			string[] outpos;
			var dic = VerbPartTagger.MakePartialTree(sentence, posSentence, out outpos, lemmas, verbDicPath);
			int indexOfOriginalWords = 0;
			bool addZamir = false;
			string zamirString = "";
			NumberType ZamirNumberType = NumberType.INVALID;
			ShakhsType ZamirShakhsType = ShakhsType.Shakhs_NONE;
			string zamirLemma = "";
			int offset = 0;
			int realPosition = 0;
			foreach (KeyValuePair<int, KeyValuePair<string, KeyValuePair<int, object>>> keyValuePair in dic)
			{
				addZamir = false;
				zamirString = "";
				ZamirShakhsType = ShakhsType.Shakhs_NONE;
				ZamirNumberType = NumberType.INVALID;
				zamirLemma = "";
				realPosition = keyValuePair.Key + 1;
				int position = keyValuePair.Key + 1 + offset;
				string wordForm = keyValuePair.Value.Key;
				int head = keyValuePair.Value.Value.Key;
				string deprel = "_";
				object obj = keyValuePair.Value.Value.Value;
				string lemma = "_";
				string FPOS = "_";
				int wordCount = wordForm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
				ShakhsType person = ShakhsType.Shakhs_NONE;
				NumberType number = NumberType.INVALID;
				TenseFormationType tma = TenseFormationType.TenseFormationType_NONE;

				if (wordCount == 1)
				{
					lemma = lemmas[indexOfOriginalWords];
					person = morphoSyntacticFeatureses[indexOfOriginalWords].Person;
					number = morphoSyntacticFeatureses[indexOfOriginalWords].Number;
					tma = morphoSyntacticFeatureses[indexOfOriginalWords].TenseMoodAspect;
				}
				indexOfOriginalWords += wordCount;

				if (obj is VerbInflection)
				{
					var newObj = (VerbInflection)obj;
					tma = newObj.TenseForm;
					var personType = newObj.Shakhs;
					if (newObj.Passivity == TensePassivity.ACTIVE)
					{
						FPOS = "ACT";
					}
					else
					{
						FPOS = "PASS";
					}
					if (newObj.ZamirPeyvasteh != ZamirPeyvastehType.ZamirPeyvasteh_NONE)
					{
						addZamir = true;
						zamirString = newObj.ZamirPeyvastehString;
						offset++;
						switch (newObj.ZamirPeyvasteh)
						{
							case ZamirPeyvastehType.AVALSHAKHS_JAM:
								ZamirNumberType = NumberType.PLURAL;
								ZamirShakhsType = ShakhsType.AVALSHAKHS_JAM;
								zamirLemma = "مان";
								break;
							case ZamirPeyvastehType.AVALSHAKHS_MOFRAD:
								ZamirNumberType = NumberType.SINGULAR;
								ZamirShakhsType = ShakhsType.AVALSHAKHS_MOFRAD;
								zamirLemma = "م";
								break;
							case ZamirPeyvastehType.DOVVOMSHAKHS_JAM:
								ZamirNumberType = NumberType.PLURAL;
								ZamirShakhsType = ShakhsType.DOVVOMSHAKHS_JAM;
								zamirLemma = "تان";
								break;
							case ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD:
								ZamirNumberType = NumberType.SINGULAR;
								ZamirShakhsType = ShakhsType.DOVVOMSHAKHS_MOFRAD;
								zamirLemma = "ت";
								break;
							case ZamirPeyvastehType.SEVVOMSHAKHS_JAM:
								ZamirNumberType = NumberType.PLURAL;
								ZamirShakhsType = ShakhsType.SEVVOMSHAKHS_JAM;
								zamirLemma = "شان";
								break;
							case ZamirPeyvastehType.SEVVOMSHAKHS_MOFRAD:
								ZamirNumberType = NumberType.SINGULAR;
								ZamirShakhsType = ShakhsType.SEVVOMSHAKHS_MOFRAD;
								zamirLemma = "ش";
								break;
						}
					}
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
					else if (newObj == "MOSTAMAR_SAAZ_HAAL" || newObj == "MOSTAMAR_SAAZ_GOZASHTEH")
					{
						deprel = "PROG";
						lemma = "داشت#دار";
						var headObj = (VerbInflection)dic.ElementAt(head).Value.Value.Value;
						person = headObj.Shakhs;
						number = NumberType.SINGULAR;
						var personType = headObj.Shakhs;
						if (personType == ShakhsType.AVALSHAKHS_JAM || personType == ShakhsType.DOVVOMSHAKHS_JAM || personType == ShakhsType.SEVVOMSHAKHS_JAM)
						{
							number = NumberType.PLURAL;
						}
						tma = TenseFormationType.HAAL_SAADEH;
						if (newObj == "MOSTAMAR_SAAZ_GOZASHTEH")
							tma = TenseFormationType.GOZASHTEH_SADEH;
					}
				}
				if (!addZamir)
				{
					var mfeat = new MorphoSyntacticFeatures(number, person, tma);
					var dependencyToken = new DependencyBasedToken(position, wordForm, lemma, outpos[realPosition - 1], FPOS,
																   head, deprel, wordCount,
																   mfeat, Chasbidegi.TANHA);
					tree.Add(dependencyToken);
				}
				else
				{
					var mfeat1 = new MorphoSyntacticFeatures(number, person, tma);
					var mfeat2 = new MorphoSyntacticFeatures(ZamirNumberType, ZamirShakhsType, TenseFormationType.TenseFormationType_NONE);
					var dependencyToken1 = new DependencyBasedToken(position, wordForm.Substring(0, wordForm.Length - zamirString.Length), lemma, outpos[realPosition - 1], FPOS,
																   head, deprel, wordCount,
																   mfeat1, Chasbidegi.NEXT);
					var dependencyToken2 = new DependencyBasedToken(position + 1, zamirString, zamirLemma, "CPR", "CPR",
																   position, "OBJ", 1,
																   mfeat2, Chasbidegi.PREV);
					tree.Add(dependencyToken1);
					tree.Add(dependencyToken2);
				}

			}
			return tree;
		}

		public static VerbBasedSentence MakeVerbBasedSentence(string[] sentence, string[] posSentence, string[] lemmas, MorphoSyntacticFeatures[] morphoSyntacticFeatureses, string verbDicPath)
		{
			return new VerbBasedSentence(MakePartialDependencyTree(sentence, posSentence, lemmas, morphoSyntacticFeatureses, verbDicPath));
		}

		public static VerbBasedSentence MakeVerbBasedSentence(string[] sentence, string verbDicPath)
		{
			return new VerbBasedSentence(MakePartialDependencyTree(sentence, verbDicPath));
		}
	}
}
