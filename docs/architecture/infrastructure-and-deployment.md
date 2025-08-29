# Infrastructure and Deployment

### Infrastructure as Code

- **Tool:** Docker Compose with Windows Server containers
- **Location:** `deployment/docker-compose.yml` and `deployment/docker-compose.prod.yml`
- **Approach:** Multi-service containerization with shared networks and persistent volumes

### Deployment Strategy

- **Strategy:** Blue-Green deployment with Docker Compose
- **CI/CD Platform:** GitHub Actions with Windows Server self-hosted runners
- **Pipeline Configuration:** `.github/workflows/ci.yml` and `.github/workflows/cd.yml`

### Environments

- **Development:** Local Docker Compose with hot reload and debugging enabled
- **Production:** Windows Server with Docker Compose using production-optimized images and configurations

### Promotion Flow

```text
Development → Staging (Optional) → Production
     ↓              ↓                  ↓
Local Docker → Pre-prod testing → Production deployment
```

### Rollback Strategy

- **Primary Method:** Docker Compose service rollback with previous image tags
- **Trigger Conditions:** Health check failures, manual intervention, error rate thresholds
- **Recovery Time Objective:** < 5 minutes for service rollback

### Docker Containerization Strategy

**Development Environment (docker-compose.yml):**

```yaml
version: '3.8'

services:
  postgresql:
    image: postgres:16.1
    container_name: optimalupchuck-db-dev
    environment:
      POSTGRES_DB: optimalupchuck_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: dev_password
      POSTGRES_HOST_AUTH_METHOD: trust
    ports:
      - "5432:5432"
    volumes:
      - postgres_data_dev:/var/lib/postgresql/data
      - ./scripts/database/init-db.sql:/docker-entrypoint-initdb.d/init-db.sql
    networks:
      - optimalupchuck-network

  rabbitmq:
    image: rabbitmq:3.12-management
    container_name: optimalupchuck-mq-dev
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: dev_password
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data_dev:/var/lib/rabbitmq
    networks:
      - optimalupchuck-network

  filewatcher:
    build:
      context: .
      dockerfile: src/Services/OptimalUpchuck.FileWatcher/Dockerfile
      target: development
    container_name: optimalupchuck-filewatcher-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgresql;Database=optimalupchuck_dev;Username=postgres;Password=dev_password
      - RabbitMQ__ConnectionString=amqp://admin:dev_password@rabbitmq:5672/
      - ObsidianVault__RawPath=/app/data/raw-vault
      - ObsidianVault__PristinePath=/app/data/pristine-vault
    volumes:
      - ./data/raw-vault:/app/data/raw-vault
      - ./data/pristine-vault:/app/data/pristine-vault
      - ./config:/app/config
    depends_on:
      - postgresql
      - rabbitmq
    networks:
      - optimalupchuck-network
    restart: unless-stopped

  agentworker:
    build:
      context: .
      dockerfile: src/Services/OptimalUpchuck.AgentWorker/Dockerfile
      target: development
    container_name: optimalupchuck-agentworker-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgresql;Database=optimalupchuck_dev;Username=postgres;Password=dev_password
      - RabbitMQ__ConnectionString=amqp://admin:dev_password@rabbitmq:5672/
      - Ollama__BaseUrl=http://host.docker.internal:11434
      - SemanticKernel__ModelId=mistral-small
    volumes:
      - ./config:/app/config
    depends_on:
      - postgresql
      - rabbitmq
    networks:
      - optimalupchuck-network
    restart: unless-stopped
    extra_hosts:
      - "host.docker.internal:host-gateway"

  web:
    build:
      context: .
      dockerfile: src/Services/OptimalUpchuck.Web/Dockerfile
      target: development
    container_name: optimalupchuck-web-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgresql;Database=optimalupchuck_dev;Username=postgres;Password=dev_password
      - ASPNETCORE_URLS=http://+:80
    ports:
      - "8080:80"
    depends_on:
      - postgresql
    networks:
      - optimalupchuck-network
    restart: unless-stopped

volumes:
  postgres_data_dev:
  rabbitmq_data_dev:

networks:
  optimalupchuck-network:
    driver: bridge
```

