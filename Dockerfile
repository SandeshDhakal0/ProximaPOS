FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . ./ 
WORKDIR /app/TheHighInnovation.POS.WEB
RUN dotnet restore
RUN dotnet publish -c Release -o /out /p:UseAppHost=false 
FROM nginx:latest 
WORKDIR /usr/share/nginx/html
COPY --from=build-env /out/wwwroot . 
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80