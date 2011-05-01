using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Verbs;
using YAXLib;

namespace Tagger
{
	[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
	public class Token
	{
		public string Lexeme;
		public PersianPOSTag POSTag;
		public string Lemma;
		public ENUM_TENSE_PERSON Person;
		public NumberType Number;
		public int StartPos;
		public int Length;

		public Token(string lexeme, PersianPOSTag persianPOSTag, string lemma)
		{
			Lexeme = lexeme;
			POSTag = persianPOSTag;
			Lemma = lemma;
		}

		public Token()
		{

		}
		public Token(string lexeme, PersianPOSTag persianPOSTag, string lemma, NumberType numberType, int length, int startPos, ENUM_TENSE_PERSON person)
		{
			Lexeme = lexeme;
			POSTag = persianPOSTag;
			Lemma = lemma;

			Number = numberType;
			Length = length;
			Lemma = lemma;
			Lexeme = lexeme;
			Person = person;
			POSTag = persianPOSTag;
			StartPos = startPos;
		}
	}

	public enum NumberType
	{
		SINGULAR,
		PLURAL,
		INVALID
	}
}
