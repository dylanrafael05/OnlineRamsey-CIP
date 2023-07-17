# `Ramsey.Gameplayer`: Strategies in Code

This document will explain how Painter and Builder strategies work in our Online Ramsey program.

## Definitions
There is a fair amount of "lingo" which has evolved during the course of this codebase's production, and as such here is a list of their definitions:
- Strategy: code which defines how a painter or builder will interact with the board
  - Player: another word used for strategy
- Move: either a painter painting an edge or a builder building an edge

## Code Structure
First of all, each strategy available within the simulation must correspond to a class in the codebase. These classes are responsible for implementing the logic which strategies will employ to play the game, as well as providing a way for the UI to construct instances of itself.

Strategy classes all inherit from the "`IPlayer`" interface (Scripts -> Gameplayer -> Strategy -> Strategy.cs). Most importantly, these classes must therefore implement `IPlayer`'s `GetMove()` or `GetMoveAsync()` method and return moves of type "`IMove`". The two major types of moves are implemented through the structs `PainterMove` and `BuilderMove`.  They both inherit from IMove.  

For sake of ease, Painter and Builder strategies do not need to declare their implementation of `IMove`, and can instead inherit from the abstract base classes `Painter` and `Builder` (for strategies which need to use asynchronous programming in their logic) as well as `Painter.Synchronous` and `Builder.Synchronous` (for strategies which are always able to immediately produce moves once `GetMove` is invoked). These abstract classes force implementation of `GetMove` methods that return `PainterMove` and `BuilderMove` respectively, rather than the broader "`IMove`".  

In short, all you need to know is that Painter strategies must inherit from the Painter abstract class and implement a `GetMove` method that returns `PainterMove`. Builder Strategies must inherit from the Builder abstract class and implement a `GetMove` method that returns `BuilderMove`.

`BuilderMove`s are comprised of two nodes, both of which must have already been created on the graph (typically through `GameState.CreateNode()`). `PainterMove`s are comprised of an edge object and the type (or color) it should be painted. As it currently stands, this type can be `0` to represent blue and `1` to represent red, although the game engine is robust enough to handle other color schemes as well as additional colors, as determined by the array of color options provided to the constructor of `GameManager` in the `Main` class.

Moves returned from a strategy will be realized by the main `GameManager`. This class always stores a `Builder` and `Painter` object during a game (depending on the selection you make in the main menu), and will automatically handle the back-and-forth nature of gameplay. After calling `GetMove` for either of the game's two strategies, `GameManager`s will check if the returns move is valid, (see `IMove.IsValid()` in Move.cs) and will execute the move if it is. If it is not valid however, it will instead call `GetMove()` once again, repeating until a valid move is returned. This behaviour exists to prevent a `UserBuilder` or `UserPainter` from returning moves which are invalid due to incorrect user input.

### Gotchas
Certain strategies will have qualities which may impact how the engine should best handle their execution, such as being non-deterministic or non-automatic. Such attributes must be conveyed to the program using attributes placed on the class. For instance, strategies which employ randomness must denote this fact using a `[NonDeterministicPlayer]` line before their class definition. This allows for optimizations to data analysis to be made by future developers.

Furthermore, due to limitations with Unity's API, all retreivals of random numbers must be done through `ThreadSafeRandom`, provided by the Utilities package. Using the static functions available through this class mitigates the single-threaded nature of Unity's random number generator and thus avoids silent and often massively inconvenient errors.

As will be explained later, the usage of static constructors is heavily employed by strategy classes, but in a way which does not align entirely with their intended usage. As such, static constructors of classes which implement `IPlayer` are guaranteed to be run after the assemblies of the Unity application have been loaded, despite the typical rules surrounding static construction of types in the CLR.

### `GameState` and other utils
All `GetMove` implementations, asynchronous or otherwise, also take in a `GameState` object (Scripts -> Board -> GameState.cs), which contains the current state of the board and exposes helper methods. This object can be saved into fields safely, since only one is used per game. Furthermore, these objects hold all of the infomation about the state of the game, such as the newest painted edge, newest build edge, and longest available path.

