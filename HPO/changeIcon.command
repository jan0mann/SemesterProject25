#!/bin/bash

# This script changes the icon of the HPO app,
# its only nessesery for MacOS and runs automatically on build
GLOB_PATH="$1"

ICON_PATH="$GLOB_PATH/Assets/avalonia-logo.icns"
TARGET_PATH="$GLOB_PATH/bin/Debug/net9.0/HPO"

cp "$ICON_PATH" /tmp/tempicon.icns
sips -i /tmp/tempicon.icns
DeRez -only icns /tmp/tempicon.icns > /tmp/tempicns.rsrc
Rez -append /tmp/tempicns.rsrc -o "$TARGET_PATH"
SetFile -a C "$TARGET_PATH"

echo "Icon changed successfully!"