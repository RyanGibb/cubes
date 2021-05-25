using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour {
    
    public PlayerMovement movement;
    public Score score;
    public BoxCollider boxCollider;

    public float restartDelay = 3;
    
    private bool died;

    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.CompareTag("Obstacle")) {
            Die();
        }
    }
    
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            Die();
        }
    }

    private void Die() {
        if (died) return;
        died = true;
        movement.enabled = false;
        score.enabled = false;
        movement.body.constraints = RigidbodyConstraints.None;
        boxCollider.material = Resources.Load("Friction") as PhysicMaterial;
        Vector3 velocity = movement.body.velocity;
        movement.body.velocity = velocity;
        Invoke(nameof(Restart), restartDelay);
    }
    
    private void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
