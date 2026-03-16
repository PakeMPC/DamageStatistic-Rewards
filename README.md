------------
# ESPAÑOL

## DamageStatistic&Rewards ⚔️
Un plugin avanzado para servidores de Terraria (TShock) que rastrea el daño infligido a los jefes, muestra estadísticas detalladas (DPS, porcentaje, daño total) al finalizar la batalla y otorga recompensas económicas configurables a los jugadores con mejor rendimiento.

## ✨ Características Principales
* **Clasificación Detallada:** Al derrotar a un jefe, muestra una lista ordenada de menor a mayor daño con todos los participantes.
* **Top 3 Destacado**.
* **Cálculo de DPS:** Muestra la media de Daño Por Segundo exacto de cada jugador al final de la batalla.
* **Sistema de Recompensas:** Reparte dinero a los jugadores del Top 3 en base a la vida total del jefe y su porcentaje de contribución.
* **Sistema Anti-Farmeo:** Bloquea las recompensas si un solo jugador supera un límite de daño establecido (configurable desde el archivo BossDamage&Rewards.json).
* **Multi-idioma:** Soporte integrado para Español, Inglés y Portugués (`/damagelang`).



## 🔐 Comandos y Permisos

| Comando | Permiso Requerido | Descripción |
| :--- | :--- | :--- |
| `/damagelang <es/en/pt>` | `damagestatistic.lang` | Cambia el idioma personal en el que el jugador ve las estadísticas. |



## ⚙️ Configuración (`BossDamage&Rewards.json`)
Al iniciar el plugin por primera vez, se generará el archivo de configuración en la carpeta `tshock`.
```json
{
  "ShowDamage": true, // define si aparecen las estadísticas (true o false)
  "DamageReward": true, // define si se entregan las recompensas (true o false)
  "MoneyPer100Damage": "20s", // define cuánto dinero se entrega por cada 100 de daño (c=cobre, s=plata, g=oro, p=platino)
  "Top3Percent": 10, // porcentaje de recompensa que obtendrá el top 3
  "Top2Percent": 30, // porcentaje de recompensa que obtendrá el top 2
  "Top1Percent": 60, // porcentaje de recompensa que obtendrá el top 1
  "MaxDamagePercent": 80, // daño máximo para recibir recompensa. Si alguien hace más del 80%, se anula
  "DefaultLanguage": "es", //Idioma por defecto del plugin
  "PlayerLanguages": {} // idioma en que cada jugador tiene el plugin
}
```

-----
# ENGLISH

## DamageStatistic&Rewards ⚔️
An advanced plugin for Terraria (TShock) servers that tracks damage dealt to bosses, displays detailed statistics (DPS, percentage, total damage) at the end of the battle, and grants configurable economic rewards to top-performing players.

## ✨ Key Features
* **Detailed Ranking:** Upon defeating a boss, it shows an ordered list from lowest to highest damage with all participants.
* **Highlighted Top 3:**.
* **DPS Calculation:** Shows the exact average Damage Per Second of each player at the end of the battle.
* **Reward System:** Distributes money to the Top 3 players based on the boss's total health and their contribution percentage.
* **Anti-Farming System:** Blocks rewards if a single player exceeds an established damage limit (configurable from the `BossDamage&Rewards.json` file).
* **Multi-language:** Built-in support for Spanish, English, and Portuguese (`/damagelang`).



## 🔐 Commands and Permissions

| Command | Required Permission | Description |
| :--- | :--- | :--- |
| `/damagelang <en/es/pt>` | `damagestatistic.lang` | Changes the personal language in which the player sees the statistics. |



## ⚙️ Configuration (`BossDamage&Rewards.json`)
When starting the plugin for the first time, the configuration file will be generated in the `tshock` folder.

```json
{
  "ShowDamage": true, // defines if statistics are shown (true or false)
  "DamageReward": true, // defines if rewards are given (true or false)
  "MoneyPer100Damage": "20s", // defines how much money is given per 100 damage (c=copper, s=silver, g=gold, p=platinum)
  "Top3Percent": 10, // reward percentage that the top 3 will get
  "Top2Percent": 30, // reward percentage that the top 2 will get
  "Top1Percent": 60, // reward percentage that the top 1 will get
  "MaxDamagePercent": 80, // maximum damage percentage. If a player deals more than 80%, rewards are canceled
  "DefaultLanguage": "en", //default plugin language
  "PlayerLanguages": {} // language each player has set for the plugin
}
```
