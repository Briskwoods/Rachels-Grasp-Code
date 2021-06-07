using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] public int m_attackDamage = 40;							// How much damage each attack deals

	[SerializeField] private float m_attackRange = 0.5f;                        // Used to set the weapon attack range
	[SerializeField] private float m_Distance = 0.4f;                           // Raycast distance to check for wall.
	[SerializeField] private float m_SlideSpeed = -3f ;                         // Wall Slide Speed.
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[SerializeField] private float m_dashSpeed;                                 // Speed to multiply movement by for dash.
	[SerializeField] private float m_underwaterDashSpeed;						// Underwater Dash speed
	[SerializeField] private float m_crouchDashSpeed = 300f;                    // Speed to multiply movement by for crouch dash.
	[SerializeField] private float m_normalDashSpeed;                           // Used to set the dash speed variable back to normal after crouch dash(roll)
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private LayerMask m_EnemyLayers;			                // Used in detecting enemies
	
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform m_WallCheck;                             // A position marking where to check if the player is near a wall.
	[SerializeField] private Transform m_LedgeCheck;                            // A position marking where to check if the player is near a ledge.
	[SerializeField] private Transform m_AttackPoint;                           // A position marking where the player attacks land.
	
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	public Animator animator;                                                   //Used in animating the wall slide functionality	
	public SpriteRenderer spriteRenderer;										//Used in flipping the sprite for the wall slide animation


	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;			// Whether or not the player is grounded.
	const float k_CeilingRadius = .2f;	// Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	//private bool m_isNearLedge;		//To Be added later as it is an extra feature. This feature is the ledge grab mechanism. For now lets get everything else working
	private bool m_isNearWall;			//For Determining if the player is next to a wall or not. 

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_normalDashSpeed += m_dashSpeed;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
			
		}

		//This line prevents the raycast from detecting the player's box colliders
		Physics2D.queriesStartInColliders = false;
		//We use a raycast to detect the presence of a wall and if found we can perform a wall slide function.
		m_isNearWall = Physics2D.Raycast(m_WallCheck.position, Vector2.right * transform.localScale.x, m_Distance);
		Debug.DrawRay(m_WallCheck.position, Vector2.right * transform.localScale.x * m_Distance, new Color(1f, 1f, 1f));


		//This raycast detects if we are near a ledge and sets the m_isNearLedge bool to true if it detects a ledge.
		//m_isNearLedge = Physics2D.Raycast(m_LedgeCheck.position, Vector2.right*transform.localScale.x, m_Distance);

		if (!m_Grounded && m_isNearWall && GetComponent<Rigidbody2D>().velocity.y < m_SlideSpeed)
		{
			animator.SetBool("isWallSliding",true);
			m_Rigidbody2D.velocity = new Vector2(0, m_SlideSpeed);
			spriteRenderer.flipX = true;
		}
        else
        {
			animator.SetBool("isWallSliding", false);
			spriteRenderer.flipX = false;
		}
		//We use raycasts again to detect to presence of a ledge in order to perform a ledge graab functionality.
		//Ledge grab fn code


	}


	public void Move(float move, bool crouch, bool jump, bool dash, bool attack)
	{
		// Change the gravity back to normal when not swimming
		m_Rigidbody2D.gravityScale = 5f;
		
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			//m_Rigidbody2D.mass = 1.102604f;
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			/* To prevent Dash when crouching uncomment the next line */
			//dash = false;
						
			// If crouching
			if (crouch)
			{
				//m_Rigidbody2D.mass = 0.651302f; 
				m_dashSpeed = m_crouchDashSpeed;
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				m_dashSpeed = m_normalDashSpeed;
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			
			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		//If the player should dash
        if (dash)
        {
			// Dash the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * m_dashSpeed, m_Rigidbody2D.velocity.x);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		}
		animator.SetFloat("DashSpeed", Mathf.Abs(m_Rigidbody2D.velocity.x));
        //if the Player should attack
        if (attack)
        {
			//Detect enemies in range
			Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_attackRange, m_EnemyLayers);
			foreach(Collider2D enemy in hitEnemies)
            {
				//Damage enemies
				enemy.GetComponent<EnemyController>().TakeDamage(m_attackDamage);

				// Knockback the enemy
				if (m_Rigidbody2D.transform.position.x > enemy.transform.position.x)
				{
					enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * enemy.GetComponent<EnemyController>().m_knockbackForceY + transform.right * -(enemy.GetComponent<EnemyController>().m_knockbackForceX * 2f));
				}
				else if (m_Rigidbody2D.transform.position.x < enemy.transform.position.x)
				{
					enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * enemy.GetComponent<EnemyController>().m_knockbackForceY + transform.right * enemy.GetComponent<EnemyController>().m_knockbackForceX);
				}
			}
        }
	}

	public void Swim(float horizontalMove, float verticalMove,  bool dash)
    {
		// Set gravity to a lower scale to fall slower through water
		m_Rigidbody2D.gravityScale = 1.5f;

		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(horizontalMove / 10f, verticalMove / 10f);
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

		// If the input is moving the player right and the player is facing left...
		if (horizontalMove > 0 && !m_FacingRight)
		{
			// ... flip the player.
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (horizontalMove < 0 && m_FacingRight)
		{
			// ... flip the player.
			Flip();
		}


		if (dash)
		{
			// Dash the character by finding the target velocity
			Vector3 dashVelocity = new Vector2(horizontalMove * m_underwaterDashSpeed, m_Rigidbody2D.velocity.x);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, dashVelocity, ref m_Velocity, m_MovementSmoothing);
		}
		animator.SetFloat("DashSpeed", Mathf.Abs(m_Rigidbody2D.velocity.x));



	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    private void OnDrawGizmosSelected()
    {
		if(m_AttackPoint == null)
        {
			return; 
        }
		Gizmos.DrawWireSphere(m_AttackPoint.position, m_attackRange);
    }
}
