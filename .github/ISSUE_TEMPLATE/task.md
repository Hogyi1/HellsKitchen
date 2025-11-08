name: "Task"
description: "Feladat sablon nem-programozós vagy fejlesztési tevékenységekhez (UI, GDD, audio, map design, dokumentáció stb.)"
title: "[Task] <Feladat rövid címe>"
labels: ["task", "todo"]
body:
  - type: markdown
    attributes:
      value: |
        ### Leírás
        Add meg a feladat célját és rövid összefoglalóját.

  - type: textarea
    id: goal
    attributes:
      label: "Cél / Leírás"
      description: "Mit kell elérni ezzel a feladattal? (1-2 mondat)"
      placeholder: "Pl. Főmenü mockup elkészítése a style guide alapján"

  - type: dropdown
    id: priority
    attributes:
      label: "Prioritás"
      options:
        - P0 – Kritikus (vertical slice-hoz kell)
        - P1 – Fontos, de később is jöhet
        - P2 – Nice to have / polish
    validations:
      required: true

  - type: textarea
    id: deliverable
    attributes:
      label: "Deliverable (Leadandó eredmény)"
      description: "Mit kell leadni / megmutatni, ami bizonyítja, hogy a feladat elkészült?"
      placeholder: "Pl. Figma-fájl a főmenüről, PDF GDD-vázlat, assetlist táblázat stb."

  - type: textarea
    id: dod
    attributes:
      label: "Definition of Done (DoD)"
      description: "Pontosan mikor számít késznek? Add meg a készültségi feltételeket."
      placeholder: |
        - Tartalmazza az összes menüpontot
        - Csapat review megtörtént
        - Verzió feltöltve a Drive-ra

  - type: textarea
    id: dependencies
    attributes:
      label: "Függőségek / Kapcsolódások"
      description: "Más feladatoktól függ? (pl. Asset lista, UI style guide, GDD)"
      placeholder: "#12 – UI Style Guide, #8 – Audio list"

  - type: input
    id: assignee
    attributes:
      label: "Felelős / Owner"
      description: "Kié a feladat?"
      placeholder: "Pl. Boti / Hogyi / Bence / Dávid"

  - type: textarea
    id: notes
    attributes:
      label: "Megjegyzések / Extra infó"
      description: "Egyéb részletek, referenciák, linkek"
