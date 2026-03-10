# Feature Specification: Ventoux Trip Progress Pagina

**Feature Branch**: `001-trip-progress-webapp`  
**Created**: 2026-03-10  
**Status**: Draft  
**Input**: User description: "Build a dotnet application, deliverable as a docker container, that hosts a single web page. The page consists of three parts: a trip progress map, static project context for Klimmen tegen MS, and a trip log sourced from a manually updated yaml or json file. UI language is Dutch (Belgium)."

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
"Klimmen tegen MS", zodat ik het doel van de tocht begrijp.

**Why this priority**: Context verhoogt betrokkenheid en verklaart waarom de voortgang publiek wordt gedeeld.

**Independent Test**: Kan afzonderlijk getest worden door de pagina te laden zonder kaartinteractie en
te controleren dat de contexttekst in het Nederlands (Belgie) wordt getoond.

**Acceptance Scenarios**:

1. **Given** de pagina wordt geopend, **When** de contextsectie zichtbaar wordt, **Then** bevat de tekst een duidelijke verwijzing naar "Klimmen tegen MS" in het Nederlands (Belgie).
2. **Given** een mobiele of desktopweergave, **When** de gebruiker door de pagina navigeert, **Then** blijft de contexttekst leesbaar en correct gepositioneerd tussen kaart en log.

---

### User Story 3 - Logboek van activiteiten raadplegen (Priority: P3)

Als bezoeker wil ik een logboek zien met datum, kilometers en activiteit (lopen, fietsen, wandelen),
zodat ik de afgelegde etappes in detail kan volgen.

**Why this priority**: Het logboek onderbouwt de kaartvoortgang met transparante, controleerbare details.

**Independent Test**: Kan afzonderlijk getest worden door de gegevensbron handmatig aan te passen en
te valideren dat de lijst en voortgangsberekening automatisch de bijgewerkte waarden gebruiken.

**Acceptance Scenarios**:

1. **Given** een geldige json- of yaml-bron met meerdere regels, **When** de pagina laadt, **Then** toont het logboek per regel de datum, afstand in kilometers en activiteit.
2. **Given** een nieuwe regel wordt toegevoegd in het bronbestand, **When** de pagina opnieuw geladen wordt, **Then** verschijnt de extra logregel en wordt de voortgang op de kaart opnieuw berekend.
3. **Given** een regel met ongeldige of ontbrekende velden, **When** de pagina laadt, **Then** wordt de fout afgehandeld zonder volledige pagina-uitval en krijgt de bezoeker een duidelijke melding dat loggegevens onvolledig zijn.

### Edge Cases

- Wat gebeurt er wanneer het logbestand leeg is: de toepassing toont 0% voortgang, een lege loglijst en behoudt de volledige routeweergave.
- Hoe gaat het systeem om met negatieve kilometers: ongeldige regels worden geweigerd of genegeerd en niet meegeteld in de voortgang.
- Hoe gaat het systeem om met cumulatieve kilometers boven de totale routeafstand: voortgang wordt begrensd op 100%.
- Wat gebeurt er wanneer zowel yaml- als json-bestand aanwezig zijn: het systeem gebruikt een vooraf vastgelegde prioriteit en meldt welke bron actief is.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: De toepassing MUST als webapplicatie in een Docker-container uitrolbaar zijn.
- **FR-002**: De toepassing MUST een enkele publieke webpagina tonen met exact drie hoofdsecties: kaart, contexttekst en logboek.
- **FR-003**: De kaartsectie MUST standaard de volledige route tussen Wachtebeke en Mont Ventoux tonen.
- **FR-004**: De kaartsectie MUST het afgelegde deel van de route visueel onderscheiden van het nog niet afgelegde deel.
- **FR-005**: Het systeem MUST de afgelegde afstand bepalen op basis van de som van geldige logregels uit de gegevensbron.
- **FR-006**: Het systeem MUST een voortgangsindicator in procent tonen op basis van afgelegde afstand versus totale routeafstand.
- **FR-007**: De contextsectie MUST statische Nederlandstalige tekst (Belgie) bevatten die expliciet verwijst naar "Klimmen tegen MS".
- **FR-008**: Het logboek MUST per regel datum, kilometers en activiteit tonen.
- **FR-009**: Toegestane activiteiten MUST minimaal lopen, fietsen en wandelen ondersteunen.
- **FR-010**: Het systeem MUST loggegevens in json of yaml formaat kunnen inlezen uit een handmatig bijgewerkt bestand.
- **FR-011**: Bij ongeldige of onleesbare loggegevens MUST de toepassing een begrijpelijke fouttoestand tonen zonder de volledige pagina ontoegankelijk te maken.
- **FR-012**: Wijzigingen in het gegevensbestand MUST zichtbaar worden na herladen van de pagina, inclusief aangepaste kaartvoortgang en percentage.
- **FR-013**: De logregels MUST in een consistente chronologische volgorde worden weergegeven.
- **FR-014**: De applicatiebroncode en projectopbouw MUST binnen de bestaande submap `src` van de repository vallen.

