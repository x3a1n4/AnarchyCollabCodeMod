#!/bin/bash

rm AnarchyCollab.zip AnarchyCollab.dll
CELESTEGAMEPATH=$PWD/../.. dotnet build -c Release
zip AnarchyCollab.zip -r everest.yaml AnarchyCollab.dll Ahorn
