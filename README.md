Uppgifter i Shortest Route Finder 

-	Läs in städer från filen cities.json. Kod finns i MainViewModel.
-	Konverterar Longitud och Latitud till X och Y koordinater. Detta sker i samma metod i MainViewModel där vi läser in json filen.
-	Skapat en ny View som heter CityListViewControl. Så att man kan visa alla städer, Longitud, och Latitud i listan (som har laddats in från filen cities.json).
-	Skapat en Save knapp för att spara listan till en json fil i Cities. Man kan även lägga till en ny stad med Longitud och Latitud koordinater.
-	Knappen finns i filen CityListViewControl och har ett klickevent i koden bakom. Koden för att spara finns i  CityViewModel.
-	Skapat en Delete knapp i Cities. Knappen finns i ListViewControl och har ett klickevent bakom. Lade till det för att det skulle bli enkelt att ta bort en stad ur listan.
-	Validering av Longitud och Latitud när städer sparas till filen. Koden finns i CityViewModel.
-	Ändrat bakgrundsfärg i Canvas i GrapViewControl till grönt för att se vart Canvas slutar.
-	Justerat GraphViewModel så att noderna visas lite snyggare med stadsnamnet under och justerat storleken på canvas från 800x600 till 433x842 så det ser bättre ut. 
- Lagt till två filer i Model mappen RectangelCoordinates.cs och CitiesRoot.cs där jag skapat nya klasser för att använda Rectangel Coordinates i cities.json för att definera Sveriges gränser.
- Uppdaterat spara funktionen till cities.json filen när en ny stad läggs till med Longitud och Latitud koordninater så att den sparas två gånger både till bin katalogen som det var förut men också till själva cities.json filen i programmet så den finns kvar
  om man stänger ner programmet och startar det igen. Koden finns i ListViewModel.cs.
- Lagt till två Messageboxar som poppar upp om användaren försöker spara en ny stad med felaktiga värden i Cities list. Om man bara anger koordinater och ej något namn på staden så kommer en box upp "Namn på stad får ej vara tomt".
  Samma sak sker om värdet på koordinaterna ligger utanför Sveriges gränser. då får man meddelandet att "Felaktiga koordinater för namnet på staden man angett". Koden för dessa Messageboxar finns i CityViewModel.

Ovanstående är en uppdatering av den här texten efter ändringarna vi skulle göra efter Redovisningen.

Ovanstående gjort av Jonas Jansson Dalmas85. 

Vi har försökt att dela upp allt i projektet efter kunskaper och erfarenhetsnivå så vi båda har kunnat bidra till slutprodukten efter bästa förmåga och på bästa möjliga sätt.
För min del har detta projekt bjudit på en hel del utmaningar mest beroende på allt nytt som tex att jobba med GITHUB och versionshantering men också att få ihop och bidra till helheten på ett bra sätt.

Det positiva är att jag lärt mig mycket nya bregrepp och fått en inblick i vilka fanatastiska möjlighter det finns att skapa applikationer i C sharp. Hur många sätt det finns att lösa olika problem och mångsidigheten i
att återanvända kod och på så vis slippa uppfinna hjulet på nytt. Det märks verkligen att det finns alla möjligheter och att det enda som sätter gränser är fanatsin för en duktig Programmerare som lärt sig bemästra de olika delarna.

Det var lite av mina tankar kring detta.

/Jonas J

----

### Gjort av Joakim 

- Laddar data: Hämtar städer från cities.json och vägar från routes.json. Kopplar ihop vägar med städer och anpassar koordinaterna för canvas.
- Visa rutter och kan ändra: namn på Startstad, Destinationsstad, Sträcka mellan städerna och ett fiktivt pengarvärde att ta sig mellan städerna
- Visualisering: Vägar som linjer mellan städerna. Vägar och cykler markeras med färger från en färglista.
- Grafoperationer:
* * * Har dropdowns för att välja start- och slutstad samt knappar för att hitta kortaste väg, cykler och kortaste cykel.
* * Kortaste väg: Använder en algoritm för att hitta och markera kortaste vägen mellan två valda städer.
* * Cykler: Letar upp och markerar cykler i grafen med olika färger för att skilja dem åt.
* * Kortaste cykel: Söker efter den kortaste cykeln i grafen och markerar den.
* * *  Statusmeddelanden: Visar resultat från olika operationer direkt i gränssnittet.




