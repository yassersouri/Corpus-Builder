using System;
using System.Collections.Generic;

namespace VerbInflector
{
	public class Article
	{
		List<Sentence> sentences;

		public Article(){
			sentences = new List<Sentence>(10);
		}

		public void addSentence(Sentence s){
			sentences.Add(s);
		}

		public Sentence[] getSentences(){
			sentences.TrimExcess();
			return sentences.ToArray();
		}

		public Sentence getSentence(int index)
		{
			return sentences[index];
		}
	}
}
