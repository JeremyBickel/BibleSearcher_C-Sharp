header = 1 # Change header to the number of rows to skip
data = []

with open("WordIDs.csv", "r") as f:
    # Skip rows according to header value
    for i in range(header):
        next(f) 
    data = f.readlines()
    
text_list = []

for line in data:
    column_data = line.split("^")
    text = column_data[1].strip()
    text_list.append(text)

text_list.sort()
out = []

with open("WordList-oneperline.txt", "w") as w:
    for i, word in enumerate(text_list):
        out.append(word)
        out.append('\n')
    # Join the list of words with spaces
    out_string = " ".join(out)
    w.write(out_string)  
'''    
with open("WordList.txt", "w") as w:
    # Iterate over every 50th word and insert a newline
    for i, word in enumerate(text_list):
        out.append(word)
        if (i+1) % 50 == 0 and i > 0:
            out.append('\n')
    # Join the list of words with spaces
    out_string = " ".join(out)
    w.write(out_string)  
    '''