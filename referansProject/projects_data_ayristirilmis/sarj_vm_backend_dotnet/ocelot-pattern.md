# sarj_vm_backend_dotnet — Ocelot API Gateway Pattern

## Genel Açıklama

`GateWay.Api` projesi, Ocelot kütüphanesi üzerine kurulu bir API Gateway'dir. Tüm dış istekler Gateway üzerinden geçer; downstream servisler (vm.api, vmpanel.api, vmlog.api) dışarıya doğrudan port açmaz.

---

## Ocelot Temel Kavramlar

| Kavram | Açıklama |
|---|---|
| `UpstreamPathTemplate` | İstemcinin gördüğü URL |
| `DownstreamPathTemplate` | Downstream servise iletilen URL |
| `DownstreamHostAndPorts` | Servisin Docker/K8s adresi |
| `AuthenticationOptions` | JWT veya API key doğrulama |
| `RateLimitOptions` | İstemci başına istek kotası |
| `LoadBalancerOptions` | Çoklu instance için yük dağılımı |

---

## ocelot.json — Temel Konfigürasyon

```json
{
  "GlobalConfiguration": {
    "BaseUrl": "[GATEWAY_BASE_URL]",
    "RequestIdKey": "X-Request-Id",
    "ServiceDiscoveryProvider": {
      "Type": "Internal"
    }
  },
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "vm.api", "Port": 80 }
      ],
      "UpstreamPathTemplate": "/vm/{everything}",
      "UpstreamHttpMethod": [ "POST", "GET", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1s",
        "PeriodTimespan": 1,
        "Limit": 20
      }
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "vmpanel.api", "Port": 80 }
      ],
      "UpstreamPathTemplate": "/vmpanel/{everything}",
      "UpstreamHttpMethod": [ "POST", "GET", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/health",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "vm.api", "Port": 80 }
      ],
      "UpstreamPathTemplate": "/vm/health",
      "UpstreamHttpMethod": [ "GET" ]
    }
  ]
}
```

---

## Ortama Göre Ocelot Konfigürasyonu

Her ortam için ayrı ocelot JSON dosyası:

```
GateWay.Api/
├── ocelot.json                  — Base (tüm ortamlar için ortak rotalar)
├── ocelot.Development.json      — Dev ortamı overrides
├── ocelot.Local.json            — Local geliştirme
├── ocelot.Test.json             — Test sunucusu
└── ocelot.Production.json       — Production
```

### Program.cs'de Ortam Bazlı Yükleme

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json",
                 optional: true, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options => {
        options.RequireHttpsMetadata = false;
        options.Authority = builder.Configuration["JwtSettings:Issuer"];
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false
        };
    });

var app = builder.Build();
await app.UseOcelot();
app.Run();
```

### Development Override (ocelot.Development.json)

```json
{
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

### Production Override (ocelot.Production.json)

```json
{
  "GlobalConfiguration": {
    "BaseUrl": "https://api.domain.com",
    "RequestIdKey": "X-Request-Id"
  },
  "Routes": [
    {
      "UpstreamPathTemplate": "/vm/{everything}",
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1s",
        "Limit": 10
      }
    }
  ]
}
```

---

## Authentication — JWT Bearer

### Gateway'de JWT Doğrulama

```csharp
// Startup.cs veya Program.cs
services.AddAuthentication()
    .AddJwtBearer("Bearer", options => {
        options.Authority = Configuration["JwtSettings:Issuer"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"]))
        };
    });
```

### Korumalı Route Tanımı

```json
{
  "UpstreamPathTemplate": "/vmpanel/{everything}",
  "AuthenticationOptions": {
    "AuthenticationProviderKey": "Bearer",
    "AllowedScopes": []
  }
}
```

### Public Route (Auth Gerektirmeyen)

```json
{
  "UpstreamPathTemplate": "/vmpanel/api/v1/Auth/Login",
  "UpstreamHttpMethod": [ "POST" ]
  // AuthenticationOptions yok = public
}
```

---

## Rate Limiting

### Global Rate Limit

```json
{
  "GlobalConfiguration": {
    "RateLimitOptions": {
      "DisableRateLimitHeaders": false,
      "QuotaExceededMessage": "İstek limiti aşıldı. Lütfen bekleyin.",
      "HttpStatusCode": 429,
      "ClientIdHeader": "ClientId"
    }
  }
}
```

### Route Bazlı Rate Limit

```json
{
  "RateLimitOptions": {
    "ClientWhitelist": ["trusted-service"],
    "EnableRateLimiting": true,
    "Period": "1s",
    "PeriodTimespan": 1,
    "Limit": 20
  }
}
```

---

## Load Balancing (Çoklu Instance)

```json
{
  "DownstreamHostAndPorts": [
    { "Host": "vm.api.1", "Port": 80 },
    { "Host": "vm.api.2", "Port": 80 },
    { "Host": "vm.api.3", "Port": 80 }
  ],
  "LoadBalancerOptions": {
    "Type": "RoundRobin"
  }
}
```

Load Balancer tipleri:
- `RoundRobin` — sırayla dağıt
- `LeastConnection` — en az bağlantılıya gönder
- `NoLoadBalancer` — tek instance (default)

---

## Health Check Endpoint

Gateway üzerinden downstream servis health check'lerini expose et:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/health",
      "DownstreamHostAndPorts": [ { "Host": "vm.api", "Port": 80 } ],
      "UpstreamPathTemplate": "/health/vm",
      "UpstreamHttpMethod": [ "GET" ]
    },
    {
      "DownstreamPathTemplate": "/health",
      "DownstreamHostAndPorts": [ { "Host": "vmpanel.api", "Port": 80 } ],
      "UpstreamPathTemplate": "/health/vmpanel",
      "UpstreamHttpMethod": [ "GET" ]
    }
  ]
}
```

---

## Özet: Gateway Sorumlulukları

| Sorumluluk | Uygulama |
|---|---|
| **Routing** | `UpstreamPathTemplate` → `DownstreamPathTemplate` |
| **Authentication** | JWT Bearer, `AuthenticationOptions` per route |
| **Rate Limiting** | Global ve route bazlı `RateLimitOptions` |
| **Load Balancing** | `LoadBalancerOptions` (çoklu instance) |
| **Request ID** | `X-Request-Id` header ile tracing |
| **Health Check** | Downstream health route'larını expose et |
