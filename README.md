Uppgifter i Shortest Route Finder.

-	Läs in städer från filen cities.json. Kod finns i MainViewModel
-	Räkna om X och Y så att de passar in i canvas storlek 800 x 600. Eftersom vi inte kunde få det att fungera med alla svenska städer med originalkoden i Converters\MapTransformers. Koden finns i MainViewModel i metoden som läser in filen LoadDataFromFile.
-	Skapat en ny View som heter CityListViewControl. Så att man kan visa alla städer, X, Y koordinater i listan (som har laddats in från filen cities.json).
-	Skapat en knapp för att spara listan till en json fil. Man kan även lägga till en ny stad med X och Y koordinater. Knappen finns i filen CityListViewControl och har ett klickevent i koden bakom. Koden för att spara finns i CityViewModel.
-	Validering av X och Y när städer sparas till filen.
-	När X och Y konverteras till / från longitud och latitud blir det ett avrundningsfel som gör att koordinaterna blir lite annorlunda när de sparas tillbaka till filen. Försökt minska felet med Math.Round i MainViewModel.
-	Ändrat bakgrundsfärg i Canvas i GrapViewControl.
-	Justerat GraphViewModel så att noderna visas lite snyggare med stadsnamnet under och justerat storleken på canvas så det ser bättre ut. Tagit bort lite oanvänd kod i MainViewModel.

Ovanstående gjort av Jonas Jansson Dalmas85. 

Vi har försökt att dela upp allt i projektet efter kunskaper och erfarenhetsnivå så vi båda har kunnat bidra till slutprodukten efter bästa förmåga och på bästa möjliga sätt.
För min del har detta projekt bjudit på en hel del utmaningar mest beroende på allt nytt som tex att jobba med GITHUB och versionshantering men också att få ihop och bidra till helheten på ett bra sätt.

Det positiva är att jag lärt mig mycket nya bregrepp och fått en inblick i vilka fanatastiska möjlighter det finns att skapa applikationer i C sharp. Hur många sätt det finns att lösa olika problem och mångsidigheten i
att återanvända kod och på så vis slippa uppfinna hjulet på nytt. Det märks verkligen att det finns alla möjligheter och att det enda som sätter gränser är fanatsin för en duktig Programmerare som lärt sig bemästra de olika delarna.

Det var lite av mina tankar kring detta.

/Jonas J
