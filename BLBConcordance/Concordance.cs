using System.Text.RegularExpressions;

namespace BLBConcordance
{
    public class Concordance
    {
        public void CreateBLBHebrewConcordance(ref StreamReader srConcordanceInput,
            ref Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance)
        {
            Regex rgxNumbers = new("[0-9]{1,}");

            while (!srConcordanceInput.EndOfStream)
            {
                string[] strsLine = srConcordanceInput.ReadLine().Split('=');
                int intNumber = Convert.ToInt32(rgxNumbers.Match(strsLine[0]).Value) + 1; //NOTE: This makes the number deviate from the data file by +1.
                string strConc = strsLine[1].Trim();

                strConc = strConc.TrimEnd(';');
                strConc = strConc.Trim("'".ToCharArray()[0]);

                dBLBHebrewConcordance.Add(intNumber, new Dictionary<int, string>());

                if (strConc.Contains('|'))
                {
                    int intConcordanceID = 0;

                    foreach (string strConcRef in strConc.Split("|"))
                    {
                        intConcordanceID++;

                        dBLBHebrewConcordance[intNumber].Add(intConcordanceID, strConcRef);
                    }
                }
                else
                {
                    dBLBHebrewConcordance[intNumber].Add(1, strConc);
                }
            }
        }

        public void FillBLBHebrewConcordance(ref StreamReader srConcordance,
            ref Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance)
        {
            bool bSeenHeader = false;

            while (!srConcordance.EndOfStream)
            {
                string[] strsLine = srConcordance.ReadLine().Split('^');

                if (bSeenHeader == true)
                {
                    int intGroupNumber = Convert.ToInt16(strsLine[0].Trim());

                    if (!dBLBHebrewConcordance.ContainsKey(intGroupNumber))
                    {
                        dBLBHebrewConcordance.Add(intGroupNumber,
                        new Dictionary<int, string>());
                    }

                    dBLBHebrewConcordance[intGroupNumber].Add(
                        Convert.ToInt16(strsLine[1]), strsLine[2]);
                }
                else
                {
                    bSeenHeader = true;
                }
            }
        }

        public void WriteBLBHebrewConcordance(ref StreamWriter swBLBHebrewConcordance,
            ref Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance)
        {
            swBLBHebrewConcordance.WriteLine("GroupID ^ ReferenceID ^ Reference");

            foreach (int intGroupID in dBLBHebrewConcordance.Keys.OrderBy(a => a))
            {
                foreach (int intReferenceID in dBLBHebrewConcordance[intGroupID].Keys.OrderBy(a => a))
                {
                    swBLBHebrewConcordance.WriteLine(intGroupID.ToString() + " ^ " +
                        intReferenceID.ToString() + " ^ " +
                        dBLBHebrewConcordance[intGroupID][intReferenceID]);
                }
            }

            swBLBHebrewConcordance.Close();
        }

        public void CreateBLBGreekConcordance(ref StreamReader srConcordanceInput,
            ref Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance)
        {
            Regex rgxNumbers = new("[0-9]{1,}");

            while (!srConcordanceInput.EndOfStream)
            {
                string[] strsLine = srConcordanceInput.ReadLine().Split('=');
                int intNumber = Convert.ToInt32(rgxNumbers.Match(strsLine[0]).Value) + 1; //NOTE: This makes the number deviate from the data file by +1.
                string strConc = strsLine[1].Trim();

                strConc = strConc.TrimEnd(';');
                strConc = strConc.Trim("'".ToCharArray()[0]);

                dBLBGreekConcordance.Add(intNumber, new Dictionary<int, string>());

                if (strConc.Contains('|'))
                {
                    int intConcordanceID = 0;

                    foreach (string strConcRef in strConc.Split("|"))
                    {
                        intConcordanceID++;

                        dBLBGreekConcordance[intNumber].Add(intConcordanceID, strConcRef);
                    }
                }
                else
                {
                    dBLBGreekConcordance[intNumber].Add(1, strConc);
                }
            }
        }

        public void FillBLBGreekConcordance(ref StreamReader srConcordance,
            ref Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance)
        {
            bool bSeenHeader = false;

            while (!srConcordance.EndOfStream)
            {
                string[] strsLine = srConcordance.ReadLine().Split('^');

                if (bSeenHeader == true)
                {
                    int intGroupNumber = Convert.ToInt16(strsLine[0].Trim());

                    if (!dBLBGreekConcordance.ContainsKey(intGroupNumber))
                    {
                        dBLBGreekConcordance.Add(intGroupNumber,
                        new Dictionary<int, string>());
                    }

                    dBLBGreekConcordance[intGroupNumber].Add(
                        Convert.ToInt16(strsLine[1]), strsLine[2]);
                }
                else
                {
                    bSeenHeader = true;
                }
            }
        }

        public void WriteBLBGreekConcordance(ref StreamWriter swBLBGreekConcordance,
            ref Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance)
        {
            swBLBGreekConcordance.WriteLine("GroupID ^ ReferenceID ^ Reference");

            foreach (int intGroupID in dBLBGreekConcordance.Keys.OrderBy(a => a))
            {
                foreach (int intReferenceID in dBLBGreekConcordance[intGroupID].Keys.OrderBy(a => a))
                {
                    swBLBGreekConcordance.WriteLine(intGroupID.ToString() + " ^ " +
                        intReferenceID.ToString() + " ^ " +
                        dBLBGreekConcordance[intGroupID][intReferenceID]);
                }
            }

            swBLBGreekConcordance.Close();
        }

    }
}
