#!/bin/bash
rm AnarchyCollab.zip AnarchyCollab.dll
CELESTEGAMEPATH=$PWD/../.. dotnet build Code/AnarchyCollab/AnarchyCollab.csproj
zip AnarchyCollab.zip -r everest.yaml AnarchyCollab.dll Ahorn