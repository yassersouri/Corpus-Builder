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

				string file = dir + "1143.txt";
				PrintAllSentenses(file);
				
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

		static void PrintAllSentenses(String file)
		{
			StreamReader sr = new StreamReader(file);
			String line = sr.ReadToEnd();
			sr.Close();
			String[] sentences;
			sentences = StringUtil.ExtractPersianSentences(line);
			StreamWriter sw = new StreamWriter("test.txt");
			for(int i = 0; i < sentences.Length; i++)
			{
				sw.WriteLine(sentences[i]);
				Console.WriteLine(sentences[i]);
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
