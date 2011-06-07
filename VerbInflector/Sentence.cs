using System;
using System.Collections.Generic;
using System.Text;

namespace VerbInflector
{
	public class Sentence
	{
		List<Word> words;
		
		public Sentence()
		{
			words = new List<Word>(60);
		}

		public string[] getLexeme(){
			string[] sentence;
			List<string> al = new List<string>(this.words.Count);
			
			for(int i = 0; i < this.words.Count; i++){
				al.Add((this.words[i]).lexeme);
			}
			sentence = al.ToArray();

			return sentence;
		}

		public string[] getPOSTag(){
			string[] postag;
			List<string> al = new List<string>(this.words.Count);
			for (int i = 0; i < this.words.Count; i++)
			{
				al.Add((this.words[i]).cpos);
			}
			postag =al.ToArray();

			return postag;
		}

		public Word[] getWords(){
			return words.ToArray();
		}

		public Word getWord(int index)
		{
			return words[index];
		}

		public void addWord(Word w)
		{
			words.Add(w);
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder(500);

			for(int i = 0; i < words.Count; i++)
			{
				result.Append(words[i].lexeme).Append(" ");
			}

			return result.ToString();
		}

	}
}
