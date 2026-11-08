#!/bin/bash
# Запуск версии на Avalonia. Старую версию на GTK запускает run_legacy.sh.
cd "$(dirname "$0")"
dotnet run --project GreatCompany "$@"
