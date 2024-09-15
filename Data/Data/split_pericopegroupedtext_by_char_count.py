import json

def split_text(text, max_length):
    """Split the text into chunks of up to max_length, without splitting words."""
    words = text.split()
    chunks = []
    chunk = []

    for word in words:
        # If adding the next word exceeds the max_length, the current chunk is stored
        if len(' '.join(chunk + [word])) > max_length:
            chunks.append(' '.join(chunk))
            chunk = [word]
        else:
            chunk.append(word)
    if chunk:
        chunks.append(' '.join(chunk))
    return chunks

def transform_data(data, char_count):
    transformed_data = []

    for key, value in data.items():
        record = {
            'Pericope': key,
            'verse_range': value['verse_range'],
            'verse_chunks': []
        }
        
        verse_text_chunks = split_text(value['verse_text'], char_count)

        for idx_v, verse_text_chunk in enumerate(verse_text_chunks, 1):
            verse_chunk_record = {
                'verse_text': verse_text_chunk,
                'chunk_id': idx_v
            }
            record['verse_chunks'].append(verse_chunk_record)
            
        transformed_data.append(record)

    return transformed_data

def write_transformed_data_to_file(transformed_data):
    with open("PericopeGroupedVerseText-Transformed.json", "w") as f:
        json.dump(transformed_data, f, indent=4)

if __name__ == "__main__":
    with open("PericopeGroupedVerseText.json", "r") as f:
        data = json.load(f)

    char_count = int(input("Enter the character count for splitting: "))

    transformed_data = transform_data(data, char_count)
    write_transformed_data_to_file(transformed_data)
