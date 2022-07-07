// System Library
using System;
using System.IO;
using System.Xml.Serialization;
// Graphics Engine Library
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private protected byte pause = 0;

    // Player Controll Variables
    private const string PLAYER_OBJECT_KEY = "Player";

    [SerializeField] private protected Vector2[] playerLastSafePosition = new Vector2[2];
    private protected GameObject[] sisters = new GameObject[2];
    private protected byte currentPlayerSelected = 0;

    private protected float currentSpeed = 0;
    [SerializeField] private protected float[] acceleration = new float[2], startSpeed = new float[2], maxSpeed = new float[2];
    [SerializeField] private protected Vector2 currentMoveDirection = Vector2.zero;
    [SerializeField] private protected bool inMovement = false;

    [SerializeField] RaycastHit2D currentRay;

    // Save System Variables
    private protected const string FILE_LOCATE_SAVE_KEY = "/SaveData/save";
    private protected readonly string[] current_save_file = new string[3];

    // Awake is called before the scene starts
    private protected void Awake() {
        for(int i = 0; i < 3; i++) {
            current_save_file[i] = Application.dataPath + FILE_LOCATE_SAVE_KEY + (i + 1) + ".dat";
            if(!File.Exists(current_save_file[i])) File.Create(current_save_file[i]);
        }
    }

    // Update is called once per frame
    private void Update() {
        if(pause != 1) {
            if(sisters[currentPlayerSelected] == null) {
                sisters[currentPlayerSelected] = GameObject.Find(PLAYER_OBJECT_KEY + currentPlayerSelected.ToString());
                switch(currentPlayerSelected) {
                    case 0: currentPlayerSelected = 1; break;
                    case 1: currentPlayerSelected = 0; break;
                }
            }
            else 
                MovementSister();

            if(inMovement == false && Input.GetKeyDown(KeyCode.P)) ChangeSister();
        }

        // line of test
        if(Input.GetKeyDown(KeyCode.R)) LoadScene(String.Empty,1);
    }

    // Sets the chosen direction based on player input
    private protected Vector2 MoveDirecion() {
        if(Input.GetAxisRaw("Horizontal") > 0) return Vector2.right;
        else if(Input.GetAxisRaw("Horizontal") < 0) return Vector2.left;
        else if(Input.GetAxisRaw("Vertical") > 0) return Vector2.up;
        else if(Input.GetAxisRaw("Vertical") < 0) return Vector2.down;
        else return Vector2.zero;
    }

    protected private void MovementSister() {
        if(inMovement == true) {
            sisters[currentPlayerSelected].gameObject.GetComponent<Rigidbody2D>().velocity = currentMoveDirection * currentSpeed;
            
            if(currentMoveDirection == Vector2.up)
                currentRay = Physics2D.Raycast(sisters[currentPlayerSelected].transform.localPosition,Vector2.up,0.5f,1 << 3);
            else if(currentMoveDirection == Vector2.down)
                currentRay = Physics2D.Raycast(sisters[currentPlayerSelected].transform.localPosition,Vector2.down,0.5f,1 << 3);
            else if(currentMoveDirection == Vector2.left)
                currentRay = Physics2D.Raycast(sisters[currentPlayerSelected].transform.localPosition,Vector2.left,0.5f,1 << 3);
            else
                currentRay = Physics2D.Raycast(sisters[currentPlayerSelected].transform.localPosition,Vector2.right,0.5f,1 << 3);

            if(currentRay.collider) {
                currentMoveDirection = Vector2.zero;
                inMovement = false;
            }
        }

        // the player can only change the direction when the extended direction vector is equal to zero
        if(currentMoveDirection == Vector2.zero) {
            sisters[currentPlayerSelected].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            currentSpeed = 0;

            if(MoveDirecion().x != 0)
                currentMoveDirection.x = MoveDirecion().x;
            else if(MoveDirecion().y != 0)
                currentMoveDirection.y = MoveDirecion().y;
        }
        else {
            inMovement = true;

            // Acceleration of speed
            if(currentSpeed > maxSpeed[currentPlayerSelected])
                currentSpeed = maxSpeed[currentPlayerSelected];
            else {
                if(currentSpeed < startSpeed[currentPlayerSelected])
                    currentSpeed = startSpeed[currentPlayerSelected];
                currentSpeed += acceleration[currentPlayerSelected] * Time.deltaTime;
            }
        }

    }

    private protected void ChangeSister() {
        switch(currentPlayerSelected) {
            case 1: currentPlayerSelected = 0; break;
            case 0: currentPlayerSelected = 1; break;
        }
        sisters[currentPlayerSelected].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    protected private void LoadScene(string scene,int sceneID) {
        if(scene == String.Empty) SceneManager.LoadSceneAsync(sceneID);
        else SceneManager.LoadScene(scene);

        if(sceneID > 0 && scene != "MainMenu") pause = 0;
    }

    // Save Data
    private protected void SaveData(int saveID) {
        XmlSerializer ser = new XmlSerializer(typeof(GameData));
        StreamWriter writer = new StreamWriter(current_save_file[saveID - 1]);

        GameData dat = new GameData();

        // insert here the data

        dat.SaveID = saveID;

        // ====================

        ser.Serialize(writer,dat);
        writer.Close();
    }

    // Load Data
    private protected void LoadData(int saveID) {
        if(File.Exists(current_save_file[saveID - 1])) {
            XmlSerializer xml = new XmlSerializer(typeof(GameData));
            Stream reader = new FileStream(current_save_file[saveID - 1],FileMode.Open);

            GameData dat = new GameData();
            dat = (GameData)xml.Deserialize(reader);

            // insert here the data


            // ====================

            reader.Close();
        }
    }

    [SerializeField]
    public class GameData {
        private int _saveID;
        private string _lastScene;

        private Vector2[] _playerPosition;
        private byte _lastPlayerSelected;

        public int SaveID {
            get { return _saveID; }
            set { _saveID = value; }
        }

        public string LastScene {
            get { return _lastScene; }
            set { _lastScene = value; }
        }

        public Vector2[] PlayerPosition {
            get { return _playerPosition; }
            set { _playerPosition = value; }
        }

        public byte LastPlayerSelected {
            get { return _lastPlayerSelected; }
            set { _lastPlayerSelected = value; }
        }
    }
}