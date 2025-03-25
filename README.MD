# Godot 5D Chess Project

This project is a port of my original Java UI From 2021 which you can find here: <https://github.com/Slavrick/5dChessGUI>

The point of this project was just for fun, not sure how far I will take the project. The main motivation behind this project was that I had a semi decent and working implementation of 5D Chess in Java, but it was behind a janky JavaFX UI :( 

I plan to expand on my previous 5D project by making it more portable, so more people can use the GUI (Im not even sure anyone else ever used the old GUI based on what I heard and how clunky it was to get to work), to make the experiance less clunky and more smooth/better feeling. I also wish to add a study/analysis feature.





I used GitHub Copilot to initially port the code over, Im a little nervous about bugs and performance but CSharp and Java should be similar enough that hopefully there is no issues. So far the project has not had any bugs from the porting itself as far as i can tell. I plan to do a partial rewrite to consolidate some of the code, improve checkmate detection, and the data structures i was using.

### GitHub Copilot BugTraker :( & naughty list

* Translated a function spatialEquals() to Equals, breaking EnPassent.

* In general Acted Like a menace and only translated ~500 lines before making me do the rest.

* Horrible Indentation/White Space.
  
  
  
  

![Main Menu](https://img.itch.zone/aW1nLzIwMzk5NTU0LnBuZw==/315x250%23c/mehjaE.png)
![T0](https://img.itch.zone/aW1hZ2UvMzM5OTYxNS8yMDM5OTU1Ny5wbmc=/original/uDAuUA.png)
![T0](https://img.itch.zone/aW1hZ2UvMzM5OTYxNS8yMDM5OTU1OS5wbmc=/original/D2RAdS.png)
![T0](https://img.itch.zone/aW1hZ2UvMzM5OTYxNS8yMDM5OTU1OC5wbmc=/original/k0438D.png)



## Features

* GUI For 5d Chess
* All Rules and pieces of the base game implemented
* Ability to load games through PGN/FEN files
* White/Black only views

## Proposed Features (in order of when i might implement)

* Ability to save games to a PGN/FEN files(*Need to port from old project)
* Analysis tools such as sidelines, rewinding and jumping to positions(this is present in other implementations such as chessin5d but not in the base game) (*Need to port from old project)
* Study System (Loading, creating, saving)
* Ability to interface with 5d Engines in a UCI like protocol
* Human V. Bot and Bot V. Bot Matches.
* Variant Creator menu(Shad already made one of these, not sure i will get to this.)

## Limitations

Godot has several issues with FileIO. First the Filedialog node is bugged so you cannot load anything outside of the base res:// Directory. additonally the Godot FileIO is sandboxed to only open files within the res:// directory. In the future this project will port to use CSharp File IO. If you would like to load your own games into the GUI, please clone the repository and add the game locally in your file system under the project files.

## Notation

This main notation that this project uses is Shad Amethyst's notation, which is the unofficial standard notation for 5d Chess. Using this Notation, games can be loaded and will be exportable in this notation in the near future. You can read more about this notation here: <https://github.com/adri326/5dchess-notation/blob/master/README.md>



This project may not be able to load games which have multiple different boards per timeline at the start.

## Other Notations

### PGN5 Notation

Some print statements use the notation that looks like:

> [w.3T0L.a1]

This is similar to my old notation, and is only for debugging purposes, as I have no way to parse this kind of notation at the moment.

## Raw Notation

I am proposing that any chess engines use raw notation, however i do not know if there are any other engines. Raw notation would look like this:

(l<sub>Layer</sub>Tt<sub>Time</sub>)

> (l<sub>Origin</sub>Tt<sub>Origin</sub>)SAN(l<sub>dest</sub>Tt<sub>dest</sub>)SAN

For example the first move of a game would look like: (0T1)e2(0T1)e4. Moves are ordered exactly like Shads notation, with spatial moves coming first ordered by Timeline lowest to highest, then any jumping moves ordered by origin timeline lowest to highest. Finally the exact order that the branching moves were played in.

### Coordinate Notation

This GUI also can parse a custom raw notation, dissimilar to other notations developed by other 5dChess community members. This notation is not meant to be human readable, but rather meant for an engine. I have dubbed this notation __**Coordinate Notation**__.

In a 2d chess engine, you may notice that moves are denoted in long algebraic notation _ie_.

> e2e4

In this gui, notation for a move will look like

> (x<sub>origin</sub>,y<sub>origin</sub>,T<sub>origin</sub>,L<sub>origin</sub>)(x<sub>dest</sub>,y<sub>dest</sub>,T<sub>dest</sub>,L<sub>dest</sub>)
> 
> ex: (3,1,1,0)(3,3,1,0) for 1.e4

With x being the file, y being rank, T being time, and L being Layer/Timeline.A whole Turn will look like

> (x<sub>M1o</sub>,y<sub>M1o</sub>,T<sub>M1o</sub>,L<sub>M1o</sub>)(x<sub>M1d</sub>,y<sub>M1d</sub>,T<sub>M1d</sub>,L<sub>M1d</sub>);(x<sub>M2o</sub>,y<sub>M2o</sub>,T<sub>M2o</sub>,L<sub>M2o</sub>)(x<sub>M2d</sub>,y<sub>M2d</sub>,T<sub>M2d</sub>,L<sub>M2d</sub>)…

With moves separated by semicolons. A promotion would look like

> (x<sub>origin</sub>,y<sub>origin</sub>,T<sub>origin</sub>,L<sub>origin</sub>)(x<sub>dest</sub>,y<sub>dest</sub>,T<sub>dest</sub>,L<sub>dest</sub>)=Q

and castling is denoted normally, with layer added 

> &lt;MOVE&gt;;O-O,L;&lt;MOVE&gt;

The castling assumes there is only 1 unmoved king on the given board. this may need to be changed since this may not be true. However 1 unmoved king per board is standard in most if not all variants.

Lastly, there may be the need to show a null move, which is denoted as:

> 0000

Null moves are used in this project as a way for a player to pass (for example T0)

Formally, the moves should follow an order where destination layer is from top to bottom however this is not enforced by the engine. Branching moves must always be ordered how they occur however.

## Other Projects and resources

**The Best Resource for 5d Chess Programming is AlexBay's Discord Channel: <https://discord.gg/KP5vApW>**

Original 5d Implementation on steam: <https://5dchesswithmultiversetimetravel.com/>

There are several other open source projects that are also being worked on:

Alexbay's GitLab:<https://gitlab.com/alexbay218/5d-chess-js>

Alexbay's 5dChess:<https://chessin5d.net/#/> --Janky and Broken as of 3/24/2025, pray that this is fixed sometime soon.

Shad's Engine:<https://github.com/adri326/5dchess-tools/> 

L0laapk3 5dChess website: <https://multiversechess.com/> (Not Open Source, Very hard to find a game)



## Copyright

This Project was made without reference to source or decompiled code from the software "5d Chess With Multiverse Time Travel" developed by LLC. Thunkspace. All code, assets, libraries and other files in this project were either made by me, or are open source.

The project itself is open source.

For the chess pieces currently i am using CrazyPenguin's open source chess pieces that other 5D Projects use:  <https://gitlab.com/adri326/the-amazing-5d-chess-piece-set>

I also plan to implement other methods of modification of the looks, currently you can only really do a palette swap but in the future I plan to implement fully customizable pieces, chess boards, etc.

In the future, I am planning on adding UI sounds for better game feel, most likely something by Kenny see: <https://kenney.nl/assets/category:Audio?sort=update>


