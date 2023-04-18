#!/usr/bin/python

import collections
import os
from tkinter import *
import json
from turtle import width

KEY_REG = "scopedRegistries"
KEY_NAME = "name"
KEY_SCOPES = "scopes"
KEY_URL = "url"
KEY_DEP = "dependencies"

def Find(li, func):
    for i in li:
        if func(i):
            return i
    return None

class Merge:
    
    def Do(left:str, right:str, regSort=False, depSort=False)->tuple[str,str]:
        l = json.loads(left)
        r = json.loads(right)

        out = {}
        out[KEY_REG] = []
        out[KEY_DEP] = collections.OrderedDict()
        infos = []
        infos += Merge.AddOne(out,l)
        infos += Merge.AddOne(out,r)
        Merge.Sort(out,regSort,depSort)
        infoStr = ""
        for line in infos:
            if line != None:
                infoStr += line
                infoStr += '\n'

        return (json.dumps(out,indent=2), infoStr)

    def AddOne(out, one):
        selfRegs = out[KEY_REG]
        regs = one[KEY_REG]
        for r in regs:
            item = Find(selfRegs,lambda i:i[KEY_NAME]==r[KEY_NAME])
            if item==None:
                item = collections.OrderedDict()
                item[KEY_NAME]=r[KEY_NAME]
                item[KEY_SCOPES]=[]
                item[KEY_URL] = r[KEY_URL]
                out[KEY_REG].append(item)
            
            for scp in r[KEY_SCOPES]:
                if not scp in item[KEY_SCOPES]:
                    item[KEY_SCOPES].append(scp)

        selfDeps = out[KEY_DEP]
        deps = one[KEY_DEP]
        info = []
        for k in deps.keys():
            item = Find(selfDeps,lambda i:k == i)
            if item==None:
                selfDeps[k]=deps[k]
            else:
                old = selfDeps[k]
                if old<deps[k]:
                    selfDeps[k]=deps[k]
                    info.append("{0},    {1}->{2}".format(k, old, selfDeps[k]))
        
        return info

    def Sort(out,sortReg,sortDep):
        if sortReg:
            for reg in out[KEY_REG]:
                reg[KEY_SCOPES].sort()
        
        if sortDep:
            newDep = collections.OrderedDict()
            keys = list(out[KEY_DEP].keys())
            keys.sort()
            for k in keys:
                newDep[k] = out[KEY_DEP][k]
            
            out[KEY_DEP] = newDep

class MainWnd:
    def __init__(self) -> None:
        self.wnd = Tk()
        self.wnd.geometry('900x900')
        self.wnd.title("manifest merge")

        self.infoWnd = None

        frmTop = Frame(self.wnd,name='frmTop')
        frmTop.pack(side=TOP,expand=1, fill=BOTH)
        inputL = Text(frmTop,name='inputL',width=1)
        inputL.pack(side=LEFT,expand=1,fill=BOTH,padx=2)
        inputR = Text(frmTop,name='inputR',width=1)
        inputR.pack(side=LEFT,expand=1,fill=BOTH,padx=2)

        btnMerge = Button(self.wnd,name='btnMerge',text='合并',command=self.__OnBtnMergeClick)
        btnMerge.pack(padx=2,pady=2)

        self.regSort = IntVar()
        self.depSort = IntVar()
        Checkbutton(self.wnd,name='regSort',text = 'scopes排序', variable=self.regSort).pack()
        Checkbutton(self.wnd,name='depSort',text = 'dependencies排序', variable=self.depSort).pack()

        frmBottom = Frame(self.wnd,name='frmBottom')
        frmBottom.pack(side=BOTTOM,expand=1, fill=BOTH)
        inputB = Text(frmBottom,name='inputB')
        inputB.pack(side=LEFT,expand=1,fill=BOTH,pady=2)

    def __OnBtnMergeClick(self):
        left = self.wnd.children["frmTop"].children["inputL"].get(1.0,END)
        right = self.wnd.children["frmTop"].children["inputR"].get(1.0,END)
        regSort = self.regSort.get() == 1
        depSort = self.depSort.get() == 1
        merge = Merge.Do(left, right,regSort,depSort)
        out = self.wnd.children["frmBottom"].children["inputB"]
        out.delete(1.0,END)
        out.insert(1.0,merge[0])
        # 弹出一个额外窗口用以显示合并的项
        if merge[1]!="":
            self.__ShowMergeInfo(merge[1])
        elif self.infoWnd != None:
            self.infoWnd.withdraw()

    def __infoWndCallback(self):
        if self.infoWnd != None:
            self.infoWnd.withdraw()

    def __ShowMergeInfo(self, info):
        if self.infoWnd == None:
            self.infoWnd = Toplevel(self.wnd)
            self.infoWnd.geometry('800x500')
            self.infoWnd.title("merge info")
            self.infoWnd.protocol("WM_DELETE_WINDOW", self.__infoWndCallback)

            self.inputInfo = Text(self.infoWnd,name='inputInfo')
            self.inputInfo.pack(side=LEFT,expand=1,fill=BOTH,pady=2)
            
        self.infoWnd.deiconify()
        self.inputInfo.delete(1.0,END)
        self.inputInfo.insert(1.0,info)

MainWnd().wnd.mainloop()