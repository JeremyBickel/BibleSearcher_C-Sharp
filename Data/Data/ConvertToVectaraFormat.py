import json

data_in = []

with open("PericopeGroupedKJVVerses.json", "r") as fin:
    data_in = json.load(fin)

#print (data)
#input()

data_out = []
for data in data_in:

    new_data = {}
    
    new_data["title"] = data["Pericope"]
    new_data["metadataJson"] = json.dumps({"Reference Start": data["Reference Start"], "Reference End": data["Reference End"]})
    new_data["section"] = []

    for verse in data["Verses"]:
        section = {}
        section["text"] = verse["Text"]
        section["metadataJson"] = json.dumps({"reference": verse["Reference"]})
        new_data["section"].append(section)

    data_out.append(new_data)

with open("PericopeGroupedKJVVerses-Vectara.json", "w") as fout:
    json.dump(data_out, fout, indent=4)

#print(result)