using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	class Word
	{
		public int num;
		public string lexeme;
		public string lemma;
		public string cpos;
		public string fpos;
		public string person;
		public string number;
		public string tma = null; //tense mood aspect
		public int parentId = -1;
		public string parentRelation = null;

		public override string ToString()
		{
			StringBuilder result = new StringBuilder(200);
			result.Append("word: ").Append(lexeme).Append(" | ");
			result.Append("position: ").Append(num).Append(" | ");
			result.Append("lemma: ").Append(lemma).Append(" | ");
			result.Append("cpos: ").Append(cpos).Append(" | ");
			result.Append("parent: ").Append(parentId);
			return result.ToString();
		}
	}
}
