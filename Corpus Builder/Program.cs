using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.Utility;
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
				string sourceFile = dir + file;
				string destinationDir = dir + "corpus\\";
				string destinationFile = destinationDir + file;

				SeparateAllSentences(dir, destinationDir);
				
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

		static void SeparateAllSentences(String sourceDir, String destinationDir){
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

				SeparateSentences(sourcefile, destinationfile);

				Console.WriteLine("Extracted sentences of: " + sourcefile);
			}

		}

		static void SeparateSentences(String sourceFile, String destinationFile)
		{
			StreamReader sr = new StreamReader(sourceFile);
			String line = sr.ReadToEnd();
			sr.Close();
			String[] sentences;
			sentences = StringUtil.ExtractPersianSentences(line);
			StreamWriter sw = new StreamWriter(destinationFile);
			for(int i = 0; i < sentences.Length; i++)
			{
				sw.WriteLine(sentences[i]);
				sw.WriteLine();
			}
			sw.Flush();
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
			tw.Flush();
			tw.Close();
        }
    }
}
