#!/bin/bash
if [ -z "$LAMBDASHARP" ]; then
    echo "ERROR: environment variable \$LAMBDASHARP is not set"
    exit 1
fi

find "$LAMBDASHARP" -name 'bin' -or -name 'obj' | xargs rm -rf
find "$LAMBDASHARP" -name '*.csproj' | xargs -l dotnet build
