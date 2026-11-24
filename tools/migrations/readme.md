# Migration Scripts

This directory contains helper documentation for managing Entity Framework Core migrations locally during development.

## Prerequisites

- .NET SDK (repo pins a version via `global.json`)
- EF Core CLI (`dotnet-ef`) 11.0.0 installed as a global tool to match the repository's EF Core version

## Install / update `dotnet-ef`

List installed global tools:

```bash
dotnet tool list --global
```

Install (if missing):

```bash
dotnet tool install --global dotnet-ef --version 11.0.0
```

Update (if already installed):

```bash
dotnet tool update --global dotnet-ef --version 11.0.0
```
