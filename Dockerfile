FROM microsoft/dotnet:latest
WORKDIR /root
COPY bin/Debug/netcoreapp1.0/publish/ /root/
COPY keys/ /root/keys/
ENTRYPOINT dotnet /root/firefly.dll --perform=migrate --continue=true