# Kuizlet

## 📌 Описание проекта
Kuizlet  - это полнофункциональное веб-приложение, разработанное по мотивам Quizlet и помогающее пользователям создавать карточки, управлять ими и изучать их различными способами. 

---

## ⚙️ Технологический стек
### **Backend**
- **ASP.NET Core 8 (Web API)**: Основной фреймворк для REST API.
- **JWT (JSON Web Token)**: Ручная реализация аутентификации.
- **MongoDB**: NoSQL-база данных для сохранения данных приложения.
- **MongoDB.Driver**: Драйвер для работы с MongoDB.

### **Frontend**
- **HTML/CSS**: Базовая вёрстка и стилизация.
- **JavaScript**: Запросы к бэкенду.

---

## ✨ Функциональные возможности
- **Аутентификация пользователя**: Безопасный вход в систему и регистрация с использованием JWT (веб-токенов JSON).
- **Наборы карточек**: Создание, обновление и удаление наборов карточек с настраиваемыми языками, если нужно, и настройками конфиденциальности.
- **Карточки**: Добавление, редактирование и изучение карточек из набора, генерация тестов.
- **Тесты**: Возможность прохождения тестов, составленных из карточек одного набора.
- **Запросы доступа к карточкам**: Пользователи могут запрашивать доступ к наборам. Владелец подтверждает/отклоняет запросы.
- **UI**: Простой и интуитивно понятный пользовательский интерфейс с переворачивание карточек.

---

## 🏰️ Архитектура проекта 
```
kuizlet
│   .gitignore
│   README.md
|
├───backend                  # Бэкенд-проект
│   ├───Kuizlet.Web          # ASP.NET Web API проект
│   │   ├───Controllers      # Контроллеры API
│   ├───Kuizlet.Application  # Сервисная логика (прикладной слой)
│   │   ├───Services         # Сервисы, реализующие бизнес-логику
│   ├───Kuizlet.Domain       # Доменные сущности (модели данных)
│   │   ├───Entities         # Классы сущностей (например, User, Card)
│   │   ├───Exceptions       # Исключения 
│   └───Kuizlet.Infrastructure # Инфраструктура (работа с хранилищем данных)
│       ├───Repositories     # Репозитории для работы с данными (например, CardRepository)
│
└───frontend                 # Фронтенд
    ├───css                  # Стили для страниц
    ├───js                   # Скрипты для страниц
    └───pages                # HTML страницы приложения

```
Или:

```
  ─Kuizlet.WebUI         # ASP.NET Web API проект
  │   ├───Controllers      # Контроллеры API
  |   ├───wwwroot/           # Статика (HTML/CSS/JS) ← фронтенд  (или другая папка типа ClientApp)
  │         ├───index.html  
  │         ├───css/  
  │         └───js/ 
  ├───Kuizlet.Application  # Сервисная логика (прикладной слой)
  │   ├───Services         # Сервисы, реализующие бизнес-логику
  ├───Kuizlet.Domain       # Доменные сущности (модели данных)
  │   ├───Entities         # Классы сущностей (например, User, Card)
  │   ├───Exceptions       # Исключения 
  └───Kuizlet.Infrastructure # Инфраструктура (работа с хранилищем данных)
      ├───Repositories     # Репозитории для работы с данными (например, CardRepository)  

```
То есть чистая архитектура, возможно с клиент-серверным разделением.

---

## 📊 UML-диаграммы

### 1. Class Diagram 
![Class Diagram](/docs/diagrams/out/kuizlet_class.png)

### 2. Use Case Diagram 
![Use Case Diagram](/docs/diagrams/out/kuizlet_usecase.png)

### 3. Sequence Diagram
#### 3.1 Регистрация
![Sequence Registration](/docs/diagrams/out/sequence_reg.png)

#### 3.2 Авторизация 
![Sequence Login](/docs/diagrams/out/sequence_login.png)

#### 3.3 Создание набора
![Sequence Create Cardset](/docs/diagrams/out/sequence_create_cardset.png)

#### 3.4 Редактирование карточки
![Sequence Edit Card](/docs/diagrams/out/sequence_edit_card.png)

#### 3.5 Получение набора карточек
![Sequence Get Cardset](/docs/diagrams/out/sequence_get_cardset.png)  

#### 3.6 Подтверждение доступа
![Sequence Confirm Access](/docs/diagrams/out/sequence_confirm_cardset.png)

### 4. Activity Diagram
#### 4.1 Запрос доступа
![Activity Access Request](/docs/diagrams/out/activity_access_request.png)

#### 4.2 Создание набора
![Activity Create Cardset](/docs/diagrams/out/activity_create_cardset.png)

#### 4.3 Прохождение теста
![Activity Test](/docs/diagrams/out/activity_test.png)

### 5. State Diagram

#### 5.1 Набор карточек
![State Cardset](/docs/diagrams/out/state_cardset.png)

#### 5.2 Пользователь
![State User](/docs/diagrams/out/state_user.png)

#### 5.3 Запрос доступа
![State Access Request](/docs/diagrams/out/state_access_request.png)
