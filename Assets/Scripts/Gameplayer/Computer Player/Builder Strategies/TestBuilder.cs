using Ramsey.Graph;
using Ramsey.Board;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public class TestBuilder : Builder
    {
        //Tree of substrategies for every
        //possible move that loops back on itself
        //each substrategy is different logic so instead of looking at the board with clean eyes everytime
        //the builder knows which move it made which means that the branch can only go into 2 possible directions

        PainterTree currentTree;

        PainterTree initialTree;
        PainterTree loopTree;

        //Could have diff types of trees like BlindTree (simply a list of funcs), FixedTree (which is what we have currently), InfiniteTree ? (tree without fixed loop but idk how we're even gonna implement it so how would i have a name already??)
        //All implement IPainterTree and have same update method or smth

        //Start
        Node[] pendants;

        //Loop
        Node[] terminates;

        public TestBuilder()
        {
            pendants = new Node[3];

            Func<GameState, BuilderMove> move1 = g =>
            {
                pendants[0] = g.Board.Nodes[0];
                return new(g.Board.Nodes[0], g.Board.Nodes[1]);
            };
            Func<GameState, BuilderMove> move2 = g =>
            {
                pendants[1] = g.Board.Nodes[2];
                return new(g.Board.Nodes[1], g.Board.Nodes[2]);
            };
            Func<GameState, BuilderMove> move3 = g =>
            {
                pendants[2] = g.Board.Nodes[3];
                return new(g.Board.Nodes[1], g.Board.Nodes[3]);
            };

            initialTree = new(3, new Func<GameState, BuilderMove>[] { move1, move1, move2, move2, move2, move2, move3, move3, move3, move3, move3, move3, move3, move3 });
            currentTree = initialTree;

            //Func<GameState, BuilderMove> connect = g =>
        }

        public override Task<BuilderMove> GetMove(GameState gameState)
        {

            //Make sure at least k nodes (idk how many k is)

            BuilderMove? move = currentTree.Run(gameState);
            if (move is null)
            {
                currentTree = loopTree;
                //logic for setting up for the looptree and the terminating nodes and stuff
                return Task.FromResult((BuilderMove) currentTree.Run(gameState));
            }
            else return Task.FromResult((BuilderMove) move);

            throw new System.NotImplementedException();

        }
    }
}
