# Algorithmic Object Generation Project

This project aims to explore and implement various algorithmic approaches to generate objects within a defined area. The goal is to:
- Developing algorithms that can efficently populate an area with objects while meeting specific criteria, such as distribution patterns and density,
- Random placements that don't feel random
- Completing execution without major time and space penalties

Ex: Generating a massive and open forest with just a single tree object

Available Methods:

- BruteForce: Goes through the entire area, spot by spot, and checks if a building can be placed

- Iter: Goes through the entire area. After a building is placed, it skips ahead by the minimum distance

- IterRand: Goes through the entire area. After a building is placed, it skips ahead by the minimum distance plus a random amount of extra distance

- Grid(Single): Makes a grid of available spots and fills in random spots until it consecutively fails a certain number of times. Does this one spot at a time

- Grid(Batch): Makes a grid of available spots and fills in batches of random spots. Continues until a set number of failed batches occur

 Full Presentation can be viewed here: https://flpoly-my.sharepoint.com/:p:/g/personal/ddewar3190_floridapoly_edu/EZROJnSt06NMmYjjWrXmz0kBpRQnRYZogIaL6xdpYGouLQ?e=jrXbtN
 An online version of this project can be viewed here: https://manglednight.itch.io/algorithmic-object-generation


