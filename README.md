# Game Design Document- updated

1. ## Nappali műszak - konyhai rész:
### Áttekintés
A játék elején megérkezünk az étterembe, ahol csak a **konyhai terület** érhető el.  
Fokozatosan **érkeznek be a rendelések**, amelyeket a **recepthez tartozó időkorláton belül** kell elkészíteni.  
Minden recepthez tartozik egy ***saját időzítő***, amely jelzi, mennyi idő maradt a befejezésre.  

A rendelések **felhalmozódhatnak**, egyszerre maximum **4 db** aktív. Párhuzamosan is lehet a 4 rendelésen dolgozni.
Minden recept *különböző alapanyagokból* áll, és a rendszer **randomizálva** állítja össze a recepteket.

Az elkészült ételeket egy **kiadóablakon** keresztül kell továbbítani. 
- Azok az ételek, amik **nem készülnek el időben**, **[valuta] levonással** járnak.  
- Azt az ételt, amelyet **nem a jelenlegi receptek** egyike alapján készítettünk el és adjuk ki, azért **nem jár jutalom**.
- Ha az ételt a megadott időn belül készítjük el, a megmaradt idő arányában **bónusz jutalom** jár.

A nappali rész **240-300** másodpercig tartana.

---      
### Eszközök

A játék során **6** különböző konyhai berendezéssel találkozhatunk (*V1.0*).

1. **Sima pult**  
   - Nincs külön funkciója, csupán arra szolgál, hogy az alapanyagokat vagy elkészült ételeket **le tudjuk helyezni**.

2. **Vágódeszka**  
   - A **feldolgozatlan hozzávalókat** itt lehet **felszeletelni** a további előkészítéshez.

3. **Gáztűzhely**  
   - A **süthető alapanyagok** feldolgozására van.  
   - Ha **túl sokáig** marad rajta az étel, **elégettnek** minősül és **ki kell dobni**.

4. **Tányéradagoló**  
   - **Megadott időközönként** automatikusan **tányérokat spawnol**, amelyeken az ételeket **össze kell állítani** a tálaláshoz.

5. **Kuka**  
   - Az **elégett** vagy **rosszul összeállított** ételeket ide lehet **kidobni**.  

6. **Tálalószalag**  
   - Ide kell **felhelyezni a kész ételeket** a kiszolgáláshoz.  
   - Ha **rossz ételt** helyezünk rá, az **visszautasításra kerül**

***

### Ételek
| Hamburger             | V2.0          |
|-----------------------|---------------|
| Hamburger buci        |               |
| Saláta                |               |
| Paradicsom            |               |
| Sajt                  |               |
| Hamburger húspogácsa  |               |

2. ## Éjszakai műszak – éttermi rész

### Áttekintés
A nappali műszak lezárulta után az étterem **bezár**, a fények elhalványulnak, és kezdetét veszi az **éjszakai műszak**.  
A játékos a **konyhában** marad, ahol most már **karbantartási és takarítási feladatokat** kell elvégeznie.  
A cél, hogy **minden kijelölt feladatot** teljesítsen, mielőtt **felkel a nap** – ha nem sikerül, Jose **nem kap fizetést**, de nem veszti el az állását.

---

### Éjszakák és feladatok

| Éjszaka | Robotok | Feladatok | Megjegyzés |
|----------|----------|------------|-------------|
| **1. éjszaka** | 1 robot | Felmosás, mosogató, sütő tisztítása |  |
| **2. éjszaka** | 2 robot | Felmosás, mosogató, rendelés és bepakolás (5 perces timer) |  |
| **3. éjszaka** | 1. & 2. robot | Felmosás, mosogató, sütő tisztítása, kések élezése |  |
| **4. éjszaka** | 1. & 2. robot (nehezített) | Felmosás, mosogató, rendelés és bepakolás, kések élezése, sütő tisztítása |  |
| **5. éjszaka** | 1. & 2. robot (lelőni őket) | Felmosás, mosogató (nem kötelező) | A cél a robotok legyőzése |

