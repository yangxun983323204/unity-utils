#!/usr/bin/python

import os
from tkinter import *
from tkinter import ttk
import json
import ctypes

kernel32 = ctypes.windll.kernel32
kernel32.SetConsoleMode(kernel32.GetStdHandle(-10), 128)# 关闭控制台的“快速编辑模式”，否则鼠标点击控制台窗体后会阻塞

class Pair:
    Key = None
    Value = None

class MyJsonEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o,Pair):
            kv = {}
            kv['Key'] = o.Key
            kv['Value'] = o.Value
            return kv
        else:
            return super().default(o)

class AppData:
    def __init__(self) -> None:
        self.__saveFile = 'adb_tool_config.json'
        self.Devices = []
        self.Apks = []
        self.QuickDirs = []
        self.Tags = []
        self.onDevicesChange = None
        self.onApksChange = None
        self.onQuickDirChange = None
        self.onTagsChange = None
        self.Load()
    
    def AddDev(self,val:str):
        self.Devices.append(val)
        self.Save()
        if self.onDevicesChange !=None:
            self.onDevicesChange(1,val, len(self.Devices)-1)

    def RemoveDev(self,idx:int):
        if idx < len(self.Devices) and idx>=0:
            val = self.Devices[idx]
            self.Devices.remove(val)
            self.Save()
            if self.onDevicesChange !=None:
                self.onDevicesChange(-1,val,idx)

    def AddApk(self,val:str):
        self.Apks.append(val)
        self.Save()
        if self.onApksChange !=None:
            self.onApksChange(1,val, len(self.Devices)-1)

    def RemoveApk(self,idx:int):
        if idx < len(self.Apks) and idx>=0:
            val = self.Apks[idx]
            self.Apks.remove(val)
            self.Save()
            if self.onApksChange !=None:
                self.onApksChange(-1,val,idx)

    def AddQuickDir(self,val:str):
        self.QuickDirs.append(val)
        self.Save()
        if self.onQuickDirChange !=None:
            self.onQuickDirChange(1,val, len(self.QuickDirs)-1)

    def RemoveQuickDir(self,idx:int):
        if idx < len(self.QuickDirs) and idx>=0:
            val = self.QuickDirs[idx]
            self.QuickDirs.remove(val)
            self.Save()
            if self.onQuickDirChange !=None:
                self.onQuickDirChange(-1,val,idx)

    def __FindTag(self,val:str)->Pair:
        for p in self.Tags:
            if p.Key == val:
                return p
        
        return None

    def AddTag(self,val:str,tag:str):
        kv = self.__FindTag(val)
        if kv == None:
            kv = Pair()
            kv.Key = val
            kv.Value = tag
            self.Tags.append(kv)
        else:
            print('{0}已经存在，请先移除再添加'.format(val))
            return

        self.Save()
        if self.onTagsChange!=None:
            self.onTagsChange(1,kv, len(self.Tags)-1)

    def RemoveTag(self,idx:int):
        if idx < len(self.Tags) and idx>=0:
            kv = self.Tags[idx]
            self.Tags.remove(kv)
            self.Save()
            if self.onTagsChange !=None:
                self.onTagsChange(-1,kv,idx)

    def GetTag(self,val:str):
        kv = self.__FindTag(val)
        if kv!=None:
            return kv.Value
        else:
            return None

    def Load(self):
        file = None
        try:
            file = open(self.__saveFile,mode='tr')
            s = file.read()
            o = json.loads(s)
            self.Devices = o['Devices']
            self.Apks = o['Apks']
            self.QuickDirs = o['QuickDirs']
            for d in o['Tags']:
                n = Pair()
                n.Key = d['Key']
                n.Value = d['Value']
                self.Tags.append(n)

        except IOError as e:
            pass
        except BaseException as e:
            print(e)
        finally:
            if file:
                file.close()

    def Save(self):
        file = None
        try:
            file = open(self.__saveFile,mode='tw')
            o = {}
            o['Devices'] = self.Devices
            o['Apks'] = self.Apks
            o['QuickDirs'] = self.QuickDirs
            o['Tags'] = self.Tags
            s = json.dumps(o, cls=MyJsonEncoder)
            file.write(s)
        except BaseException as e:
            print(e)
        finally:
            if file:
                file.close()

