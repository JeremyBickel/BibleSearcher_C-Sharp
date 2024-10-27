import json

# Mapping of book numbers to book names as provided
book_mapping = {
    1: "Genesis",
    2: "Exodus",
    3: "Leviticus",
    4: "Numbers",
    5: "Deuteronomy",
    6: "Joshua",
    7: "Judges",
    8: "Ruth",
    9: "1 Samuel",
    10: "2 Samuel",
    11: "1 Kings",
    12: "2 Kings",
    13: "1 Chronicles",
    14: "2 Chronicles",
    15: "Ezra",
    16: "Nehemiah",
    17: "Esther",
    18: "Job",
    19: "Psalms",
    20: "Proverbs",
    21: "Ecclesiastes",
    22: "Song of Solomon",
    23: "Isaiah",
    24: "Jeremiah",
    25: "Lamentations",
    26: "Ezekiel",
    27: "Daniel",
    28: "Hosea",
    29: "Joel",
    30: "Amos",
    31: "Obadiah",
    32: "Jonah",
    33: "Micah",
    34: "Nahum",
    35: "Habakkuk",
    36: "Zephaniah",
    37: "Haggai",
    38: "Zechariah",
    39: "Malachi"
}

# Load the pericope file (p)
with open('PericopeGroupedKJVVerses.json', 'r') as file:
    pericope_data = json.load(file)

# Load the mab-ot file (m)
with open('MAB-OT.json', 'r') as file:
    mab_ot_data = json.load(file)

# Mapping verses in pericope data to their corresponding pericope info
pericope_mapping = {}
for pericope in pericope_data:
    pericope_info = {
        'Pericope': pericope['Pericope'],
        'Reference Start': pericope['Reference Start'],
        'Reference End': pericope['Reference End']
    }
    for verse in pericope['Verses']:
        pericope_mapping[verse['Reference']] = pericope_info

# Function to format book, chapter, and verse numbers for matching with pericope references
def format_reference(book_num, chap_num, verse_num):
    # Convert book_num to integer for correct mapping
    book_num_int = int(book_num)
    book_name = book_mapping.get(book_num_int, "Unknown")
    return f"{book_name} {chap_num}:{verse_num}"

# Updating mab-ot data with pericope information
for entry in mab_ot_data:
    ref = format_reference(entry['book_num'], entry['chap_num'], entry['verse_num'])
    if ref in pericope_mapping:
        entry.update(pericope_mapping[ref])

# Save the updated MAB-OT data to a new JSON file
updated_mab_ot_filename = 'Updated_MAB_OT.json'
with open(updated_mab_ot_filename, 'w') as file:
    json.dump(mab_ot_data, file, indent=4)
