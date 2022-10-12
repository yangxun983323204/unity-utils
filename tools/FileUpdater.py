import os
import shutil
import tkinter as tk
from tkinter import messagebox
from tkinter import filedialog

sw=480
sh=256

window = tk.Tk()
msw,msh=window.maxsize()
window.title("文件更新工具")
window.geometry("{}x{}+{}+{}".format(sw,sh,int((msw-sw)/2),int((msh-sh)/2)))
window.resizable(width=False, height=False)
#=========================================
lh=30

currHIdx=0
def GetCurrH(goNext=True):
    global currHIdx
    val = lh * currHIdx
    if(goNext):
        currHIdx+=1
    return val

def GetWidth(percent=0):
    return int(percent*sw)

def GetLineH():
    return lh*0.8
#=========================================
class MyGUI:
    def Build(self):
        self.tarLbl = tk.Label(window,text="待更新目录",anchor="w")
        self.tarLbl.place(x=0,y=GetCurrH(),anchor="nw",width=GetWidth(1),height=GetLineH())

        self.tarInput = tk.Entry(window)
        self.tarInput.place(x=0,y=GetCurrH(False),anchor="nw",width=GetWidth(0.8),height=GetLineH())
        self.tarBtn = tk.Button(window,text="选择目录",command=self.OnChoiceTarDir)
        self.tarBtn.place(x=GetWidth(0.8)+4,y=GetCurrH()-4,anchor="nw")

        self.refLbl = tk.Label(window,text="参考目录",anchor="w")
        self.refLbl.place(x=0,y=GetCurrH(),anchor="nw",width=GetWidth(1),height=GetLineH())

        self.refInput = tk.Entry(window)
        self.refInput.place(x=0,y=GetCurrH(False),anchor="nw",width=GetWidth(0.8),height=GetLineH())
        self.refBtn = tk.Button(window,text="选择目录",command=self.OnChoiceRefDir)
        self.refBtn.place(x=GetWidth(0.8)+4,y=GetCurrH()-4,anchor="nw")

        self.readmeLbl = tk.Label(window,text="将会用【待更新目录】里的每个文件去【参考目录】里寻找并替换【待更新目录】自身的",anchor="w",fg="light green",bg="dark green")
        GetCurrH()
        self.readmeLbl.place(x=0,y=GetCurrH(),anchor="nw",width=GetWidth(1),height=GetLineH())

        self.updateBtn = tk.Button(window,text="更新!",fg="blue",command=self.OnUpdate)
        self.updateBtn.pack(side="bottom")
        self.Readme()
        window.mainloop()

    def OnChoiceTarDir(self):
        path = filedialog.askdirectory()
        if(path!=None):
            self.tarInput.select_clear()
            self.tarInput.insert(0,path)

    def OnChoiceRefDir(self):
        path = filedialog.askdirectory()
        if(path!=None):
            self.refInput.select_clear()
            self.refInput.insert(0,path)

    def OnUpdate(self):
        tarPath = self.tarInput.get()
        refPath = self.refInput.get()
        if(os.path.isdir(tarPath) and os.path.isdir(refPath)):
            self.UpdateFile(tarPath,refPath)
        else:
            messagebox.showinfo("错误", "【待更新目录】或【参考目录】不存在")

    def FindFile(self,dir,name):
        fn = os.path.basename(name)
        for folderName,subfolders,fileNames in os.walk(dir):
            for fileName in fileNames:
                if(fileName==fn):
                    return os.path.join(folderName,fileName)
        return None

    def UpdateFile(self,tarDir,refDir):
        li=[]
        for folderName,subfolders,fileNames in os.walk(tarDir):
            for fileName in fileNames:
                fp = os.path.join(folderName,fileName)
                reffp = self.FindFile(refDir,fp)
                if(reffp!=None):
                    li.append("【有】{}<->{}，更新它！".format(fp,reffp))
                    shutil.copy(reffp,fp)
                else:
                    li.append("【无】"+fp)
        self.ShowUpdateInfo(li)

    def ShowUpdateInfo(self,msgList):
        top_level = tk.Toplevel()
        top_level.title("更新信息")
        w = 480
        h=lh*min(20,len(msgList))
        top_level.geometry("{}x{}".format(w,h))
        #top_level.resizable(width=False, height=False)

        xscroll = tk.Scrollbar(top_level,orient="horizontal")
        xscroll.pack(side="bottom",fill="x")
        yscroll = tk.Scrollbar(top_level)
        yscroll.pack(side="right",fill="y")
        li = tk.Listbox(top_level,bg="green",xscrollcommand=xscroll.set,yscrollcommand=yscroll.set)
        for msg in msgList:
            print(msg)
            li.insert("end",msg)
        li.pack(side="left",fill="both",expand=True)
        xscroll.config(command=li.xview)
        yscroll.config(command=li.yview)

    def Readme(self):
        messagebox.showinfo("关于", "此工具开发的目地是用来把测试版本的dll替换到开发工程中，用来复现某些bug。\n作者：杨循")
#=========================================

app = MyGUI()
app.Build()