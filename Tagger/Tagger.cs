using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Lemmatization;
using SCICT.NLP.Utility.Verbs;

namespace Tagger
{
	class Tagger
	{
		public Dictionary<string, PersianPOSTag> Mapper;
		private readonly PersianSuffixRecognizer _lemmatizer;
		private static Dictionary<string, KeyValuePair<string, PersianPOSTag>> m_LemmaDic;
		//public static Finder Finder;

		public Tagger()
		{
			var dic = new Dictionary();
			dic.LoadVerbEntries(@"../../FarsiVerbs.dat");
			//if (Finder == null)
			//    Finder = new Finder(dic, ENUM_FIND_MODE.DATABASE);
			m_LemmaDic = new Dictionary<string, KeyValuePair<string, PersianPOSTag>>();

			_lemmatizer = new PersianSuffixRecognizer(false);
			Mapper = new Dictionary<string, PersianPOSTag>();
			const string mapperfile = @"../../words.dat";

			var reader = new StreamReader(mapperfile);
			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				string[] parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				Mapper.Add(parts[0].Trim(), (PersianPOSTag)Enum.Parse(typeof(PersianPOSTag), parts[1].Trim()));
			}
			reader.Close();
		}
		public Token TaggerHelper(string sentenceToken)
		{
			Token result;

			int pos = 0;
			var lexeme = sentenceToken;

			PersianPOSTag posTag;
			string lemma = GetLemma(lexeme, out posTag);

			Token token = Mapper.ContainsKey(lexeme) ? (new Token(lexeme, Mapper[lexeme], lexeme)) : (new Token(lexeme, posTag, lemma));
			token.Person = ENUM_TENSE_PERSON.INVALID;
			token.Number = NumberType.INVALID;
			if (token.POSTag == PersianPOSTag.PRO)
			{
				if (token.Lexeme == "من")
				{
					token.Person = ENUM_TENSE_PERSON.SINGULAR_FIRST;
					token.Number = NumberType.SINGULAR;
				}
				else if (token.Lexeme == "وی")
				{
					token.Person = ENUM_TENSE_PERSON.SINGULAR_FIRST;
					token.Number = NumberType.SINGULAR;
				}
				else if (token.Lexeme == "تو")
				{
					token.Person = ENUM_TENSE_PERSON.SINGULAR_SECOND;
					token.Number = NumberType.SINGULAR;
				}
				else if (token.Lexeme == "او")
				{
					token.Person = ENUM_TENSE_PERSON.SINGULAR_THIRD;
					token.Number = NumberType.SINGULAR;
				}
				else if (token.Lexeme == "ما")
				{
					token.Person = ENUM_TENSE_PERSON.PLURAL_FIRST;
					token.Number = NumberType.PLURAL;
				}
				else if (token.Lexeme == "شما")
				{
					token.Person = ENUM_TENSE_PERSON.PLURAL_SECOND;
					token.Number = NumberType.PLURAL;
				}
				else if (token.Lexeme == "ایشان")
				{
					token.Person = ENUM_TENSE_PERSON.PLURAL_THIRD;
					token.Number = NumberType.PLURAL;
				}
			}
			if (token.POSTag == PersianPOSTag.N)
			{
				var suffix = token.Lexeme.Replace(token.Lemma, "").Trim();
				if (((_lemmatizer.SuffixCategory(suffix) & PersianSuffixesCategory.PluralSignAan) == PersianSuffixesCategory.PluralSignAan) ||
					((_lemmatizer.SuffixCategory(suffix) & PersianSuffixesCategory.PluralSignHaa) == PersianSuffixesCategory.PluralSignHaa) ||
					(lexeme.Length > 4 && suffix == "های") ||
					(lexeme.Length > 4 && suffix == "انی") ||
					(lexeme.Length > 5 && suffix == "هایی") ||
					(lexeme.Length > 5 && suffix == "هائی")
					)
				{
					token.Number = NumberType.PLURAL;
				}
				else
				{
					token.Number = NumberType.SINGULAR;
				}


			}
			token.StartPos = pos;
			token.Length = lexeme.Length;
			pos += token.Length + 1;
			result = token;

			return result;
		}

		// In fact, it's just a simple stemmer, not a _lemmatizer
		private string GetLemma(string token, out PersianPOSTag posTag)
		{
			string lemma = token;
			if (m_LemmaDic.ContainsKey(token))
			{
				KeyValuePair<string, PersianPOSTag> valuePair = m_LemmaDic[token];
				posTag = valuePair.Value;
				return valuePair.Key;
			}
			posTag = PersianPOSTag.UserPOS;

			var rpmpis = _lemmatizer.MatchForSuffix(token);
			for (int index = rpmpis.Length - 1; index >= 0; index--)
			{
				var rpmpi = rpmpis[index];
				if (Mapper.ContainsKey(rpmpi.BaseWord))
				{
					lemma = rpmpi.BaseWord;

					// Manual rules go here, probably from a seprate fromatted file
					if (Mapper[rpmpi.BaseWord] == PersianPOSTag.N && rpmpi.Suffix == "ی")
					{
						posTag = PersianPOSTag.AJ;
						break;
					}

					posTag = Mapper[rpmpi.BaseWord];
					break;
				}
				PersianPOSTag possibletags = _lemmatizer.AcceptingPOS(_lemmatizer.SuffixCategory(rpmpi.Suffix));
				posTag = GetMostFrequent(possibletags);
			}
			m_LemmaDic.Add(token, new KeyValuePair<string, PersianPOSTag>(lemma, posTag));
			return lemma;
		}

		private static PersianPOSTag GetMostFrequent(PersianPOSTag possibletags)
		{
			// In order of frequency
			if ((possibletags & PersianPOSTag.N) == PersianPOSTag.N)
				return PersianPOSTag.N;
			if ((possibletags & PersianPOSTag.P) == PersianPOSTag.P)
				return PersianPOSTag.P;
			if ((possibletags & PersianPOSTag.PUNC) == PersianPOSTag.PUNC)
				return PersianPOSTag.PUNC;
			if ((possibletags & PersianPOSTag.V) == PersianPOSTag.V)
				return PersianPOSTag.V;
			if ((possibletags & PersianPOSTag.AJ) == PersianPOSTag.AJ)
				return PersianPOSTag.AJ;
			if ((possibletags & PersianPOSTag.CONJ) == PersianPOSTag.CONJ)
				return PersianPOSTag.CONJ;
			if ((possibletags & PersianPOSTag.NUM) == PersianPOSTag.NUM)
				return PersianPOSTag.NUM;
			if ((possibletags & PersianPOSTag.NUM) == PersianPOSTag.NUM)
				return PersianPOSTag.NUM;

			return PersianPOSTag.UserPOS;
		}
	}
}
