'''
1 ^ 1215 ^ a root ^  ^ n m ^ awb ^ 'ab ^ 4a ^ False ^ True ^ ~{chief ^ 2} ^ {desire ^ 1} ^ {families ^ 2}% ^ ~{1 ^ father of an individual} ^ {2 ^ of God as father of his people} ^ {3 ^ head or founder of a household, group, family, or clan}%

1 ^ 1215 ^ a root ^  ^ n m ^ awb ^ 'ab ^ 4a ^ False ^ True ^ chief 2, desire 1, families 2 ^ 1 father of an individual, 2 of God as father of his people, 3 head or founder of a household, group, family, or clan
'''

import re

def transform_input(input_str, special_count = 4):
    #LexicalID ^ IsRoot ^ Total Translated ^ Connection ^ Gender ^ POS ^ Pronunciation ^ Transliteration ^ TWOT Number ^ ~Conjugated% ^ ~Translation Counts% ^ ~Extra TDNT Information% ^ ~Lexical Hierarchy%
    gr_pattern = re.compile(r'^([^~]{0,})~')
    gr_pattern_2 = re.compile(r'(.*)$')
    
    part_1_match = gr_pattern.match(input_str)
    part_1_str = part_1_match.group(1)
    part_1_length = len(part_1_str)

    input_str_2 = input_str[part_1_length:]

    part_2_match = gr_pattern_2.match(input_str_2)
    part_2_str = part_2_match.group(1)
    part_2_length = len(part_2_str)

    #print(f"1: {part_1_str}  2: {part_2_str}")

    gr_pattern_3 = re.compile(r'~([^%]{0,})%')
    gr_pattern_4 = re.compile(r'\{([^\}]{0,})\}')

    ctr = 0
    out_str = ""
    for m in re.findall(gr_pattern_3, part_2_str):
        part_3_str = ""
        for n in re.findall(gr_pattern_4, str(m)):
            nn = n
            if '^' in n:
                nn = re.sub(r'\^', ' ', n)
                #print(f"N: {n}  NN: {nn}")
            part_3_str += nn + '~'

        if len(part_3_str) > 0 and part_3_str[-1] == '~':
            part_3_str = part_3_str[:-1]
        ctr += 1
        if ctr < special_count:
            out_str += part_3_str + " ^ "
        else:
            out_str += part_3_str

    return part_1_str + out_str


input_file = 'BLBGreekLexicon.csv'
output_file = 'BLBGreekLexicon-out.csv'
with open(input_file, 'r') as fin, open(output_file, 'w') as fout:
    lin_ctr = 0
    for lin in fin:
        lin_ctr += 1
        if lin_ctr > 1:
            fout.write(transform_input(lin, 4) + "\n")
        else:
            fout.write("LexicalID ^ IsRoot ^ Total Translated ^ Connection ^ Gender ^ POS ^ Pronunciation ^ Transliteration ^ TWOT Number ^ Conjugated ^ Translation Counts ^ Extra TDNT Information ^ Lexical Hierarchy\n")


input_file = 'BLBHebrewLexicon.csv'
output_file = 'BLBHebrewLexicon-out.csv'
with open(input_file, 'r') as fin, open(output_file, 'w') as fout:
    lin_ctr = 0
    for lin in fin:
        lin_ctr += 1
        if lin_ctr > 1:
            fout.write(transform_input(lin, 2) + "\n")
        else:
            fout.write("LexicalID ^ Total Translated ^ Connection ^ Gender ^ POS ^ Pronunciation ^ Transliteration ^ TWOT Number ^ IsAramaic ^ IsRoot ^ Translation Counts ^ Lexical Hierarchy\n")