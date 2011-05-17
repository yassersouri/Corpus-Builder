using System;
using System.Collections;

namespace VerbInflector
{
	class Sentence
	{
		ArrayList words;
		
		public Sentence()
		{
			words = new ArrayList(60);
		}

		public string[] getLexeme(){
			string[] sentence;
			ArrayList al = new ArrayList(this.words.Count);
			
			for(int i = 0; i < this.words.Count; i++){
				al.Add(((Word)this.words[i]).lexeme);
			}
			sentence = (string[]) al.ToArray();

			return sentence;
		}

		public string[] getPOSTag(){
			string[] postag;
			ArrayList al = new ArrayList(this.words.Count);
			for (int i = 0; i < this.words.Count; i++)
			{
				al.Add(((Word)this.words[i]).postag);
			}
			postag = (string[])al.ToArray();

			return postag;
		}

		public Word[] getWords(){
			return (Word[]) words.ToArray();
		}

		public Word getWord(int index)
		{
			return (Word) words[index];
		}

		public void addWord(Word w)
		{
			words.Add(w);
		}

	}
}
