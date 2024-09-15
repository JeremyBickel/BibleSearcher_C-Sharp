import json
import re

# input data
verses = []
pericopes_data = []
refs = []

# compiling data
data = []

with open("KJV.json", 'r', encoding='utf-8') as fin:
    verses = json.load(fin)

with open("ContiguousWholeBiblePericopes.json", 'r', encoding='utf-8') as fin:
    pericopes_data = json.load(fin)

with open("KJVRefs.txt", "r", encoding='utf-8') as inf:
    refs = [line for line in inf]
    
def get_next_pericope_dict():
    #each of these contains "Pericope", "Reference Start", "Reference End", and "Pericope Hierarchy"
    for per_rec in pericopes_data:
        #print(rec)
        yield per_rec

def get_verse_dict_by_ref(data, ref):
    for record in data:
        if record.get("Reference") == ref:
            return record
    return None

next_per = get_next_pericope_dict()

current_per_record = next(next_per)
del current_per_record["Pericope Hierarchy"]
current_per_record["Verses"] = []

for ref in refs:
    current_per_record["Verses"].append(get_verse_dict_by_ref(verses, ref.strip()))
        
    if ref.strip() == current_per_record["Reference End"]:
        data.append(current_per_record)

        try:
            current_per_record = next(next_per)
            del current_per_record["Pericope Hierarchy"]
            current_per_record["Verses"] = []
        except:
            pass

with open("PericopeGroupedKJVVerses.json", 'w', encoding='utf-8') as fout:
    json.dump(data, fout, indent=4)

print("DONE")
