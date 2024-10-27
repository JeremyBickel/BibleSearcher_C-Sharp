import json

jdata = []

with open("KJV.json", "r", encoding='utf-8') as inf:
	jdata = json.load(inf)

with open("KJVRefs.txt", "w", encoding='utf-8') as outf:
	for rec in jdata:
		outf.write(rec["Reference"] + "\n")
