﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger;
using System.IO;

namespace Tagger
{
	class Program
	{
		static Tagger tagger = new Tagger();
		static Token token;

		static void Main(string[] args)
		{
			string sDir = "D:\\sample\\corpus\\";
			string dDir = "D:\\sample\\corpus_changed\\";
			string file = "1244.txt";
			string sFile = sDir + file;
			string dFile = dDir + file;

			TagAllFiles(sDir, dDir);
		}

		static void TagAllFiles(string sourceDirectory, string destinationDirectory){
			string[] files = Directory.GetFiles(sourceDirectory);
			Directory.CreateDirectory(destinationDirectory);

			string destinationFile;
			string sourceFile;
			int lastIndex;
			string fileName;
			int len;

			for(int i = 0; i < files.Length; i++)
			{
				sourceFile = files[i];

				lastIndex = sourceFile.LastIndexOf('\\');
				len = sourceFile.Length;
				fileName = sourceFile.Substring(lastIndex+1, len-lastIndex-1);
				destinationFile = destinationDirectory + fileName;

				TagFile(sourceFile, destinationFile);
				Console.WriteLine("Tagged file: " + sourceFile);
			}
		}

		static void TagFile(string sourceFile, string destinationFile){
			StreamReader sr = new StreamReader(sourceFile);
			String file = sr.ReadToEnd();
			sr.Close();

			StreamWriter sw = new StreamWriter(destinationFile);
			string[] words = file.Split('\n');
			string line;
			string word;
			int pos;
			
			for(int i = 0; i < words.Length-1; i++){
				line = words[i];
				if(line != ""){
					pos = line.IndexOf('\t');
					word = line.Substring(pos).Trim();

					token = tagger.TaggerHelper(word);

					line += "\t" + token.Lemma + "\t" + token.POSTag + "\t" + token.Person + "\t" + token.Number;

				}
				sw.Write(line + "\n");
			}
			sw.Close();
		}
	}
}
