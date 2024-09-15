import csv
import json

def read_and_group_data(filename):
    # Store data grouped by pericope
    grouped_data = {}

    with open(filename, "r") as f:
        reader = csv.DictReader(f, delimiter='^')
        
        for row in reader:
            # List pericopes that are not empty for the verse
            valid_pericopes = [row[f"Pericope {i}"].strip() for i in range(1, 5) if row[f"Pericope {i}"].strip()]

            # If there's no pericope, we skip the row
            if not valid_pericopes:
                continue

            # Find the best pericope (with the smallest range of verses)
            best_pericope = min(valid_pericopes, key=lambda p: len(grouped_data.get(p, {'verses': []})['verses']))

            if best_pericope not in grouped_data:
                grouped_data[best_pericope] = {'verses': [], 'start_ref': row['Verse Reference'], 'end_ref': row['Verse Reference']}
            else:
                grouped_data[best_pericope]['end_ref'] = row['Verse Reference']

            grouped_data[best_pericope]['verses'].append(row['Verse Text'])

    return grouped_data

def transform_to_json(data):
    output_data = {}
    
    for pericope, details in data.items():
        verse_range = f"{details['start_ref']}-{details['end_ref']}" if details['start_ref'] != details['end_ref'] else details['start_ref']
        concatenated_text = ' '.join(details['verses'])
        
        output_data[pericope] = {'verse_text': concatenated_text, 'verse_range': verse_range}

    return output_data

if __name__ == '__main__':
    input_file = "PericopeRangesVerseText.csv"
    output_file = "PericopeGroupedVerseText.json"
    
    grouped_data = read_and_group_data(input_file)
    json_data = transform_to_json(grouped_data)
    
    with open(output_file, "w") as f:
        json.dump(json_data, f, indent=4)