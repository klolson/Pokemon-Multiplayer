using UnityEngine;

namespace Mirror.Examples.Pong
{
    public class Player : NetworkBehaviour
    {
        public float speed = 30;
        public Rigidbody2D rigidbody2d;

        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            if (isLocalPlayer)
            {
                rigidbody2d.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
            }
            //rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
        }
        private void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                rigidbody2d.transform.localScale = new Vector3(2, 2, 1);
            }
            if (Input.GetKeyDown("tab"))
            {
                rigidbody2d.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
