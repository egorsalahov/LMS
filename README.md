https://lms-vlvx.onrender.com/ 
(Подождать 1 минуту после перехода по ссылке для полноценного запуска приложения из-за ограничений бесплатного хостинга)

# Dreams LMS — Learning Management System

Платформа для онлайн-обучения на **ASP.NET Core (.NET 10)** с ролями Admin, Instructor, Student. Курсы, корзина, заказы, чат, админ-панель.

## 🚀 Технологии

- **Backend:** ASP.NET Core (.NET 10), Entity Framework Core, SignalR
- **Frontend:** Razor Views, Bootstrap 5, jQuery, AJAX
- **База данных:** SQL Server (Somee.com)
- **Аутентификация:** ASP.NET Core Identity (3 роли)
- **Логирование:** Serilog (файлы + консоль)
- **Деплой:** Render.com (Docker)

## 📦 Архитектура

```
Sem.Core/              ← Сущности, Enum
Sem.Infrastructure/    ← EF Core, миграции, Middleware
Sem.Web/               ← Контроллеры, View, ViewModel, сервисы, репозитории
```

**Паттерны:** Repository + Service Layer + Dependency Injection

## 👥 Роли

| Роль           | Доступ                                                  |
| -------------- | ------------------------------------------------------- |
| **Admin**      | Админ-панель, аналитика                                 |
| **Instructor** | Создание курсов, просмотр студентов, чат                |
| **Student**    | Покупка курсов, просмотр уроков, корзина, wishlist, чат |

## 🔧 Функционал

- Регистрация / вход / подтверждение Email / сброс пароля
- Каталог курсов с фильтрацией и пагинацией (AJAX)
- Корзина → оформление заказа
- Личный кабинет студента (курсы, wishlist, история заказов)
- Просмотр видео-уроков
- Оценки и отзывы курсов
- Создание курсов инструктором (загрузка видео)
- Чат студент-инструктор (SignalR)
- Админ-панель (аналитика, графики, таблицы данных)
- Логирование действий пользователей (Serilog)
- Кастомные страницы ошибок (404, 403)

## 🗄️ База данных

- **17 таблиц** (Identity, курсы, заказы, сообщения и др.)
- Нормализована до 3NF
- Автоматические миграции при старте

## 🚢 Запуск

### Локально
```bash
dotnet restore
dotnet ef database update
dotnet run
```

### Деплой (Render.com)
1. `dotnet publish -c Release -r linux-x64 -o publish`
2. Загружен на GitHub
3. Подключен к Render (через Docker)

## 👤 Тестовые аккаунты

| Роль       | Email                  | Пароль     |
| ---------- | ---------------------- | ---------- |
| Admin      | admin@example.com      | Admin123!  |
| Instructor | egorsalahov2@gmail.com | fff111FF   |
| Student    | friestr65@mail.ru      | fff111fFFF |

## 📁 Логи

`Logs/log-YYYYMMDD.txt` — действия пользователей и ошибки.

