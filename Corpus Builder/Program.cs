using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.Utility;
using SCICT.NLP.Persian;
using System.IO;
using VerbInflector;

namespace Corpus_Builder
{
    class Program
    {
        static void Main(string[] args)
        {
			//try
			//{
			//    string dir = "D:\\sample\\";
			//    string destinationDir = dir + "corpus\\";

			//    RefineAllFiles(dir);
			//    SeparateAllSentencesAndWords(dir,destinationDir);
				
			//}
			//catch (Exception e)
			//{
			//    Console.WriteLine("The file could not be read:");
			//    Console.WriteLine(e.Message);
			//}


			string verbDicPath = "../../VerbList.txt";

			string sourceDir = "D:\\sample\\corpus_tagged\\";
			string destinationDir = sourceDir + "again\\";
			string file = "1088245.txt";

			Directory.CreateDirectory(destinationDir);
			string sourceFile = sourceDir + file;
			string destinationFile = destinationDir + file;

			VerbInflector_OneFile(sourceFile, verbDicPath);
        }

		static WordPatternInfo[] SeparateWords(string sentence){
			
			WordTokenizerOptions wto = WordTokenizerOptions.ReturnPunctuations;
			WordTokenizer st = new WordTokenizer(wto);
			
			WordPatternInfo[] wpi = st.ExtractWords(sentence).ToArray();
			return wpi;
		}

		static void SeparateAllSentencesAndWords(String sourceDir, String destinationDir){
			string[] sourcefiles = Directory.GetFiles(sourceDir);
			Directory.CreateDirectory(destinationDir);

			string destinationfile;
			string sourcefile;
			int lastIndex;
			string fileName;
			int len;

			for(int i = 0; i < sourcefiles.Length; i++)
			{	
				sourcefile = sourcefiles[i];

				lastIndex = sourcefile.LastIndexOf('\\');
				len = sourcefile.Length;
				fileName = sourcefile.Substring(lastIndex+1, len-lastIndex-1);

				destinationfile = destinationDir + fileName;

				SeparateSentencesAndWords(sourcefile, destinationfile);

				Console.WriteLine("Extracted sentences of: " + sourcefile);
			}

		}

		static void SeparateSentencesAndWords(String sourceFile, String destinationFile)
		{
			StreamReader sr = new StreamReader(sourceFile);
			String line = sr.ReadToEnd();
			sr.Close();
			String[] sentences;
			sentences = StringUtil.ExtractPersianSentences(line);
			StreamWriter sw = new StreamWriter(destinationFile);
			string sentence;
			WordPatternInfo[] wpi;
			for(int i = 0; i < sentences.Length; i++)
			{
				sentence = sentences[i];
				wpi = SeparateWords(sentence);

				for(int j = 0; j < wpi.Length; j++)
				{
					sw.Write((j+1) + "\t");
					sw.Write(wpi[j].Word + "\n");
				}

				sw.Write("\n");
			}
			sw.Close();
		}

        static void RefineAllFiles(String dir)
        {
			string[] files = Directory.GetFiles(dir);
			for(int i = 0; i < files.Length; i++){
				RefineFile(files[i]);
				Console.WriteLine("Refined file: " + files[i]);
			}
        }

        private static void RefineFile(String file)
        {
			string line;
			StreamReader sr = new StreamReader(file);
			line = sr.ReadToEnd();
			sr.Close();

			line = StringUtil.RefineAndFilterPersianWord(line);

			TextWriter tw = new StreamWriter(file);
			tw.Write(line);
			tw.Close();
        }

		public static void VerbInflector_OneFile(string sourceFile, string verbDicPath)
		{
			Article currentArticle = ArticleUtils.getArticle(sourceFile);

			Article newArticle = new Article();

			Sentence currentSentence = null;
			Sentence newSentence = null;
			String[] currentLexemes = null;
			String[] currentPOSTags = null;
			for (int i = 0; i < currentArticle.getSentences().Length; i++)
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
				for (int j = 0; j < list.Count(); j++)
				{
					currentDBT = list[j];

					newWord = new Word();

					newWord.num = currentDBT.Position;
					newWord.lexeme = currentDBT.WordForm;
					if (currentDBT.Lemma != "_")
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
					else
					{
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
