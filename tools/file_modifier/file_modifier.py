#!/usr/bin/python

import os
from typing import Union

class FileModifier:
	'''文件修改器'''
	class Line:
		def __init__(self) -> None:
			self.Num:int = -1
			self.Text:str = ""
			self.Add:list[str] = []
			
	def __init__(self) -> None:
		self._isOpen = False
		self._lines:list[str] = []
		self._p = FileModifier.Line()

	def Open(self, path:str) -> bool:
		if self.IsOpen():
			print('[打开文件] {0}，{1}，原因：{2}'.format(path, False, "当前未关闭"))
			return False
		
		try:
			self._file = open(path,mode='r+', encoding='utf8')
			self._lines = self._file.readlines()
			self._isOpen = True
			self._path = path
			print('[打开文件] {0}，{1}'.format(path, True))
		except BaseException as e:
			self._isOpen = False
			print('[打开文件] {0}，{1}，原因：{2}'.format(path, False, e))

		return self.IsOpen()


	def IsOpen(self)->bool:
		return self._isOpen
	
	def Save(self)->None:
		if self.IsOpen():
			self._file.seek(0, 0)
			self._file.writelines(self._lines)
			print('[保存文件] {0}'.format(self._path))

	def Close(self)->None:
		if	self.IsOpen():
			self._file.flush()
			self._file.close()
			self._isOpen = False
			print('[关闭文件] {0}'.format(self._path))

	def Append(self, lines:list[str])->None:
		for i in range(len(lines)):
			l = lines[i]
			if not l.endswith('\n'):
				lines[i] = l+'\n'

		self._lines.extend(lines)
		print('[文件末尾附加] 附加：{0}'.format(lines))
		return len(lines)

	def LocateByPattern(self, patter, callback)->None:
		'''
		patter声明为：void (*func)(FileModifier.Line& line)\n
		callback声明为：int (*func)(FileModifier.Line& line) 返回值表明对行数的影响,可取范围为大于等于-1的整数
		'''
		i = 0
		for rawLine in self._lines:
			self._p.Num = i
			self._p.Text = rawLine
			self._p.Add.clear()

			if patter(self._p):
				n = callback(self._p)
				if n < 0 :
					self._lines.pop(i)
					i=i-1
				elif n == 0:
					self._lines[i] = self._p.Text
				else:
					self._lines[i] = self._p.Text
					for j in range(n):
						self._lines.insert(i, self._p.Add[j])
						i=i+1

			i=i+1

	def Mod_Del(line:Line)->int:
		print('[删除] 删除第{0}行，原文为：{1}'.format(line.Num, line.Text))
		return -1
	
	def Mod_AddBeginIf(line:Line, chrs:str)->int:
		if not line.Text.startswith(chrs):
			old = line.Text
			line.Text = chrs + line.Text
			print('[行首添加] 为第{0}行首添加：{1}，原文为:{2}'.format(line.Num, chrs, old))

		return 0

	def Mod_Replace(line:Line, src:str, dst:str)->int:
		old = line.Text
		line.Text = line.Text.replace(src, dst)
		print('[文本替换] 对第{0}行进行文本替换：{1}->{2},原文为:{3}'.format(line.Num, src, dst, old))
		return 0

	def Mod_InsertAt(line:Line, lines:list[str])->int:
		line.Add = lines
		for i in range(len(line.Add)):
			l = line.Add[i]
			if not l.endswith('\n'):
				line.Add[i] = l+'\n'

		print('[行前插入] 在第{0}行前插入,原文为:{1}，插入为：{2}'.format(line.Num, line.Text, lines))
		return len(lines)

	def QuickReplace(self, condition:Union[str,int], src:str, dst:str)->None:
		if isinstance(condition, str):
			self.LocateByPattern(lambda line:condition in line.Text, lambda line:FileModifier.Mod_Replace(line, src, dst))
		elif isinstance(condition, int):
			self.LocateByPattern(lambda line:line.Num==condition, lambda line:FileModifier.Mod_Replace(line, src, dst))
		else:
			print('QuickReplace不支持的条件:'+str(condition))

	def QuickComment(self, condition:Union[str,int], chrs:str)->None:
		if isinstance(condition, str):
			self.LocateByPattern(lambda line:condition in line.Text, lambda line:FileModifier.Mod_AddBeginIf(line, chrs))
		elif isinstance(condition, int):
			self.LocateByPattern(lambda line:line.Num==condition, lambda line:FileModifier.Mod_AddBeginIf(line, chrs))
		else:
			print('QuickComment不支持的条件:'+str(condition))

	def QuickInsert(self, condition:Union[str,int], lines:list[str])->None:
		if isinstance(condition, str):
			self.LocateByPattern(lambda line:condition in line.Text, lambda line:FileModifier.Mod_InsertAt(line,lines))
		elif isinstance(condition, int):
			self.LocateByPattern(lambda line:line.Num==condition, lambda line:FileModifier.Mod_InsertAt(line,lines))
		else:
			print('QuickInsert不支持的条件:'+str(condition))

	def QuickDel(self, condition:Union[str,int])->None:
		if isinstance(condition, str):
			self.LocateByPattern(lambda line:condition in line.Text, lambda line:FileModifier.Mod_Del(line))
		elif isinstance(condition, int):
			self.LocateByPattern(lambda line:line.Num==condition, lambda line:FileModifier.Mod_Del(line))
		else:
			print('QuickDel不支持的条件:'+str(condition))

