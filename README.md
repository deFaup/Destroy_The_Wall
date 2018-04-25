# Destroy_The_Wall
A simple Unity game powered by OpenCV (computer vision library)

In this game you have to destroy a wall of bricks by throwing balls at it !
It requires a webcam. If you have more than one webcam activated I suggest that you desactivate them, to only keep on the one which you are going to play with.

The program will open a window in which the images taken by your webcam will appear. 
The upper part of those images is divided in three squares. 
To throw balls you will need to move your hands in front of these squares. If the borders of the square remains green it meands your hand hans't been detected, if it turns red your hand has been detected.

The left sided square allows you to move the beam of balls to the left and throw balls. The right square do the same but for the right =)
Finally the square in the middle throw balls straight at the last position of your beam.

When you start the game, moving your hands in the middle square will throw a ball directly into the wall

To stat the game(Windows only right now as I can't build it for MAC OS on my Unity) you just need to click on the Unity window!
(The window that OpenCV opens, pauses the game) 
