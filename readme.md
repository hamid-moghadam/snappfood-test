# Description

Snappfood test

## Requirement

Docker

## Getting started

```
docker compose up -d
```

## Usage

Check API endpoints using: ```http://localhost:5300/swagger```

## Tests
Unit tests run automatically in docker image build. In order to run integration tests, you must fill "AppDbContextTest" value in appsettings and then run ```dotnet test```.
