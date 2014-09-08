Graphics and Interaction
Project 1
Mitchell Brunton #537642

Controls:
Use WSAD keys for forward/back/left/right movement. 
Use QE keys for rolling CCW/CW. 
Use mouse for arbitrary camera rotation.
Note: 	Attempting to exit landscape bounds will reposition camera at 
	world start point.

Implementation:
In order to generate random terrain, I used the diamond-square algorithm
to determine a square grid of altitudes. The size of this grid is a
function of worldSize (the side length of one side of the square landscape),
in such a way so as to have a constant number of altitude points per unit
area, regardless of how large the world is. 

In order to prevent the camera from venturing underground, I calculated its
"bounding square" each frame. The maximum y-value of the vertices of this 
square was taken to be the ground height below the camera. If the camera
was within a certain buffer distance of this height, it was shunted upwards
so that it was above it. The process of finding a bounding square was made 
fast by keeping track of the (i,j) indices of the previous bounding square.  
Since squares were kept in a grid formation, I was able to search outwardly 
in rings from the previous bounding square, to find the new bounding square.  

Lighting was implemented by updating the direction vector of two HeavenlyBody 
instances, sun and moon, each with their own strength properties. Each 
subclass of the GameObject class has its own Ambient, Diffuse, and Spectral
properties.

The ocean maintained a list of waves (to allow for wave motion in the future)
with an alpha value allowing transparency. The camera is allowed to venture
underwater, and the user will notice appropriate coloring of the seabed, and
realistic vision when looking upwards from underwater.


Settings/Performance:
If performance is an issue, please adjust the "size" float within the World
constructor (landscape section), which is passed to the Landscape constructor,
to a lower value.
If you don't want to sacrifice size, turn up Landscape.sizePerPoint for
fewer land polygons per unit area.
Feel free to adjust the rockiness float (same location) to a different
value to experience different levels of rockiness.
Also feel free to change roundedLandscape to false within World's constructor,
to experience a cool retro feel.
If the full screen annoys you, please alter GraphicsDevice.IsFullScreen
within Project1Game.Initialize, and uncomment the two lines setting
the backbuffer width/height. Please don't alter the line preventing window
resizing, for reasons detailed in the Notes section below.

TODOs:
While I have met the specifications in this project, there are a number
of improvements I would still like to make:

- Fix Wave.Update, so that if movingWaves is set to true, waves oscillate
in a realistic fashion (set movingWaves to true now if you wish to see what
unrealistic waves look like!)

- Improve Landscape.getGroundHeight so that we can calculate the height at
a particular (x,z) coord rapidly, without having to know where an object
was recently. 

- Use the improved Landscape.getGroundHeight method to filter out those
ocean vertices which are below groundlevel


Notes: 
	The MouseManager.getMouseState.X/Y floats are in the range
	[0,1] (if the mouse is within the app window). If the app
	window is active, I use the 
	MouseManager.setMousePosition(float x, float y) method to
	set the mouse back to the centre of the screen (0.5f, 0.5f) 
	after each update call. This allows users to rotate the camera
	with the mouse without being limited by the dimensions of their
	display. SharpDX appears to have a bug, in which the logical centre
	of the app window (0.5f, 0.5f) can change if the screen is resized 
	while the app is running. This has the effect of the camera rotating
	even when the mouse is stationary. To prevent this, I have set
	the Game.Window.AllowUserResizing to false. 


Code from other sources:
I used a .gitignore file for Visual Studio (which I modified slightly)
source: https://github.com/github/gitignore/blob/master/VisualStudio.gitignore


