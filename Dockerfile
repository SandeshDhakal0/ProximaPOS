FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app

COPY TheHighInnovation.POS.Web.sln ./
COPY ./TheHighInnovation.POS.Web/TheHighInnovation.POS.csproj ./TheHighInnovation.POS.Web/

RUN dotnet restore
COPY ../
RUN dotnet publish -c Release -o out

FROM nginx:1.23.0-alpine
WORKDIR /app
EXPOSE 80
COPY nginx.conf /etc/nginx/nginx.conf 
COPY --from=build /app/out/wwwroot /usr/share/nginx/html


