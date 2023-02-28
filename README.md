# FRC2023_UnityBot
 This simulation for the FRC 2023 game "Charged Up" was created by Team 4572 as a means of familiarizing the driver and operator with the game and operation of the bot. It has also been useful in generating some discussion about how the bot should work and exposing problems that the team needs to address in real life. The differential drive bot depicted here is a simplified representation of the one we plan to complete with this season. All robot behaviors are implemented within Unity and there is no connection to robot code (e.g., usng WebSockets to WPILIB) currently implemented.

 NOTE - Operation of this game currently requires use of a LogiTech F310 game pad

 ## Driver Controls
 * Left Stick - Speed
 * Right Stick - Turn
 * Right Bumper - Auto align to nearest gamepiece seen by claw camera.
 * Left bumper - open/close the claw

 ## Operator Controls
 * Move arm to grid top row - "Q" on the keyboard
 * Move arm to grid middle row - "A" on the keyboard
 * Move arm to grid bottom row - "Z" on the keyboard
 * Move arm to floor pick-up position - "F" on the keyboard
 * Move arm to double substation pick-up position - "S" on the keyboard.
 * Move arm to "home" position on the robot - "space bar" on the keyboard
 * Manually open/close the claw - "C" on the keyboard.
 * Add cone gamepiece at the dual substation - "O" on the keyboard
 * Add cube gamepiece at the dual substation - "U" on the keyboard

 ## Cameras
 * Display 1 - shows driver's viewpoint from behind the grid. This display also includes a small version oof the view from the camera attached to the robots claw.
 * Display 3 - an audiance level view of the field and the bot.

## Automatic Features
* If claw is open and a gamepiece is detected inside the claw, the claw will automatically close to grab it.
* The right bumper button will cause the bot to align to the nearest gamepiece within view of the claw camera. If the bot is stationary, it will automatically pivot to the piece. If the driver holds down the right bumper and drives forward with the left stick, the bot will autosteer to the game piece and capture it if the claw is open. 

 ## Known Issues/To Do
 * Cones are not hollow and cannot be placed on nodes. They will fall off rather than go on.
 * Claw needs to have colliders added so that a piece cannot be picked up from the side.
 * Charging station not functional and needs to have hinge mechanism added
 * No way to add a new gamepiece to the field. Plan to make this a key press operation which will add a cone to the double substation.
 * Position of elements on the field is close, but may not be perfect.
 * Need to be able to place game piece on the robot when the simulation starts.
 * Probably a bunch of stuff as this is very much a work in progress

