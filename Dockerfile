FROM microsoft/dotnet:latest
WORKDIR /srv
COPY bin/Debug/netcoreapp1.0/publish/ /srv/
COPY keys/ /srv/keys/
ENTRYPOINT sh -c "dotnet ef database update && dotnet /srv/firefly.dll"