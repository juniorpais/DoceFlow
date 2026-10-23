# Página estática "em construção" servida por nginx.
# Será substituído pelo build multi-stage da API/PWA nas próximas etapas.
FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY index.html /usr/share/nginx/html/index.html
ENV PORT=80
EXPOSE 80
