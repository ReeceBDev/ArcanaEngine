FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Thoth.slnx ./
COPY Thoth/ Thoth/
COPY Thoth.WebAPI/ Thoth.WebAPI/
RUN dotnet publish Thoth.WebAPI -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ARG APP_VERSION=local
ENV APP_VERSION=$APP_VERSION
USER app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Thoth.WebAPI.dll"]