class MainWnd:
    def __init__(self,data:AppData) -> None:
        self._data = data
        wnd = Tk()
        self.Wnd = wnd
        wnd.geometry('400x900')
        wnd.title = "adb工具"
        # 设备列表
        devLabel = Label(wnd,text='设备列表',fg='#0000ff')
        devLabel.pack(side=TOP,expand=0, fill=X)
        fmDevList = Frame(wnd,name='fmDevList',width=100,height=100)
        fmDevList.pack(side=TOP,expand=0, fill=X)
        selectDev = ttk.Combobox(
            master=fmDevList, # 父容器
            name='selectDev',
            height=10, # 高度,下拉显示的条目数量
            width=20, # 宽度
            state='readonly', # 设置状态 normal(可选可输入)、readonly(只可选)、 disabled
            cursor='arrow', # 鼠标移动时样式 arrow, circle, cross, plus...
            )
        selectDev.pack(side=TOP,fill=BOTH)
        devListYScroll = Scrollbar(fmDevList)
        devListYScroll.pack(side=RIGHT,fill= Y)
        devList = Listbox(fmDevList,name='devList',selectmode=SINGLE,activestyle='dotbox',height=5,yscrollcommand=devListYScroll.set)
        devList.pack(side=TOP,fill=BOTH)
        devListYScroll.config(command=devList.yview)
        
        fmDevBtn = Frame(wnd,name='fmDevBtn')
        fmDevBtn.pack(side=TOP,expand=0, fill=X)
        inputDevLbl = Label(fmDevBtn,text='输入框')
        inputDevLbl.pack(side=TOP,expand=0,fill='none')
        inputDev = Entry(fmDevBtn,name='inputDev')
        inputDev.pack(side=TOP,expand=1,fill=X)
        btnAddDev = Button(fmDevBtn,name='btnAddDev',text='添加',command=self.__OnBtnAddDevClick)
        btnAddDev.pack(side=LEFT,expand=1,fill=X)
        btnRemoveDev = Button(fmDevBtn,name='btnRemoveDev',text='删除',command=self.__OnBtnRemoveDevClick)
        btnRemoveDev.pack(side=LEFT,expand=1,fill=X)
        # 安装包列表
        apkLabel = Label(wnd,text='安装包列表',fg='#0000ff')
        apkLabel.pack(side=TOP,expand=0, fill=X)
        fmApkList = Frame(wnd,name='fmApkList',width=100,height=100)
        fmApkList.pack(side=TOP,expand=0, fill=X)
        selectApk = ttk.Combobox(
            master=fmApkList, # 父容器
            name='selectApk',
            height=10, # 高度,下拉显示的条目数量
            width=20, # 宽度
            state='readonly', # 设置状态 normal(可选可输入)、readonly(只可选)、 disabled
            cursor='arrow', # 鼠标移动时样式 arrow, circle, cross, plus...
            )
        selectApk.pack(side=TOP,fill=BOTH)
        apkListYScroll = Scrollbar(fmApkList)
        apkListYScroll.pack(side=RIGHT,fill= Y)
        apkList = Listbox(fmApkList,name='apkList',selectmode=SINGLE,activestyle='dotbox',height=5,yscrollcommand=apkListYScroll.set)
        apkList.pack(side=TOP,fill=BOTH)
        apkListYScroll.config(command=devList.yview)
        
        fmApkBtn = Frame(wnd,name='fmApkBtn')
        fmApkBtn.pack(side=TOP,expand=0, fill=X)
        inputApkLbl = Label(fmApkBtn,text='输入框')
        inputApkLbl.pack(side=TOP,expand=0,fill='none')
        inputApk = Entry(fmApkBtn,name='inputApk')
        inputApk.pack(side=TOP,expand=1,fill=X)
        btnAddApk = Button(fmApkBtn,name='btnAddApk',text='添加',command=self.__OnBtnAddApkClick)
        btnAddApk.pack(side=LEFT,expand=1,fill=X)
        btnRemoveApk = Button(fmApkBtn,name='btnRemoveApk',text='删除',command=self.__OnBtnRemoveApkClick)
        btnRemoveApk.pack(side=LEFT,expand=1,fill=X)
        # cmd列表
        cmdLabel = Label(wnd,text='操作列表',fg='#0000ff')
        cmdLabel.pack(side=TOP,expand=0, fill=X)

        fmCmd0 = Frame(wnd,name='fmCmd',width=100,height=100)
        fmCmd0.pack(side=TOP,expand=0, fill=X)
        btnAdbStartSvr = Button(fmCmd0,name='btnAdbStartSvr',text='启动adb服务',command=self.__CmdAdbStartSvr)
        btnAdbStartSvr.pack(side=LEFT,expand=1,fill=X)
        btnAdbKillSvr = Button(fmCmd0,name='btnAdbKillSvr',text='中止adb服务',command=self.__CmdAdbKillSvr)
        btnAdbKillSvr.pack(side=LEFT,expand=1,fill=X)
        btnAdbDevList = Button(fmCmd0,name='btnAdbDevList',text='adb设备列表',command=self.__CmdAdbDevList)
        btnAdbDevList.pack(side=LEFT,expand=1,fill=X)

        fmCmd1 = Frame(wnd,name='fmCmd1',width=100,height=100)
        fmCmd1.pack(side=TOP,expand=0, fill=X)
        btnAdbConnect = Button(fmCmd1,name='btnAdbConnect',text='adb连接设备',command=self.__CmdAdbConnect)
        btnAdbConnect.pack(side=LEFT,expand=1,fill=X)
        btnAdbInstall = Button(fmCmd1,name='btnAdbInstall',text='adb安装apk',command=self.__CmdAdbInstall)
        btnAdbInstall.pack(side=LEFT,expand=1,fill=X)

        fmCmd2 = Frame(wnd,name='fmCmd2',width=100,height=100)
        fmCmd2.pack(side=TOP,expand=0, fill=X)
        inputCmdLbl = Label(fmCmd2,text='输入框')
        inputCmdLbl.pack(side=TOP,expand=0,fill='none')
        inputCmd = Entry(fmCmd2,name='inputCmd')
        inputCmd.pack(side=TOP,expand=1,fill=X)
        btnCmd = Button(fmCmd2,name='btnCmd',text='执行输入命令',command=lambda : self.__Cmd(inputCmd.get()))
        btnCmd.pack(side=LEFT,expand=1,fill=X)
        # 快速打开目录列表
        quickDirLabel = Label(wnd,text='快速打开目录列表',fg='#0000ff')
        quickDirLabel.pack(side=TOP,expand=0, fill=X)
        fmQuickDirList = Frame(wnd,name='fmQuickDirList',width=100,height=100)
        fmQuickDirList.pack(side=TOP,expand=0, fill=X)
        selectQuickDir = ttk.Combobox(
            master=fmQuickDirList, # 父容器
            name='selectQuickDir',
            height=10, # 高度,下拉显示的条目数量
            width=20, # 宽度
            state='readonly', # 设置状态 normal(可选可输入)、readonly(只可选)、 disabled
            cursor='arrow', # 鼠标移动时样式 arrow, circle, cross, plus...
            )
        selectQuickDir.pack(side=TOP,fill=BOTH)
        quickDirListYScroll = Scrollbar(fmQuickDirList)
        quickDirListYScroll.pack(side=RIGHT,fill= Y)
        quickDirList = Listbox(fmQuickDirList,name='quickDirList',selectmode=SINGLE,activestyle='dotbox',height=5,yscrollcommand=quickDirListYScroll.set)
        quickDirList.pack(side=TOP,fill=BOTH)
        quickDirListYScroll.config(command=quickDirList.yview)
        
        fmQuickDirBtn = Frame(wnd,name='fmQuickDirBtn')
        fmQuickDirBtn.pack(side=TOP,expand=0, fill=X)
        inputQuickDirLbl = Label(fmQuickDirBtn,text='输入框')
        inputQuickDirLbl.pack(side=TOP,expand=0,fill='none')
        inputQuickDir = Entry(fmQuickDirBtn,name='inputQuickDir')
        inputQuickDir.pack(side=TOP,expand=1,fill=X)
        btnAddQuickDir = Button(fmQuickDirBtn,name='btnAddQuickDir',text='添加',command=self.__OnBtnAddQuickDirClick)
        btnAddQuickDir.pack(side=LEFT,expand=1,fill=X)
        btnRemoveQuickDir = Button(fmQuickDirBtn,name='btnRemoveQuickDir',text='删除',command=self.__OnBtnRemoveQuickDirClick)
        btnRemoveQuickDir.pack(side=LEFT,expand=1,fill=X)
        btnOpenQuickDir = Button(fmQuickDirBtn,name='btnOpenQuickDir',text='打开',command=self.__OnBtnQuickOpenClick)
        btnOpenQuickDir.pack(side=LEFT,expand=1,fill=X)
        #
        btnTagMgr = Button(wnd,name='btnTagMgr',text='标签管理',command=self.__OnBtnTagMgr)
        btnTagMgr.pack(side=BOTTOM,expand=0,fill=NONE)
        #
        self._data.onDevicesChange = self.__OnDevicesChange
        self._data.onApksChange = self.__OnApksChange
        self._data.onQuickDirChange = self.__OnQuickDirsChange

        self.__InitData()

    def CurrentDev(self)->str:
        '''当前选中的设备'''
        select = self.Wnd.children['fmDevList'].children['selectDev']
        i = select.current()
        if i>=0 :
            return self._data.Devices[i]
        else:
            return None

    def CurrentApk(self)->str:
        '''当前选中的设备'''
        select = self.Wnd.children['fmApkList'].children['selectApk']
        i = select.current()
        if i >= 0 :
            return self._data.Apks[i]
        else:
            return None

    def CurrentQuickPath(self)->str:
        '''当前选中的快速路径'''
        select = self.Wnd.children['fmQuickDirList'].children['selectQuickDir']
        i = select.current()
        if i >= 0 :
            return self._data.QuickDirs[i]
        else:
            return None

    def __InitData(self):
        i = 0
        for v in self._data.Devices:
            self.__OnDevicesChange(1,v,i)
            i+=1

        i = 0
        for v in self._data.Apks:
            self.__OnApksChange(1,v,i)
            i+=1

        i = 0
        for v in self._data.QuickDirs:
            self.__OnQuickDirsChange(1,v,i)
            i+=1


    def __OnBtnAddDevClick(self):
        val = self.Wnd.children['fmDevBtn'].children['inputDev'].get()
        if val!=None and val!='':
            self._data.AddDev(val)

    def __OnBtnRemoveDevClick(self):
        sels = self.Wnd.children['fmDevList'].children['devList'].curselection()
        if len(sels) > 0 :
            self._data.RemoveDev(sels[0])
        else:
            print('请先选中列表中的项再点击移除')

    def __OnBtnAddApkClick(self):
        val = self.Wnd.children['fmApkBtn'].children['inputApk'].get()
        if val!=None and val!='':
            self._data.AddApk(val)

    def __OnBtnRemoveApkClick(self):
        sels = self.Wnd.children['fmApkList'].children['apkList'].curselection()
        if len(sels) > 0 :
            self._data.RemoveApk(sels[0])
        else:
            print('请先选中列表中的项再点击移除')

    def __OnBtnAddQuickDirClick(self):
        val = self.Wnd.children['fmQuickDirBtn'].children['inputQuickDir'].get()
        if val!=None and val!='':
            self._data.AddQuickDir(val)

    def __OnBtnRemoveQuickDirClick(self):
        sels = self.Wnd.children['fmQuickDirList'].children['quickDirList'].curselection()
        if len(sels) > 0 :
            self._data.RemoveQuickDir(sels[0])
        else:
            print('请先选中列表中的项再点击移除')

    def __OnBtnQuickOpenClick(self):
        path = self.CurrentQuickPath()
        self.__Cmd('start explorer "{0}"'.format(path))
        
    def __OnDevicesChange(self,type:int,val:str,idx:int):
        tag = self._data.GetTag(val)
        if tag != None:
            val = '【{0}】'.format(tag) + val

        if type == -1:
            self.Wnd.children['fmDevList'].children['devList'].delete(idx)
        elif type == 1:
            self.Wnd.children['fmDevList'].children['devList'].insert(END,val)

        select = self.Wnd.children['fmDevList'].children['selectDev']
        # 构造带标签的列表
        l = list()
        for dev in self._data.Devices:
            tag = self._data.GetTag(dev)
            if tag != None:
                dev = '【{0}】'.format(tag) + dev
            l.append(dev)

        select['values'] = l
        if select.current() < 0 and len(self._data.Devices)>0:
            select.current(len(self._data.Devices)-1)
        
    
    def __OnApksChange(self,type:int,val:str,idx:int):
        tag = self._data.GetTag(val)
        if tag != None:
            val = '【{0}】'.format(tag) + val

        if type == -1:
            self.Wnd.children['fmApkList'].children['apkList'].delete(idx)
        elif type == 1:
            self.Wnd.children['fmApkList'].children['apkList'].insert(END,val)

        select = self.Wnd.children['fmApkList'].children['selectApk']
        # 构造带标签的列表
        l = list()
        for apk in self._data.Apks:
            tag = self._data.GetTag(apk)
            if tag != None:
                apk = '【{0}】'.format(tag) + apk
            l.append(apk)

        select['values'] = l
        if select.current() < 0 and len(self._data.Apks)>0:
            select.current(len(self._data.Apks)-1)

    def __OnQuickDirsChange(self,type:int,val:str,idx:int):
        tag = self._data.GetTag(val)
        if tag != None:
            val = '【{0}】'.format(tag) + val

        if type == -1:
            self.Wnd.children['fmQuickDirList'].children['quickDirList'].delete(idx)
        elif type == 1:
            self.Wnd.children['fmQuickDirList'].children['quickDirList'].insert(END,val)

        select = self.Wnd.children['fmQuickDirList'].children['selectQuickDir']
        # 构造带标签的列表
        l = list()
        for dir in self._data.QuickDirs:
            tag = self._data.GetTag(dir)
            if tag != None:
                dir = '【{0}】'.format(tag) + dir
            l.append(dir)

        select['values'] = l
        if select.current() < 0 and len(self._data.QuickDirs)>0:
            select.current(len(self._data.QuickDirs)-1)

    def __CmdAdbStartSvr(self):
        self.__Cmd('adb start-server')

    def __CmdAdbKillSvr(self):
        self.__Cmd('adb kill-server')

    def __CmdAdbConnect(self):
        dev = self.CurrentDev()
        if dev!=None:
            cmd = 'adb connect {0}'.format(dev)
            self.__Cmd(cmd)
        else:
            print('未选择设备')

    def __CmdAdbInstall(self):
        dev = self.CurrentDev()
        apk = self.CurrentApk()
        if dev!=None and apk != None:
            cmd = 'adb -s {0} install -r -t "{1}"'.format(dev,apk)
            self.__Cmd(cmd)
        else:
            print('未选择设备或安装包')

    def __CmdAdbDevList(self):
        self.__Cmd('adb devices')

    def __Cmd(self,s:str):
        if s!=None and s!='':
            print(s)
            os.system(s)
        else:
            print('要执行的命令为空')

    def __OnBtnTagMgr(self):
        TagMgrWnd(data).Wnd.mainloop()


