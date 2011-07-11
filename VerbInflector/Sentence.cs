using System;
using System.Collections.Generic;
using System.Text;

namespace VerbInflector
{
	public class Sentence : IEquatable<Sentence>
	{
		List<Word> words;
		
		public Sentence()
		{
			words = new List<Word>(60);
		}

		public string[] getLexemes(){
			string[] lexemes;
			List<string> al = new List<string>(this.words.Count);
			
			for(int i = 0; i < this.words.Count; i++){
				al.Add((this.words[i]).lexeme);
			}
			lexemes = al.ToArray();

			return lexemes;
		}

		public string[] getPOSTags(){
			string[] postag;
			List<string> al = new List<string>(this.words.Count);
			for (int i = 0; i < this.words.Count; i++)
			{
				al.Add((this.words[i]).cpos);
			}
			postag =al.ToArray();

			return postag;
		}

		public string[] getLemmas()
		{
			string[] lemmalist;
			List<string> al = new List<string>(this.words.Count);

			for (int i = 0; i < this.words.Count; i++)
			{
				al.Add((this.words[i]).lemma);
			}
			lemmalist = al.ToArray();

			return lemmalist;
		}

		public MorphoSyntacticFeatures[] getFeatures()
		{
			MorphoSyntacticFeatures[] featurelist;
			List<MorphoSyntacticFeatures> al = new List<MorphoSyntacticFeatures>(this.words.Count);

			for (int i = 0; i < this.words.Count; i++)
			{
				al.Add(new MorphoSyntacticFeatures(this.words[i].number, this.words[i].person, this.words[i].tma));
			}
			featurelist = al.ToArray();

			return featurelist;
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

		public bool Equals(Sentence other)
		{
			if(this.words.Count != other.words.Count) return false;

			for(int i = 0; i < this.words.Count; i++)
			{
				if(!this.getWord(i).Equals(other.getWord(i))) return false;
			}

			return true;
		}
	}
}
