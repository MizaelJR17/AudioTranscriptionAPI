# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY AudioTranscriptionAPI.csproj ./
RUN dotnet restore AudioTranscriptionAPI.csproj

# Copiar código fonte e compilar aplicação
COPY . .
RUN dotnet publish -c Release -o out

# Estágio de produção
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Instalar ffmpeg
RUN apt-get update && apt-get install -y ffmpeg

# Copiar arquivos da build
COPY --from=build /app/out ./

# Copiar o modelo Vosk
COPY Models/vosk-model-small-pt-0.3 /app/model

# Configurar variável de ambiente para o caminho do modelo Vosk
ENV VOSK_MODEL_PATH=/app/model

# Expor porta
EXPOSE 5102

# Iniciar aplicação
ENTRYPOINT ["dotnet", "AudioTranscriptionAPI.dll"]
