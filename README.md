# AlterBank
## Тестовый план (описание пунктов тестирования и покрытие требований)

| Требование             | Что тестируется            |   Результат                       |
|:-----------------------|:---------------------------|:----------------------------------|
|Получить список счетов  |Получение пустого списка    |200, в теле ответа пустой список                                   |
|                        |Список из 10000 элементов   |200, в теле ответа список из 10000 элементов счет и баланс                                    |
|Получить баланс по счету|Баланс по счету             |200, в теле ответа сумма баланса   |
|                        |Баланс по несуществующему счету|404, в теле ответа сообщение "Account <счет из параметров запроса> not found"|
|                        |Пустой счет в параметрах запроса|400, Bad request|
|Создание счета|Создание счета с балансом >= 0| 201, в теле ответа номер счета и баланс|
|                   |Создание счета с балансом < 0| 400 Bad request, в теле ответа "Balance must be equal or great zero"|
|                   |Нагрузочное тестирование. создание 10000 счетов в 200 конкурирующих пользователей. Время тестирования 5 минут|Средний throughput при постоянной загрузке >=100. Корректность определяется как сумма баланса по всем счетам равной сумме арифметической прогрессии 1 000, 2 000,...,10 000 000|
|Перевод со счета на счет|Перевод при достаоточном балансе|200, в теле ответа номера счетов с актуальным балансом после выполения операции и признак успешности true|
|                        |Перевод при недостаточном балансе|200, в теле ответа номера счетов с актуальным балансом и признак успешности false|
|                        |Перевод с\на несуществующий счет|400, сообщение "One of the accounts not exist"|
|                        |Перевод с одного счета на тотже самый| 200, в теле ответа номера счетов с актуальным балансом и признак успешности false. Баланс не изменился|
|                        |Нагрузочное тестирование. Перевод с\на одной и той же пары счетов 1, при исходной сумме баланса 1000. при 500 конкурирующих пользователях в течении 60 минут. Т.е. в каждый момент, времени каждый поток делает перевод 1 на ту же самую пару счетов в одном направлении, что и соседний поток|Вся сумма переведена с одного счета на другой. % ошибок при получении ответа 503 о том что сервис не доступен 74-50%. В ошибке 503 текст "One of the accounts you are being update is modified by another transaction" Средний throughput при постоянной загрузке >=100. Ответы 200, 503. После обнуления счета источника только 200. Проверка корректности по сумме по завершении теста.|
|                        |Нагрузочное тестирование на случайно выбранную пару счетов из 10000 на протяжении 60 минут. На случайно выбранную сумму из диапазона от 0 до 1000 включительно. 500 пользователей.|Ответы 200 с признаком успешности\не успешности. 503 с текстом "One of the accounts you are being update is modified by another transaction". Сумма на всех счетах после завершения теста равна сумме арифметической прогрессии 1 000, 2 000,...,10 000 000| 





## Тестовая среда
http://84.201.175.35:5000/swagger

!!!Приложени развернуто на **прерываемой** облачной машине из за стоимости (Яндекс выдал грант только на такой тип). Облако гарантирует недетерменированный shutdown машины 1 раз в сутки на короткий интервал. Поднимать приходится вручную :( хоть и вопрос пары минут. Прошу связатся со мной в случае недоступности, перезапущу машину и обновлю адрес в доке, т.к. он может измениться после перезапуска!!!

VM: vCPU 8, 32Gb, SSD, Ubuntu 18.04 LTS, docker 19.3, сompose 1.26.2
Для БД включен уровень чтения SNAPSHOT и версионирование строк.

В БД 10 000 счетов в формате 0000000001, 0000000002
Баланс на счете число(номер счета) * 1000

Доступ к БД можно получать под пользователем sa напрямую порт 5433. Пароль в [docker-compose.override.yml](src/docker-compose.override.yml)

## Допущения
* Так как это тестовое задание и есть ограничение по свободному времени на задачу  поэтому юнит тесты не написал, но есть тесты покрывающие требования.
* Нет скриптов автоматического развертывания т.к. писалось на 1 раз
* Некоторые моменты не обсуждались с заказчиком, опять же ввиду характера задания и лимита по времени
* Не реализована модель безопасности
* Не реализован healthcheck сервиса
* В качестве первичного ключа использован натуральный - номер счета
* Логирование добавлено просто чтобы показать что вот оно есть

## Описание приложения
* Стек: .Net Core 3.1, C#, MS SQL Server 2019, docker, compose
## Описание репозитория
## Ответы на доп вопросы

## Как развернуть приложение
* Установить docker
* Установить compose
* Запустить docker up с файлами из каталога <каталог ссылка>
* Подключиться к SQL серверу по порту 5433 под пользователем sa
* Запустить по очереди скрипты <каталог ссылка> (будет создана БД, таблица, хранимая процедура)
* Проверить готовность приложения http://\<host\>:5000/swagger
 
 ### Заполнение таблицы тестовыми данными (опционально)
 * Запустить скрипт <ссылка> либо воспользоваться существующим сsv файлом <>
 * установитьjMeter
 * Запустить для нагрузочного тетсирования <тест>
## А как было бы на реальной задаче

