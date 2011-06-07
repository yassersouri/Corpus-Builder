using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VerbInflector
{
	class Program
	{
		static void Main(string[] args)
		{
			string verbDicPath = "../../VerbList.txt";

			string sourceDir = "D:\\sample\\corpus_tagged\\";
			string destinationDir = sourceDir + "again\\";
			string file = "1088245.txt";

			Directory.CreateDirectory(destinationDir);
			string sourceFile = sourceDir + file;
			string destinationFile = destinationDir + file;

			VerbInflector_OneFile(sourceFile, verbDicPath);
		}

		

		public static void VerbInflector_OneFile(string sourceFile, string verbDicPath){
			Article currentArticle = ArticleUtils.getArticle(sourceFile);

			Article newArticle = new Article();

			Sentence currentSentence = null;
			Sentence newSentence = null;
			String[] currentLexemes = null;
			String[] currentPOSTags = null;
			for(int i = 0; i < currentArticle.getSentences().Length; i++)
			{
				//initialize the new sentence
				newSentence = new Sentence();

				//load the current sentence
				currentSentence = currentArticle.getSentence(i);
				currentLexemes = currentSentence.getLexeme();
				currentPOSTags = currentSentence.getPOSTag();

				//test input, 
				string[] testSentence = "به گردش درآمده است این مسائل".Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] testPos = "P N V V PR N".Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				//currentLexemes = testSentence;
				//currentPOSTags = testPos;

				//MakePartialDependencyTree for the current sentence.
				List<DependencyBasedToken> list = VerbPartTagger.MakePartialDependencyTree(currentLexemes, currentPOSTags, verbDicPath);

				//word index pointer in the current sentence
				int sentenceIndex = 0;
				
				Word currentWord = null;
				Word newWord = null;
				DependencyBasedToken currentDBT;
				for(int j = 0; j < list.Count() ; j++)
				{
					currentDBT = list[j];

					newWord = new Word();

					newWord.num = currentDBT.Position;
					newWord.lexeme = currentDBT.WordForm;
					if(currentDBT.Lemma != "_")
					{
						//if lemma is changed
						newWord.lemma = currentDBT.Lemma;
						newWord.cpos = currentDBT.CPOSTag;
						newWord.fpos = currentDBT.FPOSTag;
						newWord.person = currentDBT.MorphoSyntacticFeats.Person.ToString();
						newWord.number = currentDBT.MorphoSyntacticFeats.Number.ToString();
						newWord.tma = currentDBT.MorphoSyntacticFeats.TenseMoodAspect.ToString();
						newWord.parentId = currentDBT.HeadNumber;
						newWord.parentRelation = currentDBT.DependencyRelation;
					}
					else{
						//lemma is unchanged
						currentWord = currentSentence.getWord(sentenceIndex);
						newWord.lemma = currentWord.lemma;
						newWord.cpos = currentWord.cpos;
						newWord.fpos = currentWord.fpos;
						newWord.person = currentWord.person;
						newWord.number = currentWord.number;
						newWord.tma = "_";
						newWord.parentId = currentWord.parentId;
						newWord.parentRelation = currentWord.parentRelation;
					}
					newSentence.addWord(newWord);
					sentenceIndex += currentDBT.TokenCount;
				}

				newArticle.addSentence(newSentence);

				System.Console.WriteLine(list);

			}
		}
	}
}
