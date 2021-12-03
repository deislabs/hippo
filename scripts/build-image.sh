#!/bin/bash

# This script is used to build the Docker Image for hippo for test purposes. 

pushd $PWD
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR
cd ..
ROOT_DIR=$PWD

cd $ROOT_DIR

dotnet publish Hippo/Hippo.csproj -c Release --self-contained -r linux-x64

cd $ROOT_DIR/.github/release-image

mkdir -p $ROOT_DIR/src/Web/bin/Release/net6.0/linux-x64/publish/certs
cp localhost.conf $ROOT_DIR/src/Web/bin/Release/net6.0/linux-x64/publish/certs
docker build -t hippo -f  $ROOT_DIR/.github/release-image/Dockerfile  $ROOT_DIR/src/Web/bin/Release/net6.0/linux-x64/publish
