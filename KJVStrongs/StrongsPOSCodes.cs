namespace KJVStrongs
{
    public class StrongsPOSCodes
    {
        public string ConvertPOSPartToPOSCode(string strPOSPart)
        {
            ///Possible POS Codes:
            ///
            ///noun
            ///verb
            ///adjective
            ///adverb
            ///pronoun
            ///preposition
            ///conjunction
            ///particle - unable to be inflected and not fitting in other POS categories (went *away*, started *out*, talk *over*, *to* go, *to* decide, *now* what do.., *well* I will.., *not* do, *not* travel, *not* have, moving *along*, ate *up*)
            ///participle - verb used in 1) adjective (burnt toast, dyed fabric), 2) noun (loves sitting [as in "loves to sit"], fine breeding) or 3) compound verb (was sung, are sharing, has been)
            ///
            string strReturn = "";

            switch (strPOSPart)
            {
                default:
                    break;

                case "":
                    break;
                case "adj":
                    break;
                case "adj  adv":
                    break;
                case "adj  adv  n m":
                    break;
                case "adj  adv  subst":
                    break;
                case "adj  coll":
                    break;
                case "adj  n":
                    break;
                case "adj  n m":
                    break;
                case "adj  n m  n f":
                    break;
                case "adj  n pr m":
                    break;
                case "adj  subst":
                    break;
                case "adj  subst  n pr m":
                    break;
                case "adj / part m":
                    break;
                case "adj bdb  n m clbl":
                    break;
                case "adj comparative":
                    break;
                case "adj f":
                    break;
                case "adj f  n pr f clbl":
                    break;
                case "adj gent":
                    break;
                case "adj intens":
                    break;
                case "adj m":
                    break;
                case "adj m/f":
                    break;
                case "adj n":
                    break;
                case "adj patr":
                    break;
                case "adj pr":
                    break;
                case "adj pr f used as adv":
                    break;
                case "adj pr patr":
                    break;
                case "adj pr pl":
                    break;
                case "adj subst":
                    break;
                case "adj subst pl emphatic":
                    break;
                case "adj verbal":
                    break;
                case "adj/subst":
                    break;
                case "adjective":
                    break;
                case "adv":
                    break;
                case "adv  conj":
                    break;
                case "adv  n m":
                    break;
                case "adv  prep":
                    break;
                case "adv  subst":
                    break;
                case "adv  subst  n m":
                    break;
                case "adv comparative":
                    break;
                case "adv f":
                    break;
                case "adv n":
                    break;
                case "adv prep conj subst":
                    break;
                case "adv subst":
                    break;
                case "adv subst and acc":
                    break;
                case "adv superlative":
                    break;
                case "adv with restrictive force":
                    break;
                case "adv/conj":
                    break;
                case "adverb":
                    break;
                case "aramaic transliteration":
                    break;
                case "article":
                    break;
                case "comparative":
                    break;
                case "conditional part":
                    break;
                case "conj":
                    break;
                case "conj  adv":
                    break;
                case "conj  prep":
                    break;
                case "conj denoting addition  esp of something greater  adv":
                    break;
                case "conj particle":
                    break;
                case "conj prep":
                    break;
                case "conj/adv":
                    break;
                case "conjunction":
                    break;
                case "contr":
                    break;
                case "dem adv":
                    break;
                case "demon pron":
                    break;
                case "demons part":
                    break;
                case "demons particle":
                    break;
                case "demons pron":
                    break;
                case "demons pron  adv":
                    break;
                case "demons pron  rel pron":
                    break;
                case "demons pron  rel pron f":
                    break;
                case "demons pron f":
                    break;
                case "demons pron f / adv":
                    break;
                case "demonstr pron":
                    break;
                case "demonstr pron p":
                    break;
                case "demonstr pron pl":
                    break;
                case "direct object indicator":
                    break;
                case "exclamation":
                    break;
                case "f":
                    break;
                case "h f":
                    break;
                case "idiom":
                    break;
                case "imperative":
                    break;
                case "indef pron":
                    break;
                case "inter":
                    break;
                case "inter adv":
                    break;
                case "interj":
                    break;
                case "interj  hypothetical part":
                    break;
                case "interj  v":
                    break;
                case "interj imper":
                    break;
                case "interj subst":
                    break;
                case "interr adv":
                    break;
                case "interr pron":
                    break;
                case "interr pron  indef pron":
                    break;
                case "interrog adv":
                    break;
                case "interrog adv  interj":
                    break;
                case "letter":
                    break;
                case "m adj":
                    break;
                case "m pr loc":
                    break;
                case "n":
                    break;
                case "n  adj m  f":
                    break;
                case "n  adv":
                    break;
                case "n  adv  adj":
                    break;
                case "n  n pr loc  n pr m":
                    break;
                case "n adj m f":
                    break;
                case "n adj pl":
                    break;
                case "n f":
                    break;
                case "n f  adj":
                    break;
                case "n f  adv":
                    break;
                case "n f  adv clbl":
                    break;
                case "n f  conj":
                    break;
                case "n f  n m":
                    break;
                case "n f  n pr f":
                    break;
                case "n f  n pr loc":
                    break;
                case "n f  n pr loc bdb":
                    break;
                case "n f  prep  conj":
                    break;
                case "n f abstr":
                    break;
                case "n f act part":
                    break;
                case "n f coll":
                    break;
                case "n f dual":
                    break;
                case "n f p":
                    break;
                case "n f pass part":
                    break;
                case "n f pl":
                    break;
                case "n f pr loc":
                    break;
                case "n f/m":
                    break;
                case "n indec  adj":
                    break;
                case "n indecl":
                    break;
                case "n indecl  adj":
                    break;
                case "n m":
                    break;
                case "n m  adj":
                    break;
                case "n m  adj clbl":
                    break;
                case "n m  adj v":
                    break;
                case "n m  adv":
                    break;
                case "n m  adv  conj":
                    break;
                case "n m  adv  prep":
                    break;
                case "n m  adv  with prep":
                    break;
                case "n m  adv accus  prep  conj  in compounds":
                    break;
                case "n m  adv clbl":
                    break;
                case "n m  interj":
                    break;
                case "n m  n f":
                    break;
                case "n m  n m abs pl intens":
                    break;
                case "n m  n pr dei":
                    break;
                case "n m  n pr loc":
                    break;
                case "n m  n pr m":
                    break;
                case "n m  n pr m bdb":
                    break;
                case "n m  n pr m clbl":
                    break;
                case "n m  persian":
                    break;
                case "n m  prep":
                    break;
                case "n m  subst":
                    break;
                case "n m  v":
                    break;
                case "n m bdb":
                    break;
                case "n m coll":
                    break;
                case "n m coll  n m":
                    break;
                case "n m collective":
                    break;
                case "n m constr with prep":
                    break;
                case "n m dei":
                    break;
                case "n m dual":
                    break;
                case "n m exten":
                    break;
                case "n m f coll":
                    break;
                case "n m loc":
                    break;
                case "n m or f":
                    break;
                case "n m p":
                    break;
                case "n m pater":
                    break;
                case "n m pl":
                    break;
                case "n m pl intens":
                    break;
                case "n m pl intensive":
                    break;
                case "n m used as adj":
                    break;
                case "n m/f":
                    break;
                case "n m/f  adj":
                    break;
                case "n m/f  n pr loc":
                    break;
                case "n m/f dual  adj":
                    break;
                case "n m/f/n":
                    break;
                case "n m/n":
                    break;
                case "n n":
                    break;
                case "n n pl":
                    break;
                case "n patr":
                    break;
                case "n patr m":
                    break;
                case "n patr m pl":
                    break;
                case "n pl":
                    break;
                case "n pl gent":
                    break;
                case "n pl pr loc":
                    break;
                case "n pr":
                    break;
                case "n pr  adj":
                    break;
                case "n pr coll":
                    break;
                case "n pr dei":
                    break;
                case "n pr dei  n pr m  n pr loc":
                    break;
                case "n pr deity":
                    break;
                case "n pr f":
                    break;
                case "n pr f  n pr f loc":
                    break;
                case "n pr f  n pr loc":
                    break;
                case "n pr f  n pr m":
                    break;
                case "n pr f act part":
                    break;
                case "n pr f deity  n pr loc":
                    break;
                case "n pr f loc":
                    break;
                case "n pr f pl":
                    break;
                case "n pr f pl loc":
                    break;
                case "n pr gent":
                    break;
                case "n pr gent  adj patr  n pr m":
                    break;
                case "n pr gent pl":
                    break;
                case "n pr loc":
                    break;
                case "n pr loc  adj":
                    break;
                case "n pr loc  adj gent":
                    break;
                case "n pr loc  m":
                    break;
                case "n pr loc  n pr deity":
                    break;
                case "n pr loc  n pr f":
                    break;
                case "n pr loc  n pr m":
                    break;
                case "n pr loc  n pr m clbl":
                    break;
                case "n pr loc m":
                    break;
                case "n pr loc m pl":
                    break;
                case "n pr loc pl":
                    break;
                case "n pr m":
                    break;
                case "n pr m  adj":
                    break;
                case "n pr m  adj pr":
                    break;
                case "n pr m  f":
                    break;
                case "n pr m  loc":
                    break;
                case "n pr m  n m":
                    break;
                case "n pr m  n patr":
                    break;
                case "n pr m  n patr m":
                    break;
                case "n pr m  n pr":
                    break;
                case "n pr m  n pr coll  n pr loc":
                    break;
                case "n pr m  n pr f":
                    break;
                case "n pr m  n pr f  n pr":
                    break;
                case "n pr m  n pr gent":
                    break;
                case "n pr m  n pr loc":
                    break;
                case "n pr m  n pr loc  n m":
                    break;
                case "n pr m  n pr people":
                    break;
                case "n pr m  n pr terr":
                    break;
                case "n pr m  n pr terr  n pr mont":
                    break;
                case "n pr m and loc":
                    break;
                case "n pr m coll":
                    break;
                case "n pr m loc":
                    break;
                case "n pr m loc  n pr m":
                    break;
                case "n pr m or adj":
                    break;
                case "n pr m or f":
                    break;
                case "n pr m p":
                    break;
                case "n pr m pl":
                    break;
                case "n pr m pl  persian":
                    break;
                case "n pr m pl loc":
                    break;
                case "n pr m/adj gent":
                    break;
                case "n pr m/loc":
                    break;
                case "n pr mont":
                    break;
                case "n pr mont  n pr loc  n pr arbour":
                    break;
                case "n pr obj":
                    break;
                case "n pr people":
                    break;
                case "n pr pl":
                    break;
                case "n pr pl loc":
                    break;
                case "n pr river":
                    break;
                case "n pr stream":
                    break;
                case "n pr terr/people":
                    break;
                case "n verbal":
                    break;
                case "neg adv":
                    break;
                case "noun feminine":
                    break;
                case "number representation":
                    break;
                case "numeral":
                    break;
                case "part":
                    break;
                case "part of entreaty":
                    break;
                case "part of relation  mark of genitive  conj":
                    break;
                case "participle":
                    break;
                case "particle":
                    break;
                case "particle indeclinable":
                    break;
                case "pass part":
                    break;
                case "pass part/inf f":
                    break;
                case "pers pron":
                    break;
                case "pers pron f pl":
                    break;
                case "phrase":
                    break;
                case "pr n f":
                    break;
                case "pr n loc":
                    break;
                case "pr n m":
                    break;
                case "prep":
                    break;
                case "prep  conj":
                    break;
                case "prep poetic form":
                    break;
                case "prep/conj":
                    break;
                case "preposition":
                    break;
                case "pron":
                    break;
                case "pron  f pl":
                    break;
                case "pron  p pl":
                    break;
                case "pron acc":
                    break;
                case "pron dat":
                    break;
                case "pron genn/abl":
                    break;
                case "pron interr/indef":
                    break;
                case "pron p m f":
                    break;
                case "pron p m pl":
                    break;
                case "pron p pl":
                    break;
                case "pron p s":
                    break;
                case "pron p s  demons pron":
                    break;
                case "pron pl reciprocal":
                    break;
                case "proper noun location":
                    break;
                case "relative part  conj":
                    break;
                case "subst":
                    break;
                case "subst  adv":
                    break;
                case "subst  adv  conj  with prep":
                    break;
                case "subst  adv  prep":
                    break;
                case "subst  adv  prep  with locative":
                    break;
                case "subst  adv  with prep  prep":
                    break;
                case "subst  adv of negation":
                    break;
                case "subst  prep  conj":
                    break;
                case "subst  prep  conj  adv":
                    break;
                case "subst m always used as a    prep":
                    break;
                case "subst n neg adv w/prep  n  neg  adv  w/prep":
                    break;
                case "subst prep":
                    break;
                case "untranslated particle":
                    break;
                case "v":
                    break;
                case "v  adj":
                    break;
                case "v  interj":
                    break;
                case "v  n":
                    break;
                case "v  n f":
                    break;
                case "v  n m":
                    break;
                case "v  n m  n f":
                    break;
                case "v  n m pl abst":
                    break;
                case "v  n m pl abstr":
                    break;
                case "v  v denom":
                    break;
                case "v  v inf as adv":
                    break;
                case "v adj":
                    break;
                case "v clbl":
                    break;
                case "v clbl  adj bdb":
                    break;
                case "v clbl  adj bdb/twot":
                    break;
                case "v clbl  adj v bdb/twot":
                    break;
                case "v denom":
                    break;
                case "v f":
                    break;
                case "v inf":
                    break;
                case "v part":
                    break;
                case "v part  n m":
                    break;
                case "v part pass":
                    break;
                case "v participle":
                    break;
                case "v/n m":
                    break;
                case "verb":
                    break;
                case "verb  n fem pl":
                    break;
            }

            return strReturn;
        }

        public string POSTagPhrase(string strPhrase)
        {
            string strReturn = "";

            return strReturn;
        }
    }
}
