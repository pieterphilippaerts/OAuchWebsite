FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /OAuch

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o docker-build
# Create HTTPS certificates
RUN dotnet dev-certs https

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /OAuch
COPY --from=build-env /OAuch/docker-build .
# Copy the HTTPS certificates from the build environment
COPY --from=build-env /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
# Expose port 443 to the outside
EXPOSE 443
# Start OAuch
ENV DOTNET_ENVIRONMENT Docker
ENTRYPOINT ["dotnet", "OAuch.dll"]
