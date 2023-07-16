# WebAPI para cadastro de clientes

- A API possibilita o cadastro de clientes possuindo o nome, email e uma lista de telefones.
- Cada telefone e email são únicos por cliente.

## Conceitos Aplicados
- Separação da aplicação em camadas seguindo a Clean Architecture;
- Cache LRU em memória com mitigação de "cache stampede" para diminuir o uso do banco, que geralmente é o maior gargalo de uma API.
- Controller anêmica que faze o papel de "adapter" entre a request HTTP e a aplicação. O que possibilita a 
troca/adição de tecnologias de comunicação sem afetar a aplicação;
- Validação e caching feitos na aplicação através do padrão decorator, utilizando o PipelineBehaviour da biblioteca Mediator;
- [Keyset pagination](https://learn.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination) para evitar leitura desnecessária de "paginas" no banco, similar ao que o Github faz na listagem de commits de um repositório;
- Foram aplicados alguns conceitos de DDD, sendo eles:
  - O agregado Cliente é responsável por executar alterações na entidade Telefone;
  - O padrão Repository reforça a regra acima, não permitindo a construção da entidade Telefone separadamente;
  - Cliente e Telefone possuem Ids fortemente tipados;
  - Alterações no Cliente são comunicadas para a aplicação através de DomainEvents;
- Também foram aplicados conceitos de CQRS:
  - Separação dos casos de uso da aplicação entre commands e queries;
  - Queries tem acesso direto ao banco através do QueryContext, pois na leitura não é nescessário garantir a construção
correta de agregados;

## Como testar
- Na raiz do projeto executar o comando:

`dotnet run --project src/Clientes.WebApi/Clientes.WebApi.csproj`

Acesse a documentação da API em: https://localhost:7222/swagger/index.html ou http://localhost:5291/swagger/index.html
