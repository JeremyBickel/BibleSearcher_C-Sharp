import json

a = []
l = 0

with open('NTWords.csv', 'r') as ntw, open('NTWords.json', 'w') as ou:
    for item in ntw:
        l += 1
        if l > 1:
            la = item.strip().split('^')
            td = {
                "gloss": la[0],
                "book_number": la[1],
                "chapter_number": la[2],
                "verse_number": la[3],
                "word_id": la[4],
                "word_function": la[5],
                "clause_level": la[6],
                "clause": la[7],
                "clause_function": la[8],
                "subclause": la[9],
                "subclause_function": la[10],
                "greek_clause_level": la[11],
                "greek_id": la[12],
                "strongs": la[13],
                "morphology": la[14],
                "greek_word": la[15]
            }
            a.append(td)

    ou.write(json.dumps(a))

