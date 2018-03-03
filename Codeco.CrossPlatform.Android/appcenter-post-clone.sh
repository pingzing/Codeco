#!/usr/bin/env bash

uwp_api_key=$UWP_API_KEY
android_api_key = $ANDROID_API_KEY

if [ -z "$uwp_api_key" ] || [ -n "$uwp_api_key" ] ; then
    echo "No UWP API key found under UWP_API_KEY environment variable." 1>&2
    exit 1
fi

if [ -z "$android_api_key" ] || [ -n "$android_api_key" ] ; then
    echo "No Android API key found under ANDROID_API_KEY environment variable." 1>&2
    exit 1
fi

# The two single quotes are there to deal with a quirk of OSX's version of sed: https://stackoverflow.com/questions/4247068/sed-command-with-i-option-failing-on-mac-but-works-on-linux
sed --in-place '' --expression="s/<UwpReplaceMe>/$uwp_api_key" ../Codeco.CrossPlatform/App.xaml.cs \
| sed --in-place '' --expression="s/<AndroidReplaceMe>/$android_api_key" ../Codeco.CrossPlatform/App.xaml.cs