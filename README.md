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
|                        |Нагрузочное тестирование. Перевод с\на одной и той же пары счетов 1, при исходной сумме баланса 1000. при 500 конкурирующих пользователях в течении 60 минут. Т.е. в каждый момент, времени каждый поток делает перевод 1 на ту же самую пару счетов в одном направлении, что и соседний поток|Вся сумма переведена с одного счета на другой. % ошибок при получении ответа 503 о том что сервис не доступен 82-50%. В ошибке 503 текст "One of the accounts you are being update is modified by another transaction" Средний throughput при постоянной загрузке >=100. Ответы 200, 503. После обнуления счета источника только 200. Проверка корректности по сумме по завершении теста.|
|                        |Нагрузочное тестирование. Перевод с\на случайно выбранную пару счетов из 10000 на протяжении 60 минут. На случайно выбранную сумму из диапазона от 0 до 1000 включительно. 500 пользователей.|Ответы 200 с признаком успешности\не успешности. 503 с текстом "One of the accounts you are being update is modified by another transaction". Сумма на всех счетах после завершения теста равна сумме арифметической прогрессии 1 000, 2 000,...,10 000 000|
|                        |Нагрузочное тестирование. Перевод на одну и ту де пару счетов в одном направлении 2 пользователями на протяжении 30 минут. При исходном балансе 1000. на протяжении 60 минут|200 с признаком успешности\неуспешности перевода. 503 с текстом "One of the accounts you are being update is modified by another transaction", количество ошибок 62-50% (количество ответов 503). Корректность - сумма равна сумме на счетах перед началом теста.|
|Прочее                   |Корректность формата запроса|400, Bad request| 


Для тестирования использовался jMeter c Constant Throughput Timer со значением 6000.0 параметра Target throughput




## Тестовая среда
http://84.201.132.28:5000/swagger

!!!Приложени развернуто на **прерываемой** облачной машине из за стоимости (Яндекс выдал грант только на такой тип). Облако гарантирует недетерменированный shutdown машины 1 раз в сутки на короткий интервал. Поднимать приходится вручную :( хоть и вопрос пары минут. Прошу связатся со мной в случае недоступности, перезапущу машину и обновлю адрес в доке, т.к. он может измениться после перезапуска!!!

VM: vCPU 8, 32Gb, SSD, Ubuntu 18.04 LTS, docker 19.3, сompose 1.26.2
Для БД включен уровень чтения SNAPSHOT и версионирование строк для уменьшения кол-ва блокировок.

В БД 10 000 счетов в формате 0000000001, 0000000002
Баланс на счете число(номер счета) * 1000

Доступ к SQL Servdr можно получать под админом (пользователь sa) напрямую порт 5433. Пароль в [docker-compose.override.yml](src/docker-compose.override.yml)

## Допущения ограничения
* Так как это тестовое задание и есть ограничение по свободному времени на задачу  поэтому юнит тесты не написал, но есть тесты покрывающие требования.
* Нет скриптов автоматического развертывания т.к. писалось на 1 раз
* Некоторые моменты не обсуждались с заказчиком, опять же ввиду характера задания и лимита по времени
* Не реализована модель безопасности
* Не реализован healthcheck сервиса
* Не реализована пагинация в методе list
* В качестве первичного ключа использован натуральный - номер счета
* Логирование добавлено в несколько место, просто чтобы показать что вот оно есть

## Описание приложения
* Стек: .Net Core 3.1, C#, MS SQL Server 2019, docker, compose
  
  Реализовано с применением CQRS паттерна.
  
  Все что меняет состояние счета - Команда, рализовано в классах [*Сommand](src/AlterBankApi/Application/Commands)
  
  Все что не меняет состояние - Запрос, реализовано в классах [*Request](src/AlterBankApi/Application/Requests)

  Логика реализована в обработчиках:
  * [Обработчик команд](src/AlterBankApi/Application/Handlers/AccountCommandHandler.cs)
  * [Обработчик запросов](src/AlterBankApi/Application/Handlers/AccountRequestHandler.cs)


## Описание репозитория
[Собственно код](src/AlterBankApi)

* [Бизнес логика](src/AlterBankApi/Application)
* [Работа с БД](src/AlterBankApi/Infrastructure/Repositories)
* [Контроллеры](src/AlterBankApi/Controllers)

[Скрипты по созданию базы](src/SQL%20Scripts)

[Тесты и заведение аккаунтов](ыкс/LoadTests)

## Ответы на доп вопросы
Как выдержать бОльший RPS?
* Вариант 1 (технический)
  
  Добавить еще один инстанс приложения + балансировщик.
  Отрефакторить и сократить кол-во аллокаций. Отказаться от рефлексии в контейнере и медиаторе

* Вариант 2 (логический)
    
    Основная проблема текущей реализации в том, что после перевода мы сразу ожидаем ответ от сервиса с актуальным балансом, что приводит к блокировке как минимум пары строк в БД. Значит цель - сократить кол-во блокировок в БД уменьшением кол-ва запросов. Например отправлять агригированные запросы на перевод. Например пусть есть 2 транзакции (X->Y, 10), (Y->X, 9) -> (X-Y, 1) получим свернутую в итоге. Для реализации необходимо в пайплайн обработки запросов на перевод добавить некий редьюсер, который будет агрегировать суммы. Принимать решение о том когда надо отправлять запрос в БД необходимо при достижении порога количества агрегируемых запросов, либо срабатывания таймера. При таком подходе можно обеспечить бОльшее кол-во успешно сделанных переводов. Но данное решение не расширяемое и приведет все равно к лимиту.

* Вариант 3 (корректировка требований)

    Сделать операцию перевода асинхронной. Это позволит поставить запросы в очередь и даже упарвлять ими. Т.е. Тут классическое решение классической проблемы Reducer\Consumer. Сервис перевода принимает перевод и складывает его в очередь, сервис обработки платежей достает из очереди и обрабатывает так быстро как сможет.

## Как развернуть приложение
* Установить docker
* Установить compose
* Запустить docker-compose up с файлами docker-compose.yml, docker-compose.override.yml [отсюда](src)
* Подключиться к SQL серверу по порту [5433](src/docker-compose.override.yml) под пользователем sa (пароль [здесь](src/docker-compose.override.yml))
* Запустить по очереди [скрипты](src/SQL%20Scripts) (будет создана БД, таблица, хранимая процедура)
* Проверить готовность приложения http://\<host\>:[5000](src/docker-compose.override.yml)/swagger
 
 ### Заполнение таблицы тестовыми данными (опционально)
 * Запустить [скрипт](src/LoadTests/SeedData/accounts.py) либо воспользоваться существующим сsv [файлом](src/LoadTests/SeedData/accounts_balance.csv)
 * установитьjMeter
 * Запустить для нагрузочного тетсирования [тест](https://github.com/vzorgv/AlterBank/tree/master/src/LoadTests)

## А как было бы на реальной задаче
Примерно также но более a формальное применение ATDD
* Постановка требований
* Определение Defenition of ready, Defenition of done
* Написание тест плана и согласование его с заказчиком
* Написание кода и тест кейсов
* Валидация заказчиком по тест плану
* Развертывание и поддержка

