StreamReader srPericopeRangesVerseText = new StreamReader("PericopeRangesVerseText.csv");
StreamWriter swPericopeBlockText = new StreamWriter("PericopeBlockText.csv");
string strLastPericope1 = "";
string strLastPericope2 = "";
string strLastPericope3 = "";
string strLastPericope4 = "";
string strFirstReference = "";
string strLastReference = "";
string strVersesBlock = "";
bool bStart = true;
string[] strsLine = (string[])Array.CreateInstance(typeof(string), 6);

srPericopeRangesVerseText.ReadLine(); //skip header

while (!srPericopeRangesVerseText.EndOfStream)
{
    string strLine = srPericopeRangesVerseText.ReadLine();

    strsLine = strLine.Split('^');

    if (bStart == true)
    {
        strLastPericope1 = strsLine[2];
        strLastPericope2 = strsLine[3];
        strLastPericope3 = strsLine[4];
        strLastPericope4 = strsLine[5];

        strFirstReference = strsLine[0];
        strLastReference = strsLine[0];

        bStart = false;
    }


    if (strLastPericope1 == strsLine[2] &&
        strLastPericope2 == strsLine[3] &&
        strLastPericope3 == strsLine[4] &&
        strLastPericope4 == strsLine[5]) //pericope hasn't changed
    {
        strVersesBlock += " " + strsLine[1];

        strLastReference = strsLine[0];
    }
    else
    {
        swPericopeBlockText.WriteLine(strLastPericope1 + "^" + strLastPericope2 + "^" + strLastPericope3 +
            "^" + strLastPericope4 + "^" + strFirstReference + " - " + strLastReference + "^" + strVersesBlock.Trim());

        strVersesBlock = strsLine[1];
        strFirstReference = strsLine[0];
        strLastReference = strsLine[0];
        strLastPericope1 = strsLine[2];
        strLastPericope2 = strsLine[3];
        strLastPericope3 = strsLine[4];
        strLastPericope4 = strsLine[5];

    }
}

swPericopeBlockText.WriteLine(strsLine[2] + "^" + strsLine[3] + "^" + strsLine[4] +
        "^" + strsLine[5] + "^" + strFirstReference + " - " + strLastReference + "^" + strVersesBlock.Trim());
swPericopeBlockText.Close();