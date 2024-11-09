# racing-neural-network

I've always been fascinated by racing AI's. My first linear autopilot was in Scrap Mechanic in 2018. I was pretty much interested in the concept for a long time compared to my life.

I wanted to write an AI algo after i saw people made racing AIs for Trackmania.

I watched a lot of 3Blue1Brown, Reducible and Artem Kirsanov to learn the basics of it.

I wasn't interested in just importing a python library to do this. So i just did it by myself from scratch.

I had lots of issues with the learning algo altough it looks fairly simple.

People made lot's of recognition scripts and i was not interested in the matter.

I wanted to make an AI that could learn how to drive at first.

Update 11/9/2024 I have achieved that yesterday.

I started this project i think this tuesday. And in about 4 days of work i learned a lot of things that actually made me even more interested at every bump.

The weird thing about this also is that whatever you search for is fairly recent or just haven't been touched since 80s.

That made me feel like i could realize somehing myself.

Since the racing AI market arent that big but may get me my dream job, I am going to continue on this AI with a MIT license.

I'm hoping a commercial recognition if I continue; but honestly, i just love this.


HOW DOES IT WORK?

There are 4 main classes for the network, all made for the learning simplicity so dont expect any optimization.
So that in C# I could not use the Python NumPy or any mathmetical libraries, the matrix functions are just object operations.
And since the game runs on two update types it is not such a problem.
Every object holds an ID except connections. You can get a node by it's ID.

Neural Node: Holds the connection objects from back and forth, has an input variable.

Neural Connection: Holds the two nodes and the connection weight. Symetrical.

Neural Layer: Initializes the nodes and holds them.

Neural Network: Holds everything, has the functions to train or tease. Initializes layers.
