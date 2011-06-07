using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class SentenceAnalyzer
	{
		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			string[] outpos;
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
															   mfeat);
				tree.Add(dependencyToken);
			}
			return tree;
		}
		public static List<DependencyBasedToken> MakePartialDependencyTree(string[] sentence, string[] posSentence, string verbDicPath)
		{
			var tree = new List<DependencyBasedToken>();
			string[] outpos;
			var dic = VerbPartTagger.MakePartialTree(sentence, posSentence, out outpos, verbDicPath);
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

				var mfeat = new MorphoSyntacticFeatures(number, person, tma);
				var dependencyToken = new DependencyBasedToken(position, wordForm, lemma, outpos[position - 1], "_", head, deprel, wordCount,
															   mfeat);
				tree.Add(dependencyToken);
			}
			return tree;
		}


		public static VerbBasedSentence MakeVerbBasedSentence(string[] sentence, string[] posSentence, string verbDicPath)
		{
			return new VerbBasedSentence(MakePartialDependencyTree(sentence, posSentence, verbDicPath));
		}

		public static VerbBasedSentence MakeVerbBasedSentence(string[] sentence, string verbDicPath)
		{
			return new VerbBasedSentence(MakePartialDependencyTree(sentence, verbDicPath));
		}
	}
}
