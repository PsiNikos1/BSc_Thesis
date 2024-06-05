using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class ForceGenerator : MonoBehaviour{

    
    //Fields for the environment objects
    private GameObject pokingDevice;
    private GameObject plane;
    private GameObject pokingDeviceNose;
    private GameObject pokingDeviceTail;
    private GameObject sphere;
    private Rigidbody pokingDeviceRigidBody;
    private Vector3 pokingDeviceVelocity;
    private Vector3 collisionPoint; 
    private double timeWhenCollisionStarted ;
    private List<Vector3> contactPoints = new List<Vector3>();


    //Fields for calculating the forces
    private double k = 0.1f;
    private double staticFrictionCoEfficient=0.05f;
    private double dumpingConstant=0.08f;
    private double N = 1.0f; //force acted on the needle from the blob
    private double F_striffness = 0;
    private double F_friction = 0;
    private double angleToXAxis =0f;
    private double angleToYAxis =0f;
    List<double> dxList = new List<double>();
    List<double> dyList = new List<double>();
    List<double> forceList = new List<double>();
    List<double> timeList = new List<double>();
    
    public bool linearModelForce;

    List<double> nonlinearForcesList = new List<double>();
    DateTime startTime;
   
    private Vector3 initialPos;
    private bool collidersOverlap = false;
    private Vector3 previousPosition;
    private float deltaTime;


    [HideInInspector]
    public bool loadIsDone=false;
    
    [HideInInspector]
    public double totalForce = 0;



    // Start is called before the first frame update
    void Start()    {
        this.pokingDevice = GameObject.Find("Collider");
        this.plane = GameObject.Find("whatEverObject");
        this.pokingDeviceNose = GameObject.Find("Nose");  
        this.pokingDeviceTail = GameObject.Find("Tail");  
        this.pokingDeviceRigidBody =  this.pokingDevice.GetComponent<Rigidbody>();  
        this.sphere = GameObject.Find("Sphere"); 
        startTime = DateTime.Now;
        this.deltaTime = Time.deltaTime;

        //print("start"+ calculateNonLinearForces(0.188));
    }//end of method   


    void OnCollisionEnter(Collision collision){
        this.initialPos = this.pokingDevice.transform.position;
        this.collidersOverlap= true;
        this.pokingDeviceVelocity=pokingDeviceRigidBody.velocity; 
        if(this.loadIsDone){
            this.timeWhenCollisionStarted = Time.time;
        }

    }//end of method   

    private void OnCollisionStay(Collision collision){
        if (collision.gameObject == this.pokingDevice)
        {
            // Clear the list of contact points
            contactPoints.Clear();

            // Iterate through all contact points to get the contact point(s)
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 contactPoint = contact.point;

                // Add each contact point to the list
                contactPoints.Add(contactPoint);
            }

            // Now you have all the contact points between the sphere and the plane
            
        }
    }

    void OnCollisionExit(Collision collision){

        collidersOverlap = false;
    }//end of method   

    double calculateFstiffness(double dx){ //F_striffness == F_spring
           return this.k * dx;
    }//end of method   

    double calculateFfriction(){
           return (this.staticFrictionCoEfficient + this.dumpingConstant *this.pokingDeviceVelocity.magnitude)*N;
    }//end of method   


    public List<double> getForceAndAngles(){
        List<double> list = new List<double>();
        list.Add(this.totalForce);
        list.Add(this.angleToXAxis);
        list.Add(this.angleToYAxis);
        return list;
    }

  
    public void ExportToCSV(string fileName, List<double> data1, string columnName1, List<double> data2, string columnName2){
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write headers
                writer.WriteLine(columnName1 + "," + columnName2);

                // Write data
                int count = Mathf.Max(data1.Count, data2.Count);
                for (int i = 0; i < count; i++)
                {
                    string value1 = (i < data1.Count) ? data1[i].ToString() : "";
                    string value2 = (i < data2.Count) ? data2[i].ToString() : "";
                    writer.WriteLine(value1 + "," + value2);
                }
            }

            Debug.Log("CSV file exported to: " + filePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error exporting CSV file: " + ex.Message);
        }
    }

   

    private double k_j(double x, int j){
        double[] K_j = { 2.03, 0.438, 0.102 };
        double[] k_j =  { 909.9, 1522, 81.18 };
        return K_j[j] * Math.Exp( k_j[j] * x);

    }
    

    private double calculateNonLinearForces(double x_d, double t){
        // Define parameters
        double K0 = 2.03f;
        double K1 = 0.438f;
        double K2 = 0.102f;
        double b1 = 5073f;
        double b2 = 39.24f;
        double kappa0 = 909.9;
        double kappa1 = 1522;
        double kappa2 = 81.18;
        double k0 = K0*Math.Exp(kappa0*x_d);
        double k1 = K1*Math.Exp(kappa1*x_d);
        double k2 = K2*Math.Exp(kappa2*x_d);

        return x_d * ( k0 + ( k1 * (1 + Math.Exp(-k1*t/b1))) + (k2 * (1 + Math.Exp(-k2*t/b2))));
        
       
    

    }//end of method


    

    // Update is called once per frame
    void Update(){
        //print(collidersOverlap);
      
        // if(collidersOverlap && this.loadIsDone ){
        if(collidersOverlap  ){


            double t = Time.time - this.timeWhenCollisionStarted;



            Vector3 currYPos = this.pokingDeviceNose.transform.position;   
            double dx = Vector3.Distance(currYPos, this.initialPos)/100;
            float deltaTime = Time.deltaTime;
            print("dx"+dx);


            if(this.linearModelForce){

                
                this.F_striffness = calculateFstiffness(dx);
                this.F_friction = calculateFfriction();
                this.totalForce = this.F_striffness + this.F_friction;

                //-----------------------------------GRAPHS FOR THIS MODEL----------------------------------------//

                // this.forceList.Add(this.totalForce);
                // this.timeList.Add(t);
                // this.dxList.Add(dx);

            }
            else{
                
               
                if(dx > 0.003 && dx < 0.005){
                    this.totalForce = calculateNonLinearForces(dx, t);
                    print("F"+totalForce);
                }



            //-------------------------GRAPHS FOR NON LINEAR MODEL -------------------------------------//

                // this.forceList.Add(F );
                // this.dxList.Add(dx);
                // this.timeList.Add(t);
               
            }
             
            //Print the direction of the force.
            foreach (Vector3 contactPoint in contactPoints){
               // Debug.Log("Contact Point: " + contactPoint);
                //Debug.DrawLine(contactPoint, this.sphere.transform.position, Color.red);
                Vector3 directionToCenter = this.sphere.transform.position - contactPoint;
                Ray ray = new Ray(contactPoint, directionToCenter.normalized);
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
                this.angleToXAxis = Vector3.Angle(ray.direction, Vector3.right);  // Angle to X-axis
                this.angleToYAxis = Vector3.Angle(ray.direction, Vector3.up);      // Angle to Y-axis


            }
            
                // Ray ray = new Ray(contactPoints[0], this.sphere.transform.position - contactPoints[0]);
                // Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

            // Ray ray = new Ray(contactPoint, this.sphere.transform.position );
            // //Calculate the angle between the ray and each axis
            // float angleToXAxis = Vector3.Angle(ray.direction, Vector3.right);  // Angle to X-axis
            // float angleToYAxis = Vector3.Angle(ray.direction, Vector3.up);      // Angle to Y-axis
            // float angleToZAxis = Vector3.Angle(ray.direction, Vector3.forward); // Angle to Z-axis

            // // Print the results (in degrees)
            // Debug.Log("Angle to X-axis: " + angleToXAxis);
            // Debug.Log("Angle to Y-axis: " + angleToYAxis);
            // Debug.Log("Angle to Z-axis: " + angleToZAxis);
        
        }//end if
        else{
            this.totalForce = 0;
            this.angleToXAxis = 0;
            this.angleToYAxis = 0;

        }
        
    }//end of method   

    void OnApplicationQuit (){
        // ExportToCSV("C:\\Users\\nikos\\Desktop\\f-dx.csv", this.forceList, "force", this.dxList, "dx");
        // ExportToCSV("C:\\Users\\nikos\\Desktop\\f-t.csv", this.forceList, "force", this.timeList, "time");

        }
}
       

