ENV_FILE ?= .env.local

up:
	podman compose --env-file $(ENV_FILE) up -d

up-build:
	podman compose --env-file $(ENV_FILE) up -d --build

down:
	podman compose --env-file $(ENV_FILE) down

down-volumes:
	podman compose --env-file $(ENV_FILE) down -v

restart:
	podman compose --env-file $(ENV_FILE) restart

logs:
	podman compose --env-file $(ENV_FILE) logs -f

logs-backend:
	podman compose --env-file $(ENV_FILE) logs -f backend

logs-gateway:
	podman compose --env-file $(ENV_FILE) logs -f gateway

logs-nginx:
	podman compose --env-file $(ENV_FILE) logs -f nginx

ps:
	podman compose --env-file $(ENV_FILE) ps

.PHONY: up up-build down down-volumes restart logs logs-backend logs-gateway logs-nginx ps
