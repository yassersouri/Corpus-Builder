using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VerbInflector;
using SentenceRecognizer;

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

			string sourceFile = sourceDir + file;
			string destinationFile = destinationDir + file;
			ValencyDicManager.RefreshVerbList("../../VerbList.txt", "../../valency list.txt");
						
			VerbInflector_OneFile(sourceFile, destinationFile, verbDicPath);
		}
		

		public static void VerbInflector_OneFile(string sourceFile, string destinationFile, string verbDicPath){
			Article currentArticle = ArticleUtils.getArticle(sourceFile);

			Article newArticle = generateNewArticle(currentArticle, verbDicPath);

			ArticleUtils.putArticle(newArticle, destinationFile);

			System.Console.WriteLine(newArticle);
		}

		private static Article generateNewArticle(Article currentArticle, string verbDicPath)
		{
			Article newArticle = new Article();

			Sentence currentSentence = null;
			Sentence newSentence = null;
			String[] currentLexemes = null;
			String[] currentPOSTags = null;
			String[] currentLemmas = null;
			MorphoSyntacticFeatures[] currentFeatures = null;

			for(int i = 0; i < currentArticle.getSentences().Length; i++) //for each sentence in this article.
			{
				//initialize the new sentence
				newSentence = new Sentence();

				//load the current sentence
				currentSentence = currentArticle.getSentence(i);
				currentLexemes = currentSentence.getLexemes();
				currentPOSTags = currentSentence.getPOSTags();
				currentLemmas = currentSentence.getLemmas();
				currentFeatures = currentSentence.getFeatures();

				//test input, 
				string[] testSentence = "به گردش درآمده است این مسائل".Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] testPos = "P N V V PR N".Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				//currentLexemes = testSentence; currentPOSTags = testPos;

				//MakePartialDependencyTree for the current sentence.
				List<DependencyBasedToken> list;
				//list = VerbPartTagger.MakePartialDependencyTree(currentLexemes, currentPOSTags, verbDicPath);
				
				VerbBasedSentence vbs = SentenceAnalyzer.MakeVerbBasedSentence(currentLexemes, currentPOSTags, currentLemmas, currentFeatures, verbDicPath);
				list = vbs.SentenceTokens;


				////////////////////////////////////
				//           TEST CODE
				////////////////////////////////////

				foreach(var verbInSentence in vbs.VerbsInSentence)
				{
					//special string representation of the verb
					String currentVerbString = ValencyDicManager.GetVerbString(ref vbs, verbInSentence); 

					List<BaseStructure> basestruct = new List<BaseStructure>();
					if (ValencyDicManager.BaseStrucDic.ContainsKey(currentVerbString))
					{
						basestruct = ValencyDicManager.BaseStrucDic[currentVerbString];
						foreach (var baseStructure in basestruct)
						{
							bool istrue = baseStructure.Satisfy(vbs, verbInSentence);
							//if true then 
						}
					}
				}

				////////////////////////////////////
				
				//Counting the verbs
				//++++++++++++++++++++++++++++++++

				//++++++++++++++++++++++++++++++++
				//\\Counting the verbs

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

						if(currentDBT.MorphoSyntacticFeats.Person != ShakhsType.Shakhs_NONE)
							newWord.person = currentDBT.MorphoSyntacticFeats.Person.ToString();
						else
							newWord.person = "_";

						if(currentDBT.MorphoSyntacticFeats.Number != NumberType.INVALID)
							newWord.number = currentDBT.MorphoSyntacticFeats.Number.ToString();
						else
							newWord.number = "_";
						
						newWord.tma = currentDBT.MorphoSyntacticFeats.TenseMoodAspect.ToString();
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
					}

					//are changed any way
					newWord.parentId = currentDBT.HeadNumber;
					newWord.parentRelation = currentDBT.DependencyRelation;

					newSentence.addWord(newWord);
					sentenceIndex += currentDBT.TokenCount;
				}
				newArticle.addSentence(newSentence);
			}

			return newArticle;
		}

	}
}
