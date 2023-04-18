import os.path
import tkinter as tk
# 需要先在命令行中执行以下命令安装pywin32模块
# pip install pywin32
import win32com.shell.shell as shell
# 需要先在命令行中执行以下命令安装tkinterdnd2模块
# pip install tkinterdnd2
from tkinterdnd2 import DND_FILES, TkinterDnD

root = TkinterDnD.Tk()  # notice - use this instead of tk.Tk()
root.title('符号链接生成')

tk.Label(root, text='目标父目录').pack(fill=tk.X)
sv_dst = tk.StringVar()
entry_dst = tk.Entry(root, textvariable=sv_dst, width=80)
entry_dst.pack(fill=tk.X)
tk.Label(root, text='源列表').pack(fill=tk.X)
lb_src = tk.Listbox(root)
lb_src.pack(fill=tk.X)

entry_dst.drop_target_register(DND_FILES)
lb_src.drop_target_register(DND_FILES)
entry_dst.dnd_bind('<<Drop>>', lambda e: sv_dst.set(e.data))
lb_src.dnd_bind('<<Drop>>', lambda e: lb_src.insert(tk.END, e.data))

tk.Label(root, text='后缀').pack(fill=tk.X)
sv_ex = tk.StringVar()
sv_ex.set('.link')
tk.Entry(root, textvariable=sv_ex).pack(fill=tk.X)

def btn_click():
    dst_root = sv_dst.get()
    dst_root.replace('{','')
    dst_root.replace('}','')
    ex = sv_ex.get()
    commands = 'echo begin'
    for i in enumerate(lb_src.get(0, tk.END)):
        n = i[1]
        n.replace('{','')
        n.replace('}','')
        name = os.path.basename(n)
        str_from = "\"{0}\"".format(os.path.normpath(n))
        str_to = "\"{0}\"".format(os.path.normpath(os.path.join(dst_root, name+ex)))
        print("{0}->{1}".format(str_from, str_to))
        if os.path.isdir(n):
            commands += ' & mklink /D {0} {1}'.format(str_to, str_from)
        else:
            commands += ' & mklink {0} {1}'.format(str_to, str_from)
    
    commands += ' & echo end & pause'
    print(commands)
    shell.ShellExecuteEx(lpVerb='runas', lpFile='cmd.exe', lpParameters='/c '+commands)

btn = tk.Button(root, text='生成符号链接', command=btn_click)
btn.pack(fill=tk.X, side=tk.BOTTOM)

root.mainloop()