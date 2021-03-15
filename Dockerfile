FROM mcr.microsoft.com/dotnet/sdk:5.0 AS BUILD
WORKDIR /app
COPY ./src ./
EXPOSE 5000
RUN dotnet restore ./PlayerFeedbackService/PlayerFeedbackService.csproj
RUN dotnet publish ./PlayerFeedbackService/PlayerFeedbackService.csproj -c Release -o bin

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
COPY --from=BUILD /app/bin /app
ENTRYPOINT ["dotnet", "PlayerFeedbackService.dll"]
