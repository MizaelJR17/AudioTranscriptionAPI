# AudioTranscriptionAPI




---

# AudioTranscriptionAPI

AudioTranscriptionAPI é uma API para transcrição de áudio utilizando a biblioteca Vosk e o FFmpeg para processar e transcrever arquivos de áudio em tempo real ou por upload.

## Sumário

- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso](#uso)
- [Testando a API](#testando-a-api)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)

## Pré-requisitos

Antes de começar, certifique-se de ter os seguintes requisitos instalados em sua máquina:

- Docker
- .NET SDK 7.0
- FFmpeg
- [Modelo Vosk](https://alphacephei.com/vosk/models) (para transcrição de áudio)

## Instalação

Siga os passos abaixo para configurar o projeto localmente:

1. **Clone o repositório**

   ```bash
   git clone https://github.com/MizaelJR17AudioTranscriptionAPI.git
   cd AudioTranscriptionAPI
   ```

2. **Adicionar o Modelo Vosk**

   Baixe o modelo de Vosk desejado [aqui](https://alphacephei.com/vosk/models) e coloque-o na pasta `Models/` do projeto. Renomeie para `vosk-model-small-pt-0.3` ou altere conforme sua configuração.

3. **Configurar o Docker**

   O projeto utiliza um `Dockerfile` para construir a imagem da API com .NET 7, FFmpeg e o modelo de Vosk. Para construir e rodar o projeto:

   ```bash
   docker build -t audio-transcription-api .
   docker run -p 5102:5102 audio-transcription-api
   ```

4. **Verificar o funcionamento**

   A API deve estar disponível em: `http://localhost:5102`

## Configuração

### appsettings.json

O arquivo `appsettings.json` contém as configurações de log e as portas para o Kestrel. O conteúdo principal do arquivo é:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5102"
      }
    }
  }
}
```

Certifique-se de que a porta `5102` está sendo utilizada corretamente tanto no `appsettings.json` quanto no Docker.

## Uso

A API aceita uploads de arquivos de áudio ou streams e retorna a transcrição do conteúdo. 

### Endpoints disponíveis:

1. **/api/transcribe**
   - Método: `POST`
   - Descrição: Transcreve um arquivo de áudio enviado.
   - Parâmetros: Um arquivo de áudio (ex.: `.wav`, `.mp3`, `.opus`)

Exemplo de uso via `curl`:

```bash
curl -X POST http://localhost:5102/api/transcribe \
  -F "file=@/caminho/do/audio.wav" \
  -H "Content-Type: multipart/form-data"
```

## Testando a API

Para testar a API localmente:

1. Use ferramentas como **Postman** ou **Insomnia** para enviar requisições `POST` para o endpoint `/api/transcribe`.
2. Verifique se o áudio é corretamente transcrito.

## Tecnologias Utilizadas

- **.NET 7**: Framework para a construção da API.
- **Docker**: Para containerizar a aplicação.
- **Vosk**: Biblioteca de reconhecimento de fala.
- **FFmpeg**: Ferramenta para manipulação e conversão de arquivos de áudio.
- **Kestrel**: Servidor web embutido no .NET.

## Contribuindo

1. Faça o fork deste repositório.
2. Crie uma branch para sua feature (`git checkout -b minha-feature`).
3. Faça commit das suas alterações (`git commit -m 'Minha nova feature'`).
4. Faça o push para a branch (`git push origin minha-feature`).
5. Abra um Pull Request.

## Licença

Este projeto está sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

Este `README` contém todas as informações essenciais para rodar o projeto localmente, configurar o Docker, e contribuir com o desenvolvimento.
