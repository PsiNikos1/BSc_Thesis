using System;
using System.Collections; 
using System.IO;
using UnityEngine;

public class GraphGenerator : MonoBehaviour{

    private bool collidersOverlap = false;
    private GameObject pokingDevice;
    private GameObject plane;
    private DateTime  startTime;
    private DateTime  endTime;
    private ArrayList xCoordinates = new ArrayList();
    private ArrayList yCoordinates = new ArrayList();
    private ArrayList zCoordinates = new ArrayList();

    private ArrayList springForce = new ArrayList();
    private ArrayList timeFrames = new ArrayList();


    [SerializeField]
    public ForceGenerator springForceScript;

    // Start is called before the first frame update
    void Start(){
        this.pokingDevice = GameObject.Find("Nose");
        this.plane = GameObject.Find("whatEverObject");
    }

    void OnCollisionEnter(Collision collision){

        collidersOverlap = true;
        this.startTime = DateTime.Now;

    }

    void OnCollisionExit(Collision collision){

        collidersOverlap = false;

        this.endTime = DateTime.Now;
        print(this.startTime - this.endTime);
        createCsvFile("x", this.xCoordinates, "time", this.timeFrames, "x-t.csv");
        createCsvFile("y", this.yCoordinates, "time", this.timeFrames, "y-t.csv");
        createCsvFile("z", this.xCoordinates, "time", this.timeFrames, "z-t.csv");
        createCsvFile("F", this.springForce, "time", this.timeFrames, "F-t.csv");



    }

    void createCsvFile(string label1, ArrayList arr1, String label2, ArrayList arr2, string fileName){
    
        using (StreamWriter writer = new StreamWriter(fileName)){
            // Write the header row with column names if needed
            writer.WriteLine(label1, label2);

            // Iterate over the lists and write the data to the CSV file
            for (int i = 0; i < arr1.Count; i++){
                writer.WriteLine($"{arr1[i]},{arr2[i]}");
            }
        }


    }



    // bool collidersOverlapCollider( GameObject a, GameObject b){
    //     Vector3 direction ;
    //     float distance ;
    //     bool overlapped = Physics.ComputePenetration(a.GetComponent<Collider>(), a.GetComponent<Collider>().transform.position, a.GetComponent<Collider>().transform.rotation,
    //                                                  b.GetComponent<Collider>(), b.GetComponent<Collider>().transform.position, b.GetComponent<Collider>().transform.rotation,
    //                                                  out direction, out distance);
                
    //     //print(overlapped);
    //     return overlapped;
    // }

    // Update is called once per frame
    void Update(){

        if(this.collidersOverlap){
            TimeSpan currentTimeElaped = DateTime.Now - this.startTime;

            this.xCoordinates.Add(this.pokingDevice.transform.position.x);
            this.yCoordinates.Add(this.pokingDevice.transform.position.y);
            this.zCoordinates.Add(this.pokingDevice.transform.position.z);
            this.springForce.Add(this.springForceScript.totalForce);
            this.timeFrames.Add(currentTimeElaped);

            
        }    



    }
}