For Builder strategies, there also exists the `BuilderUtils` static class (Scripts -> Gameplayer -> Builder Helper -> BuilderUtils.cs), which implements a few methods that could be helpful in some Builder strategies.

Since many Builder strategies have logic that isn't purely based on current state of the board, but instead similar to a tree of choices based on each Painter move, there exists a utility class to simplify implementation of this behaviour. Instead of `GetMove` simply being a function of the current state of the board that returns a `BuilderMove`, its value can be determined through a sequence of choices as well as how previous iterations of the board have impacted these choices. Furthermore, due to the potentially infinite nature of the Online Ramsey game, such a tree of decisions must be infinitely extendable. For example, the Cap Builder strategy, the one that will complete a game in under 4n-6 moves, (where n is the length of the path needed to win the game) follows a loop sequence that starts with the longest red and blue path.

The implementation of this sequence system uses `IEnumerable`s and a class named `SequenceNavigator` [1]. Simply put, a `SequenceNavigator` stores a serires of functions which take in `GameState`s and produce a sequence of moves, then automatically calls these functions and unwraps these sequences in such a way that the sequences are returned in order until the last one is visited, where it is then repeated infinitely. 

[1] An `IEnumerable` can be seen as a function that can return its result and then pause, waiting to be called again, at which point it will resume until the next return point, where it will pause again. They can be thought of as a generalized version of a non-deterministic, potentially infinite sequence.

## `StrategyInitializer` and Binding a Strategy to the UI
Strategies are technically usable as soon as they implement their logic, but they are not recognized by the UI systems nor displayed on the main menu until code to do so is added. The main handlers of this logic, "`StrategyInitializer`s" (Scripts -> Gameplayer -> Strategy -> Initializer -> StrategyInitializer.cs), must be created by each strategy class, typically inside a static constructor. This class is responsible for retrieving any parameters which a strategy might require to function, such as weights for random behaviour, lengths of tasks to repeat, etc. It is encouraged to look at the various pre-included strategies, especially ones which require parameters such as `RandomBuilder`, as examples. 

`StrategyInitializer` exposes 3 methods for create a strategy initializer.

### Initialization via `new()`
One can create a strategy initializer that takes in no parameters and will construct the strategy using a parameterless constructor. This only requires the simple function call `StrategyInitializer.RegisterFor<MyStrategyType>();`

### Initialization via Parameterless Lambda
One can create a strategy initializer that takes in menu parameters, but constructs a strategy without using a parameterless constructor using a lambda function returning an instance of the strategy type. For example, a strategy which employs this method would include the following line in its static constructor: `StrategyInitializer.RegisterFor(() => new MyStrategyType(ThreadSafeRandom.NextInt(1, 10)))`. 

### Initialization via Lambda and `params TextParameter[]`
Finally, one can create a strategy initializer that takes in parameters from the actual menu. This requires two things, a function which takes a list of preprocessed string parameters and constructs an instance of the desired strategy type, and a list of `TextParameter` structs declaring how each parameter should be retrieved and parsed from the text input on the menu. Each `TextParameter` must supply an `IInputVerifier` which converts a string to some type, providing error messages if the input is invalid. Predefined verifiers exist for common types, i.e. `IInputVerifier.Integer` for `int`, `IInputVerifier.Float` for `float`, and `IInputVerifier.None` for `string`.

## Important Files and Folders
- `GameState.cs` (Scripts -> Board -> GameState.cs)
- `Strategy.cs` (Scripts -> Gameplayer -> Strategy -> Strategy.cs)
- `Move.cs` (Scripts -> Gameplayer -> Strategy -> Move.cs)
- `BuilderUtils.cs` (Scripts -> Gameplayer -> Computer Player -> BuilderUtils.cs)
- `Builder Strategies` (Scripts -> Gameplayer -> Computer Player -> Builder Strategies)
- `Painter Strategies` (Scripts -> Gameplayer -> Computer Player -> Painter Strategies)
- `StrategyInitializer.cs` (Scripts -> Gameplayer -> Strategy -> Initializer -> StrategyInitializer.cs)
- `TextParameter.cs` (Scripts -> Utilities -> UI -> TextParameter.cs)
- `IInputVerifier.cs` (Scripts -> Utilities -> UI -> IInputVerifier.cs)