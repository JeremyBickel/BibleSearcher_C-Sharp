using BLBLexicon;
using KJVStrongs;
using MAB;
using Marvel;
using System.Text;
using System.Text.RegularExpressions;

namespace Data
{
    public class Welcome
    {
        public DataObjects dataobjects;

        public Welcome()
        {
            Directory.SetCurrentDirectory(@"F:\BibleSearcher\Data\");

            //false = create
            //true = load
            bool bBLBC = DoesBLBConcordanceExist();
            bool bBLBL = DoesBLBLexiconExist();
            bool bMoby = DoesMobyExist();
            bool bKJVStrongs = DoesKJVStrongsExist();
            bool bMAB = DoesMABExist();
            bool bPericope = DoesPericopeExist();
            bool bPericopeMAB = DoesPericopeMABExist();
            bool bSTEP = DoesSTEPExist();
            bool bWriteEnglishPhraseWithReferences = false;
            bool bSynonyms = DoesExtractKJVSynonymsExist();
            bool bAddMobySynonyms = DoesAddMobySynonymsExist();

            bool bExtractKJVSynonyms = DoesExtractKJVSynonymsExist();

            bool bSemanticCategories = DoesSemanticCategoriesExist();

            bool bMarvel = DoesMarvelDataExist();

            //Primary Data
            dataobjects = new DataObjects(bBLBC, bBLBL, bMoby,
                bKJVStrongs, bMAB, bPericope, bPericopeMAB, bSTEP, bWriteEnglishPhraseWithReferences,
                bSynonyms, bAddMobySynonyms, bExtractKJVSynonyms, bSemanticCategories, bMarvel);

            //Secondary Data - Reformulates the Primary Data
            dataobjects.kjvsynonyms.RecalculateKJVWordDataWithSynonyms(ref dataobjects.kjvs.kjvdata.dWordIDs,
                ref dataobjects.kjvs.kjvdata.dWordPositions, ref dataobjects.kjvs.kjvdata.dWordCounts);

            dataobjects.intersectingPhrases = new IntersectingPhrases();

            //CreateWordIDMobySynonyms(ref dataobjects.kjvs.kjvdata, ref dataobjects.moby); //fills moby.dKJVSynonyms
        }

        public bool DoesBLBConcordanceExist()
        {
            return File.Exists(@"Data\Processed\BLBHebrewConcordance.csv") &&
                File.Exists(@"Data\Processed\BLBGreekConcordance.csv");
        }

        public bool DoesBLBLexiconExist()
        {
            return File.Exists(@"Data\Processed\BLBHebrewLexicon.csv") &&
               File.Exists(@"Data\Processed\BLBGreekLexicon.csv");
        }

        public bool DoesMobyExist()
        {
            return File.Exists(@"Data\Processed\Synonyms\Moby.csv") &&
               File.Exists(@"Data\Processed\Synonyms\MobyIndex.csv") &&
               File.Exists(@"Data\Processed\Synonyms\MobySuperIndex.csv");
        }

        public bool DoesKJVStrongsExist()
        {
            return File.Exists(@"Data\Processed\KJV-RawText.txt") &&
            File.Exists(@"Data\Processed\KJVStrongsVector.csv") &&
            File.Exists(@"Data\Processed\KJVStrongsVectorStrongsOnly.txt") &&
            File.Exists(@"Data\Processed\KJVStrongsMajorWords-English.txt") &&
            File.Exists(@"Data\Processed\WordIDs.csv") &&
            File.Exists(@"Data\Processed\PositionIDs.txt") &&
            File.Exists(@"Data\Processed\WordCounts.csv") &&
            File.Exists(@"Data\Processed\BookWordCounts.csv") &&
            File.Exists(@"Data\Processed\PhrasalConcordance.csv") &&
            File.Exists(@"Data\Processed\Phrases.txt") &&
            File.Exists(@"Data\Processed\EnglishPhraseCountsByCount.csv") &&
            File.Exists(@"Data\Processed\EnglishPhraseCountsByPhrase.csv") &&
            File.Exists(@"Data\Processed\EnglishPhraseWithReferences.csv") &&
            File.Exists(@"Data\Processed\StrongsPhrasalConcordanceEnglish.csv") &&
            File.Exists(@"Data\Processed\StrongsPhrasalConcordanceStrongs.csv") &&
            File.Exists(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers.csv") &&
            File.Exists(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers, First English Translation.csv") &&
            File.Exists(@"Data\Processed\SimpleSVOs.csv") &&
            File.Exists(@"Data\Processed\LocalPhraseClusters.csv") &&
            File.Exists(@"Data\Processed\CrossReferences.csv") &&
            File.Exists(@"Data\Processed\BLBHebrewSyllableComparisons.csv") &&
            File.Exists(@"Data\Processed\BLBGreekSyllableComparisons.csv") &&
            File.Exists(@"Data\Processed\LastVerseIDsInEachBook.csv") &&
            File.Exists(@"Data\Processed\Verses.csv") &&
            File.Exists(@"Data\Processed\References.csv");
        }

        public bool DoesMABExist()
        {
            return File.Exists(@"Data\Processed\MAB\MAB-OT.csv") &&
                File.Exists(@"Data\Processed\MAB\MAB-NT.csv");
        }
        public bool DoesPericopeExist()
        {
            return File.Exists(@"Data\Processed\Pericopes\ProcessedPericopes.csv") &&
               File.Exists(@"Data\Processed\Pericopes\AllHeadings.csv") &&
               File.Exists(@"Data\Processed\Pericopes\WordsPericopeHeadingsCount.csv") &&
               File.Exists(@"Data\Processed\Pericopes\HeadingsPositionWordID.csv") &&
               File.Exists(@"Data\Processed\Pericopes\VerseIDHeadings.csv") &&
               File.Exists(@"Data\Processed\Pericopes\HeadingVerseIDs.csv") &&
               File.Exists(@"Data\Processed\Pericopes\TSK.csv");
        }

        public bool DoesPericopeMABExist()
        {
            return File.Exists(@"Data\Processed\PericopeMAB.csv");
        }

        public bool DoesSTEPExist()
        {
            return File.Exists(@"Data\Processed\STEPHebrew.csv");
        }

        public bool DoesTSKExist()
        {
            return File.Exists(@"Data\Processed\Pericopes\TSK.csv");
        }

        public bool DoesSynonymsExist()
        {
            return File.Exists(@"Data\Processed\Synonyms\Synonyms.csv") &&
                File.Exists(@"Data\Processed\Synonyms\SynonymsIndex.csv");
        }

        public bool DoesAddMobySynonymsExist()
        {
            return File.Exists(@"Data\Processed\Synonyms\MobySynonymsAdded.txt");
        }

        public bool DoesExtractKJVSynonymsExist()
        {
            return File.Exists(@"Data\Processed\Synonyms\KJVSynonyms.txt");
        }

        public bool DoesSemanticCategoriesExist()
        {
            return File.Exists(@"Data\Processed\WordCategories.txt") &&
                File.Exists(@"Data\Processed\PhraseCategories.txt") &&
                File.Exists(@"Data\Processed\HandTaggedKJVStrongs.txt");
        }

        public bool DoesMarvelDataExist()
        {
            return false;
        }

        public void CreateIntersectingPhrases(ref KJVData kjvdata, int intMinimumVerseID = -1, int intMaximumVerseID = -1)
        {
            dataobjects.intersectingPhrases.CreateIntersectingPhrases(ref kjvdata.dVerses, intMinimumVerseID, intMaximumVerseID); //Inline Write()
        }

        //HARD DRIVE WARNING: These files become large.
        //For instance, they top 100MB at length 24.
        //54 is the last length that produces phrases
        //with a count greater than 1 for the kjvstrongs.csv file
        public void CreateChainedPhrasalConcordance(ref KJVData kjvdata, int intMinimumLength = 2, int intMaximumLength = 3)
        {
            for (int i = intMinimumLength; i <= intMaximumLength; i++) //54 is max for kjv
            {
                kjvdata.cpconc.CreateChainedPhrasalConcordance(ref kjvdata, i); //Inline Write()
            }
        }

        //creates a Dictionary<string, List<string>> of KJV words with KJV synonyms
        public void CreateWordIDMobySynonyms(ref KJVData kjvdata, ref Moby.Welcome moby)
        {
            Word word = new();

            moby.mobydata.dKJVSynonyms = word.FindWordIDMobySynonyms(ref kjvdata.dWordIDs,
                ref moby.mobydata.dMoby, ref moby.mobydata.dMobySuperIndex);
        }

        public void WriteWordIDMobySynonyms(ref DataObjects data, ref StreamWriter swKJVMobySynonyms)
        {
            swKJVMobySynonyms.WriteLine("Word ^ Synonyms"); //Synonyms are comma-separated

            foreach (string strWord in data.moby.mobydata.dKJVSynonyms.Keys.OrderBy(a => a))
            {
                StringBuilder sbSynonyms = new();

                swKJVMobySynonyms.Write(strWord + " ^ ");

                foreach (string strSynonym in data.moby.mobydata.dKJVSynonyms[strWord].OrderBy(a => a))
                {
                    _ = sbSynonyms.Append(strSynonym);
                    _ = sbSynonyms.Append(" , ");
                }

                swKJVMobySynonyms.WriteLine(sbSynonyms.Remove(sbSynonyms.Length - 3, 3).ToString());
            }

            swKJVMobySynonyms.Close();
        }
    }


    public class DataObjects
    {
        //Input Sources
        public BLBConcordance.Welcome blbConcordance;
        public BLBLexicon.Welcome blbLexicon;
        public Moby.Welcome moby;
        public Pericope.Welcome pericope;
        public STEPLexicon.Welcome stepLexicon;
        public KJVStrongs.Welcome kjvs;
        public MAB.Welcome mab;
        
        public Synonyms.Welcome synonyms;

        public Marvel.Welcome marvel;

        //Data Structures Constructed from Input Sources
        public KJVSynonyms.Welcome kjvsynonyms;
        public IntersectingPhrases intersectingPhrases;

        //Hand-Tagged
        public Categories.Welcome categories;

        private readonly Regex rgxNumbers = new(@"[0-9]{1,}");

        public DataObjects(bool bBLBC, bool bBLBL, bool bMoby,
            bool bKJVStrongs, bool bMAB, bool bPericope, bool bPericopeMAB, bool bSTEP,
            bool bWriteEnglishPhraseWithReferences, bool bSynonyms,
            bool bAddMobySynonyms, bool bExtractKJVSynonyms,
            bool bSemanticCategories, bool bMarvel)
        {
            //
            //BLB Concordance
            //
            if (bBLBC == false) //create and write
            {
                StreamReader srHebrewConcordanceIn = new(@"Data\Original\BLB\HebConc.js"); //Original data file
                StreamReader srGreekConcordanceIn = new(@"Data\Original\BLB\GrkConc.js"); //Original data file

                StreamWriter swHebrewConcordance = new(@"Data\Processed\BLBHebrewConcordance.csv"); //Formatted for this program
                StreamWriter swGreekConcordance = new(@"Data\Processed\BLBGreekConcordance.csv"); //Formatted for this program

                blbConcordance = new(ref srHebrewConcordanceIn, ref srGreekConcordanceIn,
                    ref swHebrewConcordance, ref swGreekConcordance);
            }
            else //load
            {
                StreamReader srHebrewConcordance = new(@"Data\Processed\BLBHebrewConcordance.csv"); //Formatted for this program
                StreamReader srGreekConcordance = new(@"Data\Processed\BLBGreekConcordance.csv"); //Formatted for this program

                blbConcordance = new(ref srHebrewConcordance, ref srGreekConcordance);
            }

            //
            //BLB Lexicon
            //
            if (bBLBL == false) //create
            {
                StreamReader srBLBHebrewLexiconIn = new(@"Data\Original\BLB\HebLex.js"); //Original data file
                StreamReader srBLBGreekLexiconIn = new(@"Data\Original\BLB\GrkLex.js"); //Original data file
                StreamReader srRoots = new(@"Data\Original\HebrewRoots.txt"); //Original data file
                StreamReader srNonRoots = new(@"Data\Original\HebrewNon-Roots.txt"); //Original data file

                StreamWriter swBLBHebrewLexicon = new(@"Data\Processed\BLBHebrewLexicon.csv"); //Formatted for this program
                StreamWriter swBLBGreekLexicon = new(@"Data\Processed\BLBGreekLexicon.csv"); //Formatted for this program
                StreamWriter swBLBDerivations = new(@"Data\Processed\BLBDerivations.csv"); //Formatted for this program
                StreamWriter swBLBConnections = new(@"Data\Processed\BLBConnections.csv"); //ONLY GREEK SO FAR; Formatted for this program
                StreamWriter swBLBHRoots = new(@"Data\Processed\BLBHRoots.csv"); //Formatted for this program
                StreamWriter swBLBHNonRoots = new(@"Data\Processed\BLBHNonRoots.csv"); //Formatted for this program
                StreamWriter swBLBHAramaic = new(@"Data\Processed\BLBHAramaic.csv"); //Formatted for this program
                StreamWriter swBLBHNonAramaic = new(@"Data\Processed\BLBHNonAramaic.csv"); //Formatted for this program
                StreamWriter swBLBHRootedAramaic = new(@"Data\Processed\BLBHRootedAramaic.csv"); //Formatted for this program

                blbLexicon = new(ref srBLBHebrewLexiconIn, ref srBLBGreekLexiconIn,
                    ref swBLBHebrewLexicon, ref swBLBGreekLexicon, ref swBLBDerivations,
                    ref swBLBConnections, ref swBLBHRoots, ref swBLBHNonRoots,
                    ref swBLBHAramaic, ref swBLBHNonAramaic, ref swBLBHRootedAramaic,
                    ref srRoots, ref srNonRoots);
            }
            else //load
            {
                StreamReader srBLBHebrewLexicon = new(@"Data\Processed\BLBHebrewLexicon.csv"); //Formatted for this program
                StreamReader srBLBGreekLexicon = new(@"Data\Processed\BLBGreekLexicon.csv"); //Formatted for this program
                StreamReader srBLBDerivations = new(@"Data\Processed\BLBDerivations.csv"); //Formatted for this program
                StreamReader srBLBConnections = new(@"Data\Processed\BLBConnections.csv"); //Formatted for this program
                StreamReader srBLBHRoots = new(@"Data\Processed\BLBHRoots.csv"); //Formatted for this program
                StreamReader srBLBHNonRoots = new(@"Data\Processed\BLBHNonRoots.csv"); //Formatted for this program
                StreamReader srBLBHAramaic = new(@"Data\Processed\BLBHAramaic.csv"); //Formatted for this program
                StreamReader srBLBHNonAramaic = new(@"Data\Processed\BLBHNonAramaic.csv"); //Formatted for this program
                StreamReader srBLBHRootedAramaic = new(@"Data\Processed\BLBHRootedAramaic.csv"); //Formatted for this program

                blbLexicon = new(ref srBLBHebrewLexicon, ref srBLBGreekLexicon,
                    ref srBLBDerivations, ref srBLBConnections, ref srBLBHRoots,
                    ref srBLBHNonRoots, ref srBLBHAramaic, ref srBLBHNonAramaic,
                    ref srBLBHRootedAramaic);
            }

            //
            //Moby
            //
            if (bMoby == false) //create
            {
                StreamReader srMobyIn = new(@"Data\Original\Synonyms\mthesaur.txt"); //Original data file

                StreamWriter swMoby = new(@"Data\Processed\Synonyms\Moby.csv"); //Formatted for this program
                StreamWriter swMobyIndex = new(@"Data\Processed\Synonyms\MobyIndex.csv"); //Formatted for this program
                StreamWriter swMobySuperIndex = new(@"Data\Processed\Synonyms\MobySuperIndex.csv"); //Formatted for this program

                moby = new(ref srMobyIn, ref swMoby, ref swMobyIndex, ref swMobySuperIndex);
            }
            else //load
            {
                StreamReader srMoby = new(@"Data\Processed\Synonyms\Moby.csv"); //Formatted for this program
                StreamReader srMobyIndex = new(@"Data\Processed\Synonyms\MobyIndex.csv"); //Formatted for this program
                StreamReader srMobySuperIndex = new(@"Data\Processed\Synonyms\MobySuperIndex.csv"); //Formatted for this program

                moby = new(ref srMoby, ref srMobyIndex, ref srMobySuperIndex);
            }

            //
            //KJV Strongs
            //
            if (bKJVStrongs == false)
            {
                StreamReader srBible = new(@"Data\Original\kjvstrongs.csv");
                StreamWriter swRawText = new(@"Data\Processed\KJV-RawText.txt");
                StreamWriter swKJVStrongsVector = new(@"Data\Processed\KJVStrongsVector.csv");
                StreamWriter swKJVStrongsVectorStrongsOnly = new(@"Data\Processed\KJVStrongsVectorStrongsOnly.txt");
                StreamWriter swStrongsMajorWordEnglish = new(@"Data\Processed\KJVStrongsMajorWords-English.txt");
                StreamWriter swWordIDs = new(@"Data\Processed\WordIDs.csv");
                StreamWriter swPositionIDs = new(@"Data\Processed\PositionIDs.txt");
                StreamWriter swWordCounts = new(@"Data\Processed\WordCounts.csv");
                StreamWriter swBookWordCounts = new(@"Data\Processed\BookWordCounts.csv");
                StreamWriter swPhrasalConcordance = new(@"Data\Processed\PhrasalConcordance.csv");
                StreamWriter swPhrases = new(@"Data\Processed\Phrases.txt");
                StreamWriter swEnglishPhraseCountsByCount = new(@"Data\Processed\EnglishPhraseCountsByCount.csv");
                StreamWriter swEnglishPhraseCountsByPhrase = new(@"Data\Processed\EnglishPhraseCountsByPhrase.csv");
                StreamWriter swEnglishPhraseWithReferences = new(@"Data\Processed\EnglishPhraseWithReferences.csv");
                StreamWriter swStrongsPhrasalConcordanceEnglish = new(@"Data\Processed\StrongsPhrasalConcordanceEnglish.csv");
                StreamWriter swStrongsPhrasalConcordanceStrongs = new(@"Data\Processed\StrongsPhrasalConcordanceStrongs.csv");
                StreamWriter swSSComplex = new(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers.csv"); //Strong's sequence contains a "-"
                StreamWriter swSSComplexTranslation = new(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers, First English Translation.csv"); //First english translation when the Strong's sequence contains a "-"
                StreamWriter swSimpleSVOs = new(@"Data\Processed\SimpleSVOs.csv");
                StreamWriter swLocalPhraseClusters = new(@"Data\Processed\LocalPhraseClusters.csv");
                StreamWriter swCrossrefs = new(@"Data\Processed\CrossReferences.csv");
                StreamWriter swSyllableScoreHebrew = new(@"Data\Processed\BLBHebrewSyllableComparisons.csv");
                StreamReader srSyllableScoreHebrew = new(@"Data\Processed\BLBHebrewLexicon.csv");
                StreamWriter swSyllableScoreGreek = new(@"Data\Processed\BLBGreekSyllableComparisons.csv");
                StreamReader srSyllableScoreGreek = new(@"Data\Processed\BLBGreekLexicon.csv");
                StreamReader srCrossrefs = new(@"Data\Original\cross_references.txt");
                StreamWriter swLastVerseIDInBook = new(@"Data\Processed\LastVerseIDsInEachBook.csv");
                StreamWriter swVerses = new(@"Data\Processed\Verses.csv");
                StreamWriter swReferences = new(@"Data\Processed\References.csv");

                kjvs = new KJVStrongs.Welcome(ref blbLexicon.blblexicondata, ref srBible,
                    ref swRawText, ref swVerses, ref swLastVerseIDInBook, ref swKJVStrongsVector, ref swKJVStrongsVectorStrongsOnly,
                    ref swStrongsMajorWordEnglish, ref swWordIDs, ref swPositionIDs,
                    ref swWordCounts, ref swBookWordCounts, ref swPhrasalConcordance, ref swPhrases, ref swEnglishPhraseCountsByCount,
                    ref swEnglishPhraseCountsByPhrase, ref swEnglishPhraseWithReferences, ref swStrongsPhrasalConcordanceEnglish,
                    ref swStrongsPhrasalConcordanceStrongs, ref swSSComplex, ref swSSComplexTranslation, ref swSimpleSVOs,
                    ref swLocalPhraseClusters, ref swCrossrefs, ref swSyllableScoreHebrew, ref srSyllableScoreHebrew,
                    ref swSyllableScoreGreek, ref srSyllableScoreGreek, ref srCrossrefs, ref swReferences, bWriteEnglishPhraseWithReferences);
            }
            else //Load
            {
                StreamReader srLastVerseIDInBook = new(@"Data\Processed\LastVerseIDsInEachBook.csv");
                StreamReader srVerses = new(@"Data\Processed\Verses.csv");
                StreamReader srReferences = new(@"Data\Processed\References.csv");
                StreamReader srKJVStrongsVector = new(@"Data\Processed\KJVStrongsVector.csv");
                StreamReader srWordIDs = new(@"Data\Processed\WordIDs.csv");
                StreamReader srPositionIDs = new(@"Data\Processed\PositionIDs.txt");
                StreamReader srWordCounts = new(@"Data\Processed\WordCounts.csv");
                StreamReader srBookWordCounts = new(@"Data\Processed\BookWordCounts.csv");
                StreamReader srPhrasalConcordance = new(@"Data\Processed\PhrasalConcordance.csv");
                StreamReader srStrongsPhrasalConcordanceEnglish = new(@"Data\Processed\StrongsPhrasalConcordanceEnglish.csv");
                StreamReader srStrongsPhrasalConcordanceStrongs = new(@"Data\Processed\StrongsPhrasalConcordanceStrongs.csv");
                StreamReader srSSComplex = new(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers.csv");
                StreamReader srSSComplexTranslation = new(@"Data\Processed\Counts for Phrases with Multiple Strongs Numbers, First English Translation.csv");
                StreamReader srSimpleSVOs = new(@"Data\Processed\SimpleSVOs.csv");
                StreamReader srLocalPhraseClusters = new(@"Data\Processed\LocalPhraseClusters.csv");
                StreamReader srCrossrefs = new(@"Data\Processed\CrossReferences.csv");
                StreamReader srSyllableScoreHebrew = new(@"Data\Processed\BLBHebrewSyllableComparisons.csv");
                StreamReader srSyllableScoreGreek = new(@"Data\Processed\BLBGreekSyllableComparisons.csv");
                StreamReader srRawText = new(@"Data\Processed\KJV-RawText.txt");

                //
                //These files are not loaded into their own structures, but they are derived from loadable data structures
                //
                //KJVStrongsVectorStrongsOnly - kjvdata.intsStrongsPhrases[intVerseID, intPhraseID, intSSIncrease]
                //srStrongsMajorWordEnglish - dBLBHebrewLexiconEntries[intStrongsNumber - 8674].dAVTranslations.Keys
                //srPhrases - kjvdata.dPhrasalConcordance.Keys
                //srEnglishPhraseCountsByCount - kjvdata.dPhrasalConcordance.OrderBy(a => dPhrasalConcordanceLocal[a.Key].Count).Select(a => a.Key)
                //srEnglishPhraseCountsByPhrase - from phrase in kjvdata.dPhrasalConcordance.Keys orderby phrase select phrase;
                //srEnglishPhraseWithReferences - 10s of GB!! -
                //foreach (kjvdata.dPhrasalConcordance.OrderBy(a => dPhrasalConcordanceLocal[a.Key].Count).Select(a => a.Key))
                //  foreach (kjvdata.dPhrasalConcordance[strPhrase])

                kjvs = new KJVStrongs.Welcome(ref srLastVerseIDInBook, ref srVerses, ref srReferences,
                    ref srKJVStrongsVector, ref srWordIDs, ref srPositionIDs, ref srWordCounts,
                    ref srBookWordCounts, ref srPhrasalConcordance, ref srStrongsPhrasalConcordanceEnglish,
                    ref srStrongsPhrasalConcordanceStrongs, ref srSSComplex, ref srSSComplexTranslation,
                    ref srSimpleSVOs, ref srLocalPhraseClusters, ref srCrossrefs, ref srSyllableScoreHebrew,
                    ref srSyllableScoreGreek, ref srRawText);
            }

            //
            //Pericope
            //
            //Depends on References.csv from KJV's Create()
            if (bPericope == false) //create
            {
                string strTSKDirectory = @"Data\Original\Pericopes\TSK_Headings\";
                string strPericopeDivisionDataDirectory = @"Data\Processed\Pericopes\";

                StreamReader srWholeBiblePericope = new(@"Data\Original\Pericopes\Semantic Bible\Whole Bible.txt"); //The whole Bible, from Berean Bible
                StreamReader srWikipediaActs = new(@"Data\Original\Pericopes\Wikipedia\Acts.txt");
                //Wikipedia\Galatians.txt
                //Cranfordville
                StreamWriter swTSK = new(@"Data\Processed\Pericopes\TSK.csv");
                StreamWriter swPericopes = new(@"Data\Processed\Pericopes\ProcessedPericopes.csv");
                StreamWriter swHeadings = new(@"Data\Processed\Pericopes\AllHeadings.csv");
                StreamWriter swWordsPericopeHeadingsCount =
                    new(@"Data\Processed\Pericopes\WordsPericopeHeadingsCount.csv");
                StreamWriter swHeadingsPositionWordID =
                    new(@"Data\Processed\Pericopes\HeadingsPositionWordID.csv");
                StreamWriter swVerseIDHeadings = new(@"Data\Processed\Pericopes\VerseIDHeadings.csv");
                StreamWriter swHeadingVerseIDs = new(@"Data\Processed\Pericopes\HeadingVerseIDs.csv");
                StreamWriter swIntersectingPhrases = new(@"Data\Processed\VerseRelatednessByPhraseSimilarity.csv");
                StreamWriter swNewPericopeWordIDs = new(@"Data\Processed\WordIDs.csv", true);
                StreamReader srReferences = new(@"Data\Processed\References.csv");
                StreamWriter swPericopeRanges = new(@"Data\Processed\Pericopes\PericopeRanges.csv");

                pericope = new(ref srWholeBiblePericope, ref srWikipediaActs, ref swPericopes,
                    ref swHeadings, ref swWordsPericopeHeadingsCount, ref swHeadingsPositionWordID,
                    ref swVerseIDHeadings, ref swHeadingVerseIDs, ref kjvs.kjvdata, ref swTSK,
                    ref swIntersectingPhrases, ref swNewPericopeWordIDs, ref srReferences,
                    ref swPericopeRanges, strTSKDirectory, strPericopeDivisionDataDirectory);
            }
            else //load
            {
                StreamReader srPericopes = new(@"Data\Processed\Pericopes\ProcessedPericopes.csv");
                StreamReader srHeadings = new(@"Data\Processed\Pericopes\AllHeadings.csv");
                StreamReader srWordsPericopeHeadingsCount =
                    new(@"Data\Processed\Pericopes\WordsPericopeHeadingsCount.csv");
                StreamReader srHeadingsPositionWordID =
                    new(@"Data\Processed\Pericopes\HeadingsPositionWordID.csv");
                StreamReader srVerseIDHeadings = new(@"Data\Processed\Pericopes\VerseIDHeadings.csv");
                StreamReader srHeadingVerseIDs = new(@"Data\Processed\Pericopes\HeadingVerseIDs.csv");
                StreamReader srTSK = new(@"Data\Processed\Pericopes\TSK.csv");
                StreamReader srPericopeRanges = new(@"Data\Processed\Pericopes\PericopeRanges.csv");

                pericope = new Pericope.Welcome(ref srPericopes, ref srHeadings,
                    ref srWordsPericopeHeadingsCount, ref srHeadingsPositionWordID,
                    ref srVerseIDHeadings, ref srHeadingVerseIDs, ref srTSK, ref srPericopeRanges);
            }

            //
            //MAB
            //
            //Depends on Pericopes
            if (bMAB == false)
            {
                string strIntermediateFilename = @"Data\Processed\MAB\MAB-NT-Intermediate.csv";
                string strProcessedMABOTFilename = @"Data\Processed\MAB\MAB-OT.csv";
                string strProcessedMABNTFilename = @"Data\Processed\MAB\MAB-NT.csv";
                string strWordsFilename = @"Data\Processed\MAB\NTWords.csv";
                string strRMACMorphologyFilename = @"Data\Original\MAB\OpenGNT_Morphology_English.csv";
                StreamReader srHebrewParseCodes = new StreamReader(@"Data\Original\MAB\MAB-Hebrew-Parse-Codes.txt");
                StreamReader srHebrewParseCodes2;
                FileStream fsMABOT = new FileStream(@"Data\Original\MAB\MAB-OT.xml", FileMode.Open);
                StreamWriter swMABOT = new StreamWriter(strProcessedMABOTFilename);
                StreamWriter swCombinations = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations.txt");
                StreamWriter swWordCombinations = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-Word.txt");
                StreamWriter swWordKind = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-WordKind.txt");
                StreamWriter swWordType = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-WordType.txt");
                StreamWriter swWordPD = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-WordPD.txt");
                StreamWriter swWordPT = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-WordPT.txt");
                StreamWriter swWordPU = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Combinations-WordPU.txt");
                StreamWriter swFormatted = new StreamWriter(@"Data\Processed\MAB\MAB-Parse-Formatted.txt");
                StreamReader srMABNT = new StreamReader(@"Data\Original\MAB\MAB-NT.xml");
                StreamWriter swWords = new StreamWriter(strWordsFilename);
                StreamWriter swIntermediateMABNT = new StreamWriter(strIntermediateFilename);
                StreamWriter swProcessedMABNT = new StreamWriter(strProcessedMABNTFilename);
                StreamWriter swWFM = new(@"Data\Processed\MAB\MeshWordFunctionMorphology.csv");
                StreamWriter swWFSF = new(@"Data\Processed\MAB\MeshWordFunctionSubclauseFunction.csv");
                StreamWriter swWFCF = new(@"Data\Processed\MAB\MeshWordFunctionClauseFunction.csv");
                StreamWriter swMSF = new(@"Data\Processed\MAB\MeshMorphologySubclauseFunction.csv");
                StreamWriter swMCF = new(@"Data\Processed\MAB\MeshMorphologyClauseFunction.csv");
                StreamWriter swSFCF = new(@"Data\Processed\MAB\MeshSubclauseFunctionClauseFunction.csv");
                StreamWriter swAll = new(@"Data\Processed\MAB\MeshAll.csv");
                StreamWriter swCounts = new(@"Data\Processed\MAB\MeshCounts.csv");
                StreamWriter swCountsByCount = new(@"Data\Processed\MAB\MeshCountsByCount.csv");
                StreamWriter swGlossSFCounts = new(@"Data\Processed\MAB\GlossSFCounts.csv");

                mab = new MAB.Welcome(ref srHebrewParseCodes, ref fsMABOT, ref swMABOT, ref srMABNT,
                    ref swIntermediateMABNT, ref swProcessedMABNT, ref swWords, strIntermediateFilename,
                    ref swWFM, ref swWFSF, ref swWFCF, ref swMSF, ref swMCF, ref swSFCF, ref swAll, ref swCounts,
                    ref swCountsByCount, ref swGlossSFCounts, ref pericope.pericopedata.psHeadings.dRangedVerses,
                    strProcessedMABOTFilename, strProcessedMABNTFilename, strWordsFilename, strRMACMorphologyFilename);

                srHebrewParseCodes2 = new StreamReader(@"Data\Original\MAB\MAB-Hebrew-Parse-Codes.txt");
                mab.versesOT.ReadHebrewParseCodes(ref srHebrewParseCodes2);
                mab.versesOT.WriteParseCombinations(ref swCombinations, ref swWordCombinations, ref swWordKind, ref swWordType,
                    ref swWordPD, ref swWordPT, ref swWordPU, ref swFormatted);
            }
            else
            {//This doesn't load the 8 Parse Combinations files, but only the processed MAB-OT.xml and MAB-NT.csv
                StreamReader srHebrewParseCodes = new StreamReader(@"Data\Original\MAB\MAB-Hebrew-Parse-Codes.txt");
                StreamReader srMABOT = new StreamReader(@"Data\Processed\MAB\MAB-OT.csv");
                StreamReader srMABNT = new StreamReader(@"Data\Processed\MAB\MAB-NT.csv");

                mab = new MAB.Welcome(ref srHebrewParseCodes, ref srMABOT, ref srMABNT);
            }

            //
            //PericopeMAB
            //This only creates. It doesn't load.
            //
            if (bPericopeMAB == false)
            {
                //Create PericopeMAB.csv
                StreamReader srNTWords = new StreamReader(@"Data\Processed\MAB\NTWords.csv");
                StreamReader srPericopes = new StreamReader(@"Data\Processed\Pericopes\PericopeRanges.csv");
                StreamWriter swPericopeMAB = new StreamWriter(@"Data\Processed\PericopeMAB.csv");
                Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, string>>>> dPericopeLocations = //D<book number, D<chapter number, D<verse number, data row>>>>
                    new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, string>>>>();
                string strHeader = srNTWords.ReadLine().Replace(" ", "") + "^Pericope1^Pericope2^Pericope3^Pericope4"; //squeeze the spaces out
                List<string> lPericopeLocations = new List<string>();

                srPericopes.ReadLine(); //go past header

                while (!srPericopes.EndOfStream)
                {
                    string strLine = srPericopes.ReadLine();
                    if (strLine.Trim() != "")
                    {
                        string[] strsLine = strLine.Split('^', StringSplitOptions.TrimEntries);
                        int intBookNumber = Convert.ToInt32(strsLine[3]);
                        int intChapterNumber = Convert.ToInt32(strsLine[4]);
                        int intVerseNumber = Convert.ToInt32(strsLine[5]);

                        //lPericopeLocations.Add(strsLine[3] + "~" + strsLine[4] + "~" + strsLine[5] + "~" + strsLine[6] + "~" + strsLine[7] + "~" + strsLine[8] + "~" + strsLine[9]);

                        if (!dPericopeLocations.ContainsKey(intBookNumber))
                        {
                            dPericopeLocations.Add(intBookNumber, new Dictionary<int, Dictionary<int, Dictionary<int, string>>>());
                        }

                        if (!dPericopeLocations[intBookNumber].ContainsKey(intChapterNumber))
                        {
                            dPericopeLocations[intBookNumber].Add(intChapterNumber, new Dictionary<int, Dictionary<int, string>>());
                        }

                        if (!dPericopeLocations[intBookNumber][intChapterNumber].ContainsKey(intVerseNumber))
                        {
                            dPericopeLocations[intBookNumber][intChapterNumber].Add(intVerseNumber, new Dictionary<int, string>());
                        }

                        if (!dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].ContainsKey(1))
                        {
                            dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].Add(1, strsLine[6]);
                        }

                        if (!dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].ContainsKey(2))
                        {
                            dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].Add(2, strsLine[7]);
                        }

                        if (!dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].ContainsKey(3))
                        {
                            dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].Add(3, strsLine[8]);
                        }

                        if (!dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].ContainsKey(4))
                        {
                            dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber].Add(4, strsLine[9]);
                        }
                    }
                }
                srPericopes.Close();

