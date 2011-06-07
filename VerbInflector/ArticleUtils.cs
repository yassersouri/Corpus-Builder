using System;
using System.IO;

namespace VerbInflector
{
	public class ArticleUtils
	{
		public static Article getArticle(string file)
		{
			Article article = new Article();

			StreamReader sr = new StreamReader(file);
			string content = sr.ReadToEnd();
			sr.Close();

			addSentencesToArticle(content, article);


			return article;
		}

		public static void putArticle(string file)
		{
			
		}

		private static void addSentencesToArticle(string file, Article article)
		{
			Sentence current = new Sentence();
			string sentence = null;
			string[] words = file.Split('\n');
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] == "")
				{
					//now we are at the end of a sentence
					if (sentence != null)
					{
						addWordsToSentence(sentence, current);
						sentence = null;
						article.addSentence(current); //add sentence to the article
						current = new Sentence(); //empty the sentence;
					}
				}
				else
				{
					sentence += (words[i] + '\n'.ToString());
				}
			}
			if (sentence != null) //maybe the last sentence
			{
				addWordsToSentence(sentence, current);
				article.addSentence(current);
			}
		}

		private static void addWordsToSentence(string sentenceString, Sentence sentence)
		{
			Word current;
			string word = null;
			string[] words = sentenceString.Split('\n');
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] == "")
				{
					break;
				}
				else
				{
					word = words[i];
					current = getWord(word);
					sentence.addWord(current);
				}
			}
		}

		private static Word getWord(string word)
		{
			Word result = new Word();

			string[] parts = word.Split('\t');

			if (parts.Length < 7) throw new Exception("Word is not recognized!");
			for (int i = 0; i < parts.Length; i++) if (parts[i] == "") throw new Exception("Word part is empty");
			
			//the first 7 elements are mandatory and must exist
			result.num = Convert.ToInt32(parts[0]);
			result.lexeme = parts[1];
			result.lemma = parts[2];
			result.cpos = parts[3];
			result.fpos = parts[4];
			result.person = parts[5];
			result.number = parts[6];

			if(parts.Length > 7)
				result.tma = parts[7];

			if(parts.Length > 8)
				result.parentId = Convert.ToInt32(parts[8]);

			if(parts.Length > 9)
				result.parentRelation = parts[9];


			return result;
		}
	}
}
