#!/usr/bin/python

import os
from file_modifier import *


modifier = FileModifier()
dir = "../../vrtv/"

p = dir + "local.properties"
modifier.Open(p)
if modifier.IsOpen():
    modifier.QuickReplace("localUnity", "false", "true")
    modifier.Save()
    modifier.Close()

p = dir + "build.gradle"
modifier.Open(p)
if modifier.IsOpen():
    modifier.QuickComment("apply plugin: 'androidx-adapt'", "//")
    modifier.Save()
    modifier.Close()

p = dir + "app/build.gradle.kts"
modifier.Open(p)
if modifier.IsOpen():
    modifier.QuickReplace("ndk.abiFilters", "armeabi-v7a", "arm64-v8a")
    modifier.Save()
    modifier.Close()

p = dir + "unity/unityLibrary/build.gradle"
modifier.Open(p)
if modifier.IsOpen():
    modifier.QuickComment("apply from: 'http://", "//")
    modifier.QuickReplace("':unityLibrary'", "':unityLibrary'", "':unity:unityLibrary'")
    modifier.Save()
    modifier.Close()

os.system("pause")