                string strDifferingNumberingSchemeFix = ""; //Because some versions have slightly different verse numbering

                //write the header
                swPericopeMAB.WriteLine("GreekID^Gloss^BookNumber^ChapterNumber^VerseNumber^WordID^WordFunction^ClauseLevel^Clause^ClauseFunction^Subclause^SubclauseFunction^GreekClauseLevel^Strongs^Morphology^GreekWord^Pericope1^Pericope2^Pericope3^Pericope4");
                
                while (!srNTWords.EndOfStream)
                {
                    string strLine = srNTWords.ReadLine();
                    if (strLine.Trim() != "")
                    {
                        string[] strsLine = strLine.Split('^', StringSplitOptions.TrimEntries);
                        int intBookNumber = Convert.ToInt32(strsLine[1]);
                        int intChapterNumber = Convert.ToInt32(strsLine[2]);
                        int intVerseNumber = Convert.ToInt32(strsLine[3]);
                        int intWordNumber = Convert.ToInt32(strsLine[4]);
                        string strPericopes = "";

                        if (intBookNumber == 64 && intChapterNumber == 1 && intVerseNumber == 15)
                        {
                            strPericopes = strDifferingNumberingSchemeFix;
                        }
                        else if (intBookNumber == 66 && intChapterNumber == 12 && intVerseNumber == 18)
                        {
                            strPericopes = strDifferingNumberingSchemeFix;
                        }
                        else
                        {
                            //strPericopes = lPericopeLocations.Where(a => a.StartsWith(
                            //        intBookNumber.ToString() + "~" + intChapterNumber.ToString()
                            //        + "~" + intVerseNumber.ToString()
                            //        )).First().Substring(
                            //            intBookNumber.ToString().Length + intChapterNumber.ToString().Length +
                            //            intVerseNumber.ToString().Length + 3);
                            //List<string> lTemp = lPericopeLocations.Where(a => a.StartsWith(
                            //        intBookNumber.ToString() + "~" + intChapterNumber.ToString()
                            //        + "~" + intVerseNumber.ToString()
                            //        )).ToList<string>();
                            //if (strPericopes.Count(a => a == '~') == 3)
                            //{
                            //    string a = "";
                            //}
                            //else if (strPericopes.Count(a => a == '~') == 4)
                            //{
                            //    string a = "";
                            //}

                            strPericopes = dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber][1] + "^" +
                                dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber][2] + "^" +
                                dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber][3] + "^" +
                                dPericopeLocations[intBookNumber][intChapterNumber][intVerseNumber][4];

