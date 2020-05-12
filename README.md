# Multi-Currency E-Pocket
## Мультивалютный электронный кошелек

Тестовый проект реализации электронного кошелька, поддерживающего несколько счетов в разных валютах. 
Предполагается, что разрешен только один счет в одной валюте, т.е. не разрешается иметь несколько разных счетов в одной валюте.

Реализует следующие действия:

- a. Пополнить кошелек в одной из валют; 
- b. Снять деньги в одной из валют; 
- c. Перевести деньги из одной валюты в другую; 
- d. Получить состояние своего кошелька (сумму денег в каждой из валют).

Без авторитизации и аутентификации.

### Модель данных 

Базовая сущность - **PocketHolder**.

Сущность содержит описания профиля пользователя (имя, пол, дата рожения).
А также поля для мастер-счета - MasterAccount - уникальные 6 цифр, и PinCode - 4 цифры (валидируются regexp). Несмотря на то, что аутнетификаци не производится, в запросе проверяется как номер аккаунта, так и pincode,
а так же что номер аккаунта из запроса совпадает с переданным в теле запроса .
В качестве Id используется int identity, но поиск аккаунтов осуществляется по уникальному AccountNumber.

В реальном проекте такую сущность лучше декомпозировать на Profile и PocketAccount.

Валютный счет - **CurrencyAccount**

Id - int identity;
Number - номер аккаунта 6 цифр;
Debit - средства на счете; 
Currency - трехбуквенный код валюты, используется как FK к Currency.

Справочник валют - **Currency** 

Отключен на текущий момент, т.к. не полностью инициализирован списком валют.
В качестве ключа выбран трехбуквенный код валюты.
Нужно полностью инициализировать и включить привязку CurrencyAccount, для валидации FK к Currency.

### Api

-  ConvertCurrency - используется для перевода средств с счета одной валюты на счет другой;
 Для идентификации счета снятия и счета назначения используется код валюты.
 Для курса конвертации используется сервис https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml, 
 данные курса кэшируются на день (настраивается ссылка и время кэширования в секундах в файле конфигурации).
 Обновляет средства на каждом из счетов в качестве ответа возвращает либо статус 201 либо badRequest c описанием ошибки.
 
 - DepositAccount [POST] - пополнения кошелька по мастер счету, пинкоду, коду валюты. Возвращает либо сумму на валютном счету пополнения, либо BadRequest с ошибкой.
 - DepositCurrencyAccount [POST] - пополняет кошелек по номеру валютного счета, пинкоду, сумме. Возвращает сумму после пополнения, либо BadRequest c ошибкой. 
 - WithdrawAccount [POST] - снятие с кошелька по мастер счету, пинкоду, коду валюты. Возвращает либо сумму на валютном счету пополнения, либо BadRequest с ошибкой.
 - WithdrawCurrencyAccount [POST] -  снятие c кошелека по номеру валютного счета, пинкоду, сумме. Возвращает сумму после пополнения. либо BadRequest c ошибкой. 
 - GetPocketStatus [GET] - возвращает список статусов по всем счетам.
 
 Методы DepositAccount/DepositCurrencyAccount и WithdrawAccount/WithdrawCurrencyAccount взаимозаменяемые и, думаю, избыточные, но 
 не выбрал какой из них более предпочтительный.

Все методы в случае необработанной ошибки возвращают 500, подробности надо будет добавить в логирование.


### TODO's:

- Уточнить сигнатуру WebApi контреллера; На текущий момент сигнатура, возможно, больше похожа на SOAP сервис; Отдавать результат в типизированном DTO;
- Добавить юнит тесты на сервисы бизнес логики;
- Добавить логирование;
- Загрузить справочник всех валют из Curency.csv (взят с rbc.) 