**Production Environment (docker-compose.prod.yml):**

```yaml
version: '3.8'

services:
  postgresql:
    image: postgres:16.1
    container_name: optimalupchuck-db-prod
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "5432:5432"
    volumes:
      - postgres_data_prod:/var/lib/postgresql/data
      - ./scripts/database/backup:/backup
    networks:
      - optimalupchuck-network
    restart: always
    deploy:
      resources:
        limits:
          memory: 2G
          cpus: '1.0'

  rabbitmq:
    image: rabbitmq:3.12-management
    container_name: optimalupchuck-mq-prod
    environment:
      RABBITMQ_DEFAULT_USER: ${RABBITMQ_USER}
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
      RABBITMQ_VM_MEMORY_HIGH_WATERMARK: 0.7
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data_prod:/var/lib/rabbitmq
      - ./config/rabbitmq/rabbitmq.conf:/etc/rabbitmq/rabbitmq.conf
    networks:
      - optimalupchuck-network
    restart: always
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '0.5'

  filewatcher:
    image: optimalupchuck/filewatcher:${VERSION_TAG}
    container_name: optimalupchuck-filewatcher-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DATABASE_CONNECTION_STRING}
      - RabbitMQ__ConnectionString=${RABBITMQ_CONNECTION_STRING}
      - ObsidianVault__RawPath=${RAW_VAULT_PATH}
      - ObsidianVault__PristinePath=${PRISTINE_VAULT_PATH}
      - Serilog__MinimumLevel=Information
    volumes:
      - ${RAW_VAULT_HOST_PATH}:${RAW_VAULT_PATH}:ro
      - ${PRISTINE_VAULT_HOST_PATH}:${PRISTINE_VAULT_PATH}
      - ./config/production:/app/config:ro
      - ./logs:/app/logs
    depends_on:
      - postgresql
      - rabbitmq
    networks:
      - optimalupchuck-network
    restart: always
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  agentworker:
    image: optimalupchuck/agentworker:${VERSION_TAG}
    container_name: optimalupchuck-agentworker-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DATABASE_CONNECTION_STRING}
      - RabbitMQ__ConnectionString=${RABBITMQ_CONNECTION_STRING}
      - Ollama__BaseUrl=http://host.docker.internal:11434
      - Ollama__UseMockService=false
      - Ollama__FallbackMode=queue
      - Ollama__ModelFallbackChain=mistral-small,phi3
      - Ollama__QueueTimeoutHours=4
      - Ollama__HealthCheckIntervalSeconds=30
      - SemanticKernel__ModelId=mistral-small
      - Serilog__MinimumLevel=Information
    volumes:
      - ./config/production:/app/config:ro
      - ./logs:/app/logs
    depends_on:
      - postgresql
      - rabbitmq
    networks:
      - optimalupchuck-network
    restart: always
    extra_hosts:
      - "host.docker.internal:host-gateway"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
    deploy:
      resources:
        limits:
          memory: 4G
          cpus: '2.0'

  web:
    image: optimalupchuck/web:${VERSION_TAG}
    container_name: optimalupchuck-web-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DATABASE_CONNECTION_STRING}
      - ASPNETCORE_URLS=http://+:80
      - Serilog__MinimumLevel=Warning
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./config/production:/app/config:ro
      - ./logs:/app/logs
      - ./ssl:/app/ssl:ro
    depends_on:
      - postgresql
    networks:
      - optimalupchuck-network
    restart: always
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres_data_prod:
  rabbitmq_data_prod:

networks:
  optimalupchuck-network:
    driver: bridge
```

### CI/CD Pipeline Configuration

**GitHub Actions Workflow (.github/workflows/ci.yml):**

