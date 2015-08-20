#!/usr/bin/python
import os
import sys
import re
from distutils import dir_util
from HZmod_pbxproj import XcodeProject

def edit_pbxproj_file():
    try:
        unityProjectTopDirectory = sys.argv[1]
        for xcodeproj in os.listdir(unityProjectTopDirectory):
            if not re.search('\.xcodeproj', xcodeproj):
                continue
            xcodeproj = os.path.join(unityProjectTopDirectory, xcodeproj)
            for pbxproj in os.listdir(xcodeproj):
                if not re.search('\.pbxproj', pbxproj):
                    continue
                pbxproj = os.path.join(xcodeproj, pbxproj)
                
                # locate the id of the "Frameworks" group of the pbxproj file so that frameworks will go to that group
                frameworksGroupID = None
                textfile = open(pbxproj, 'r')
                filetext = textfile.read()
                textfile.close()
                matches = re.findall("([0-9A-F]*) /\* Frameworks \*/ = \{\n\s*isa = PBXGroup;", filetext)
                try:
                    frameworksGroupID = matches[0];
                except:
                    pass

                project = XcodeProject.Load(pbxproj)

                # the below paths are relative to the SDKROOT, i.e.: `/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.3.sdk/`
                # Add the Frameworks needed
                project.add_file_if_doesnt_exist('usr/lib/libxml2.dylib',                       parent=frameworksGroupID, tree='SDKROOT')

                # for AdColony
                project.add_file_if_doesnt_exist('System/Library/Frameworks/WebKit.framework',  parent=frameworksGroupID, tree='SDKROOT')
                project.add_file_if_doesnt_exist('usr/lib/libz.dylib',                          parent=frameworksGroupID, tree='SDKROOT')

                # Add -ObjC for the benefit of AppLovin/FAN
                project.add_other_ldflags("-ObjC")

                # Enable modules for the benefit of AdMob.
                # (This allows automatic linking for the frameworks they use)
                project.add_flags({"CLANG_ENABLE_MODULES": "YES"})

                project.save()
                print "Heyzap: successfully modified file: ", pbxproj
                return
        raise FileExistsError("Could not find a .pbxproj file to edit")
    except Exception as e:
      print "Heyzap: ERROR modifying .pbxproj, error: ", e

edit_pbxproj_file()