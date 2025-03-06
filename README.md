# Hyzen Auth

Um microserviço de autenticação robusto e seguro, implementado com base na [Arquitetura Onion](https://en.wikipedia.org/wiki/Onion_architecture). O Hyzen Auth utiliza JWT com assinatura RS256 (RsaSha256Signature) para autenticação e autorização, fornecendo endpoints para descoberta e validação das chaves públicas.

## Visão Geral

O Hyzen Auth foi desenvolvido para oferecer uma solução escalável e segura para gerenciamento de autenticação em aplicações modernas. Baseado na Arquitetura Onion, o projeto separa claramente as responsabilidades, facilitando a manutenção, testes e evolução do código.

## Segurança

- **JWT com RS256**: Geração e validação de tokens JWT utilizando assinatura com algoritmo RSA SHA256.
- **Segurança de Senhas**:
  - Utilização de **PBKDF2** com SHA512.
  - Salt de 64 bytes.
  - Hash de 64 bytes.
  - 256 mil iterações.

### Endpoints de descoberta

- **/.well-known/jwks**: Retorna a chave pública (JSON Web Key Set) para validação dos tokens.
- **/.well-known/openid-configuration**: Fornece os metadados de configuração, seguindo os padrões do OpenID Connect.

## Exemplo de Configuração em uma API ASP.NET

Para adicionar a autenticação baseada em JWT com o Hyzen Auth em sua API ASP.NET, siga as etapas abaixo.

1. **Instalação dos Pacotes**:
   
   Você precisa adicionar os pacotes NuGet para o middleware de autenticação JWT:
   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```

   2. **Configuração da Autenticação no `Program.cs` ou `Startup.cs`**:

      No arquivo `Program.cs` (ou `Startup.cs` dependendo da sua versão do ASP.NET Core), adicione a configuração para utilizar o JWT Bearer Authentication:

      ```cs
      using Microsoft.AspNetCore.Authentication.JwtBearer;
      using Microsoft.IdentityModel.Tokens;

      var builder = WebApplication.CreateBuilder(args);

      // Configuração do serviço de autenticação utilizando JWT Bearer
      builder.Services.AddAuthentication(options =>
      {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
          // URL do Hyzen Auth que fornece os metadados de OpenID Connect, incluindo o JWKS (chaves públicas)
          options.Authority = "http://localhost:5021";
       
          // Definindo o público (Audience) que representa a API protegida
          options.Audience = "all";
   
          // Não recomendado para produção, desabilita a validação do certificado SSL
          options.RequireHttpsMetadata = false;
       
          // Parâmetros de validação do token
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true
          };
      });

      var app = builder.Build();

      // Habilita os middlewares de autenticação e autorização
      app.UseAuthentication();
      app.UseAuthorization();

      // Mapeia os controllers ou endpoints protegidos
      app.MapControllers();

      app.Run();
      ```

### Explicação do Funcionamento

- **Integração com o Hyzen Auth**:  
  A propriedade `Authority` aponta para o endereço do serviço Hyzen Auth, que expõe os endpoints de descoberta (`/.well-known/openid-configuration` e `/.well-known/jwks`). Esses endpoints fornecem os metadados e as chaves públicas necessárias para validar a assinatura dos tokens JWT.

- **Validação do Token JWT**:  
  O middleware de autenticação JWT extrai o token enviado pelo cliente (geralmente via header `Authorization: Bearer <token>`), e utiliza as chaves públicas disponibilizadas pelo Hyzen Auth para verificar a assinatura RS256 do token. Além disso, ele valida outros aspectos, como o emissor, o público (Audience) e a validade temporal do token.

- **Proteção dos Endpoints da API**:  
  Após a configuração, os middlewares `UseAuthentication()` e `UseAuthorization()` garantem que apenas requisições autenticadas e autorizadas possam acessar os endpoints protegidos. Caso o token seja inválido ou ausente, a API retorna um status de não autorizado.