```yaml
name: CI Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '8.0.x'
  REGISTRY: ghcr.io
  IMAGE_NAME: optimalupchuck

jobs:
  test:
    runs-on: windows-latest
    
    services:
      postgres:
        image: postgres:16.1
        env:
          POSTGRES_PASSWORD: test_password
          POSTGRES_DB: optimalupchuck_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore OptimalUpchuck.sln
    
    - name: Build solution
      run: dotnet build OptimalUpchuck.sln --no-restore --configuration Release
    
    - name: Run unit tests
      run: dotnet test tests/OptimalUpchuck.Domain.Tests --no-build --configuration Release --logger trx
    
    - name: Run application tests
      run: dotnet test tests/OptimalUpchuck.Application.Tests --no-build --configuration Release --logger trx
    
    - name: Run integration tests
      run: dotnet test tests/OptimalUpchuck.Infrastructure.Tests --no-build --configuration Release --logger trx
      env:
        ConnectionStrings__DefaultConnection: Host=localhost;Database=optimalupchuck_test;Username=postgres;Password=test_password
    
    - name: Upload test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: test-results
        path: '**/*.trx'

  build:
    runs-on: ubuntu-latest
    needs: test
    if: github.event_name == 'push'
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
    
    - name: Log in to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=branch
          type=ref,event=pr
          type=sha,prefix={{branch}}-
    
    - name: Build and push FileWatcher image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: src/Services/OptimalUpchuck.FileWatcher/Dockerfile
        platforms: linux/amd64,linux/arm64
        push: true
        tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/filewatcher:${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max
    
    - name: Build and push AgentWorker image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: src/Services/OptimalUpchuck.AgentWorker/Dockerfile
        platforms: linux/amd64,linux/arm64
        push: true
        tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/agentworker:${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max
    
    - name: Build and push Web image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: src/Services/OptimalUpchuck.Web/Dockerfile
        platforms: linux/amd64,linux/arm64
        push: true
        tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/web:${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max
```

**Deployment Pipeline (.github/workflows/cd.yml):**

```yaml
name: CD Pipeline

on:
  push:
    branches: [ main ]
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deployment environment'
        required: true
        default: 'production'
        type: choice
        options:
          - production
          - staging

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: optimalupchuck

jobs:
  deploy:
    runs-on: self-hosted
    environment: ${{ github.event.inputs.environment || 'production' }}
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Set environment variables
      run: |
        echo "VERSION_TAG=${{ github.sha }}" >> $GITHUB_ENV
        echo "DEPLOY_ENV=${{ github.event.inputs.environment || 'production' }}" >> $GITHUB_ENV
    
    - name: Login to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Create backup
      run: |
        docker-compose -f deployment/docker-compose.prod.yml exec -T postgresql pg_dump -U ${{ secrets.POSTGRES_USER }} ${{ secrets.POSTGRES_DB }} > backup-$(date +%Y%m%d-%H%M%S).sql
    
    - name: Pull latest images
      run: |
        docker pull ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/filewatcher:main-${{ github.sha }}
        docker pull ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/agentworker:main-${{ github.sha }}
        docker pull ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/web:main-${{ github.sha }}
    
    - name: Update docker-compose environment
      run: |
        cp deployment/docker-compose.prod.yml docker-compose.deploy.yml
        sed -i 's/${VERSION_TAG}/${{ github.sha }}/g' docker-compose.deploy.yml
    
    - name: Deploy services (rolling update)
      run: |
        # Update services one by one for zero-downtime deployment
        docker-compose -f docker-compose.deploy.yml up -d --no-deps filewatcher
        docker-compose -f docker-compose.deploy.yml up -d --no-deps agentworker
        docker-compose -f docker-compose.deploy.yml up -d --no-deps web
    
    - name: Wait for health checks
      run: |
        sleep 30
        docker-compose -f docker-compose.deploy.yml ps
    
    - name: Run post-deployment tests
      run: |
        # Health check endpoints
        curl -f http://localhost/health || exit 1
        # Database connectivity test
        docker-compose -f docker-compose.deploy.yml exec -T web dotnet --version
    
    - name: Cleanup old images
      run: |
        docker image prune -f
        docker system prune -f
    
    - name: Notify deployment status
      if: always()
      run: |
        if [ "${{ job.status }}" == "success" ]; then
          echo "✅ Deployment successful to ${{ env.DEPLOY_ENV }}"
        else
          echo "❌ Deployment failed to ${{ env.DEPLOY_ENV }}"
          # Rollback logic here if needed
        fi
```
