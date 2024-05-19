# Micro-serviço de Autenticação em .NET 8 ASP.NET Core

Este projeto é um micro-serviço de autenticação baseado em JWT, criado com .NET 8 ASP.NET Core. Ele implementa um sistema de permissão robusto baseado em roles e groups, garantindo consistência e segurança nas permissões de acesso.

## Funcionalidades

- Autenticação baseada em JWT.
- Sistema de permissão com roles e groups.
- Gerenciamento de usuários, grupos e roles.
- Validações para garantir a consistência das permissões.
- Criptografia de senhas usando SHA256 + Salt.

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

### Role

- **GET /api/v1/Role**
  - Retorna a lista de roles.
- **POST /api/v1/Role**
  - Cria uma nova role.
- **DELETE /api/v1/Role**
  - Deleta uma role.
- **PUT /api/v1/Role**
  - Atualiza uma role.

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
- **POST /api/v1/User/AddRole**
  - Adiciona uma role a um usuário.
- **POST /api/v1/User/RemoveRole**
  - Remove uma role de um usuário.
- **POST /api/v1/User/AddGroup**
  - Adiciona um grupo a um usuário.
- **POST /api/v1/User/RemoveGroup**
  - Remove um grupo de um usuário.

## Exemplo de JWT Retornado (Decodificado)

```json
{
  "primarysid": "931023cd-5c9a-420c-821d-d8ed2d2a1eab",
  "given_name": "Tester",
  "email": "tester@test.com.br",
  "groupsid": "Admin",
  "role": [
    "hyzen_auth:*",
    "hyzen_auth:get",
    "hyzen_auth:update"
  ],
  "nbf": 1716154875,
  "exp": 1716165675,
  "iat": 1716154875
}
```
