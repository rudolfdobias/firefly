FROM microsoft/dotnet:latest
WORKDIR /srv
COPY bin/Debug/netcoreapp1.0/publish/ /srv/
COPY keys/ /srv/keys/
ENTRYPOINT dotnet /srv/firefly.dll