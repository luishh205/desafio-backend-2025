Documentação da API - Usuário

Endpoint: GET /api/usuario
Descrição: Retorna uma lista de todos os usuários cadastrados no banco de dados.
Autenticação: Requer token JWT.

Modelo de retorno 
{
  "success": true,
  "data": [
    {
      "id": 1,
      "email": "usuario1@example.com",
      "password": "senha123"
    }
  ],
  "message": null
}

Endpoint: GET /api/usuario/{id}
Descrição: Retorna os detalhes de um usuário com base no ID fornecido.
Autenticação: Requer token JWT.

Modelo de retorno 
{
  "success": true,
  "data": {
    "id": 1,
    "email": "usuario1@example.com",
    "password": "senha123"
  },
  "message": null
}

Endpoint: POST /api/usuario/register
Descrição: Cria um novo usuário com os dados fornecidos.
Autenticação: Não requer autenticação.

Corpo da Requisição:
{
  "email": "novo_usuario@example.com",
  "password": "senha123"
}

Modelo de retorno 
{
  "success": true,
  "data": 1, retorna o id do usuario
  "message": null
}


Endpoint: POST /api/usuario/login
Descrição: Realiza o login do usuário e retorna um token JWT.
Autenticação: Não requer autenticação.

Corpo da Requisição:
{
  "email": "usuario@example.com",
  "password": "senha123"
}

Modelo de retorno
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "message": null
}

Endpoint: PUT /api/usuario
Descrição: Atualiza os dados de um usuário existente.
Autenticação: Requer token JWT.

Corpo da Requisição:
{
  "id": 1,
  "email": "usuario_atualizado@example.com",
  "password": "nova_senha123"
}

Modelo de retorno
{
  "success": false,
  "data": null,
  "message": "Dados inválidos fornecidos."
}

Endpoint: DELETE /api/usuario/{id}
Descrição: Exclui um usuário com base no ID fornecido.
Autenticação: Requer token JWT.

Modelo de retorno 
{
  "success": false,
  "data": null,
  "message": "Usuário com ID 1 não encontrado."
}

Documentação da API - Conta

Endpoint: GET /api/contas
Descrição: Retorna uma lista de todas as contas cadastradas no banco de dados.
Autenticação: Requer token JWT.

Modelo de retorno
{
  "success": true,
  "data": [
    {
      "id": 1,
      "nome": "Empresa A",
      "fantasia": "Empresa A Fantasia",
      "email": "empresaA@example.com",
      "telefone": "11999999999",
      "cnpj": "12345678000100",
      "numeroConta": 12345,
      "agencia": 6789,
      "imagemDocumento": "/images/documento1.jpg"
    }
  ],
  "message": null
}

Endpoint: GET /api/contas/{id}
Descrição: Retorna os detalhes de uma conta com base no ID fornecido.
Autenticação: Requer token JWT.

Modelo de retorno
{
  "success": true,
  "data": {
    "id": 1,
    "nome": "Empresa A",
    "fantasia": "Empresa A Fantasia",
    "email": "empresaA@example.com",
    "telefone": "11999999999",
    "cnpj": "12345678000100",
    "numeroConta": 12345,
    "agencia": 6789,
    "imagemDocumento": "/images/documento1.jpg"
  },
  "message": null
}

Endpoint: POST /api/contas
Descrição: Cria uma nova conta com os dados fornecidos.
Autenticação: Requer token JWT.

Corpo da Requisição:
Formato: multipart/form-data
Campos:
cnpj (string, obrigatório): CNPJ da empresa.
numeroConta (int, obrigatório): Número da conta.
agencia (int, obrigatório): Agência da conta.
documento (file, opcional): Arquivo de documento (imagem).

Modelo de retorno 
{
  "success": true,
  "data": 1, // ID da conta criada
  "message": null
}

Endpoint: PUT /api/contas
Descrição: Atualiza os dados de uma conta existente.
Autenticação: Requer token JWT.

Corpo da Requisição:
Formato: multipart/form-data
Campos:
id (int, obrigatório): ID da conta.
cnpj (string, obrigatório): CNPJ da empresa.
numeroConta (int, obrigatório): Número da conta.
agencia (int, obrigatório): Agência da conta.
documento (file, opcional): Arquivo de documento (imagem).

