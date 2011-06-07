using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerbInflector;

namespace VerbInflector
{
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
