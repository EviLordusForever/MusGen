# Pick a specific version of the .NET runtime as the base image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base

# Create a non-privileged user that the app will run under
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser

WORKDIR .

# Copy the executable to the container
COPY bin/Debug/net6.0-windows/ /app/

# Specify the command to run when the container is started
ENTRYPOINT [ "/app/MusGen.exe" ]