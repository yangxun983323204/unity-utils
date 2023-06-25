# encoding=utf8
# addr2line -e out/target/product/spm8666p1_64_car/system/lib64/libandroid_servers.so 0000000000037918 -f
import os
import sys
import re

addr2line = "x86_64-linux-android-addr2line.exe"

def GetLine(addr, so):
    cmd = f"{addr2line} -e {so} {addr} -f -i -p"
    p = os.popen(cmd)
    ret = p.read()
    print(f"{addr}->{ret}")
    return ret

def AddComment(line, soList)->str:
    tag = 'YXComment:'
    exp = r'pc [0-9a-zA-Z]+'
    if tag in line:
        return line
    else:
        for so in soList:
            if so in line:
                addrs = re.findall(exp, line)
                if not addrs is None and len(addrs)>=1:
                    addr = addrs[0]
                    line = f"{line}    {tag}{GetLine(addr, so)}"
                    return line
    
    return line



def Main():
    tombstone = sys.argv[1]
    output = tombstone+".line.txt"

    soList = ["libil2cpp.so"]
    with open(tombstone, 'r',encoding='utf-8') as f, open(output,'w+',encoding='utf-8') as of:
        for line in f:
            line = AddComment(line, soList)
            of.write(line)


Main()