### Constitution Alignment *(mandatory)*

- **CA-001 Functional Core Boundary**: Berekening van cumulatieve kilometers, voortgangspercentage,
  routebegrenzing en validatie van logregels worden als pure domeinfuncties gespecificeerd; bestandstoegang,
  kaartweergave en HTTP-afhandeling zijn side-effect adapters.
- **CA-002 Canonical Progress Model**: Er is een enkele canonieke route Wachtebeke -> Mont Ventoux met
  vaste totale afstand; alle kaart- en logweergaven gebruiken dezelfde gevalideerde dataset; voortgang is
  monotone stijgend behalve expliciete correctieregels.
- **CA-003 Test-First Evidence**: Voor elke user story worden eerst falende tests voorzien voor
  routevoortgangsberekening, bronbestandvalidatie en UI-weergave van vereiste secties.
- **CA-004 Container Delivery Impact**: De feature vereist containeruitvoering als standaard distributievorm,
  met configureerbaar pad/bron voor loggegevens via omgevingsconfiguratie.
- **CA-005 Standards and Transparency**: De feature gebruikt actuele stabiele runtime- en afhankelijkheidsversies,
  en documenteert het publieke gegevensschema voor logregels en voortgangsuitvoer.

### Key Entities *(include if feature involves data)*

- **TripRoute**: Canonieke trajectdefinitie tussen startpunt en eindpunt, inclusief totale afstand en routegeometrie.
- **TripLogEntry**: Een logregel met datum, afgelegde kilometers en activiteitstype.
- **TripProgressSnapshot**: Afgeleide projectie van totale afgelegde afstand, resterende afstand en voortgangspercentage.
- **ProjectContext**: Statische tekstinhoud die het doel van het initiatief en de relatie met "Klimmen tegen MS" beschrijft.

## Assumptions

- De routeafstand is vooraf bepaald en beheerd als vaste referentiewaarde voor alle voortgangsberekeningen.
- De beheerder werkt telkens slechts een bronbestand bij (json of yaml) voor de actieve omgeving.
- De publieke pagina vereist geen gebruikersaanmelding.
- Activiteiten kunnen intern als gestandaardiseerde waarden worden opgeslagen, maar worden in de UI in het Nederlands getoond.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 95% van paginaweergaven toont binnen 2 seconden de drie hoofdsecties volledig.
- **SC-002**: In 100% van testgevallen met geldige logdata komt het getoonde voortgangspercentage overeen met de verwachte cumulatieve kilometerberekening.
- **SC-003**: In gebruikerstests kan minstens 90% van de deelnemers binnen 10 seconden zowel de huidige voortgang als de meest recente logactiviteit identificeren.
- **SC-004**: Na het toevoegen van een nieuwe geldige logregel en herladen van de pagina is de nieuwe regel in 100% van de gevallen zichtbaar en verwerkt in kaartvoortgang en percentage.