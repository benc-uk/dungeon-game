﻿using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    public GameObject playerGameObj;

    public static bool MOVING = false;
    private float lerp_time = Main.MOVE_LERP_TIME;
    Vector3 target;
    Vector3 start;
    Quaternion turn_from;
    Quaternion turn_to;
    PlayerAudio paudio;
    float stuntime;
    float default_stun = 0.3f;

    // Use this for initialization
    void Start ()
    {
        //hud = (Canvas)GetComponents<Canvas>()[0];

        target = playerGameObj.transform.position;
        start = playerGameObj.transform.position;
        turn_from = playerGameObj.transform.rotation;
        turn_to = playerGameObj.transform.rotation;

        paudio = playerGameObj.GetComponentInChildren<PlayerAudio>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(stuntime > 0) {
            stuntime -= Time.deltaTime;
            return;
        }

        if (Input.GetKey("a")) {
            // TODO! 
            //Main.map.data[1, 1].monster.attack();
        }

        if ((Input.GetKey("up") || Input.GetKey("down")) && !MOVING) {
            start = playerGameObj.transform.position;
            target = playerGameObj.transform.position;
            int direction = 1;
            if (Input.GetKey("up")) {
                direction = 1;
            } else {
                direction = -1;
            }

            int py = Main.player.y;
            int px = Main.player.x;
            switch (Main.player.facing) {
                case Map.NORTH:
                    if(py - direction < 0 || Main.map.data[px, py - direction].blocksMove()) {
                        paudio.playGrunt();
                        stuntime = default_stun;
                        playerGameObj.GetComponentInChildren<Animation>().Play("Player Bump");
                        return;
                    } else {
                        Main.player.y -= direction;
                    }
                    target.z += direction * Main.CELL_SIZE;
                    break;
                case Map.SOUTH:
                    if (py + direction >= Map.SIZE || Main.map.data[px, py + direction].blocksMove()) {
                        paudio.playGrunt();
                        playerGameObj.GetComponentInChildren<Animation>().Play("Player Bump");
                        stuntime = default_stun;
                        return;
                    } else {
                        Main.player.y += direction;
                    }
                    target.z -= direction * Main.CELL_SIZE;
                    break;
                case Map.EAST:
                    print("EAST" + (px - direction));
                    if (px + direction >= Map.SIZE || Main.map.data[px + direction, py].blocksMove()) {
                        paudio.playGrunt();
                        playerGameObj.GetComponentInChildren<Animation>().Play("Player Bump");
                        stuntime = default_stun;
                        return;
                    }
                    else {
                        Main.player.x += direction;
                    }
                    target.x += direction * Main.CELL_SIZE;
                    break;
                case Map.WEST:
                    print("WEST"+(px - direction));
                    if (px - direction < 0 || Main.map.data[px - direction, py].blocksMove()) {
                        paudio.playGrunt();
                        playerGameObj.GetComponentInChildren<Animation>().Play("Player Bump");
                        stuntime = default_stun;
                        return;
                    }
                    else {
                        Main.player.x -= direction;
                    }
                    target.x -= direction * Main.CELL_SIZE;
                    break;
            }

            lerp_time = 0f;
            MOVING = true;
            turn_from = playerGameObj.transform.rotation;
            turn_to = playerGameObj.transform.rotation;
            paudio.playFootstep();

            //Debug.Log("Player loc= " + Main.player.x + ", " + Main.player.y+"  FACE:"+Main.player.facing);
        }

        if (Input.GetKeyDown("right") && !MOVING) {

            Main.player.facing++;
            if (Main.player.facing > Map.WEST) Main.player.facing = Map.NORTH;

            start = playerGameObj.transform.position;
            target = playerGameObj.transform.position;
            turn_from = playerGameObj.transform.rotation;
            turn_to = playerGameObj.transform.rotation = Quaternion.Euler(0, Main.player.facing * 90, 0);
            MOVING = true;
            lerp_time = 0f;
        }
        if (Input.GetKeyDown("left") && !MOVING) {

            Main.player.facing--;
            if (Main.player.facing < Map.NORTH) Main.player.facing = Map.WEST;

            start = playerGameObj.transform.position;
            target = playerGameObj.transform.position;
            turn_from = playerGameObj.transform.rotation;
            turn_to = playerGameObj.transform.rotation = Quaternion.Euler(0, Main.player.facing * 90, 0);
            MOVING = true;
            lerp_time = 0f;
        }

        // Increment timer once per frame
        lerp_time += Time.deltaTime;
        // End the movement (lerp) if max time is reached, this is when movement has stopped
        if (lerp_time >= Main.MOVE_LERP_TIME) {
            lerp_time = Main.MOVE_LERP_TIME;
            MOVING = false;    
        }

        // If we get here, then move the player
        movePlayerLerp();
    }

    private void movePlayerLerp()
    {
        // lerp to new location
        float t = lerp_time / Main.MOVE_LERP_TIME;
        // cosine lerp - removed
        // if(!(Input.GetKey("up") || Input.GetKey("down"))) {
        //     t = Mathf.Sin(t * Mathf.PI * 0.5f);
        // }

        playerGameObj.transform.position = Vector3.Lerp(start, target, t);
        playerGameObj.transform.rotation = Quaternion.Lerp(turn_from, turn_to, t);
    }
}
