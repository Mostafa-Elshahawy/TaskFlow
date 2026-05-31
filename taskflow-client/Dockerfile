FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN cp src/environments/environment.docker.ts src/environments/environment.ts
RUN npx ng build --configuration production

FROM nginx:alpine
COPY --from=build /app/dist/taskflow-client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
