import csv
import re

# Initialize an empty dictionary to store the verses
verses = {}

# Function to split text into sentences based on provided delimiters
def split_into_sentences(text, delimiters):
    # Remove all characters except a-z, A-Z, 0-9, and sentence break characters
    cleaned_text = re.sub(r'[^a-zA-Z0-9.!?;:,-]', ' ', text)
    sentences = re.split(f'([{delimiters}])', cleaned_text)  # Keep the delimiters
    # Group the delimiters with the sentences and strip whitespace
    sentences = [sentences[i].strip() + (sentences[i + 1] if i + 1 < len(sentences) else '') for i in range(0, len(sentences), 2)]
    return [sentence for sentence in sentences if sentence.strip()]

# Read the CSV file and populate the verses dictionary
with open('Processed\\PericopeMAB.csv', 'r', encoding='utf-8') as csvfile:
    reader = csv.reader(csvfile, delimiter='^')
    next(reader)  # Skip the header row
    for row in reader:
        reference = f"{row[2]}.{row[3]}.{row[4]}"
        gloss = row[1].replace("[", "").replace("]", "")  # Remove square brackets
        if reference not in verses:
            verses[reference] = []
        verses[reference].append(gloss)

with open('Processed\\MAB.txt', 'w', encoding='utf-8') as outfile:
    outfile.write("reference^sentence\n") # write the header
    for reference, glosses in verses.items():
        verse_text = ' '.join(glosses)
        sentences = split_into_sentences(verse_text, ".!?:;") # Keep the - as a translation of some Greek words
        for i, sentence in enumerate(sentences):
            sentence_number = i + 1
            full_reference = f"{reference}.{sentence_number}"
            outfile.write(f"{full_reference}^{sentence.strip()}\n")
            print(f"{full_reference}^{sentence.strip()}")