Modelo de retorno 
{
  "success": true,
  "data": 1, // ID da conta atualizada
  "message": null
}

Endpoint: DELETE /api/contas/{id}
Descrição: Exclui uma conta com base no ID fornecido.
Autenticação: Requer token JWT.

Parâmetros:
id (int, obrigatório): ID da conta.

Modelo de retorno
{
  "success": false,
  "data": null,
  "message": "Conta com ID 1 não encontrada."
}

Documentação da API - Transação

Endpoint: GET /api/transacao/saldo/{id}
Descrição: Retorna o saldo atual de uma conta com base no ID fornecido.
Autenticação: Requer token JWT.

Modelo de retorno
{
    "success": true,
    "message": null,
    "data": [
        {
            "id": 0,
            "valor": 0,
            "saldo": 500000.60,
            "tipo": 0,
            "tipoNome": "Saldo",
            "contaId": 22,
            "contaDestinoId": 0,
            "dataTransacao": "0001-01-01T00:00:00",
            "conta": {
                "id": 22,
                "cnpj": "19368947000188",
                "numeroConta": 1012,
                "agencia": 1020,
                "imagemDocumento": "/images/0b242b6a-d0c2-4b89-94d9-3401605c9d5b_carreta.png",
                "documento": null,
                "nome": "AM COMERCIO DE MATERIAIS LTDA",
                "fantasia": "AM PAPELARIA",
                "email": "amcomercio@globomail.com",
                "telefone": "(21) 3254-2017 / (21) 3254-2027"
            },
            "contaDestino": null
        }
    ]
}

Endpoint: GET /api/transacao/extrato/{id}
Descrição: Retorna o extrato de transações de uma conta com base no ID fornecido.
Autenticação: Requer token JWT.

Parâmetros:
id (int, obrigatório): ID da conta.

Modelo de retorno
{
    "success": true,
    "message": null,
    "data": [
        {
            "id": 96,
            "valor": 500000.00,
            "saldo": 0,
            "tipo": 2,
            "tipoNome": "Deposito",
            "contaId": 22,
            "contaDestinoId": 0,
            "dataTransacao": "2025-03-15T22:39:08-03:00",
            "conta": {
                "id": 22,
                "cnpj": "19368947000188",
                "numeroConta": 1012,
                "agencia": 1020,
                "imagemDocumento": "/images/0b242b6a-d0c2-4b89-94d9-3401605c9d5b_carreta.png",
                "documento": null,
                "nome": "AM COMERCIO DE MATERIAIS LTDA",
                "fantasia": "AM PAPELARIA",
                "email": "amcomercio@globomail.com",
                "telefone": "(21) 3254-2017 / (21) 3254-2027"
            },
            "contaDestino": {
                "id": 0,
                "cnpj": "00.000.000/0001-00",
                "numeroConta": 0,
                "agencia": 0,
                "imagemDocumento": null,
                "documento": null,
                "nome": null,
                "fantasia": null,
                "email": null,
                "telefone": null
            }
        }
        ]
}

Endpoint: POST /api/transacao/deposito
Descrição: Realiza um depósito em uma conta.
Autenticação: Requer token JWT.

Parâmetros:
ContaId (int, obrigatório): ID da conta.
Valor (decimal, obrigatório): Valor a ser transferido.

Modelo de retorno
{
  "success": true,
  "data": true,
  "message": null
}

Endpoint: POST /api/transacao/saque
Descrição: Realiza um saque em uma conta.
Autenticação: Requer token JWT.

Parâmetros:
ContaId (int, obrigatório): ID da conta que realizará o saque.
Valor (decimal, obrigatório): Valor a ser sacado.

Modelo de retorno
{
  "success": true,
  "data": true,
  "message": null
}

Endpoint: POST /api/transacao/transferencia
Descrição: Realiza uma transferência entre duas contas.
Autenticação: Requer token JWT.

Parâmetros:
contaOrigemId (int, obrigatório): ID da conta de origem.
contaDestinoId (int, obrigatório): ID da conta de destino.
Valor (decimal, obrigatório): Valor a ser transferido.

Modelo de retorno
{
  "success": true,
  "data": true,
  "message": null
}