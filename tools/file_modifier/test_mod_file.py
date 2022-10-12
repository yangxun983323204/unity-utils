#!/usr/bin/python

from file_modifier import *

testPath = './test_file.txt'
modifier = FileModifier()
modifier.Open(testPath)
if modifier.IsOpen():
    modifier.Append(["末尾一\n","末尾二","mowei3"])
    modifier.QuickInsert(0, ["插入一行","插入两行"])
    modifier.QuickDel("1122一")
    modifier.QuickComment("经验嘱咐刘江河", "//")
    modifier.Save()
    modifier.Close()