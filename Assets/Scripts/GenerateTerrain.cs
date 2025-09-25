using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    public enum genType {Bruteforce, Grid, Iter, IterRand}
    public genType GenerationMethod;
    [SerializeField] private Vector2Int Area;
    [SerializeField] private GameObject Building;
    [SerializeField] [Range(0,1)] private float spawnProb;
    [SerializeField] private int minDistance;
    private Vector2Int scaledArea;
    private List<GameObject> allObjects;
    private Vector3 scaler;
    [SerializeField][Header("Variables For Rand")] private int maxRandStep = 10;
    [SerializeField] [Header("Variables For Grid")] private int minCheck;
    [SerializeField] private int TotalCount;
    [SerializeField] private int batchSize;
    [SerializeField] [Header("If Grid Gen and false, give a Batch Size")]private bool Single;
    private Vector3 Offset;

    [SerializeField] private TextMeshProUGUI DurationOutput;
    [SerializeField] private TextMeshProUGUI CountOutput;

    void Update() { transform.Rotate(10 * Time.deltaTime * Vector3.up); }
    void OnValidate()
    {
        //Variable Checks so that they are not negative
        if (Area.x < 0) { Area.x = 1; }
        if (Area.y < 0) { Area.y = 1; }
        if (minCheck < 0) { minCheck = 1; }
        if (TotalCount < 0) { TotalCount = 1; }
        if (batchSize < 0) { batchSize = 1; }
        if (minDistance < 0) { minDistance = 0; }
        if (maxRandStep < 0) { maxRandStep = 1; }
    }

    public void UIGen(int genMethod, int[] values, float prob, bool batchtoggle)
    {
        Area.x = values[0];
        Area.y = values[1];
        minDistance = values[2];
        maxRandStep = values[3];
        minCheck = values[4];
        TotalCount = values[5];
        batchSize = values[6];
        spawnProb = prob;
        Single = !batchtoggle;

        switch (genMethod)
        {
            case 0:
                GenerationMethod = genType.Bruteforce;
                break;

            case 1:
                GenerationMethod = genType.Iter;
                break;

            case 2:
                GenerationMethod = genType.IterRand;
                break;

            case 3:
                GenerationMethod = genType.Grid;
                break;
        }
        Generate();
    }

    public void Generate()
    {
        ClearChildren();
        allObjects = new List<GameObject>();

        //Generates a floor so buildings aren't floating in the air
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.transform.SetParent(transform);
        floor.transform.localScale = new Vector3(Area.x, Area.y, 1);
        floor.transform.rotation = Quaternion.Euler(90, 0, 0);
        //floor.transform.position = new Vector3(Area.x/2, 0, Area.y/2);

        //Scale the Area based on the object size
        scaler = Building.transform.localScale;
        scaledArea = new Vector2Int(Area.x / (int)scaler.x, Area.y / (int)scaler.z);

        //Makes so that buildings don't hang off the edge of the area
        Offset = scaler * 0.5f;

        DurationOutput.text = $"Generating....";
        CountOutput.text = $"Generating....";

        var stopwatch = Stopwatch.StartNew();
        switch (GenerationMethod)
        {
            case genType.Bruteforce:
                BruteForce();
                break;

            case genType.Iter:
                Iter();
                break;

            case genType.IterRand:
                IterRand();
                break;

            case genType.Grid:
                Grid();
                break;
        }
        stopwatch.Stop();

        if (GenerationMethod is genType.Grid)
        {
            CountOutput.text = $"Buildings Spawned: {allObjects.Count}/{TotalCount}";

        }
        else
        {
            CountOutput.text = $"Buildings Spawned: {allObjects.Count}";
        }

        DurationOutput.text = $"Generation Time: {stopwatch.ElapsedMilliseconds} ms";
    }



    void BruteForce()
    {
        //Iterate through total area
        for(int i = -scaledArea.y/2; i < scaledArea.y/2; i++)
        {
            for(int j = -scaledArea.y/2; j < scaledArea.y/2; j++)
            {
                //Check If Conditions Are Met to Spawn an Object at this Point
                if(CanSpawn(new Vector3(j * scaler.x, 0, i * scaler.z )))
                {
                    //Probability Check to spawn object
                    if(Prob(spawnProb))
                    {
                        //Spawn Object at this point
                        allObjects.Add(
                            Instantiate(
                                Building, 
                                new Vector3(j * scaler.x, 0, i * scaler.z ) + Offset, 
                                Quaternion.identity,
                                this.transform
                                )
                            );
                    }
                }
            }
        }
    }


    void Iter()
    {
        Vector2Int skip = new Vector2Int(minDistance/(int)scaler.x, minDistance/(int)scaler.z);
        bool skipAhead = false;

        for(int i = -scaledArea.y/2; i < scaledArea.y/2; i++)
        {
            for(int j = -scaledArea.y/2; j < scaledArea.y/2; j++)
            {
                //Probability Check to spawn object
                if(Prob(spawnProb))
                {
                    //Spawn Object at this point
                    allObjects.Add(
                        Instantiate(
                            Building, 
                            new Vector3(j * scaler.x, 0, i * scaler.z ) + Offset, 
                            Quaternion.identity,
                            this.transform
                            )
                    );
                    skipAhead = true;
                    j += skip.x;
                }
            }
            if(skipAhead) { skipAhead = false; i += skip.y; }
        }
    }

    void IterRand()
    {
        Vector2Int skip = new Vector2Int(minDistance/(int)scaler.x, minDistance/(int)scaler.z);
        bool skipAhead = false;

        for(int i = -scaledArea.y/2; i < scaledArea.y/2; i+= Random.Range(1, maxRandStep+1))
        {
            for(int j = -scaledArea.y/2; j < scaledArea.y/2; j+= Random.Range(1, maxRandStep+1))
            {
                //Probability Check to spawn object
                if(Prob(spawnProb))
                {
                    //Spawn Object at this point
                    allObjects.Add(
                        Instantiate(
                            Building, 
                            new Vector3(j * scaler.x, 0, i * scaler.z) + Offset, 
                            Quaternion.identity,
                            this.transform
                            )
                    );
                    skipAhead = true;
                    j += skip.x;
                }
            }
            if(skipAhead) { skipAhead = false; i += skip.y; }
        }
    }

    void Grid()
    {
        //Generate a boolean grid of the scaled area
        bool[,] spawnPoints = new bool[scaledArea.x, scaledArea.y];
        int failCount = 0;
        int total = 0;

        while(total != TotalCount && failCount != minCheck)
        {
            //If we are doing single points or a batch of points
            if(Single)
            {
                //Generate a random positon to spawn at within the area range
                Vector3Int pos = new(Random.Range(-scaledArea.x/2, scaledArea.x/2), 0, Random.Range(-scaledArea.y/2, scaledArea.y/2));
                Vector3Int arrPos = new(pos.x + scaledArea.x / 2, 0, pos.z + scaledArea.y / 2);

                //Check if we meet minDistance requirement, if fail increase the fail count
                if (!SpawnCheck(arrPos, spawnPoints)) { failCount++; continue; }
                else
                {
                    failCount = 0;
                    if(Prob(spawnProb))
                    {
                        //Decrease the total and mark that point as true on the grid
                        total++;
                        spawnPoints[arrPos.x, arrPos.z] = true;
                        
                        //Spawn at this point
                        allObjects.Add(
                        Instantiate(
                            Building, 
                            new Vector3(pos.x * scaler.x, 0, pos.z * scaler.z) + Offset, 
                            Quaternion.identity,
                            this.transform
                            )
                        );
                    }
                }

            }
            else
            {
                //Get a batch of points within the area
                Vector3Int[] batch = new Vector3Int[batchSize];
                for(int i = 0; i < batchSize; i++) 
                    { batch[i] = new(Random.Range(-scaledArea.x/2, scaledArea.x/2), 0, Random.Range(-scaledArea.y/2, scaledArea.y/2)); }
                
                //Check all points within a batch
                int batchFail = 0;
                foreach(Vector3Int pos in batch)
                {
                    Vector3Int arrPos = new(pos.x + scaledArea.x / 2, 0, pos.z + scaledArea.y / 2);
                    if (total == TotalCount || failCount == minCheck) { break; }
                    //Check if we meet minDistance requirement, if fail increase the batchfail count
                    if (!SpawnCheck(arrPos, spawnPoints)) { batchFail++; continue; }
                    else
                    {
                        if(Prob(spawnProb))
                        {
                            //Decrease the total and mark that point as true on the grid
                            total++;
                            spawnPoints[arrPos.x, arrPos.z] = true;
                            
                            //Spawn at this point
                            allObjects.Add(
                            Instantiate(
                                Building, 
                                new Vector3(pos.x * scaler.x, 0, pos.z * scaler.z) + Offset, 
                                Quaternion.identity,
                                transform
                                )
                            );
                        }
                    }
                }
                
                ///if all points within a batch fails, increase the fail counter
                if(batchFail == batchSize) { failCount++; }
            }
        }
    }

    private bool SpawnCheck(Vector3Int p, bool[,] Area)
    {
        Vector2Int skip = new(minDistance/(int)scaler.x, minDistance/(int)scaler.z);
        
        for(int i = -skip.y; i <= skip.y; i++)
        {
            for(int j = -skip.x; j <= skip.x; j++)
            {
                if(p.x + j < 0 
                    || p.z + i < 0 
                    || p.x + j >= scaledArea.x-1 
                    || p.z + i >= scaledArea.y-1
                    || (i == 0 && j == 0)) 
                { continue; }

                if(Area[p.x + j, p.z + i]) { return false; }
            }
        }
        
        return true;
    }

    private bool CanSpawn(Vector3 pos)
    {
        //Do a check here to see if we can spawn an object at that point
        foreach(GameObject b in allObjects)
        {
            if(Vector3.Distance(b.transform.position, pos) <= minDistance + scaler.x)
            {
                return false;
            }
        }

        return true;
    }

    private bool Prob(float p)
    {
        float var = Random.value;
        return var < p;
    }

    private void ClearChildren()
    {
        foreach(Transform c in transform)
        {
            if(c.CompareTag("MainCamera")) { continue; }
            Destroy(c.gameObject);
        }
    }
}
