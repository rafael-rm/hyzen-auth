# Micro-serviço de Autenticação em .NET 8 ASP.NET Core

Este projeto é um micro-serviço de autenticação baseado em JWT, criado com .NET 8 ASP.NET Core. Ele implementa um sistema de permissão robusto baseado em roles e groups, garantindo consistência e segurança nas permissões de acesso.

## Funcionalidades

- Autenticação baseada em JWT.
- Sistema de permissão com roles e groups.
- Gerenciamento de usuários, grupos e roles.
- Validações para garantir a consistência das permissões.
- Integração com o Sentry para monitoramento de erros e falhas.

## Criptografia de Senhas

Para garantir a segurança das senhas dos usuários, utilizamos o algoritmo PBKDF2 com SHA512. Abaixo estão os parâmetros de configuração usados para a criptografia das senhas:

- **Salt**: 64 bytes - Um valor único de 64 bytes é gerado para cada senha, adicionando uma camada extra de segurança contra ataques de força bruta.
- **Hash**: 64 bytes - O resultado da hash gerada possui 64 bytes, assegurando um alto nível de segurança.
- **Iterations**: 256.000 - A quantidade de iterações realizadas pelo algoritmo é 256.000, aumentando significativamente a dificuldade de ataques de cracking.

Esses parâmetros garantem que as senhas armazenadas sejam altamente resistentes a tentativas de quebra, protegendo eficazmente os dados dos usuários.

## Endpoints

### Auth

- **POST /api/v1/Auth/Login**
  - Realiza o login do usuário e retorna um token JWT.
- **POST /api/v1/Auth/Verify**
  - Verifica se um token JWT é válido e retorna as informações do usuário.

### Group

- **GET /api/v1/Group**
  - Retorna a lista de grupos.
- **POST /api/v1/Group**
  - Cria um novo grupo.
- **DELETE /api/v1/Group**
  - Deleta um grupo.
- **PUT /api/v1/Group**
  - Atualiza um grupo.
- **POST /api/v1/Group/HasRole**
  - Verifica se um grupo possui determinada role.
- **POST /api/v1/Group/AddRole**
  - Adiciona uma role a um grupo.
- **POST /api/v1/Group/RemoveRole**
  - Remove uma role de um grupo.
- **POST /api/v1/Group/AddGroupToUser**
  - Adiciona um grupo a um usuário.
- **POST /api/v1/Group/RemoveGroupToUser**
  - Remove um grupo de um usuário.

### Role

- **GET /api/v1/Role**
  - Retorna a lista de roles.
- **POST /api/v1/Role**
  - Cria uma nova role.
- **DELETE /api/v1/Role**
  - Deleta uma role.
- **PUT /api/v1/Role**
  - Atualiza uma role.
- **POST /api/v1/Role/AddRoleToUser**
  - Adiciona uma role a um usuário.
- **POST /api/v1/Role/RemoveRoleToUser**
  - Remove uma role de um usuário.

### User

- **GET /api/v1/User**
  - Retorna a lista de usuários.
- **POST /api/v1/User**
  - Cria um novo usuário.
- **DELETE /api/v1/User**
  - Deleta um usuário.
- **PUT /api/v1/User**
  - Atualiza um usuário.
- **POST /api/v1/User/HasRole**
  - Verifica se um usuário possui determinada role.

## Exemplo de JWT Retornado (Decodificado)

Para garantir que o JWT não ultrapasse 4KB quando utilizado em cookies do navegador e para garantir uma autorização em tempo real, os grupos e roles não são retornados diretamente no JWT. Em vez disso, essas informações são obtidas em tempo real pelo endpoint `/api/v1/Auth/Verify`.

```json
{
  "primarysid": "931023cd-5c9a-420c-821d-d8ed2d2a1eab",
  "given_name": "Tester",
  "email": "tester@test.com.br",
  "nbf": 1716154875,
  "exp": 1716165675,
  "iat": 1716154875
}
