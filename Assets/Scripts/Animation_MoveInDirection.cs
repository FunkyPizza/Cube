using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_MoveInDirection : MonoBehaviour {
    
    public enum ScaleAnim { Nothing, Scale_Up, Scale_Down };
    public enum DirectionAnim { Down, Up };

    [Header("Animation customisation")]
    public ScaleAnim ScaleAnimation;
    public DirectionAnim Direction;
    public bool KeepOriginalScale;
    public bool KeepOriginalPosition;
    public float OffsetSize;
    public float StartDelay;
    public float AnimationSpeedInS = 1f;

    [Header("Live feedback")]
    public bool AnimationTrigger;
    public float LerpValue;


    //Animation lerp variables
    bool AnimationInitialised;
    float TempTime;
    Vector3 OldScale;
    Vector3 NewScale;
    Vector3 OldPosition;
    Vector3 NewPosition;
    public bool KillTile;

    public void SpawnAnimation()
    {
        AnimationSpeedInS = 0.8f;
        KeepOriginalPosition = true;
        KeepOriginalScale = true;
        ScaleAnimation = Animation_MoveInDirection.ScaleAnim.Scale_Up;

        KillTile = false;
        AnimationTrigger = true;
    }

    public void ActivateAnimation()
    {
        AnimationTrigger = true;
    }

    public void TileDeath()
    {
        ScaleAnimation = ScaleAnim.Scale_Down;
        Direction = DirectionAnim.Down;
        KeepOriginalPosition = false;
        KeepOriginalScale = false;
        AnimationSpeedInS = 0.5f;
        KillTile = true;

        ActivateAnimation();

    }

    // Use this for initialization
    void Start ()
    {
            StartDelay += 1f;

        if (OffsetSize == 0)
        {
            OffsetSize = 26;
        }
    }

    // Update is called once per frame
    void Update () {

        if (AnimationTrigger)
        {
            if (StartDelay > 0)
            {
                StartDelay -= Time.deltaTime;
                return;
            }

            else if (StartDelay <= 0)
            {
                StartCoroutine("Animation");

            }
                
        }


    }

    IEnumerator Animation()
    {
        if (!AnimationInitialised)
        {
            TempTime = AnimationSpeedInS;

            switch (Direction)
            {
                case DirectionAnim.Down:
                    OldPosition = transform.position;
                    NewPosition = transform.position + new Vector3(0, -OffsetSize, 0);

                    if (KeepOriginalPosition)
                    {
                        NewPosition = transform.position;
                        OldPosition = transform.position - new Vector3(0, -OffsetSize, 0);
                    }
                    

                    break;

                case DirectionAnim.Up:
                    OldPosition = transform.position;
                    NewPosition = transform.position + new Vector3(0, OffsetSize, 0);

                    if (KeepOriginalPosition)
                    {
                        NewPosition = transform.position;
                        OldPosition = transform.position - new Vector3(0, -OffsetSize, 0);
                    }

                    break;

            }

            switch (ScaleAnimation)
            {
                case ScaleAnim.Nothing:
                    OldScale = transform.localScale;
                    NewScale = transform.localScale;
                    break;

                case ScaleAnim.Scale_Down:
                    NewScale = new Vector3(0, 0, 0);
                    OldScale = new Vector3(1, 1, 1);

                    if (KeepOriginalScale)
                    {
                        NewScale = transform.localScale;
                    }
                    
                    break;

                case ScaleAnim.Scale_Up:
                    NewScale = new Vector3(1, 1, 1);
                    OldScale = new Vector3(0, 0, 0);

                    if (KeepOriginalScale)
                    {
                        NewScale = transform.localScale;
                    }

                    break;
            }

            AnimationInitialised = true;
        }


        transform.localScale = Vector3.Lerp(OldScale, NewScale, LerpValue);
        transform.position = Vector3.Slerp(OldPosition, NewPosition, LerpValue);

        TempTime -= Time.deltaTime;
        float t = TempTime / AnimationSpeedInS;

        LerpValue = 1 - Mathf.SmoothStep(0, 1, t);


        if (LerpValue > 0.1)
        {
            GetComponent<MeshRenderer>().enabled = true;
            if (GetComponentInChildren<SpriteRenderer>()) { GetComponentInChildren<SpriteRenderer>().enabled = true; }
        }

        if (LerpValue < 1)
        {
            yield return null;
        }

        if (LerpValue >= 0.9999f)
        {
            ResetAnimation();
        } 
    }

    void ResetAnimation()
    {
        transform.position = NewPosition;
        AnimationInitialised = false;
        LerpValue = 0;
        StopCoroutine("DeathAnimation");

        AnimationTrigger = false;

        if (KillTile) { GetComponent<MeshRenderer>().enabled = false; if (GetComponentInChildren<SpriteRenderer>()) { GetComponentInChildren<SpriteRenderer>().enabled = false; } }

    }


}

