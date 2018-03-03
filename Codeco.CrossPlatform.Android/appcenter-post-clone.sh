#!/usr/bin/env bash

uwp_api_key=$UWP_API_KEY
android_api_key=$ANDROID_API_KEY

echo "Checking UWP API key."

if [ -z "$uwp_api_key" ] ; then
    echo "No UWP API key found under UWP_API_KEY environment variable." 1>&2    
    exit 1
fi

echo "Checking Android API key."

if [ -z $android_api_key ] ; then
    echo "No Android API key found under ANDROID_API_KEY environment variable." 1>&2
    exit 1
fi

echo "Replacing API keys in App.xaml.cs."

cd ..

echo "Went up a level, running ls:"

ls

sed -i .'' -e "s/<UwpReplaceMe>/$uwp_api_key/" ../Codeco.CrossPlatform/App.xaml.cs | sed -i '' -e "s/<AndroidReplaceMe>/$android_api_key/" ../Codeco.CrossPlatform/App.xaml.cs