![Logo del juego](https://github.com/user-attachments/assets/fcb374f0-9f41-4972-8020-cea38ef42101)
![Badge en Desarollo](https://img.shields.io/badge/STATUS-EN%20DESAROLLO-green)
![Badge en Desarollo](https://img.shields.io/badge/VERSION-1.0.0.2-blue)

## Índice

- [Título e imagen de portada](#Título-e-imagen-de-portada)
- [Insignias](#insignias)
- [Índice](#índice)
- [Autor](#autor)
- [Descripción técnica del proyecto](#descripción-del-proyecto)
- [Descripción del juego](#Descripción-del-juego)
   - [El inicio](#Descripción-del-juego)
   - [El turno](#Descripción-del-juego)
- [Turorial de compilación](#Tutorial-de-compilación)

# Autor
__Alejandro Alfara Torres__

Creador digital, estudiante de la Licenciatura en Ciencias de la Computación, en la Facultad de Matemática y Computación (MATCOM) de la Universidad de la Habana.

## Descripción técnica del proyecto
### Segundo Proyecto de Programación. Curso 2024.
La creación del proyecto puede ser facilmente explicada en dos fases:
- La primera fase se resume a la implementación de un juego de cartas digitales basado en el formato de jugabilidad de GWENT. Con un tablero funcional, mazo con diversidad de cartas jugables y una interfaz visual presentable. Mecánicas propiamentes explicadas a continuación.
- La segunda fase consistió en el desarrollo de un pequeño compilador para este juego con un Lenguaje de Propósito Específico, capaz de compilar, efectos y cartas en un deck ejecutable.

# Descripción del juego
Bienvenido a __War of Cyberage__ un juego estratégico de cartas digitales, animado en un universo futurista, donde la habilidad, no la suerte, es tu mejor arma. Une al mundo más clásico con la actualidad contemporánea, en una encarnecida guerra por el control total del entretenimiento mundial. Elije tu bando, construye un ejército y diviértete con decenas de personajes, carismáticos líderes, climas desafiantes y habilidades especiales. Cada nueva estratégia está... a tiro de carta. 
Lanza una carta a través de tres filas tácticamente distintas: cuerpo a cuerpo, a distancia y asedio. Acumula más puntos que tu oponente para ganar una ronda. Gana dos de las tres rondas para ganar la batalla, pero cuidado si los puntos se igualan, esto quedará en empate. No será fácil, pero nadie dijo que debiera serlo.

<h2 align="center"> SIN CONTENCIÓN, SIN TOMARSE DE LA MANO </h2>
- Comienzas con 10 cartas en tu mano, pudiendo jugar cada una desde el principio. Depende de ti abrir el juego con tu unidad más fuerte o guardar lo mejor para más tarde.

- Antes de que arranque la batalla, los jugadores pueden escoger hasta 2 cartas para regresarlas a la baraja y robar la misma cantidad.
- Todos los jugadores disponen de cartas de unidades platas y oro siendo estas ultimas inmunes a los efectos.
- Además de las cartas de unidades, existe una serie de cartas especiales, climas, despejes, aumentos, señuelos y obviamente, un líder.

### El turno
Consiste en jugar una carta, utilizar una habilidad de líder o pasar. El hecho de pasar implica que el jugador terminó de jugar por la ronda actual y por tanto no utilizar´a ninguna otra carta. En el momento en que los dos jugadores hayan pasado, la ronda termina, todas las cartas son enviadas al cementerio y la persona con mayor fuerza en el tablero ganará.



# Tutorial de compilación
Para la compilación de efectos y cartas es suficinete realizar la declaración de estos en el InputField In, un código similar al siguiente y con dicha sintaxis:

```
//Efecto para robar una carta del deck
effect {
Name: 'Drow',
Action: (targets, context) => {
carta = context.Deck.Pop();
context.Hand.Add(carta);
context.Hand.Shufle();
}
}

//Carta con efecto para robar una carta asignado
card {
Name: 'Papiriki',
Type: 'Oro',
Faction: 'Matcom',
Power: 5,
Range: ['Siege'],
OnActivation: [
{
Effect: {
Name: 'Drow',
}
Selector: {
Source: 'deck',
Single: false,
Predicate: (unit) => unit.Power >= 0',
}
}
]
}
```
Al compilar 25 cartas en mismo deck este estará disponible para su selección y utilización en el juego.
