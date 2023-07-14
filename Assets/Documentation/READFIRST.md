# The Online Ramsey Problem

This document will explain the overarching use of the Online Ramsey program. 
This program's purpose is to model the Online Ramsey problem.

## Brief Explanation
The Online Ramsey Problem revolves around a game wherein one player, the "Builder", and another, the "Painter", play against each other on a board comprising an infinite set of nodes. 
While there are many variants of this game, this document will explain the specific variant our program analyzes. 

The game is turn-based, and a turn is comprised of a Builder move followed by a Painter move. The Builder builds an edge between any two nodes that aren't already connected. The Painter then paints it either red or blue. The game ends when there is a monochromatic path of length N, where N is an input to the game. The Builder's goal is to make the game end as quickly as possible, while the Painter's goal is to prolong the game for as long as possible.

Internally, this board and gameplay is represented using a Graph data structure, which stores all the used nodes and edges for a game. This data structure, alongside various ways in which the data structure is parsed, can be found at (Scripts -> Board -> Graph).

The most important part of this project for anyone looking to edit it or make additions is the "Gameplayer" folder (Scripts -> Gameplayer).  It contains the Builder and Painter strategies.  For more information on how this system works and how to create new strategies, look to the Gameplayer Documentation.md" file in the directory you found this file.  For controls and how to use the program itself, look to the "Controls Documentation.md" file also contained in this directory.