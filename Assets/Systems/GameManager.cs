using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //public PlayerInputActions playerInputs;

    // 0 = Yellow Sister / 1 = Purple Sister
    [SerializeField] protected GameObject[] sisters = new GameObject[2]; // GameObject referent of the players
    [SerializeField] protected float[] startSpeed = new float[2], maxSpeed = new float[2], acceleration = new float[2];
    [SerializeField] protected Vector2 atualMovDirection; // direction of movment seted (prolonged)

    protected private bool pause = true;
    protected private int playerSelected = 0;
    protected private bool inMovment = false;
    protected private float speed = 0;

    // Awake is called before the scene starts
    protected private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        //playerInputs = GetComponent<PlayerInputActions>();

        //playerInputs.PlayerActions.TouchTest.started += ctx => InputActions_onActionTriggered(ctx);
        //playerInputs.PlayerActions.Movment.started += ctx => InputActions_onActionTriggered(ctx);
    }

    // Save game data and set start variables
    protected private void SaveData(int saveID) {

    }

    // Load game data and get start variables
    protected private void LoadData(int saveID) {

    }

    private void InputActions_onActionTriggered(InputAction.CallbackContext context) {
        Debug.Log(context.phase);
        //Debug.Log(playerInputs.PlayerActions.TouchTest.ReadValue<Vector2>());
    }

    // Start is called in the first frame of the scene
    protected private void Start() {
        StartPhase(); // temporario
    }

    // Sets the direction the player chooses
    protected private Vector2 MovDirection() {
        if(Input.GetAxisRaw("Horizontal") > 0) return Vector2.right;
        else if(Input.GetAxisRaw("Horizontal") < 0) return Vector2.left;
        else if(Input.GetAxisRaw("Vertical") > 0) return Vector2.up;
        else if(Input.GetAxisRaw("Vertical") < 0) return Vector2.down;
        else return Vector2.zero;
    }

    // Update is called once per frame
    private void Update() {
        if(pause != false) {
            MovementSister();
            MovementCamera();

            if(inMovment == false && Input.GetKeyDown(KeyCode.P)) ChangeSister();
        }

        if(Input.GetKeyDown(KeyCode.R)) LoadScene("none",1);
    }

    protected private void LoadScene(string scene,int sceneID) {
        if(scene == "none") SceneManager.LoadSceneAsync(sceneID);
        else SceneManager.LoadScene(scene);

        StartPhase();
    }

    // Load Start Atributts in the phase
    protected private void StartPhase() {
        // Find the sisters game objects in the scene
        for(int i = 0; i < sisters.Length; i++) 
            sisters[i] = GameObject.Find("Player" + i.ToString());
    }

    protected private void MovementSister() {

        // o jogador so podera trocar a direção quando o vector de direção prolongada for igual a zero
        if(atualMovDirection == Vector2.zero) {
            sisters[playerSelected].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            inMovment = false;
            speed = 0;

            if(MovDirection().x != 0)
                atualMovDirection.x = MovDirection().x;
            else if(MovDirection().y != 0)
                atualMovDirection.y = MovDirection().y;
        }
        else {
            inMovment = true;

            // Acceleration of speed
            if(speed > maxSpeed[playerSelected]) speed = maxSpeed[playerSelected];
            else {
                if(speed < startSpeed[playerSelected]) speed = startSpeed[playerSelected];
                speed += acceleration[playerSelected] * Time.deltaTime;
            }

            sisters[playerSelected].gameObject.GetComponent<Rigidbody2D>().velocity = atualMovDirection * speed;
        }

        if(Input.GetKeyDown(KeyCode.L)) atualMovDirection = Vector2.zero;

    }

    protected private void MovementCamera() {

    }

    void ChangeSister() {
        if(playerSelected == 0) {
            sisters[0].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerSelected = 1;
        }
        else {
            sisters[1].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerSelected = 0;
        }
    }

    protected private class Data {
        private float _exemplo;

        public float Exemplo {
            get { return _exemplo; }
            set { _exemplo = value; }
        }


    }
}