# Feature Specification: Ventoux Trip Progress Pagina

**Feature Branch**: `001-trip-progress-webapp`  
**Created**: 2026-03-10  
**Status**: Draft  
**Input**: User description: "Build a dotnet application, deliverable as a docker container, that hosts a single web page with trip progress map, fundraiser context, and trip log, using MariaDB with Entity Framework Core for manually maintained entries. UI language is Dutch (Belgium)."

## Clarifications

### Session 2026-03-10

- Q: Welke projectcontext moet expliciet zichtbaar zijn voor bezoekers? → A: Het project is een liefdadigheidsactie/fondsenwerving voor "Klimmen tegen MS" met streefdoel 500 euro, en backers volgen Tina's voortgang via deze webapplicatie.
- Q: Moet de triplog uit JSON/YAML of uit een database komen? → A: Gebruik een database; entries worden manueel toegevoegd in MariaDB, met Entity Framework Core als data-toegangslaag.
- Q: Moet enkel de contexttekst Nederlands zijn, en is lokale compose-orkestratie nodig? → A: De volledige UI moet Nederlands zijn; er moet een `docker-compose.yml` aanwezig zijn met alle vereiste services en een persistente MariaDB-volume.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Voortgang op de kaart bekijken (Priority: P1)

Als bezoeker wil ik op een kaart de volledige route van Wachtebeke naar de Mont Ventoux zien,
met duidelijk gemarkeerde afgelegde afstand, zodat ik direct de actuele voortgang van de tocht begrijp.

**Why this priority**: Dit is de kernwaarde van het project: publieke, visuele voortgang van de tocht.

**Independent Test**: Kan volledig getest worden door een bekende logbron met kilometers in te laden,
de pagina te openen, en te verifiëren dat de volledige route zichtbaar is en het afgelegde segment overeenkomt met de cumulatieve afstand.

**Acceptance Scenarios**:

1. **Given** een geldige route en loggegevens met afgelegde kilometers, **When** de bezoeker de pagina opent, **Then** ziet de bezoeker de volledige route en een gemarkeerd segment dat de afgelegde afstand voorstelt.
2. **Given** loggegevens met een cumulatieve afstand van 0 km, **When** de pagina laadt, **Then** blijft de volledige route zichtbaar en staat het voortgangssegment op 0%.
3. **Given** loggegevens met cumulatieve afstand gelijk aan of groter dan de routeafstand, **When** de pagina laadt, **Then** toont de kaart 100% afgelegd en markeert de toepassing de tocht als voltooid.

---

### User Story 2 - Context van het initiatief lezen (Priority: P2)

Als bezoeker wil ik een korte Nederlandstalige uitleg lezen over het project en de link met
"Klimmen tegen MS", inclusief het fondsenwervingsdoel van 500 euro, zodat ik het doel van de tocht begrijp.

**Why this priority**: Context verhoogt betrokkenheid en verklaart waarom de voortgang publiek wordt gedeeld.

**Independent Test**: Kan afzonderlijk getest worden door de pagina te laden zonder kaartinteractie en
te controleren dat de contexttekst in het Nederlands (Belgie) wordt getoond.

**Acceptance Scenarios**:

1. **Given** de pagina wordt geopend, **When** de contextsectie zichtbaar wordt, **Then** bevat de tekst een duidelijke verwijzing naar "Klimmen tegen MS" als liefdadigheidsactie in het Nederlands (Belgie) met streefdoel van 500 euro.
2. **Given** een mobiele of desktopweergave, **When** de gebruiker door de pagina navigeert, **Then** blijft de contexttekst leesbaar en correct gepositioneerd tussen kaart en log.
3. **Given** de gebruiker navigeert door menu, kaart, statuslabels en log, **When** de pagina volledig geladen is, **Then** zijn alle zichtbare UI-teksten in het Nederlands (Belgie).

---

### User Story 4 - Als backer voortgang volgen (Priority: P2)

Als backer wil ik Tina's voortgang op de webpagina kunnen volgen,
zodat ik de evolutie van de fondsenwervingsactie in relatie tot de tocht kan meevolgen.

**Why this priority**: Backers zijn een primaire doelgroep van de publieke pagina en bepalen de zichtbaarheid van de actie.

**Independent Test**: Kan afzonderlijk getest worden door de pagina als anonieme bezoeker te openen en
te verifiëren dat de contexttekst backers aanspreekt en dat voortgangsinformatie zonder login beschikbaar is.

