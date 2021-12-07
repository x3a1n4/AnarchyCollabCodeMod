#!/bin/bash

rm AnarchyCollab.zip AnarchyCollab.dll
CELESTEGAMEPATH=$PWD/../.. dotnet build
zip AnarchyCollab.zip -r everest.yaml AnarchyCollab.dll Ahorn
