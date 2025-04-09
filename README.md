V-Arcade is a framework with serious games to support upper limbs rehabilitation for children.

This framework has been developed with Unity 2017 and it is compatible with Leap Motion V2, MacOs and Windows. 

The framework contains:

- A "children app"  with four serious games supporting wrists and forearm rehabilitation (ulnar and radial deviation, flexion and extension, pronation and supination of the forearm)

- A "therapist app"  specific for the therapist to manage children's progresses remotely and a server to store data about the exercises' performed by children

This github contains the Unity project with both the therapist and the child applications that can be build separately.

The four serious games are: Rich Race, Music Beats, Alien Invasion and Shooting Gallery; each serious game offers different physiotherapy exercises. 
The child plays with the game through the Leap Motion Controller that tracks child's hand movements converting it in movements of the character. In Rich Race the child controls a car and has to drive in a circuit, in Music Beats the child plays with both hands and should beat them following music rythm, in Shooting Gallery the child controls the pointer of a gun and has to hit each target with the pointer, in Alien Invasion the child controls a spaceship and has to shoot to all the aliens.

The four SG, from top left to bottom right: Rich Race, Music Beats, Shooting Gallery and Alien Invasion:
![alt text](https://github.com/eleKit/v-arcade/blob/master/4-serious-games.png)


The hand tracking data are collected through the leap motion (the Leap frames) and they can be stored and saved in a server allowing the therapist to watch through the therapist app a digital hand performing the same gestures made by the child. 

A screenshot of the replay of a Rich Race game from therapists' application:
![alt text](https://github.com/eleKit/v-arcade/blob/master/car-replay.png)

The therapist, with their app, can also personalize the exercises by generating new levels for each game in addition to the standard-preloaded levels present in each game: the therapist can decide the amplitude of the curve that the child should perform wioth the hand movement and the direction of the movement (if moving the hand to the left or to the right).

A screenhot of the level generation of an Alien Invasion exercise from therapists' application:
![alt text](https://github.com/eleKit/v-arcade/blob/master/Space-path-generator.png)


Please cite as:

[E. Chitti et al., "V-Arcade: design and development of a serious games framework to support the upper limbs rehabilitation," 2021 IEEE 9th International Conference on Serious Games and Applications for Health(SeGAH), Dubai, United Arab Emirates, 2021, pp. 1-8, doi: 10.1109/SEGAH52098.2021.9551858](https://ieeexplore.ieee.org/document/9551858)

[E. Chitti et al., "Evaluation of the V-Arcade serious games framework to support upper limbs rehabilitation at home for children with Juvenile Idiopathic Arthritis," 2022 IEEE 10th International Conference on Serious Games and Applications for Health(SeGAH), Sydney, Australia, 2022, pp. 1-6, doi: 10.1109/SEGAH54908.2022.9978554](https://ieeexplore.ieee.org/document/9978554)
