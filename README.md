# Path-Finder-Drone

## Software prerequisites: 
* Unity Hub (Download it with this [link](https://unity3d.com/get-unity/download))
* Unity3d 2019.3 or above. (currently using 2019.4.10f)
* Visual Studio installed (normally comes on installing Unity)
* All the required packages are installed just by opening with opening the project with appropriate unity version.

-----

## How to run the game:
* Clone this [repository](https://github.com/Hemanth759/Path-Finder-Drone)
* Open the cloned repository with UnityHub by clicking add in unity hub and opening the clone repository folder.
* Open the `Assets` -> `Autonomous Drone` -> `Scenes` then open the `Training for Movement` then open the `Training for Movement` scene by double clicking on it.
* *play*/*Edit* the game if you want and to run the click on play button on middle top of the unity editor.
* To build the cloud point information from the lidar sensor just turn on the `saveToFile` boolean in EnvironmentTraining gameobject in the scene hierarchy and on playing scene in editor should create the cloud point file just after the game directory inside the `cloudpoints\cloudpoints.txt`.
* To visualize the cloud data stored in the `cloudpoints\cloudpoints.txt` just open the `Assets` -> `Autonomous Drone` -> `Scenes` then open the `Training for Movement` then open the `ExternalVisualization` scene then play the scene in editor.
* That should show the cloudpoints in game view and you can roam around the scene using `w\a\s\d` keys and `left mouse button` to free lock the scene.
* To build the program simple go to `file` -> `build settings` -> `build and run` (make sure the build is on pc-standalone or webGL (note webgl is not supported by ML-Agents))
* Select the folder to build the program and wait for the build process to complete.
* Now play the game in your local computer.
