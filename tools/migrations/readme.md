# Migration Scripts

This directory contains helper documentation for managing Entity Framework Core migrations locally during development.

## Prerequisites

- .NET SDK (repo pins a version via `global.json`)
- EF Core CLI (`dotnet-ef`) installed as a global tool

## Install / update `dotnet-ef`

List installed global tools:

```bash
dotnet tool list --global
```

Install (if missing):

```bash
dotnet tool install --global dotnet-ef
```

Update (if already installed):

```bash
dotnet tool update --global dotnet-ef
```
