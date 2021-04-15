# Roguelike merchant game
Name: Luck of the Isles

## Overview
### Project scope
Single player procedurally generated game that mixes casino and roguelike mechanics and features simple click/tap controls.

### Elevator pitch
Play as a merchant deep in debt. 

Pay your debt installments by trading with the different cities in the procedurally generated map. 

In this world you can only get goods by gambling, hit the slot machine in a city to try and get some goods to sell to other cities before the time to pay is up. 

Every time you pay an installment to your creditor you get an item that will help you in your journey.

If you can’t pay the game is over. 

Your objective is to last for the longest time possible.

### Target platforms
The game suits mobile platforms since it doesn’t require complex 3D graphics and the controls can be implemented with only tap/click.

Releasing on desktop (PC) is also viable because the game has enough complexity on its mechanics to make it interesting for experienced pc gamers.

The game shouldn’t target a console release since point and click controls don’t mix very well with controllers, and this genre of game isn’t very popular on consoles.

### Monetization

The ideal monetization model would be an upfront payment. PC gamers are used to paying for games but for mobile users it could be a hard sell since most games on mobile are free to play, for this reason releasing a free to play limited version and a priced full version should be considered. 

## Gameplay

### Game mechanics

#### Game mechanic 1: Map movement
![Map](/documentation/resources/pic_map.jpg)

*	In each game the map will be procedurally generated.
*	The map will contain cities and paths connecting these cities through the sea.
*	Each city can be of a certain type and that will influence what goods are available in their market. Some city types could be castle, village, monastery, city…
*	Travelling to each city will cost at least 1 turn ⌚ and taking some paths will cost more.

#### Game mechanic 2: Obtaining goods
![Slot machine](/documentation/resources/pic_slot.jpg)

*	To stock your boat with goods you will have to play the slot machine on a city.
*	The slot machine will be filled with the goods that the city offers.
*	Spinning the slot machine costs 1 turn ⌚.
*	If after spinning the machine you get at least 3 of the same good adjacent to each other, you will add one of that good to your inventory.
*	Goods of the same type that were adjacent in the previous spin will be removed from the machine.

#### Game mechanic 3: Selling goods
![Market](/documentation/resources/pic_sell.jpg)

*	To earn some gold, you will have to sell the goods that you won playing the slot machines.
*	You can sell goods when you arrive to any city.
*	The type of city will influence what goods you can sell and their price.
*	The prices for a given good will be especially low if the city has had stock of that object during the day.

#### Game mechanic 4: Day progression

*	Each day will last for a very limited number of turns ⌚.
*	Once all turns have passed, the day is over, and you will be asked to pay your debt installment 💰.
*	The quantity to pay each day will increase.
*	If you can’t afford to pay, the game ends.
*	If you are able to pay, you can choose one of N items that will give you a bonus passive effect for the rest of the game.
*	At the start of each day the cities will restock their machines with additional goods.
*	The goal of the game is to last for the maximum amount of days possible.


#### Game mechanic 5: Items
![Items](/documentation/resources/pic_items.jpg)

*	You will be able to add an item to your collection when a new day begins.
*	The items will influence different aspects of the game such as:
  *	Value or quantity of goods.
  *	Length of day.
  *	Special interactions when specific goods are adjacent to each other on the slot machine.
  *	Special effects when specific goods are in your inventory.
  *	Bonuses when spinning the slot machine in certain types of cities.
  *	Resistance to negative events that might occur during the game.
  *	Bonuses when selling or acquiring certain goods.
*	Items’ effects can synergize and stack with each other.

### Gameplay loop
![Items](/documentation/resources/gameplay_loop.png)

