using System;
using System.Collections.Generic;

namespace VerbInflector
{
	public class Article : IEquatable<Article>
	{
		List<Sentence> sentences;
		long articleNumber;

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

		public bool Equals(Article other)
		{
			if(this.sentences.Count != other.sentences.Count) return false;

			for(int i = 0; i < this.sentences.Count; i++)
			{
				if(!this.getSentence(i).Equals(other.getSentence(i))) return false;
			}

			return true;
		}

		public void setArticleNumber(long articleNumber)
		{
			this.articleNumber = articleNumber;
		}

		public long getArticleNumber()
		{
			return this.articleNumber;
		}
	}
}
