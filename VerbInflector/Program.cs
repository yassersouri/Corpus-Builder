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

		

		public static void VerbInflector_OneFile(string file, string verbDicPath){
			Article article = ArticleUtils.getArticle(file);
			Sentence s = null;
			String[] lexemes = null;
			String[] postags = null;
			for(int i = 0; i < article.getSentences().Length; i++)
			{
				s = article.getSentence(i);
				lexemes = s.getLexeme();
				postags = s.getPOSTag();

				string[] testSentence = "به گردش درآمده است این مسائل".Split(" ".ToCharArray(),
																	 StringSplitOptions.RemoveEmptyEntries);
				string[] testPos = "P N V V PR N".Split(" ".ToCharArray(),
																	   StringSplitOptions.RemoveEmptyEntries);

				//testPos = postags;
				//testSentence = lexemes;
				var dic = VerbPartTagger.MakePartialDependencyTree(testSentence, testPos, verbDicPath);

				System.Console.WriteLine(dic);

			}
		}
	}
}
