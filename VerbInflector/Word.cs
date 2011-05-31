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
	}
}
