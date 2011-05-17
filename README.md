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
>1	کیفیت
>2	آثار
>3	سینمای
>4	ایران
>5	را
>6	در
>7	سالهای
>8	اخیر
>9	چگونه
>10	ارزیابی
>11	می
>12	کنید
>13	؟
>
>
>1	در
>2	یک
>3	نگاه
>4	کلی
v5	می
v6	توان
>7	گفت
>8	آثاری
>9	که
>10	در
>11	سالهای
>12	گذشته
>13	ارایه
>14	شده
>15	اند
>16	از
>17	کیفیت
>18	بهتری
>19	برخوردار
>20	هستند
>21	.
>

Tagger
------
##References
This project uses lots of SCICT.NLP DLLs which is built by [SCICT](http://SCICT.ir) and is opensource under GPL.
You can find the source code [here](http://sourceforge.net/projects/virastyar/). It also uses YAXLib.
[Tagger.cs](https://github.com/yassersouri/Corpus-Builder/blob/master/Tagger/Tagger.cs) and [Token.cs](https://github.com/yassersouri/Corpus-Builder/blob/master/Tagger/Token.cs) is written by ?.
##What it does
Output of the [Corpus Builder poject](https://github.com/yassersouri/Corpus-Builder/tree/master/Corpus%20Builder) is a set of words in each line where each sentence ends with an empty line at the end.
Each word then is tagged with the tagger and its Lemma POStag person and number is extracted. these information is then placed at the same line of the word with tab separated.
##Example Output
>1	کیفیت	کیفیت	N	-	SINGULAR
>2	آثار	آثار	N	-	SINGULAR
>3	سینمای	سینما	AJ	-	-
>4	ایران	ایران	N	-	SINGULAR
>5	را	را	POSTP	-	-
>6	در	در	P	-	-
>7	سالهای	سالها	AJ	-	-
>8	اخیر	اخیر	AJ	-	-
>9	چگونه	چگونه	ADV	-	-
>10	ارزیابی	ارزیاب	AJ	-	-
>11	می	می	AJ	-	-
>12	کنید	کنید	V	-	-
>13	؟	؟	PUNC	-	-
>
>1	در	در	P	-	-
>2	یک	یک	NUM	-	-
>3	نگاه	نگاه	N	-	SINGULAR
>4	کلی	کلی	AJ	-	-
>5	می	می	AJ	-	-
>6	توان	توان	N	-	SINGULAR
>7	گفت	گفت	V	-	-
>8	آثاری	آثار	AJ	-	-
>9	که	که	CONJ	-	-
>10	در	در	P	-	-
>11	سالهای	سالها	AJ	-	-
>12	گذشته	گذشته	AJ	-	-
>13	ارایه	ارایه	N	-	SINGULAR
>14	شده	شده	V	-	-
>15	اند	اند	N	-	SINGULAR
>16	از	از	P	-	-
>17	کیفیت	کیفیت	N	-	SINGULAR
>18	بهتری	بهتر	AJ	-	-
>19	برخوردار	برخوردار	AJ	-	-
>20	هستند	هست	V	-	-
>21	.	.	PUNC	-	-
>
