using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mover : MonoBehaviour
{
    public rail rail;
    public playMod mode;
    public float speed = 2.5f;
    public bool isReversed;
    public bool isLooping;
    public bool pingPong;

    private int currentSeg;
    private float transition;
    private bool isComplited;

    private void Update()
    {
        if(!rail) return;

        if (!isComplited)
        {
            play(!isReversed);
        }
    }

    private void play(bool forward = true)
    {
        float m = (rail.nodes[currentSeg + 1].position - rail.nodes[currentSeg].position).magnitude;//keep track on the length of the rail for proper speed
        float s = (Time.deltaTime * 1 / m) * speed; //keep track on the speed
        transition += (forward) ? s : -s;
        if(transition > 1)
        {
            transition = 0;
            currentSeg++;
            if(currentSeg == rail.nodes.Length - 1)
            {
                if(isLooping)
                {
                    if(pingPong)
                    {
                        transition = 1;
                        currentSeg = rail.nodes.Length - 2;
                        isReversed = !isReversed;
                    }
                    else
                    {
                        currentSeg = 0;
                    }
                }
                else
                {
                    isComplited = true;
                    return;
                }
            }
        }
        else if(transition < 0) 
        {
            transition = 1;
            currentSeg--;

            if (currentSeg == - 1)
            {
                if (isLooping)
                {
                    if (pingPong)
                    {
                        transition = 0;
                        currentSeg = 0;
                        isReversed = !isReversed;
                    }
                    else
                    {
                        currentSeg = rail.nodes.Length - 2;
                    }
                }
                else
                {
                    isComplited = true;
                    return;
                }
            }

        }

        transform.position = rail.positionOnRail(currentSeg, transition, mode);
        transform.rotation = rail.orientation(currentSeg, transition);
    }
}
