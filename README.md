# 🍽️ Rezeptverwaltungs-Applikation

## Überblick

Dieses Projekt ist eine webbasierte Applikation zur Verwaltung und Erfassung von Kochrezepten aus verschiedenen Küchen weltweit.

Benutzer können Rezepte erstellen, kategorisieren und verwalten. Die Anwendung basiert auf einer strukturierten Datenbank mit Rezepten, Zutaten, Einheiten, Kategorien und Zubereitungsschritten.

Ziel des Projekts war es, eine saubere, wartbare Full-Stack-Applikation unter Verwendung moderner ASP.NET Core Technologien zu entwickeln.

---

## Motivation

Ich habe dieses Projekt umgesetzt, um meine Kenntnisse in der strukturierten Webentwicklung mit **ASP.NET Core MVC** und **Entity Framework Core** zu vertiefen.

Besonders wichtig war mir dabei:

- die saubere Umsetzung des MVC-Architekturprinzips
- eine klare Trennung von Verantwortlichkeiten
- ein durchdachtes Datenbankdesign
- die Integration von AJAX, um eine dynamische Benutzererfahrung ohne vollständige Seitenneuladung zu ermöglichen

Dieses Projekt hat mir geholfen, Theorie aus der Ausbildung praxisnah umzusetzen und meine Fähigkeiten im Backend-Design weiterzuentwickeln.

---

## Technologien / Tech Stack

### Backend

- C#
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- Dependency Injection

### Frontend

- HTML
- CSS
- JavaScript
- AJAX

### Datenbank / ORM

- Entity Framework Core
- `DbContext` & `DbSet<T>`
- Relationales Datenmodell

---

## Architektur / Technischer Ansatz

Die Anwendung basiert auf dem **Model-View-Controller (MVC)**-Architekturprinzip.

### Struktur

#### Models
Repräsentieren Rezepte, Zutaten, Einheiten, Kategorien sowie deren Beziehungen.

#### Controller
Verarbeiten HTTP-Anfragen und koordinieren die Interaktion zwischen Datenmodell und Benutzeroberfläche.

#### Views
Dynamische Razor-Views zur Darstellung der Daten.

#### Datenzugriff
Umsetzung mittels Entity Framework Core über `DbContext`.

---

### Technische Konzepte

- MVC-Pattern in ASP.NET Core
- Entity Framework Core als ORM
- Attributbasierte Konfiguration (`[ResponseCache]`)
- .NET 8 Projektstruktur
- Asynchrone Benutzerinteraktionen mittels AJAX

---

## Meine Rolle

In diesem Projekt übernahm ich die Rolle eines Full-Stack-Entwicklers und war verantwortlich für:

- Konzeption und Modellierung der Datenbankstruktur
- Implementierung der Controller und Geschäftslogik
- Erstellung dynamischer Razor-Views
- Integration von AJAX zur Optimierung der Benutzerinteraktion
- Strukturierung des Projekts gemäss MVC-Prinzipien

---

## Wichtige Funktionen

- Erstellen und Bearbeiten von Rezepten
- Verwaltung von Zutaten und Einheiten
- Kategorisierung von Rezepten
- Dynamische Aktualisierung von Inhalten mittels AJAX
- Klare Trennung zwischen Präsentations- und Logikschicht

---

## Was ich gelernt habe

Durch dieses Projekt konnte ich mein Verständnis für folgende Themen vertiefen:

- Praktische Anwendung der MVC-Architektur
- Relationale Datenmodellierung mit Entity Framework Core
- Umgang mit Entitätsbeziehungen
- Asynchrone Kommunikation zwischen Frontend und Backend
- Strukturierte und wartbare Backend-Entwicklung

Das Projekt hat mir gezeigt, wie wichtig saubere Architektur und klare Code-Strukturen für langfristig wartbare Anwendungen sind.
