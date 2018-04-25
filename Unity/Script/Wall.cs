using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Wall : MonoBehaviour {
    public GameObject brickPrefab; // Reference to the brick in Prefab folder
    public GameObject ballPrefab; // Reference to the ball in Prefab folder
    private int numBrickX = 4; // Initial width of your wall
    private int numBrickY = 5; // Initial height of your wall
    private float forceMultiplier = 1700;// Force applied to the balls

    public ArrayList balls = new ArrayList(); // We store here the thrown balls so we can delete some if there are too many of them
    //public ArrayList briques = new ArrayList();
    //public ArrayList coordonnees = new ArrayList();
    public DateTime initDate, initDate2, initDate3;
    public DateTime localDate;
    public Text minuteur;

    private int shift = 0;
    public int decompte = 15; //Time you have to destroy the wall at each level
    private int delay = 15; //delay =decompte
    private int level = 1;

    GameObject empty;
    OpenCVGetMseFromWebcam lesmse;

    public int mse_h_g, mse_h, mse_h_d; // Contient la valeur des mse

    private void Awake(){
        Cursor.visible = true;
        empty = GameObject.Find("GameObjectPrincipal");
    }

    void GetMse() //Value of the mean square error of the three square in the opencv window
    {
        lesmse = empty.GetComponent<OpenCVGetMseFromWebcam>();
        mse_h_g = lesmse.mse_h_g;
        mse_h = lesmse.mse_h;
        mse_h_d = lesmse.mse_h_d;
    }

    public void DestroyAllPrefabs()
    {

        var mur = GameObject.FindGameObjectsWithTag("wall");
        foreach (var elements in mur)
        {
            Destroy(elements);
        }
        var sphere = GameObject.FindGameObjectsWithTag("ball");
        foreach (var elements in sphere)
        {
            Destroy(elements);
        }

    }

    void GameOver()
    {//Skip to the next level
        DestroyAllPrefabs();
        level++;

        /* First level 15s to break the wall; 
           Second one we double the widht of the wall and 10s to do it;
           Third one height+6 and 5s to do it.
        */
        if (level ==2)
        {
            decompte = 10;
            delay = decompte;
            numBrickX = numBrickX * 2;
        }
        else if(level ==3)
        {
            decompte = 5;
            delay = decompte;
            numBrickY = numBrickY + 3;
        }
        else
        {
            delay = decompte;
        }

        Start();
    }

    void SetTimer() {// Starts a countdown in seconds. After that you move to the next level with GameOver()
        if ((localDate - initDate).TotalSeconds > 1)
        {
            delay -= 1;
            initDate = localDate;
            if (delay < 0)
            {
                GameOver();
            }
        }
        minuteur.text = delay.ToString();
        }

    void DoWall()
    {// build a wall of bricks
        if (brickPrefab != null)
        {
            // On vérifie que la brique est référencée
            // D'abord, on récupère les dimensions de la brique

            Vector3 brickSize = brickPrefab.GetComponent<Renderer>().bounds.size;
            // Ensuite, on récupère la position de la brique
            float X = brickPrefab.transform.position.x + brickSize.x;
            float Y = brickPrefab.transform.position.y;
            float Z = brickPrefab.transform.position.z;
            // Enfin, on récupère l'orientation de la brique
            Quaternion brickOrientation = brickPrefab.transform.rotation;

            // Une fois qu'on a récupéré les caractéristiques de la brique de base,
            // On peut créer les autres pour faire le mur

            for (int i = 0; i < numBrickY; i++)
            { // Height

                for (int j = 0; j < numBrickX; j++)
                { // Width

                    if (i == 0 && j == (numBrickX - 1)) break;
                    // On crée une nouvelle instance de la brique
                    GameObject brique = Instantiate(brickPrefab, new Vector3(X, Y, Z), brickOrientation);

                    /* //Future development: if all bricks have moved you won the game
                    briques.Add(brique);
                    Vector3 coodonneeInit = brique.transform.position;
                    coordonnees.Add(coodonneeInit);
                    */
                    
                    // We move toward the right to add bricks
                    X += brickSize.x;
                }
                // Once we have finished a line me move up
                X = brickPrefab.transform.position.y;
                Y += brickSize.y;

            }
        }
        return;
    }

    public void shoot_ball(int shift)
    {
        /* The behaviour of the game in Unity is different than once built. When built if you want :
                the ball, to shoot the wall in the middle you need y = 500
                the first ball you throw, to be in the middle of the screen you need x = 400 here and Xcamera = 3
           To test the game in Unity you can take y=200, leave x the same, and change shift to 17
        */
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(400 + shift, 500, 10));

        // Instancie the sphère acccording to main camera position
        GameObject ball = Instantiate(ballPrefab, ray.origin,
                                       Quaternion.identity);
        // Add force to project the bullet
        ball.GetComponent<Rigidbody>().AddForce(ray.direction *
        forceMultiplier);
        // Add the ball to the array where all balls are stored
        balls.Add(ball);
    }

    void Start () {
        //StartCoroutine(Go(5f));
        initDate = DateTime.Now;
        initDate2 = DateTime.Now;
        SetTimer();
        DoWall();
    }
    
    // Update is called once per frame
    void Update()
    {
        GetMse();
        SetTimer();
        //we throw a ball every 1/10 seconds
        localDate = DateTime.Now;

        if (ballPrefab != null && (localDate - initDate2).TotalSeconds > 0.1 )
        {
            if (mse_h_g > lesmse.threshold && mse_h_g > mse_h_d && mse_h_g > mse_h)
            {
                shift -= 40;
                shoot_ball(shift); //move the beam on the left
            }
            if (mse_h_d > lesmse.threshold && mse_h_d > mse_h_g && mse_h_d > mse_h)
            {
                shift += 40;
                shoot_ball(shift); //move the beam on the right
            }
            if (mse_h > lesmse.threshold && mse_h > mse_h_g && mse_h > mse_h_d)
            {
                shoot_ball(shift); //shoot straight forward
            }

            // S'il y a trop de boulets, on supprime le premier arrivé
            if (balls.Count > 4)
            {
                UnityEngine.Object removeMe = balls[0] as UnityEngine.Object;
                balls.RemoveAt(0); // D'abord on l'enlève de la liste
                Destroy(removeMe); // Ensuite, on le détruit
            }
            initDate2 = DateTime.Now;
        }

    }

    IEnumerator Go(float waitTime)
    {
        print("coRoutine started" + Time.time);

        yield return new WaitForSeconds(waitTime);

        print("coRoutine clodes" + Time.time);

    }
}
