#!/bin/bash

# Shell script to generate localhost certificates for Vite (Debian/Linux)

set -e

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CERTS_DIR="$(dirname "$SCRIPT_DIR")/certs"
CERT_PATH="$CERTS_DIR/localhost.pem"
KEY_PATH="$CERTS_DIR/localhost-key.pem"

echo -e "${GREEN}Setting up HTTPS certificates for Vite...${NC}"

# Create certs directory if it doesn't exist
mkdir -p "$CERTS_DIR"

# Check if mkcert is available (preferred method)
if command -v mkcert &>/dev/null; then
    echo -e "${YELLOW}Using mkcert to generate trusted certificates...${NC}"

    # Install local CA if not already done
    mkcert -install 2>/dev/null || true

    # Generate certificates
    mkcert -cert-file "$CERT_PATH" -key-file "$KEY_PATH" localhost 127.0.0.1 ::1

    echo -e "${GREEN}Certificates generated with mkcert${NC}"
    echo -e "  ${CYAN}Certificate: $CERT_PATH${NC}"
    echo -e "  ${CYAN}Private Key: $KEY_PATH${NC}"

# Check if openssl is available (fallback)
elif command -v openssl &>/dev/null; then
    echo -e "${YELLOW}mkcert not found, falling back to openssl (self-signed)...${NC}"
    echo -e "${GRAY}Note: The browser will show a security warning for self-signed certs.${NC}"
    echo -e "${GRAY}Install mkcert for trusted certs: sudo apt install mkcert${NC}"

    openssl req -x509 -nodes -days 365 \
        -newkey rsa:2048 \
        -keyout "$KEY_PATH" \
        -out "$CERT_PATH" \
        -subj "/CN=localhost" \
        -addext "subjectAltName=DNS:localhost,IP:127.0.0.1,IP:::1"

    echo -e "${GREEN}Self-signed certificates generated with openssl${NC}"
    echo -e "  ${CYAN}Certificate: $CERT_PATH${NC}"
    echo -e "  ${CYAN}Private Key: $KEY_PATH${NC}"

else
    echo -e "${RED}Error: Neither mkcert nor openssl found.${NC}"
    echo -e "${YELLOW}Install one of:${NC}"
    echo -e "  sudo apt install mkcert    ${GRAY}(recommended, generates trusted certs)${NC}"
    echo -e "  sudo apt install openssl    ${GRAY}(fallback, self-signed certs)${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}Setup complete! Your certificates are ready.${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo -e "  1. The vite.config.ts will automatically detect and use these certificates"
echo -e "  2. Update your .env file: http://localhost:5173 → https://localhost:5173"
echo -e "  3. Update Google OAuth authorized origins to include https://localhost:5173"
echo ""