**Acceptance Scenarios**:

1. **Given** een backer bezoekt de website, **When** de pagina geladen is, **Then** kan de backer zonder login de actuele routevoortgang en het logboek van Tina bekijken.
2. **Given** de contextsectie wordt gelezen, **When** de backer de doelstelling bekijkt, **Then** is duidelijk dat het om een liefdadigheidsactie gaat met een doelbedrag van 500 euro.

---

### User Story 3 - Logboek van activiteiten raadplegen (Priority: P3)

Als bezoeker wil ik een logboek zien met datum, kilometers en activiteit (lopen, fietsen, wandelen),
zodat ik de afgelegde etappes in detail kan volgen.

**Why this priority**: Het logboek onderbouwt de kaartvoortgang met transparante, controleerbare details.

**Independent Test**: Kan afzonderlijk getest worden door records manueel in de database aan te passen en
te valideren dat de lijst en voortgangsberekening automatisch de bijgewerkte waarden gebruiken.

**Acceptance Scenarios**:

1. **Given** geldige logrecords in MariaDB, **When** de pagina laadt, **Then** toont het logboek per regel de datum, afstand in kilometers en activiteit.
2. **Given** een nieuwe regel wordt manueel toegevoegd in MariaDB, **When** de pagina opnieuw geladen wordt, **Then** verschijnt de extra logregel en wordt de voortgang op de kaart opnieuw berekend.
3. **Given** een regel met ongeldige of ontbrekende velden, **When** de pagina laadt, **Then** wordt de fout afgehandeld zonder volledige pagina-uitval en krijgt de bezoeker een duidelijke melding dat loggegevens onvolledig zijn.

### Edge Cases

- Wat gebeurt er wanneer de logtabel leeg is: de toepassing toont 0% voortgang, een lege loglijst en behoudt de volledige routeweergave.
- Hoe gaat het systeem om met negatieve kilometers: ongeldige regels worden geweigerd of genegeerd en niet meegeteld in de voortgang.
- Hoe gaat het systeem om met cumulatieve kilometers boven de totale routeafstand: voortgang wordt begrensd op 100%.
- Wat gebeurt er wanneer MariaDB tijdelijk niet bereikbaar is: de toepassing toont een duidelijke foutmelding en blijft degradeerbaar beschikbaar voor statische context.
- Wat gebeurt er wanneer een UI-fragment onbedoeld niet-Nederlandse tekst bevat: de build/test pipeline markeert dit als afwijking.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: De toepassing MUST als webapplicatie in een Docker-container uitrolbaar zijn.
- **FR-002**: De toepassing MUST een enkele publieke webpagina tonen met exact drie hoofdsecties: kaart, contexttekst en logboek.
- **FR-003**: De kaartsectie MUST standaard de volledige route tussen Wachtebeke en Mont Ventoux tonen.
- **FR-004**: De kaartsectie MUST het afgelegde deel van de route visueel onderscheiden van het nog niet afgelegde deel.
- **FR-005**: Het systeem MUST de afgelegde afstand bepalen op basis van de som van geldige logregels uit de gegevensbron.
- **FR-006**: Het systeem MUST een voortgangsindicator in procent tonen op basis van afgelegde afstand versus totale routeafstand.
- **FR-007**: De contextsectie MUST statische Nederlandstalige tekst (Belgie) bevatten die expliciet verwijst naar "Klimmen tegen MS".
- **FR-007a**: De contextsectie MUST expliciet vermelden dat het project een liefdadigheidsactie/fondsenwerving is.
- **FR-007b**: De contextsectie MUST het streefdoel van 500 euro expliciet tonen.
- **FR-008**: Het logboek MUST per regel datum, kilometers en activiteit tonen.
- **FR-009**: Toegestane activiteiten MUST minimaal lopen, fietsen en wandelen ondersteunen.
- **FR-010**: Het systeem MUST loggegevens lezen uit een MariaDB-database.
- **FR-011**: De data-toegang MUST geïmplementeerd worden met Entity Framework Core.
- **FR-012**: Bij ongeldige, onvolledige of onleesbare loggegevens uit de database MUST de toepassing een begrijpelijke fouttoestand tonen zonder de volledige pagina ontoegankelijk te maken.
- **FR-013**: Wijzigingen in de database MUST zichtbaar worden na herladen van de pagina, inclusief aangepaste kaartvoortgang en percentage.
- **FR-014**: De logregels MUST in een consistente chronologische volgorde worden weergegeven.
- **FR-015**: De applicatiebroncode en projectopbouw MUST binnen de bestaande submap `src` van de repository vallen.
- **FR-016**: De webpagina MUST gericht zijn op publieke raadpleging door backers die Tina's voortgang willen volgen zonder authenticatie.
- **FR-017**: Alle zichtbare UI-teksten (navigatie, sectietitels, labels, knoppen, statusmeldingen en foutmeldingen) MUST in het Nederlands (Belgie) zijn.
- **FR-018**: Er MUST een `docker-compose.yml` aanwezig zijn voor lokale uitvoering met minimaal webapp- en MariaDB-service.
- **FR-019**: De compose-configuratie MUST een persistente volume voor MariaDB-datastatus definiëren.

