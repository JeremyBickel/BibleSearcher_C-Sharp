import json
import re

data = []

def format_bible_verse(data):
	# Split the data by comma
    parts = data.split(',')
    if len(parts) < 3:
        raise ValueError("Data does not contain enough parts")

    # Extract book, chapter, and verse
    book = parts[0].strip()
    chapter = parts[1].strip()
    verse = parts[2].strip()

    # Combine the remaining parts and remove numbers in curly braces
    text = ','.join(parts[3:]).strip()
    text = re.sub(r'\{\s*\d+\s*\}', '', text)
    text = re.sub(r'\s{2,}', ' ', text) #collapse multiple spaces to 1

    # Construct the output
    output = {
        "Reference": f"{book.capitalize()} {chapter}:{verse}",
        "Text": f"{text}"
    }

    return output

with open("kjvstrongs.csv", "r", encoding='utf-8') as inf:
    for line in inf:
        data.append(format_bible_verse(line))

with open("KJV.json", "w", encoding='utf-8') as outf:
    json.dump(data, outf, indent=4)