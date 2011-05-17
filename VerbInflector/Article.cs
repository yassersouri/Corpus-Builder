using System;
using System.Collections;

namespace VerbInflector
{
	class Article
	{
		ArrayList sentences;

		public Article(){
			sentences = new ArrayList(10);
		}

		public void addSentence(Sentence s){
			sentences.Add(s);
		}

		public Sentence[] getSentences(){
			sentences.TrimToSize();
			return (Sentence[]) sentences.ToArray();
		}

		public Sentence getSentence(int index)
		{
			return (Sentence) sentences[index];
		}
	}
}
