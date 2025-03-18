
O projeto utiliza as seguintes tecnologias principais:

    .NET 8: Framework para o desenvolvimento da API.

    JWT (JSON Web Tokens): Para autenticação de usuários e proteção das rotas da API.

    Swagger: Para documentação interativa e testes das rotas da API.

    MySQL: Banco de dados relacional para persistência de dados.

    HttpClient: Para consumo de APIs externas.

    ASP.NET Core: Para configuração de autenticação, autorização e gerenciamento de requisições HTTP.


Preparação para Colocar o Desafio em Funcionamento: Configuração de Banco de Dados, Rotas da API e Execução do Projeto C# .NET 8


1. Clonar o Repositório GitHub
Primeiro, você precisa clonar o repositório do GitHub para sua máquina local.

Instalar o Git: Se você ainda não tem o Git instalado, faça o download do Git.

Depois, você pode verificar a instalação no terminal com:
git --version

Clonar o repositório: No terminal, vá para o diretório onde você deseja clonar o projeto e execute o comando
git clone https://github.com/luishh205/desafio-backend-2025

Acessar o diretório do projeto:
cd seu-repositorio

2. Instalar Dependências
Restaurar as dependências do projeto C#: Uma vez dentro do diretório do projeto, execute o comando para restaurar as dependências do projeto:
dotnet restore
Isso garante que todas as dependências listadas no arquivo .csproj sejam instaladas.

3. Importar o Arquivo do Banco de Dados MySQL
Passos:
Localize o arquivo .sql dentro do projeto.

Importe o banco de dados usando o MySQL:
Abra o terminal ou prompt de comando e navegue até o diretório onde o MySQL está instalado ou onde o binário mysql está disponível.

Execute o comando para importar o banco de dados:
mysql -u root -p nome_do_banco < caminho_para_o_arquivo/banco-desafio-backend-2025.sql

nome_do_banco: O nome do banco de dados que você deseja importar (se não existir, crie primeiro com CREATE DATABASE nome_do_banco;).
caminho_para_o_arquivo/banco_crud.sql: Caminho completo do arquivo .sql que contém os dados do banco.

Verifique se o banco de dados foi importado corretamente: Você pode verificar no MySQL se as tabelas e os dados foram importados com o seguinte comando:
SHOW TABLES;

4. Localize o arquivo do Postman dentro do projeto.

O arquivo de exportação do Postman é geralmente um arquivo .json (por exemplo, postman_collection.json ou similar).
Importe o arquivo no Postman:

Abra o Postman.
Clique em Import no canto superior esquerdo.
Selecione File e procure o arquivo .json do Postman dentro do seu diretório de projeto.
Clique em Open para importar o arquivo.
Verifique as rotas importadas: Após importar, você verá as rotas da API já configuradas no Postman. Você pode testar essas rotas enviando as requisições diretamente.

5.Rodar o Projeto C# e Testar as Rotas no Postman
Agora que você tem o banco de dados e as rotas configuradas no Postman, siga os passos abaixo para rodar o seu projeto C# .NET 8 e testar as rotas da API:

Restaurar as dependências do projeto (se ainda não fez isso):
dotnet restore

Rodar o projeto no terminal:
dotnet run
