# 단풍 \<Danpung\>
**Danpung** is a rhythm game and autumn walking simulator, inspired by how I enjoy tapping along to music while walking around UNC's autumn campus.

My submission for the UNC Game Dev Club's [2024 Collegiate Fall Game Jam](https://itch.io/jam/2024-collegiate-fall-game-jam).

Here's the [link to my submission](https://axolotl-1.itch.io/danpung).

This project was made in Unity with C#, using the NAudio and ML.NET libraries. Inside, it uses two binary classification ML models to create playable tracks for user-inputted songs. This was a solo project, with the exception of the song included in the final release (huge props to my friend Geethan for making that song).

## Instructions
To install, just unzip the .zip file and run the `Fall 2024 game jam.exe` file. To play the game, press "Play" in the main menu, and in-game press the up arrow to hit upper beats, down arrow to hit lower beats, and escape to pause.

## Custom music
This is the big part of this game, where you can upload your own music. I definitely spent half the duration of the game jam getting the ML model set up to recognize beats and generate levels, and while its results are still a little funky I'm proud of what I learned. To use the feature, if you hit the "Open Music Folder" button, youll be directed to a spot on your computer where you can put your own mp3 files, and back in-game just click the name of the current song until it displays the name of the one you want to play. After that, for the first time you should see a button called "Generate Song", and as its machine learning model generates a playable track, the game will freeze for about 2-3 minutes depending on the song length. After that, just hit Play and you're good to go.

## Credits
Again, masive thanks to Geethan for making the song in the game. I wouldn't have felt comfortable submitting without an example song, and he provided.

All assets in the game of are my own creation, except for the song included, and the font used. Links below credit the asset and library used, as well as some informational material that was key to the creation of this project.

[Naudio library](https://github.com/naudio/NAudio) for audio manipulation in C#

[Simply Rounded Font](https://font.download/font/simply-rounded) is the the font you see the few times in the game

[ML.net Beginner Tutorial](https://www.youtube.com/watch?v=R3kRf7hNVMg) was a very solid introduction to ML.NET, even though I didn't exactly use this in my final version

[Simple Blender Character Tutorial](https://www.youtube.com/watch?v=-XYryP_GU8o) was a big big big inspiration for creating my blender character & was a plethora of blender recap that supported this project

I also used a few songs to train the ML model in the game, those being Geethan's song, Love by Keyshia Cole, MASCARA by King Gnu, Reckless Driving by Lizzy Alpine, and Kick, Push by Lupe Fiasco.
