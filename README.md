Para iniciar a API:
1. Garanta que net8.0 esteja instalado.
2. No diretório raiz restaure os pacotes: dotnet restore
3. No diretório raiz compile: dotnet build
4. No diretório raiz execute: dotnet run --project Api
5. Uma postman_collection.json (v 2.1) está disponivel para ser importada e fazer chamadas prontas para a API.

Para Teste de Integração no projeto de Testes:
1. Garanta que Docker está instalado.
2. O teste espera uma imagem com nome "api-de-vendas"
  2.2. Crie a imagem com: docker build -t api-de-vendas .

Opcionalmente caso deseje rodar localmente como container (mas o teste já roda seu container):
1. No diretório raiz: docker run -d -p 5063:5063 api-de-vendas