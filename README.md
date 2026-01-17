# 🐢 Space Turtle

<div align="center">
  <table border="0">
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/9f66ccbf-ba85-4df3-9832-da0cfe965184" width="400">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/0df97bc5-50e7-457b-8f64-f47d9052754c"  width="400" >
      </td>
       <td align="center">
        <img src="https://github.com/user-attachments/assets/d1f3c404-aff0-422b-9c3a-df5b41e455a1" width="400">
      </td>
    </tr>
  </table>
  
  <br>
  
  **Space Turtle** è un *Infinite Runner Verticale* 2D sviluppato in **Unity**.
  <br>
  Il giocatore controlla una tartaruga spaziale che deve salire sempre più in alto saltando su travi, schivando satelliti e meteoriti, e utilizzando power-up speciali per sopravvivere in un ambiente a gravità dinamica.
</div>

---

## 🎮 Core Gameplay

Il gioco combina riflessi rapidi e meccaniche fisiche avanzate.

* **Sistema di Movimento Ibrido:**
    * **Salto & Doppio Salto:** Fisica calibrata per controlli reattivi.
    * **Grind Mechanic:** Sistema di scivolata sulle travi inclinate (gestito tramite Physics Material a zero attrito) per un gameplay fluido.
    * **Volo (Scarpe Razzo):** Power-up che disabilita temporaneamente la gravità, permettendo movimento libero via touch.

<p align="center">
  <img src="https://github.com/user-attachments/assets/0b55da62-55ec-486a-87c8-5e5d66afd346" width="350">
  &nbsp; &nbsp; &nbsp; &nbsp;
  <img src="https://github.com/user-attachments/assets/0d6bc0c7-cb2c-405b-98e4-bd9b110a2ec2"  width="350" >
</p>
<p align="center"><i>Fig 1. Meccanica di Grind (Sinistra) e Volo Antigravità (Destra)</i></p>

---

## 🤖 AI Manager & Eventi

Il cuore del gioco è il `GameController`, un sistema intelligente che bilancia la difficoltà.

* **Difficoltà Dinamica:** Un algoritmo riduce i tempi di spawn dei nemici (`spawnRate`) ogni secondo, aumentando la frenesia progressivamente.
* **Eventi a "Mutua Esclusione":** Il sistema gestisce eventi speciali **Sciame di Meteoriti** e **Passaggio Navicelle**.

<p align="center">
  <img src="https://github.com/user-attachments/assets/debd5405-c149-44bb-9b87-26db97d748b6" width="350">
  &nbsp; &nbsp; &nbsp; &nbsp;
  <img src="https://github.com/user-attachments/assets/8f1c7695-eb26-459b-ac50-fa2054f8cba5" width="350">
</p>
<p align="center"><i>Fig 2. Eventi Speciali: Sciame di Meteoriti (Sinistra) e Passaggio Navicelle (Destra)</i></p>

---

## ☄️ Nemici & Ostacoli

Una varietà di ostacoli metterà alla prova i riflessi del giocatore.

<div align="center">
<table border="0">
  <tr>
    <td align="center" width="200">
        <img src="https://github.com/user-attachments/assets/e624ea91-dbb5-4f32-80c0-77d2d7ea1bd9" width="150">
        <br><b>Satellite</b><br>
    </td>
    <td align="center" width="200">
        <img src="https://github.com/user-attachments/assets/eeac7418-5538-42b2-8ac5-a4e8f44853de" width="150">
        <br><b>Meteorite</b><br>
    </td>
    <td align="center" width="200">
        <img src="https://github.com/user-attachments/assets/db1d89fa-1566-4809-ab4b-f35e52875c11" width="150">
        <br><b>Sciame</b><br>
    </td>
    <td align="center" width="200">
        <img src="https://github.com/user-attachments/assets/979127e9-b85c-4cba-b094-7c29d3d54164" width="150">
        <br><b>Navicella</b><br>
    </td>
  </tr>
</table>
</div>

---

## 🛡️ Power-Ups

Il giocatore deve gestire strategicamente le risorse per sopravvivere e battere il record.

* **Pianeti:** Ripristinano la salute persa contro i satelliti.
* **Scudo:** Sistema a raccolta (3 pezzi) che garantisce invincibilità temporanea.
* **Scarpe:** Disabilitano la gravità per volare liberamente.

<div align="center">
<table border="0">
  <tr>
    <td align="center" width="250">
        <img src="https://github.com/user-attachments/assets/cc8d5ff4-6105-4378-a40d-fcf44c7199a6" width="200">
    </td>
    <td align="center" width="250">
        <img src="https://github.com/user-attachments/assets/c17642a0-a60c-4096-9c03-0a4877ce1b6d" width="200">
    </td>
    <td align="center" width="250">
        <img src="https://github.com/user-attachments/assets/722ddd8f-6c6d-4ce5-adaa-a5d4e3e71718" width="200">
    </td>
  </tr>
</table>
</div>

---

## 🖥️ Interfaccia Utente (UI)

Il gioco include un'interfaccia completa per guidare l'utente.

<p align="center">
  <img src="https://github.com/user-attachments/assets/5524463b-8b58-4d92-8892-9f4cbac29d0c" width="350">
  &nbsp; &nbsp; &nbsp; &nbsp;
  <img src="https://github.com/user-attachments/assets/9d2d15ca-9a30-4c17-a925-be354ce28fd6" width="350">
</p>
<p align="center"><i>Fig 3. Tutorial Interattivo (Sinistra) e Menu di Pausa (Destra)</i></p>

---

## 🎨 Art Direction & Cura dei Dettagli

L'estetica di **Space Turtle** è frutto di un lavoro di design dedicato ed esclusivo.

* **Asset Originali:** Tutte le grafiche presenti nel gioco sono state **disegnate a mano o generate appositamente** per questo progetto. Nessun asset è generico o pre-fatto.
* **Selezione Maniacale:** Per ogni singolo elemento (nemici, power-up, UI) sono state create e valutate **oltre 30 varianti**, scegliendo solo quella perfetta per lo stile "Deep Space".
* **Coerenza Visiva:** Ogni pixel è stato studiato per garantire che personaggi e ambiente si fondano in un'unica identità grafica coerente.

---

## 🛠️ Architettura Tecnica

Il progetto è costruito seguendo principi di programmazione modulare in C#.

* **Backend & Cloud:** Sistema integrato per la gestione remota dei salvataggi e la persistenza dei dati utente.
* **Animator:** Macchina a stati complessa con parametri Booleani per transizioni fluide (es. entrata/uscita dalla fase di Grind senza jittering).
* **Audio System:** Canali audio separati per SFX, Loop (Volo/Scudo) e Musica.

## 📱 Installazione

1.  Clona la repository:
    ```bash
    git clone [https://github.com/PaciottiMattia/SpaceTurtle.git](https://github.com/PaciottiMattia/SpaceTurtle.git)
    ```
2.  Apri il progetto con **Unity 2022.x**.
3.  Premi Play nella scena `MainScene`.

---
*Progetto realizzato da **Paciotti Mattia**.*
