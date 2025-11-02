# Game Flow

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

2. ## Éjszakai műszak – éttermi rész:

### Áttekintés
A nappali műszak lezárulta után az étterem **bezár**, a fények elhalványulnak, és kezdetét veszi az **éjszakai műszak**.  
A játékos a **konyhában** marad, ahol most már **karbantartási és takarítási feladatokat** kell elvégeznie.  
A cél, hogy **minden kijelölt feladatot** teljesítsen, mielőtt **fel kel a nap** – ha nem sikerül, Jose **elveszíti az állását**. 
Minden helyiségbe egy belépőkártya szükséges, amiket csak akkor tudunk kinyitni, ha van áram. (Kijutni bármelyik szobából ki lehet akkor is ha nincsen áram) 

A feladatok **időigényesek**, de nem nehezek:  
- a sütő kitisztítása kb. **30 másodpercet** vesz igénybe,  
- a szemét kivitele **két fordulóban** történik,
- rendelések behozatala 
- bizonyos események (pl. áramszünet) **megszakíthatják** a munkát.

Időről időre **szörnyek**/**robotok** jelennek meg, akik **"járőröznek"**, és ha észreveszik a játékost, **vége a játéknak**.  
A játékosnak ezért **el kell rejtőznie** vagy **meg kell menekülnie** előlük, miközben tovább végzi a feladatait.
Ha idő előtt sikerül elvégezni a feladatokat, akkor a kijárati ajtó kinyílik és elhagyhatjuk az éttermet - **Vége** 

***

### Fő események

1. **Áramszünet**  
   - Időnként a rendszer **lekapcsolja az áramot**, ilyenkor a világítás és az elektromos ajtók **nem működnek**.  
   - A játékosnak meg kell keresnie a **biztosítéktáblát**, és **vissza kell kapcsolnia az áramot**.  
   - Amíg nincs áram, a szörnyek **gyorsabbak** lesznek.  
   - A **zseblámpa** ilyenkor **másodlagos fényforrásként** használható, de **korlátozott ideig** működik _(fejleszthető eszközök)_.
   - Minél tovább tartózkodunk a sötétben, annál nagyobb a valószínűsége, hogy a szörnyek "kiszagolnak" _(sanity level)_.

2. **Szörny esemény**  
   - A szörny/robot **aktív állapotba kerül**, és **vadászni kezd** a játékosra.  
   - Bizonyos **hangok vagy zajok** (pl. tárgy leejtése, ajtócsapódás) **vonzó hatásúak** lehetnek számára.  
   - A játékos **el tud bújni** (pl. szekrénybe _(V1.0)_, asztal alá vagy szellőzőbe), amíg a veszély el nem múlik.

---

### Eszközök és tárgyak

A játékos a nappali műszak alatt szerzett **valutát** felhasználhatja **biztonsági eszközök vásárlására**.  
Ezek az éjszaka során segíthetnek a túlélésben, vagy ideiglenes előnyt biztosíthatnak.

| Eszköz / Tárgy | Leírás | Használat |
|----------------|---------|-----------|
| **Zseblámpa** | Kézi fényforrás, áramszünet esetén nélkülözhetetlen. | Korlátozott ideig működik; elemekkel bővíthető. |
| ?? **Gyertya** ?? (V2.0) | Egyszer használatos fényforrás. Bárhova elhelyezhető. | Rövid ideig világít, nem vonzza a szörnyet, kerüli a kivilágított szobát. Egyszer haszsnálható, 60 másodpercig nyújt védelmet. |
| **Lakattal zárható ajtó/pince lejárat** | Egyes ajtók ideiglenesen lezárhatóak a szörny elől. | Lassítja a szörnyet, de idővel elhasználódik. |
| **Pótbiztosíték** | Azonnal visszaállítja az áramot áramszünet esetén. | Egyszer használatos. Távolról irányítható/Automatikusan az első áramszünetnél aktiválódik |

Ezeket az eszközöket az éjszaka kezdete előtt veheti meg. Az idő alapú eszközök (pl.: gyertya, lakat) egyszer használatosak, miután lehelyeztük nem tudjuk újból felvenni. Ezek egy megadott idő után "lebomlanak", nem nyújtanak örök védelmet.

Egyszerre csak egy dolog lehet a kezünkben legyen szó, szemét kivitelről, rendelések behozataláról vagy gyertyák elhelyezéséről. A zseblámpa és más távolról vezérelhető eszközök (Pótbiztosíték) gyorsbillentyűkkel ki be kapcsolható.

---

### Cél és következmények

A cél az, hogy **reggelig életben maradjunk**, és **minden kijelölt feladatot** befejezzünk.  
- Ha sikerül, Jose **bónusz jutalmat** kap, és a műszak lezárul.  
- Ha a szörny elkap, vagy a feladatok nem készülnek el időre, Jose **elveszíti az állását**. 

A teljes éjszakai rész **360 másodpercig** tartana.