### Constitution Alignment *(mandatory)*

- **CA-001 Functional Core Boundary**: Berekening van cumulatieve kilometers, voortgangspercentage,
  routebegrenzing en validatie van logregels worden als pure domeinfuncties gespecificeerd; bestandstoegang,
  kaartweergave en HTTP-afhandeling zijn side-effect adapters.
- **CA-002 Canonical Progress Model**: Er is een enkele canonieke route Wachtebeke -> Mont Ventoux met
  vaste totale afstand; alle kaart- en logweergaven gebruiken dezelfde gevalideerde dataset; voortgang is
  monotone stijgend behalve expliciete correctieregels.
- **CA-003 Test-First Evidence**: Voor elke user story worden eerst falende tests voorzien voor
  routevoortgangsberekening, databasevalidatie en UI-weergave van vereiste secties.
- **CA-004 Container Delivery Impact**: De feature vereist containeruitvoering als standaard distributievorm,
  met configureerbare MariaDB-connectie via omgevingsconfiguratie en lokale compose-orkestratie.
- **CA-005 Standards and Transparency**: De feature gebruikt actuele stabiele runtime- en afhankelijkheidsversies,
  en documenteert het publieke gegevensschema voor logregels en voortgangsuitvoer.

### Key Entities *(include if feature involves data)*

- **TripRoute**: Canonieke trajectdefinitie tussen startpunt en eindpunt, inclusief totale afstand en routegeometrie.
- **TripLogEntry**: Een logregel met datum, afgelegde kilometers en activiteitstype.
- **TripProgressSnapshot**: Afgeleide projectie van totale afgelegde afstand, resterende afstand en voortgangspercentage.
- **ProjectContext**: Statische tekstinhoud die het doel van het initiatief en de relatie met "Klimmen tegen MS" beschrijft.
- **FundraisingGoal**: Domeinwaarde met doelbedrag (500 euro) en beschrijving van de liefdadigheidscontext.

## Assumptions

- De routeafstand is vooraf bepaald en beheerd als vaste referentiewaarde voor alle voortgangsberekeningen.
- De beheerder voegt records manueel toe of wijzigt records manueel in MariaDB voor de actieve omgeving.
- De publieke pagina vereist geen gebruikersaanmelding.
- Deze feature omvat geen online donatieverwerking; ze communiceert enkel de fondsenwervingscontext en doelstelling.
- Activiteiten kunnen intern als gestandaardiseerde waarden worden opgeslagen, maar worden in de UI in het Nederlands getoond.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 95% van paginaweergaven toont binnen 2 seconden de drie hoofdsecties volledig.
- **SC-002**: In 100% van testgevallen met geldige logdata komt het getoonde voortgangspercentage overeen met de verwachte cumulatieve kilometerberekening.
- **SC-003**: In gebruikerstests kan minstens 90% van de deelnemers binnen 10 seconden zowel de huidige voortgang als de meest recente logactiviteit identificeren.
- **SC-004**: Na het manueel toevoegen van een nieuwe geldige logregel in MariaDB en herladen van de pagina is de nieuwe regel in 100% van de gevallen zichtbaar en verwerkt in kaartvoortgang en percentage.
- **SC-005**: In 100% van de gecontroleerde paginaweergaven is de vermelding van liefdadigheidsdoel en 500 euro streefbedrag zichtbaar in de contextsectie.
- **SC-006**: In 100% van de gecontroleerde paginaweergaven zijn alle zichtbare UI-teksten Nederlandstalig (Belgie).
- **SC-007**: Een lokale `docker compose up` start webapp en MariaDB succesvol, en MariaDB-data blijft behouden na herstart.