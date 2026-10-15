# Smart Marketing Platform — EF Core: Entities, Relationships & Migrations

Weekly engineering assignment — Izzaldeen (Backend Engineer)

| Folder | Contents |
| --- | --- |
| `Entities/` | 47 entity classes, the enums they use (stored as text), and the `BusinessEntity` base class |
| `Migrations/` | `InitialCreate` (tables, keys, relationships, constraints, seed data) and `AddBusinessRules` (triggers and views), with the model snapshot |
| `Database/` | `SmartMarketingDb.sql` — SQL equivalent of the migrations, generated with `dotnet ef migrations script` |

Target: .NET 10, EF Core 10.0.12, SQL Server. The migrations were applied to the local development
database `SmartMarketingDb`. A database built from `Database/SmartMarketingDb.sql` was compared with
the migrated one: tables, columns, primary/foreign keys, CHECK/default constraints, indexes, triggers
and views are identical.

Run the script with `QUOTED_IDENTIFIER ON` (default in SSMS; `sqlcmd -I`), which SQL Server requires
for filtered indexes.
