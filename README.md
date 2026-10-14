# SalesWeb — Gestão Comercial em ASP.NET Core MVC

Sistema web de gestão de **vendas, vendedores e departamentos**. Tem dashboard financeiro, busca avançada com exportação de relatórios e interface no estilo SaaS em tema escuro.

O projeto partiu do clássico *SalesWebMvc* do curso de C# do Prof. Nelio Alves e foi evoluído até virar uma aplicação com cara de produção. As mudanças incluem uma camada de serviços, validações completas, paginação no banco, migrations versionadas e um design responsivo próprio.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4)
![EF Core](https://img.shields.io/badge/EF_Core-10-6DB33F)
![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)
![Chart.js](https://img.shields.io/badge/Chart.js-4-FF6384?logo=chartdotjs&logoColor=white)

![Dashboard](docs/screenshots/dashboard.png)

---

## Sumário

- [Funcionalidades](#funcionalidades)
- [Capturas de tela](#capturas-de-tela)
- [Tecnologias](#tecnologias)
- [Como executar](#como-executar)
- [Arquitetura](#arquitetura)
- [Estrutura de pastas](#estrutura-de-pastas)
- [Modelo de dados](#modelo-de-dados)
- [Regras de negócio e validações](#regras-de-negócio-e-validações)
- [Rotas](#rotas)
- [Decisões técnicas](#decisões-técnicas)
- [Migrations e banco de dados](#migrations-e-banco-de-dados)
- [Solução de problemas](#solução-de-problemas)
- [Próximos passos](#próximos-passos)
- [Créditos](#créditos)

---

## Funcionalidades

### Dashboard
- **4 KPIs do ano corrente:** faturamento total, vendas registradas, vendedores ativos e ticket médio.
- **Indicadores de variação** (▲/▼ em %), que comparam o mês atual até hoje com o mesmo período do mês anterior.
- **Gráfico de área** com o faturamento mês a mês.
- **Gráfico de rosca** com a distribuição do faturamento por departamento, com legenda de valores e percentuais.
- **Tabela das últimas vendas** com badges de status e ações rápidas.

### Registro de Vendas
- CRUD completo (criar, detalhar, editar, excluir).
- **Filtros:** período (de/até), nome do vendedor, departamento e status.
- **Ordenação** por data, vendedor e valor, clicando no cabeçalho da coluna.
- **Paginação no servidor**, com 10, 25, 50 ou 100 itens por página.
- **Totais do resultado filtrado:** faturado e pendente, calculados sobre todo o resultado e não só a página.
- **Exportação para Excel/CSV** do resultado completo da busca, respeitando os filtros.
- **Exportação para PDF** pela impressão do navegador, com folha de estilo própria.

### Vendedores
- CRUD completo com formulário validado.
- Busca por nome ou e-mail, filtro por departamento e paginação.
- Página de detalhes com as 10 vendas mais recentes do vendedor.

### Departamentos
- CRUD completo.
- A listagem mostra a quantidade de vendedores e o faturamento acumulado de cada departamento.
- Página de detalhes com a equipe do departamento e um atalho para as vendas dele.

### Experiência de uso
- **Sidebar fixa**, que vira menu lateral deslizante no celular.
- **Topbar** com busca global (atalho de teclado `/`), notificações e menu do usuário.
- **Notificações reais:** o sino lista as vendas pendentes de faturamento.
- **Toasts (SweetAlert2)** após criar, editar ou excluir registros.
- **Confirmação de exclusão** em modal, sem precisar sair da listagem.
- **100% responsivo:** no celular, as tabelas viram cartões empilhados.
- **Acessibilidade:** link "pular para o conteúdo", foco visível, `aria-label` nos botões de ícone e status indicado por ícone + texto (não só por cor). As animações respeitam `prefers-reduced-motion`.

---

## Capturas de tela

| Registro de Vendas | Formulário com validação |
|---|---|
| ![Vendas](docs/screenshots/vendas.png) | ![Formulário](docs/screenshots/formulario-vendedor.png) |

| Mobile: Dashboard | Mobile: Vendas |
|---|---|
| ![Mobile dashboard](docs/screenshots/mobile-dashboard.png) | ![Mobile vendas](docs/screenshots/mobile-vendas.png) |

---

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Backend | C# 13, .NET 10, ASP.NET Core MVC |
| Acesso a dados | Entity Framework Core 10 (SQL Server) |
| Banco | SQL Server LocalDB (desenvolvimento) |
| UI | Razor Views, Bootstrap 5.3 (tema dark), Bootstrap Icons, fonte Inter |
| Gráficos | Chart.js 4 |
| Notificações | SweetAlert2 11 |
| Validação no cliente | jQuery Validation + Unobtrusive, com adaptadores customizados |
| Ferramentas | `dotnet-ef` (ferramenta local, via `dotnet-tools.json`) |

As bibliotecas de front-end (Bootstrap, Bootstrap Icons, Chart.js e SweetAlert2) são carregadas via **CDN jsDelivr**, com versões fixadas. O jQuery e o jQuery Validation ficam em `wwwroot/lib`.

---

## Como executar

### Pré-requisitos
- [.NET SDK 10](https://dotnet.microsoft.com/download).
- **SQL Server LocalDB**: vem com o Visual Studio (workload *ASP.NET e desenvolvimento web*) ou com o [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads). Para conferir se está instalado, rode `sqllocaldb info`.
- Opcional: Visual Studio 2022+, VS Code ou Rider.

### Passo a passo

```bash
# 1. Clonar o repositório
git clone https://github.com/formigoniricardo/SalesWebMvc.git
cd SalesWebMvc

# 2. Restaurar a ferramenta local do EF Core (dotnet-ef)
dotnet tool restore

# 3. Executar a aplicação
cd SalesWebMvc
dotnet run
```

Depois é só abrir a URL exibida no terminal (por exemplo `https://localhost:7xxx`).

> **Banco criado automaticamente:** no ambiente `Development`, a aplicação aplica as migrations pendentes e popula o banco com dados de demonstração na primeira execução. Os dados são 4 departamentos, 6 vendedores e 160 vendas distribuídas pelos últimos 14 meses. Não é preciso rodar `Update-Database` manualmente.

### Connection string

Fica em `SalesWebMvc/appsettings.json`:

```json
"ConnectionStrings": {
  "SalesWebMvcContext": "Server=(localdb)\\mssqllocaldb;Database=SalesWebMvcContext;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Para apontar para outro servidor sem alterar o arquivo, use uma variável de ambiente:

```powershell
$env:ConnectionStrings__SalesWebMvcContext = "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"
```

### Recriar os dados de demonstração

O seed **só roda com o banco vazio**. Para começar do zero:

```bash
cd SalesWebMvc
dotnet ef database drop --force
dotnet run
```

> As datas das vendas do seed são geradas em relação à data atual, então o dashboard sempre tem dados para o ano corrente. A semente aleatória é fixa, o que torna os valores reproduzíveis.

---

## Arquitetura

A aplicação segue o padrão **MVC com camada de serviços**. Os controllers ficam enxutos e delegam a regra de negócio e o acesso a dados aos services, que recebem o `DbContext` por **injeção de dependência nativa** do ASP.NET Core.

```
Requisição HTTP
   │
   ▼
Controller ── valida ModelState, monta ViewModels, define toasts (TempData)
   │
   ▼
Service ───── regras de negócio, consultas EF Core, paginação, agregações
   │
   ▼
SalesWebMvcContext (EF Core) ──► SQL Server
```

### Services (registrados como `Scoped` em `Program.cs`)

| Service | Responsabilidade |
|---|---|
| `DashboardService` | KPIs, variações mensais, séries dos gráficos e últimas vendas, com todas as agregações feitas no SQL |
| `SalesRecordService` | Busca filtrada, ordenada e paginada, totais, CRUD e geração do CSV |
| `SellerService` | Busca paginada, CRUD, checagem de e-mail único e tratamento de integridade |
| `DepartmentService` | Listagem com estatísticas, CRUD, checagem de nome único e tratamento de integridade |
| `SeedingService` | Aplica migrations e popula o banco em desenvolvimento |

### Componentes reutilizáveis

| Componente | Tipo | Uso |
|---|---|---|
| `PagedList<T>` / `IPagedList` | Helper | Paginação assíncrona (`CountAsync` + `Skip/Take`) |
| `_Pagination.cshtml` | Partial view | Navegação de páginas que **preserva os filtros** da query string |
| `<status-badge status="..." />` | Tag Helper | Badge colorido com ícone + texto do status |
| `NotificationsViewComponent` | View Component | Sino de notificações com as vendas pendentes |
| `ToastExtensions` | Helper | `TempData.ToastSuccess(...)` / `TempData.ToastError(...)` |
| `Format` | Helper | Formatação pt-BR: moeda, data, número, percentual e iniciais |
| `MinimumAgeAttribute`, `NotInFutureAttribute` | Validação | Regras customizadas com suporte a servidor **e** cliente |

### ViewModels

As telas que dependem de dados cruzados usam ViewModels tipados em vez de `ViewBag`:

- `SellerFormViewModel`: o vendedor mais a lista de departamentos.
- `SalesRecordFormViewModel`: a venda mais a lista de vendedores.
- `SellerListViewModel` / `SalesRecordListViewModel`: a página de resultados, os filtros atuais e as opções dos filtros.
- `SalesRecordFilter`: os parâmetros de busca vindos da query string. É reaproveitado pela exportação CSV.
- `DashboardViewModel`: os KPIs e as séries dos gráficos.

---

## Estrutura de pastas

```
SalesWebMvc/
├── dotnet-tools.json            # dotnet-ef como ferramenta local
├── docs/screenshots/            # imagens do README
└── SalesWebMvc/
    ├── Controllers/             # Home (dashboard), Sellers, Departments, SalesRecords
    ├── Data/                    # DbContext (Fluent API) e SeedingService
    ├── Helpers/                 # PagedList, Format, SaleStatusExtensions, ToastExtensions
    ├── Migrations/              # histórico versionado do schema
    ├── Models/
    │   ├── Enums/               # SaleStatus
    │   ├── Validation/          # atributos de validação customizados
    │   └── ViewModels/          # modelos de tela
    ├── Services/
    │   └── Exceptions/          # IntegrityException, NotFoundException
    ├── TagHelpers/              # <status-badge>
    ├── ViewComponents/          # Notifications
    ├── Views/
    │   ├── Home/                # Dashboard e Privacidade
    │   ├── SalesRecords/        # Index, Create, Edit, Details, Delete, _Form
    │   ├── Sellers/             # Index, Create, Edit, Details, Delete, _Form
    │   ├── Departments/         # Index, Create, Edit, Details, Delete, _Form
    │   └── Shared/              # _Layout, _Pagination, Error, Components/
    └── wwwroot/
        ├── css/site.css         # design system (tokens, componentes, responsivo, impressão)
        └── js/
            ├── site.js          # sidebar, toasts, confirmação de exclusão, atalhos
            ├── dashboard.js     # gráficos Chart.js
            └── validation.js    # adaptadores do jQuery Validation
```

---

## Modelo de dados

```
┌──────────────┐ 1      N ┌──────────────┐ 1      N ┌──────────────────┐
│  Department  │──────────│    Seller    │──────────│   SalesRecord    │
├──────────────┤          ├──────────────┤          ├──────────────────┤
│ Id           │          │ Id           │          │ Id               │
│ Name (único) │          │ Name         │          │ Date      (idx)  │
└──────────────┘          │ Email (único)│          │ Amount           │
                          │ BirthDate    │          │ Status    (idx)  │
                          │ BaseSalary   │          │ SellerId (FK)    │
                          │ DepartmentId │          └──────────────────┘
                          └──────────────┘
```

| Entidade | Campo | Tipo no banco | Observação |
|---|---|---|---|
| Department | `Name` | `nvarchar(50)` | Índice único |
| Seller | `Name` | `nvarchar(60)` | |
| Seller | `Email` | `nvarchar(100)` | Índice único |
| Seller | `BaseSalary` | `decimal(18,2)` | |
| Seller | `DepartmentId` | `int` (FK) | `ON DELETE RESTRICT` |
| SalesRecord | `Amount` | `decimal(18,2)` | |
| SalesRecord | `Date` | `datetime2` | Indexado (filtros e dashboard) |
| SalesRecord | `Status` | `int` | Indexado; enum `SaleStatus` |
| SalesRecord | `SellerId` | `int` (FK) | `ON DELETE RESTRICT` |

**Status da venda (`SaleStatus`):**

| Valor | Enum | Exibição |
|---|---|---|
| 0 | `Pending` | 🟡 Pendente |
| 1 | `Billed` | 🟢 Faturado |
| 2 | `Canceled` | 🔴 Cancelado |

---

## Regras de negócio e validações

Todas as regras são verificadas **no servidor** (`ModelState`) e, sempre que possível, também **no navegador** (jQuery Validation). As mensagens são em português.

### Vendedor
| Campo | Regra |
|---|---|
| Nome | Obrigatório, de 3 a 60 caracteres |
| E-mail | Obrigatório, formato válido, até 100 caracteres, **único** no sistema |
| Data de nascimento | Obrigatória, **idade entre 18 e 100 anos** (`[MinimumAge]`) |
| Salário base | Obrigatório, **não pode ser negativo**, máximo de R$ 1.000.000 |
| Departamento | Obrigatório |

### Venda
| Campo | Regra |
|---|---|
| Data | Obrigatória, **não pode ser futura** (`[NotInFuture]`) |
| Valor | Obrigatório, maior que zero, máximo de R$ 10.000.000 |
| Status | Obrigatório, precisa ser um valor válido do enum |
| Vendedor | Obrigatório |

### Departamento
| Campo | Regra |
|---|---|
| Nome | Obrigatório, de 2 a 50 caracteres, **único** |

### Integridade
- Um **vendedor com vendas** não pode ser excluído.
- Um **departamento com vendedores** não pode ser excluído.
- Nos dois casos, a FK está configurada como `Restrict`. O service converte o erro do banco em `IntegrityException`, e o usuário vê um toast de erro amigável.

### Segurança
- `[ValidateAntiForgeryToken]` em todos os POSTs. A exclusão por modal também envia o token.
- `[Bind]` com lista explícita de campos em todos os formulários, para proteger contra *overposting*.
- **Proteção contra CSV injection:** campos que começam com `=`, `+`, `-` ou `@` são neutralizados na exportação.
- Os dados injetados em `<script>` (gráficos e toasts) são serializados com `System.Text.Json`, que escapa `<`, `>` e `&`. O nome no modal de exclusão é inserido via `textContent`.

---

## Rotas

| Rota | Descrição |
|---|---|
| `GET /` | Dashboard |
| `GET /SalesRecords` | Busca de vendas: `search`, `minDate`, `maxDate`, `departmentId`, `status`, `sort`, `page`, `pageSize` |
| `GET /SalesRecords/Export` | Download do CSV (aceita os mesmos filtros da busca) |
| `GET /SalesRecords/{Create\|Details\|Edit\|Delete}/{id?}` | CRUD de vendas |
| `GET /Sellers` | Lista de vendedores: `search`, `departmentId`, `page`, `pageSize` |
| `GET /Sellers/{Create\|Details\|Edit\|Delete}/{id?}` | CRUD de vendedores |
| `GET /Departments` | Lista de departamentos com estatísticas |
| `GET /Departments/{Create\|Details\|Edit\|Delete}/{id?}` | CRUD de departamentos |
| `GET /Home/Privacy` | Página de privacidade |

Valores aceitos em `sort`: `date_desc` (padrão), `date`, `amount`, `amount_desc` e `seller`.

Exemplo de URL de busca (pode ser salva nos favoritos ou compartilhada):
```
/SalesRecords?status=Pending&departmentId=2&minDate=2026-01-01&sort=amount_desc&page=2
```

---

## Decisões técnicas

**`decimal` em vez de `double` para dinheiro.** Ponto flutuante binário acumula erros de arredondamento (`0.1 + 0.2 != 0.3`). Valores monetários usam `decimal(18,2)`.

**Paginação no servidor em vez de DataTables.** O DataTables no modo cliente carrega a tabela inteira no navegador, o que não escala. Aqui só a página atual sai do banco (`Skip/Take`), os filtros viram `WHERE` indexados e as URLs podem ser compartilhadas.

**CSV gerado no servidor para exportar.** Não adiciona dependências, exporta **todo** o resultado filtrado (até 50 mil linhas) e não só a página visível. O arquivo sai em UTF-8 com BOM e separador `;`, e abre direto e com acentos corretos no Excel em português. Se for preciso um `.xlsx` formatado, o [ClosedXML](https://github.com/ClosedXML/ClosedXML) pode substituir o método `ExportCsvAsync` sem mudar o controller.

**Cultura en-US no model binding e exibição em pt-BR.** Campos `<input type="number">` sempre enviam ponto decimal (`1234.56`). Com a cultura pt-BR, o model binding leria `123456`. Por isso a cultura das requisições é fixa em en-US, e toda a exibição (R$, dd/MM/yyyy) é formatada explicitamente pelo helper `Format`.

**Variação de KPIs com comparação justa.** Comparar o mês corrente, ainda incompleto, com o mês anterior inteiro sempre mostraria uma queda. Por isso o dashboard compara "do dia 1 até hoje" com o mesmo intervalo do mês anterior.

**Validações customizadas no servidor e no cliente.** `MinimumAgeAttribute` e `NotInFutureAttribute` implementam `IClientModelValidator`. Eles emitem atributos `data-val-*`, que o `validation.js` registra no jQuery Validation, então a mesma regra roda nos dois lados.

**Paleta dos gráficos validada.** As cores categóricas foram verificadas contra a superfície escura dos cards: contraste mínimo de 3:1 e separação para daltonismo (deuteranopia, protanopia e tritanopia).

**Tema.** Tokens CSS (`--app-*`) sobre o Bootstrap 5.3 em `data-bs-theme="dark"`. Uma única folha (`site.css`) cobre componentes, breakpoints e um modo de impressão próprio, que é a base do "Exportar PDF".

---

## Migrations e banco de dados

| Migration | Conteúdo |
|---|---|
| `OtherEntities` | Criação das tabelas `Department`, `Seller` e `SalesRecord` |
| `DecimalMoneyConstraintsAndIndexes` | Valores monetários em `decimal(18,2)`, limites de tamanho nos textos, FK de vendedor obrigatória, `ON DELETE RESTRICT`, índices em `Date`/`Status` e unicidade de `Email`/`Name` |

Comandos úteis (rode dentro de `SalesWebMvc/`):

```bash
dotnet tool restore                          # instala o dotnet-ef local
dotnet ef migrations add NomeDaMigration     # cria uma nova migration
dotnet ef database update                    # aplica as migrations manualmente
dotnet ef migrations list                    # lista as migrations e o status
dotnet ef database drop --force              # apaga o banco (o seed recria)
```

> Se o banco atual tiver vendas sem vendedor (`SellerId` nulo) ou e-mails/nomes de departamento duplicados, a migration `DecimalMoneyConstraintsAndIndexes` vai falhar. Corrija esses registros ou recrie o banco.

---

## Solução de problemas

| Problema | Solução |
|---|---|
| `dotnet-ef não existe` | Rode `dotnet tool restore` na raiz do repositório |
| Erro de conexão com `(localdb)\mssqllocaldb` | Confira com `sqllocaldb info`. Se a instância estiver parada, rode `sqllocaldb start MSSQLLocalDB` |
| Dashboard vazio | O banco já tinha dados antigos e o seed não rodou. Recrie com `dotnet ef database drop --force` e depois `dotnet run` |
| Gráficos ou ícones não aparecem | As bibliotecas vêm de CDN; verifique a conexão com a internet |
| Valores decimais salvos errado | Não altere a cultura em `Program.cs`. Leia a [decisão técnica](#decisões-técnicas) sobre cultura |
| Aviso de certificado HTTPS | Rode `dotnet dev-certs https --trust` |

---

## Próximos passos

Possíveis evoluções:

- [ ] Autenticação e autorização com ASP.NET Core Identity (perfis Admin e Vendedor)
- [ ] Interfaces para os services (`ISellerService` etc.) e testes unitários com xUnit + EF InMemory/SQLite
- [ ] Testes de integração com `WebApplicationFactory`
- [ ] Exportação `.xlsx` formatada com ClosedXML
- [ ] Alternância entre tema claro e escuro
- [ ] Docker Compose (app + SQL Server) e pipeline de CI no GitHub Actions
- [ ] Logs estruturados (Serilog) e health checks

---

## Créditos

- Projeto base: curso **"Programação Orientada a Objetos com C#"** do Prof. **Nelio Alves** (seção *SalesWebMvc*).
- Evolução, design e funcionalidades adicionais: **Ricardo Formigoni**, [@formigoniricardo](https://github.com/formigoniricardo).

Projeto de portfólio para fins educacionais. Todos os dados exibidos são fictícios.
