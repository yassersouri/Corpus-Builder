Corpus Format:
===============
CoNLL. You can find more info [here](http://nextens.uvt.nl/depparse-wiki/DataFormat).


Projects
=========
Corpus Builder
---------------
##References
This project uses SCICT.PersianTools which is built by [SCICT](http://SCICT.ir) and is opensource under GPL.
You can find the source code [here](http://sourceforge.net/projects/virastyar/).
##What it does
We use the [Program.cs](https://github.com/yassersouri/Corpus-Builder/blob/master/Corpus%20Builder/Program.cs) file to [RefineAllFiles](https://github.com/yassersouri/Corpus-Builder/blob/master/Corpus%20Builder/Program.cs#L22) that are inside the [directory specified](https://github.com/yassersouri/Corpus-Builder/blob/master/Corpus%20Builder/Program.cs#L17).
Then we will [separate all sentences and words](https://github.com/yassersouri/Corpus-Builder/blob/master/Corpus%20Builder/Program.cs#L23) and create new files in new directories. You can overwrite your current fies if you call it like `SeparateAllSentencesAndWords(dir,dir);`.

##Example Output
Check [here](https://github.com/yassersouri/Corpus-Builder/blob/master/Corpus%20Builder/example-output.txt).
Tagger
------
#####References
This project uses lots of SCICT.NLP DLLs which is built by [SCICT](http://SCICT.ir) and is opensource under GPL.
You can find the source code [here](http://sourceforge.net/projects/virastyar/). It also uses YAXLib.
[Tagger.cs](https://github.com/yassersouri/Corpus-Builder/blob/master/Tagger/Tagger.cs) and [Token.cs](https://github.com/yassersouri/Corpus-Builder/blob/master/Tagger/Token.cs) is written by ?.
#####What it does
Output of the [Corpus Builder poject](https://github.com/yassersouri/Corpus-Builder/tree/master/Corpus%20Builder) is a set of words in each line where each sentence ends with an empty line at the end.
Each word then is tagged with the tagger and its Lemma POStag person and number is extracted. these information is then placed at the same line of the word with tab separated.
#####Example Output
Check [here](https://github.com/yassersouri/Corpus-Builder/blob/master/Tagger/example-output.txt).