# Kvasirutt

Kvasirutt är en desktopapplikation utvecklad i C# och WPF (Windows Presentation Foundation). Applikationen visualiserar hur sökalgoritmer söker i en 2D karta för att hitta den kortaste vägen (inte diagonalt) mellan en startpunkt och en målpunkt.

## Skapa kartor

Användaren kan skapa kartor i följande storlekar:

- 20 x 20
- 30 x 30
- 40 x 40

## Välja algoritm

Användaren kan välja algoritmerna: 

- Breadth First Search (BFS)
- Dijkstar

## Hantera startpunkt och målpunkt

Användaren kan placera och flytta:

- Grön ruta (startpunkt)
- Blå ruta (målpunkt)

## Visualisera sökningen

Användaren kan starta en visualisering av sökningen och den hittade vägen. En karta, en startpunkt och en målpunkt måste finnas för att visualiseringen ska kunna starta.

En pågående visualisering avbryts om användaren: 

- Flyttar startpunkten eller målpunktens position.
- Skapar en ny karta.

## Resultat

När användaren startar visualiseringen visas resultatet:

- Algoritm: algoritmen som används.
- Kartstorlek: kartans dimensioner.
- Väglängd: antalet steg från startpunkt till målpunkt.
- Besökta noder: totala antalet besökta noder/rutor under sökningen.
- Körtid: tiden det tar att köra sökningen.

Noderna/rutorna besöks endast engång vardera under sökningen i kartan.

## Felhantering

Applikationen hanterar följande situationer:

### När visualiseringen startas

- Ingen karta har skapats.
- Ingen startpunkt har valts.
- Ingen målpunkt har valts.

### När en Visualisering pågår

Applikationen meddelar användaren att en visualisering redan pågår.

### När användaren skapar en ny karta

Applikationen frågar om användaren vill skapa en ny karta när en karta redan finns.

## Installation och start

- Windows
- Git
- Visual Studio Code och tilläget C# Dev Kit (för vscode användare)
- .NET SDK

## Klona projektet och starta applikationen

1. git clone https://github.com/izab2500/programmering-med-c--projekt.git
2. gå till rotmappen: cd quasi-route 
3. starta applikationen: dotnet run

## Projektrapport

[Läs projektrapporten](docs/report.pdf)

## Videopresentation

[Se videopresentation](https://youtu.be/NKQCKGbmdp8)

