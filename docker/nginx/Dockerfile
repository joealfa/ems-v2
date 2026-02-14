# Stage 1: Build frontend
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files for install
COPY application/package.json application/package-lock.json ./

# Install dependencies
RUN npm ci

# Copy frontend source
COPY application/ .

# Set build-time environment variables (relative URLs for nginx proxy)
ARG VITE_GRAPHQL_URL=/graphql
ARG VITE_GRAPHQL_WS_URL=wss://localhost/graphql
ARG VITE_GOOGLE_CLIENT_ID
ARG VITE_API_BASE_URL=

ENV VITE_GRAPHQL_URL=$VITE_GRAPHQL_URL
ENV VITE_GRAPHQL_WS_URL=$VITE_GRAPHQL_WS_URL
ENV VITE_GOOGLE_CLIENT_ID=$VITE_GOOGLE_CLIENT_ID
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

# Build
RUN npm run build

# Stage 2: Nginx
FROM nginx:alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy nginx config
COPY docker/nginx/nginx.conf /etc/nginx/nginx.conf

# Copy built frontend
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 443 80

CMD ["nginx", "-g", "daemon off;"]
