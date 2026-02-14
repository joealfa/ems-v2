FROM node:24-alpine

RUN apk add --no-cache git

WORKDIR /workspace

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
