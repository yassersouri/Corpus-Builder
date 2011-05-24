using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class DependencyBasedToken
	{
		public DependencyBasedToken(int pos, string word, string lemm, string cpos, string fpos, int head, string depRel, int wCount, MorphoSyntacticFeatures feats)
		{
			Position = pos;
			WordForm = word;
			Lemma = lemm;
			CPOSTag = cpos;
			FPOSTag = fpos;
			HeadNumber = head;
			DependencyRelation = depRel;
			TokenCount = wCount;
			MorphoSyntacticFeats = feats;
		}

		public int Position { set; get; }
		public string WordForm { set; get; }
		public string Lemma { set; get; }
		public string CPOSTag { set; get; }
		public string FPOSTag { set; get; }
		public int HeadNumber { set; get; }
		public string DependencyRelation { set; get; }
		public int TokenCount { set; get; }
		public MorphoSyntacticFeatures MorphoSyntacticFeats { set; get; }
	}

	public class MorphoSyntacticFeatures
	{
		public MorphoSyntacticFeatures(NumberType num, ShakhsType pers, TenseFormationType tma)
		{
			Number = num;
			Person = pers;
			TenseMoodAspect = tma;
		}
		public NumberType Number { set; get; }
		public ShakhsType Person { set; get; }
		public TenseFormationType TenseMoodAspect { set; get; }
	}
}
