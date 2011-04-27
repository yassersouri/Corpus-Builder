using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.Utility;
using SCICT.NLP.Persian;
using System.IO;

namespace Corpus_Builder
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
				string dir = "D:\\sample\\";
				string file = "1143.txt";
				string destinationDir = dir + "corpus\\";
				string destinationFile = destinationDir + file;

				SeparateAllSentencesAndWords(dir,destinationDir);
				
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
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
				len = sourcefile.ToString().Length;
				fileName = sourcefile.ToString().Substring(lastIndex+1, len-lastIndex-1);

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
				Console.WriteLine("Refined file: " + files[i].ToString());
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
    }
}
