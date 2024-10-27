using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepAI
{
    public class Welcome
    {
        public Welcome()
        {
            // Initialize variables
            int staticBatchSize = 45;
            List<string> allWords = new List<string>();
            string lastTruncatedWord = "";
            int startIdx = 0;

            // Read all words into a list
            using (StreamReader srWordList = new StreamReader("F:\\BibleSearcher\\Data\\Data\\Processed\\WordList.txt"))
            {
                string line;
                while ((line = srWordList.ReadLine()) != null)
                {
                    string[] words = line.Split();
                    allWords.AddRange(words);
                }
            }

            using (StreamWriter swAnnotatedWordList = new StreamWriter("F:\\BibleSearcher\\Data\\Data\\Processed\\DeepAITaggedWordList.txt"))
            {
                while (startIdx < allWords.Count)
                {
                    // Prepare the batch of words for the prompt
                    List<string> wordBatch = allWords.GetRange(startIdx, Math.Min(staticBatchSize, allWords.Count - startIdx));

                    // If there was a truncated word in the last batch, prepend it to this batch
                    if (!string.IsNullOrEmpty(lastTruncatedWord))
                    {
                        wordBatch.Insert(0, lastTruncatedWord);
                        lastTruncatedWord = "";
                    }

                    // Send the batch for processing and receive response
                    string prompt = "List some POS, synonyms, antonyms, homophones, homographs, meronyms, holonyms, hypernyms, hyponyms, troponyms, and coordinate terms for the words " + string.Join(", ", wordBatch) + ". Use N/A if there aren't any.";
                    string response = SendText(prompt);

                    // Check for truncation and handle it
                    lastTruncatedWord = CheckAndHandleTruncation(swAnnotatedWordList, response);

                    // Determine the index for the next batch
                    startIdx += staticBatchSize;
                    if (!string.IsNullOrEmpty(lastTruncatedWord))
                    {
                        startIdx = allWords.IndexOf(lastTruncatedWord);
                    }
                }
            }
        }

        public static string SendText(string strPrompt)
        {
            // directly send a text string
            DeepAI_API api = new DeepAI_API(apiKey: "3edae7fa-7996-473d-a99c-a0c5bac6d837");

            StandardApiResponse resp = api.callStandardApi("text-generator", new
            {
                text = strPrompt,
            });
            return api.objectAsJsonString(resp);
        }

        public static string CheckAndHandleTruncation(StreamWriter swAnnotatedWordList, string response)
        {
            // Split the response into records based on two consecutive newlines
            string[] records = response.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Check if the last record is truncated
            string lastRecord = records[records.Length - 1];
            bool isTruncated = !lastRecord.Contains("- Coordinate Terms:");

            if (!isTruncated)
            {
                // No truncation, write the whole response to the file
                swAnnotatedWordList.WriteLine(response);
                return ""; // No truncation, return an empty string
            }
            else
            {
                // The last record is truncated, find the last complete record
                string lastCompleteRecord = records[records.Length - 2];

                // Extract the last complete word from the last complete record
                // The word is the first line in each record
                string lastWord = lastCompleteRecord.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];

                // Write only the complete records to the file
                for (int i = 0; i < records.Length - 1; i++)
                {
                    swAnnotatedWordList.WriteLine(records[i]);
                    swAnnotatedWordList.WriteLine(); // Write an empty line between records
                }

                return lastWord; // Return the last complete word
            }
        }
    }
}

