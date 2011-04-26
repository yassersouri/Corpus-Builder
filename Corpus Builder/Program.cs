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
				string dir = "D:\\crawler\\sites\\mehrnews\\";


				RefineAllFiles(dir);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
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
