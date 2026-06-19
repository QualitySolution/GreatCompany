#!/bin/bash
set -e

echo "Что делаем?"
echo "1) Save schema of great_company database from Atlas"
read case

cd "$(dirname "$0")"

case $case in
	*1*)
mkdir -p ./Base
read -p "User [root]: " db_user
db_user=${db_user:-root}
mariadb-dump --ssl=OFF -hatlas.srv.qsolution.ru -u"${db_user}" -p --no-data --skip-dump-date great_company | sed 's/ AUTO_INCREMENT=[0-9]*//g' > ./Base/great_company.sql
;;
esac

read -p "Press enter to exit"