class TagMgrWnd:
    def __init__(self,data:AppData) -> None:
        self._data = data
        wnd = Tk()
        self.Wnd = wnd
        wnd.geometry('400x300')
        wnd.title = "标签管理"
        # value <---> tag 列表
        mgrLabel = Label(wnd,text='值-标签 列表',fg='#87CEEB')
        mgrLabel.pack(side=TOP,expand=0, fill=X)
        fmMgrList = Frame(wnd,name='fmMgrList',width=100,height=100)
        fmMgrList.pack(side=TOP,expand=0, fill=X)
        
        mgrListYScroll = Scrollbar(fmMgrList)
        mgrListYScroll.pack(side=RIGHT,fill= Y)
        mgrList = Listbox(fmMgrList,name='mgrList',selectmode=SINGLE,activestyle='dotbox',yscrollcommand=mgrListYScroll.set)
        mgrList.pack(side=TOP,fill=BOTH)
        mgrListYScroll.config(command=mgrList.yview)
        
        fmDevBtn = Frame(wnd,name='fmMgrBtn')
        fmDevBtn.pack(side=TOP,expand=0, fill=X)
        inputLbl = Label(fmDevBtn,text='分别输入值和标签')
        inputLbl.pack(side=TOP,expand=0,fill='none')
        
        inputVal = Entry(fmDevBtn,name='inputVal')
        inputVal.pack(side=TOP,expand=1,fill=X)
        inputTag = Entry(fmDevBtn,name='inputTag')
        inputTag.pack(side=TOP,expand=1,fill=X)

        btnAddDev = Button(fmDevBtn,name='btnAddKV',text='添加',command=self.__OnBtnAddKVClick)
        btnAddDev.pack(side=LEFT,expand=1,fill=X)
        btnRemoveDev = Button(fmDevBtn,name='btnRemoveKV',text='删除',command=self.__OnBtnRemoveKVClick)
        btnRemoveDev.pack(side=LEFT,expand=1,fill=X)

        self._data.onTagsChange = self.__OnTagsChange

        self.__InitData()

    def __InitData(self):
        i = 0
        for v in self._data.Tags:
            self.__OnTagsChange(1,v,i)
            i+=1

    def __OnBtnAddKVClick(self):
        val = self.Wnd.children['fmMgrBtn'].children['inputVal'].get()
        tag = self.Wnd.children['fmMgrBtn'].children['inputTag'].get()
        if val!=None and val!='':
            self._data.AddTag(val,tag)

    def __OnBtnRemoveKVClick(self):
        sels = self.Wnd.children['fmMgrList'].children['mgrList'].curselection()
        if len(sels) > 0 :
            self._data.RemoveTag(sels[0])
        else:
            print('请先选中列表中的项再点击移除')
        
    def __OnTagsChange(self,type:int,kv:Pair,idx:int):
        if type == -1:
            self.Wnd.children['fmMgrList'].children['mgrList'].delete(idx)
        elif type == 1:
            self.Wnd.children['fmMgrList'].children['mgrList'].insert(END,'{0}<---------->{1}'.format(kv.Key,kv.Value))

data = AppData()
MainWnd(data).Wnd.mainloop()