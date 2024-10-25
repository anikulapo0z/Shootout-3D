using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Crosshair : MonoBehaviour
{
    [Range(0, 100)]
    public float Value;
    public float speed;

    public float margin;
    public float multiplier;
    public float sprintMultiplier; // Define this new variable
    public float jumpValue; // Define this new variable
    public float shootingValue; // Define this new variable

    public RectTransform Top, Bottom, Left, Right, Centre;

    public RaycastPlayer Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindObjectOfType<RaycastPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //reset the value of the crosshair
        if (Player == null)
        {
            ;
            Player.GetComponent<RaycastPlayer>();
            if (Player.IsMoving)
                Value += Player.currentSpeed * multiplier;
            if (Player.IsSprinting)
                Value += Player.currentSpeed * sprintMultiplier; // Define this new multiplier
            if (Player.IsJumping)
                Value += jumpValue; // Define this new constant
            if (Player.IsShooting)
                Value += shootingValue; // Define this new constant
        }

        Value = Player.GetComponent<RaycastPlayer>().currentSpeed * multiplier;

        float TopValue, BottomValue, LeftValue, RightValue;

        TopValue = Mathf.Lerp(Top.position.y, Centre.position.y + margin + Value, speed * Time.deltaTime);
        BottomValue = Mathf.Lerp(Bottom.position.y, Centre.position.y - margin - Value, speed * Time.deltaTime);

        LeftValue = Mathf.Lerp(Left.position.x, Centre.position.x - margin - Value, speed * Time.deltaTime);
        RightValue = Mathf.Lerp(Right.position.x, Centre.position.x + margin + Value, speed * Time.deltaTime);

        Top.position = new Vector2(Top.position.x, TopValue);
        Bottom.position = new Vector2(Bottom.position.x, BottomValue);

        Left.position = new Vector2(LeftValue, Left.position.y);
        Right.position = new Vector2(RightValue, Right.position.y);
    }

}
