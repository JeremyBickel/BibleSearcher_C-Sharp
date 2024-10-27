import re
import json

regex_patterns = {
    "book_num": r"v(?P<book_num>\d+)",
    "chap_num": r"\.(?P<chap_num>\d+)\.",
    "verse_num": r"\.\d+\.(?P<verse_num>\d+)",
    "clause_num": r"c:c(?P<clause_num>\d+)",
    "clause_kind": r"CKind:(?P<clause_kind>[^,^]+)",
    "clause_type": r"CTyp:(?P<clause_type>[^,^]+)",
    "phrase_num": r"p:p(?P<phrase_num>\d+)",
    "phrase_tud": r"p[tud]:(?P<phrase_tud>[^,^]+)",
    "word_h": r"w:wh(?P<word_h>\d+)",
    "word_english": r"we:(?P<word_english>[^,^]+)",
    "word_morph": r"wp:(?P<word_morph>[^,^]+)"
}

def parse_data_line(line):
    parsed_data = {
        "book_num": "",
        "chap_num": "",
        "verse_num": "",
        "clauses": []
    }
    current_clause = None
    current_phrase = None
    current_word = None

    for segment in line.split("^"):
        segment = segment.strip()
        for key, pattern in regex_patterns.items():
            match = re.search(pattern, segment)
            if match:
                if key in ["book_num", "chap_num", "verse_num"]:
                    parsed_data[key] = match.group(key)
                elif key == "clause_num":
                    if current_clause:
                        parsed_data["clauses"].append(current_clause)
                    current_clause = {"clause_num": match.group(key), "phrases": []}
                elif key in ["clause_kind", "clause_type"]:
                    if current_clause is not None:
                        current_clause[key] = match.group(key)
                elif key == "phrase_num":
                    if current_phrase:
                        if current_clause is not None:
                            current_clause["phrases"].append(current_phrase)
                    current_phrase = {"phrase_num": match.group(key), "words": []}
                    current_word = None  # Reset the current word for a new phrase
                elif key in ["phrase_tud"]:
                    if current_phrase is not None:
                        current_phrase["phrase_type"] = match.group(key)
                elif key in ["word_h", "word_english", "word_morph"]:
                    if current_phrase is not None:
                        if current_word is None or key == "word_h":
                            # Start a new word dictionary if none exists or a new word starts
                            current_word = {"word_num": match.group(key)}
                            
                            current_phrase["words"].append(current_word)
                        else:
                            # Update the current word dictionary
                            if key == "word_english":
                                current_word["word_gloss"] = match.group(key)
                            else:
                                current_word[key] = match.group(key)

    # Append the last phrase and clause if they exist
    if current_phrase:
        if current_clause is not None:
            current_clause["phrases"].append(current_phrase)
    if current_clause:
        parsed_data["clauses"].append(current_clause)

    return parsed_data


def parse_data_file(file_path):
    with open(file_path, 'r') as file:
        data_lines = file.readlines()

    # Initialize variables
    json_data = []
    current_record_lines = []
    verse_pattern = re.compile(r"^v\d+\.\d+\.\d+")

    for line in data_lines:
        line = line.strip()
        # Check if the line is the start of a new verse record
        if verse_pattern.match(line):
            # If there are accumulated lines for the previous record, parse them
            if current_record_lines:
                parsed_record = parse_multiple_lines(current_record_lines)
                json_data.append(parsed_record)
                current_record_lines = []
        # Accumulate lines for the current verse record
        current_record_lines.append(line)

    # Parse the last accumulated record
    if current_record_lines:
        parsed_record = parse_multiple_lines(current_record_lines)
        json_data.append(parsed_record)

    return json_data

def parse_multiple_lines(data_lines):
    # This function now takes a list of lines belonging to a single verse record
    # and parses them as a whole
    combined_line = " ".join(data_lines)  # Combine all lines into one
    return parse_data_line(combined_line)  # Use the existing parse_data_line function

file_path = "MAB-OT.csv"
parsed_json = parse_data_file(file_path)

with open('MAB-OT.json', 'w') as outfile:
    json.dump(parsed_json, outfile, indent=4)