                            if (intBookNumber == 64 && intChapterNumber == 1 && intVerseNumber == 14)
                            {
                                //use this verse's pericopes for the next verse, if it appears
                                strDifferingNumberingSchemeFix = strPericopes;
                            }
                            if (intBookNumber == 66 && intChapterNumber == 12 && intVerseNumber == 17)
                            {
                                //use this verse's pericopes for the next verse, if it appears
                                strDifferingNumberingSchemeFix = strPericopes;
                            }
                        }

                        swPericopeMAB.WriteLine(strsLine[12] + "^" + strsLine[0] + "^" + strsLine[1] + "^" + //put GreekID first as a row id
                            strsLine[2] + "^" + strsLine[3] + "^" + strsLine[4] + "^" +
                            strsLine[5] + "^" + strsLine[6] + "^" + strsLine[7] + "^" +
                            strsLine[8] + "^" + strsLine[9] + "^" + strsLine[10] + "^" +
                            strsLine[11] + "^" + strsLine[13] + "^" + strsLine[14] + "^" + 
                            strsLine[15] + "^" + strPericopes);
                    }
                }

                srNTWords.Close();
                swPericopeMAB.Close();

                ////Create PericopeMABOrdered.csv
                //StreamReader srIn = new StreamReader(@"Data\Processed\PericopeMAB.csv");
                //StreamWriter swOut = new StreamWriter(@"Data\Processed\PericopeMABOrdered.csv");
                //Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, string>>>> dOrdered = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, string>>>>(); //D<BookNumber,D<ChapterNumber,D<VerseNumber,D<WordNumber,DataRow>>>>

                //// GreekID ^ Gloss ^ BookName ^ BookNumber ^ ChapterNumber ^ VerseNumber ^ WordID ^ WordFunction ^ ClauseLevel ^ Clause ^ ClauseFunction ^ Subclause ^ SubclauseFunction ^ GreekClauseLevel ^ Strongs ^ Morphology ^ GreekWord ^ PericopeID ^ Pericope1 ^ Pericope2 ^ Pericope3 ^ Pericope4
                //strHeader = srIn.ReadLine();

                //swOut.WriteLine(strHeader);

                //while (!srIn.EndOfStream)
                //{
                //    string strLine = srIn.ReadLine();
                //    if (strLine.Trim() != "")
                //    {
                //        string[] strsLine = strLine.Split('^');
                //        int intBookNumber = Convert.ToInt32(strsLine[1]);
                //        int intChapterNumber = Convert.ToInt32(strsLine[2]);
                //        int intVerseNumber = Convert.ToInt32(strsLine[3]);
                //        int intWordID = Convert.ToInt32(strsLine[4]);

                //        if (!dOrdered.ContainsKey(intBookNumber))
                //        {
                //            dOrdered.Add(intBookNumber, new Dictionary<int, Dictionary<int, Dictionary<int, string>>>());
                //        }

                //        if (!dOrdered[intBookNumber].ContainsKey(intChapterNumber))
                //        {
                //            dOrdered[intBookNumber].Add(intChapterNumber, new Dictionary<int, Dictionary<int, string>>());
                //        }

                //        if (!dOrdered[intBookNumber][intChapterNumber].ContainsKey(intVerseNumber))
                //        {
                //            dOrdered[intBookNumber][intChapterNumber].Add(intVerseNumber, new Dictionary<int, string>());
                //        }

                //        if (!dOrdered[intBookNumber][intChapterNumber][intVerseNumber].ContainsKey(intWordID))
                //        {
                //            dOrdered[intBookNumber][intChapterNumber][intVerseNumber].Add(intWordID, strLine);
                //        }
                //    }
                //}
                //srIn.Close();

                //foreach (int intBookNumber in dOrdered.Keys.OrderBy(a => a))
                //{
                //    foreach (int intChapterNumber in dOrdered[intBookNumber].Keys.OrderBy(a => a))
                //    {
                //        foreach (int intVerseNumber in dOrdered[intBookNumber][intChapterNumber].Keys.OrderBy(a => a))
                //        {
                //            foreach (int intWordID in dOrdered[intBookNumber][intChapterNumber][intVerseNumber].Keys.OrderBy(a => a))
                //            {
                //                swOut.WriteLine(dOrdered[intBookNumber][intChapterNumber][intVerseNumber][intWordID]);
                //            }
                //        }
                //    }
                //}
                //swOut.Close();
            }

            //
            //STEP Lexicon - Hebrew
            //
            if (bSTEP == false)
            {
                StreamReader srSTEPHebrew = new(@"Data\Original\STEP Bible\TOTHT.txt");
                StreamWriter swSTEPHebrew = new(@"Data\Processed\STEPHebrew.csv");

                stepLexicon = new(ref srSTEPHebrew, ref swSTEPHebrew);
            }
            else
            {
                StreamReader srSTEPHebrew = new(@"Data\Processed\STEPHebrew.csv");

                stepLexicon = new(ref srSTEPHebrew);
            }

            //
            //KJV-Moby
            //
            if (bMoby == false)
            {
                StreamReader srMobyIn = new StreamReader(@"Data\Original\Synonyms\mthesaur.txt");
                StreamWriter swMoby = new StreamWriter(@"Data\Processed\Synonyms\Moby\Moby.csv");
                StreamWriter swMobyIndex = new StreamWriter(@"Data\Processed\Synonyms\Moby\MobyIndex.csv");
                StreamWriter swMobySuperIndex = new StreamWriter(@"Data\Processed\Synonyms\Moby\MobySuperIndex.csv");

                moby = new Moby.Welcome(ref srMobyIn, ref swMoby, ref swMobyIndex, ref swMobySuperIndex);
            }
            else
            {
                StreamReader srMoby = new StreamReader(@"Data\Processed\Synonyms\Moby\Moby.csv");
                StreamReader srMobyIndex = new StreamReader(@"Data\Processed\Synonyms\Moby\MobyIndex.csv");
                StreamReader srMobySuperIndex = new StreamReader(@"Data\Processed\Synonyms\Moby\MobySuperIndex.csv");
                //@"Data\Processed\Synonyms\KJVMobySynonyms.csv"

                moby = new Moby.Welcome(ref srMoby, ref srMobyIndex, ref srMobySuperIndex);
            }

            //
            //Synonyms
            //
            if (bSynonyms == false)
            {
                StreamReader srSynonyms = new(@"Data\Original\Synonyms\Synonyms.csv");
                StreamWriter swSynonyms = new(@"Data\Processed\Synonyms\Synonyms.csv");
                StreamWriter swSynonymsIndex = new(@"Data\Processed\Synonyms\SynonymsIndex.csv");

                synonyms = new Synonyms.Welcome(ref srSynonyms, ref swSynonyms, ref swSynonymsIndex);
            }
            else
            {
                StreamReader srSynonyms = new(@"Data\Processed\Synonyms\Synonyms.csv");
                StreamReader srSynonymsIndex = new(@"Data\Processed\Synonyms\SynonymsIndex.csv");

                synonyms = new Synonyms.Welcome(ref srSynonyms, ref srSynonymsIndex);
            }

            //
            //Incorporate Moby into Synonyms
            //
            //false here doesn't mean not to add,
            //but that the file hasn't been created (so add..)
            if (bAddMobySynonyms == false)
            {
                StreamWriter swMobySynonyms = new(@"Data\Processed\Synonyms\Synonyms.csv", true);

                synonyms.WriteMobyIntoSynonyms(ref swMobySynonyms, ref synonyms, ref moby.mobydata);

                //this file will switch this code chunk off in subsequent runs
                StreamWriter swToggle = new(@"Data\Processed\Synonyms\MobySynonymsAdded.txt");
                swToggle.Close();
            }
            else
            {
                //nothing to be done; There's no data structure to populate; it's all in the synonyms data structure
                //the Synonyms.csv file has already been read
                //by the Synonyms chunk, above
            }

            //
            //KJV-Synonyms
            //Depends on KJV, Synonyms
            //
            //false here doesn't mean not to extract,
            //but that the file hasn't been created (so extract..)
            if (bExtractKJVSynonyms == false)
            {
                StreamWriter swKJVSynonyms = new(@"Data\Processed\Synonyms\KJVSynonyms.txt");

                kjvsynonyms = new KJVSynonyms.Welcome(ref synonyms.dSynonyms, ref synonyms.dSynonymsIndex,
                    ref kjvs.kjvdata.dWordIDs, ref swKJVSynonyms);
            }
            else
            {
                StreamReader srKJVSynonyms = new(@"Data\Processed\Synonyms\KJVSynonyms.txt");

                kjvsynonyms = new KJVSynonyms.Welcome(ref srKJVSynonyms);
            }

            //
            //Marvel JSON Data
            //
            if (bMarvel == false)
            {
                string strMarvelDir = @"Data\Processed\Marvel\";

                marvel = new Marvel.Welcome();
                
            }

            //
            //Hand-Tagged Word and Phrase Semantic Categories
            //
            //Assume WordCategories.txt exists. If PhraseCategories.txt doesn't exist,
            // it will be created.
            //
            string strWordFilename = @"Data\Processed\WordCategories.txt";
            string strPhraseFilename = @"Data\Processed\PhraseCategories.txt";

            //Add these hand-tagged words and phrases to KJVStrongs words and phrases and write it to a file.
            StreamWriter swHandTagged = new(@"Data\Processed\HandTaggedKJVStrongs.txt");

            categories = new Categories.Welcome(strWordFilename, strPhraseFilename);

            categories.AddSemanticTaggingToKJVStrongsPhrases(ref kjvs.kjvdata.dWordIDs,
                ref swHandTagged, kjvs.kjvdata.dPhrasalConcordance.Keys.ToList<string>());
        }

        internal void HebrewConnectionExpander(ref StreamWriter swHebrewDerivations,
            ref Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries,
            string strConnection)
        {
            if (strConnection.StartsWith("h"))
            {
                if (dBLBHebrewLexiconEntries.ContainsKey(Convert.ToInt16(strConnection[1..])))
                {
                    foreach (string strTranslation in dBLBHebrewLexiconEntries[
                            Convert.ToInt16(strConnection[1..])].dAVTranslations.Keys)
                    {
                        if (rgxNumbers.IsMatch(strTranslation))
                        {
                            foreach (Match mConnection in rgxNumbers.Matches(strTranslation))
                            {
                                HebrewConnectionExpander(ref swHebrewDerivations,
                                    ref dBLBHebrewLexiconEntries, mConnection.Value);
                            }
                        }
                        else
                        {
                            swHebrewDerivations.Write(strTranslation + ", ");
                        }
                    }
                    //TranslationDerivationExpander(ref swGreekDerivations, strConnection, true);
                }
            }
        }

        internal void GreekConnectionExpander(ref StreamWriter swGreekDerivations,
            ref Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries,
            string strConnection)
        {
            Regex rgxNumbers = new("[0-9]{1,}");

            if (strConnection.StartsWith("g"))
            {
                if (dBLBGreekLexiconEntries.ContainsKey(Convert.ToInt16(strConnection[1..])))
                {
                    foreach (string strTranslation in dBLBGreekLexiconEntries[
                            Convert.ToInt16(strConnection[1..])].dAVTranslations.Keys)
                    {
                        if (rgxNumbers.IsMatch(strTranslation))
                        {
                            foreach (Match mConnection in rgxNumbers.Matches(strTranslation))
                            {
                                GreekConnectionExpander(ref swGreekDerivations,
                                    ref dBLBGreekLexiconEntries, mConnection.Value);
                            }
                        }
                        else
                        {
                            swGreekDerivations.Write(strTranslation + ", ");
                        }
                    }
                    //TranslationDerivationExpander(ref swGreekDerivations, strConnection, false);
                }
            }
        }

    }
}