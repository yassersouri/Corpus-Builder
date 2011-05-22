﻿using System;
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
		public string postag;
		public string person;
		public string number;
		public int parentId = -1;
		public string parentRelation = null;
	}
}