A feladatok **időigényesek**, és **bizonyos események** (pl. áramszünet) megszakíthatják a folyamatot.

---

### Ellenségek – Robotok

A robotok **nem folyamatosan** vannak jelen a pályán, hanem **bizonyos eséllyel megjelennek**, majd **távoznak**.  
A minimum idő, amit a pályán töltenek: **30 másodperc**. Ezután **eséllyel aktiválódik a visszavonulás**, amikor a robot a pince felé indul.  
Ha a játékos eközben észrevehető, a robot **újra támadásba lendül**.

**Robot 1:**
- Menekülni kell tőle.
- Nem néz be a szekrénybe → biztonságos búvóhely.

**Robot 2:**
- Ha **ravilágítunk zseblámpával**, **5 másodpercre megáll**.
- **Ötszöri megvilágítás** után **visszavonul a pincébe**.
- **Mindig tudja**, hol vagy, **lassan közelít**, és **a szekrényben is megtalál**.
- **Áramszünetkor automatikusan megjelenik**.

Ha **áramszünet** lép fel, a robotok **gyorsabbak** lesznek.  
A zseblámpa **1 percig** működik, és **elemekkel bővíthető**.

---

### Fő események

1. **Áramszünet**  
   - A világítás és elektromos ajtók leállnak.  
   - A játékosnak meg kell keresnie a **biztosítéktáblát**.  
   - Áramszünet alatt a robotok **gyorsabbak**, és a **második robot** automatikusan aktiválódik.  
   - **Pótbiztosíték** vagy **manuális javítás** segítségével visszaállítható.  

2. **Robot esemény**  
   - A robotok **járőröznek** és **vadásznak** a játékosra.  
   - Hangokra reagálnak (pl. ajtócsapódás, futás, tárgyak elmozdítása).  
   - A játékos **elbújhat** (szekrény, asztal, szellőző).

---

### Shop rendszer (V2.0)

A **shop** a **pince szobában** található, egy **számítógépen** keresztül érhető el.  
A nappali műszak után lehet vásárolni, az éjszaka alatt a számítógép **ki van kapcsolva**.

| Tárgy | Funkció | Ár / Megjegyzés |
|--------|----------|----------------|
| **Zseblámpa** | 1 percig bírja (elemmel bővíthető 2–3 percre, max 2 elem) |  |
| **Gyertya** | Egyszer használatos, 60 mp-ig világít, nem vonzza a robotokat |  |
| **Medvecsapda** | Megállítja az 1. robotot 5 mp-re, a 2. kikerüli |  |
| **Lakat** | Ruhatároló és ajtók bezárására |  |
| **Pótbiztosíték** | Automatikusan visszaállítja az áramot áramszünetnél |  |
| **Shotgun + töltények** | Csak az utolsó napon érhető el, a robotok ellen |  |
| **Rendelés a konyhának** | Ingyenes, speciális eseményindító | |

---

### Cél és jutalmazás

A cél az, hogy **reggelig életben maradjunk**, és **minden kijelölt feladatot** befejezzünk.

- Ha minden sikerül, Jose **extra pénzjutalmat** kap:  
  `Jutalom = x coin + x coin * (teljesített feladat / max feladat)`
- Ha a feladatokat nem sikerül befejezni, **nem jár jutalom**, de a játék nem ér véget végleg.

---

### Ending variációk

1. **Sikeres befejezés:**  
   A játékos lelövi a robotokat, és sikeresen túléli az **öt munkanapot**.

2. **Sikertelen befejezés:**  
   Ha nincs elég pénzünk a **shotgun** megvásárlásához, a robotok **elhurcolják Jose-t a pincébe**, közvetlenül a kijárat előtt.

---

**Az éjszakai rész hossza:** ~360 másodperc (6 perc)

