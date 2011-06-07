using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	public class Word : IEquatable<Word>
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

		public bool Equals(Word other)
		{
			if (this.num != other.num) return false;
			if (this.lexeme != other.lexeme) return false;
			if (this.lemma != other.lemma) return false;
			if (this.cpos != other.cpos) return false;
			if (this.fpos != other.fpos) return false;
			if (this.person != other.person) return false;
			if (this.number != other.number) return false;
			if (this.tma != other.tma) return false;
			if (this.parentId != other.parentId) return false;
			if (this.parentRelation != other.parentRelation) return false;
			return true;
		}
	}
}
