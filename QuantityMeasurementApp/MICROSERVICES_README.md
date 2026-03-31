# QuantityMeasurementApp Microservices E2E Guide

## 1. Purpose
This document captures:
- the current microservices topology
- exact run steps
- the end-to-end validation tests executed
- observed results
- cleanup performed to remove legacy monolith-layer projects

This is the baseline handoff document for running the system using only:
- Auth Service
- Quantity Service
- History Service
- API Gateway
- Angular Frontend

## 2. Active Projects After Cleanup
### Backend
- `QuantityMeasurement.AuthService`
- `QuantityMeasurement.QuantityService`
- `QuantityMeasurement.HistoryService`
- `QuantityMeasurement.Gateway`

### Frontend
- `QuantityMeasurement.Frontend`

## 3. Removed Legacy Projects
The following legacy monolith-layer projects were removed from solution and filesystem:
- `QuantityMeasurement.API`
- `QuantityMeasurement.BusinessLayer`
- `QuantityMeasurement.Infrastructure`
- `QuantityMeasurement.ModelLayer`

Also removed from solution to prevent broken references:
- `QuantityMeasurement.Tests` (it referenced the removed `QuantityMeasurement.ModelLayer`)

## 4. Routing Topology
Gateway is the single public API entrypoint.

Gateway routes:
- `/api/auth/{**catch-all}` -> Auth Service (`http://localhost:5179`)
- `/api/quantity/{**catch-all}` -> Quantity Service (`http://localhost:5090`)
- `/api/history/{**catch-all}` -> History Service (`http://localhost:5053`)

Frontend calls `/api/*` and uses Angular proxy to forward all API calls to:
- `http://localhost:5169` (Gateway)

## 5. Required Ports
- Gateway: `5169`
- Auth Service: `5179`
- Quantity Service: `5090`
- History Service: `5053`
- Frontend (Angular dev server): `4200`

## 6. End-to-End Validation Executed
The following validations were run after frontend-to-gateway connection update.

### 6.1 Build Validation
All builds succeeded:
- Gateway build: success
- Auth Service build: success
- Quantity Service build: success
- History Service build: success
- Frontend build (`ng build`): success

### 6.2 Runtime Validation
All services started successfully on expected ports:
- Auth Service listening on `http://localhost:5179`
- Quantity Service listening on `http://localhost:5090`
- History Service listening on `http://localhost:5053`
- Gateway listening on `http://localhost:5169`
- Frontend listening on `http://localhost:4200`

### 6.3 Gateway Path Smoke Test
Smoke test sequence executed through Gateway (`http://localhost:5169`):
1. Register user
2. Login user and receive JWT
3. Call quantity convert endpoint with JWT
4. Call history endpoint with JWT

Observed outputs from executed test:
- `USER=ui_ms_195764432`
- `CONVERT_RESULT={"result":1,"unit":"Feet"}`
- `HISTORY_COUNT=18`

This confirms end-to-end flow through frontend-compatible API path:
- Auth route works
- JWT-protected Quantity route works
- JWT-protected History route works
- Gateway forwarding works

## 7. How To Run Everything
Open separate terminals from workspace root (`QuantityMeasurementApp`).

### 7.1 Start backend microservices + gateway
Terminal 1:
```bash
dotnet run --project QuantityMeasurement.AuthService/QuantityMeasurement.AuthService.csproj
```

Terminal 2:
```bash
dotnet run --project QuantityMeasurement.QuantityService/QuantityMeasurement.QuantityService.csproj
```

Terminal 3:
```bash
dotnet run --project QuantityMeasurement.HistoryService/QuantityMeasurement.HistoryService.csproj
```

Terminal 4:
```bash
dotnet run --project QuantityMeasurement.Gateway/QuantityMeasurement.Gateway.csproj
```

### 7.2 Start frontend
Terminal 5:
```bash
cd QuantityMeasurement.Frontend
npm install
npm start
```

Frontend URL:
- `http://localhost:4200`

## 8. Manual Functional Test Checklist
From frontend:
1. Register a new user
2. Login with same user
3. Run one quantity operation (example: `12 inch -> feet`)
4. Open/refresh history
5. Clear history

Expected:
- login succeeds
- operation returns result
- history shows operation
- clear history removes entries

## 9. API Quick Checks (Optional)
Using PowerShell through Gateway:
```powershell
$u='ui_ms_'+(Get-Random)
Invoke-RestMethod -Method Post -Uri ('http://localhost:5169/api/auth/register?username='+$u+'&password=123') | Out-Null
$t=Invoke-RestMethod -Method Post -Uri ('http://localhost:5169/api/auth/login?username='+$u+'&password=123')
$headers=@{ Authorization = 'Bearer '+$t }
$body=@{ value1=12; unit1='inch'; targetUnit='feet' } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri 'http://localhost:5169/api/quantity/convert' -Headers $headers -ContentType 'application/json' -Body $body
Invoke-RestMethod -Method Get -Uri 'http://localhost:5169/api/history' -Headers $headers
```

## 10. Notes
- HTTPS redirection warnings may appear in console when running HTTP-only launch profiles; this does not block local functional testing.
- Current architecture is valid bare-minimum microservices with gateway routing and independent service processes.
- For production hardening, add health checks, centralized logging/trace correlation, and separate data ownership strategy where needed.
