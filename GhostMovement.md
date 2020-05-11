## Ghost movement logic

Alternate Scatter 7 secs / chase 7 secs, duration changes as level increases.

We need direction on the ghost object

Ghosts only turn when at a junction (or corner)

Ghosts reverse direction when switching mode.

Ghosts are always moving towards a target square.

* Is the ghost at a junction
* Ghost checks the valid squares it can move to (can't move backwards)
* Calculates which one is closest to target square
* Turns in that direction


### Scatter mode

Head to their own home square.

### Chase mode

* Blinky - heads to Pacmans square
* Pinky - heads to square 4 moves in front of Pacman (ignoring walls)
* Inky - take two squares in front of Pacman. Take vector from Blinky to that square. Double it
* Clyde - if further than eight cells from Pacman, copy Blinky. Less the eight cells, use scatter mode

### Frightened mode

Turn randomly at a junction

### Eyes mode

After being eaten, target is ghost house

### Initial location

* Blinky always starts outside ghost house
* Pinky starts inside, but immediately leaves
* Inky leaves are 30 dots consumed
* Clyde leaves after 1/3 dots consumed
