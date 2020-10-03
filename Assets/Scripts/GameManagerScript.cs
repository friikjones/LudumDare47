using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

    public UnityEvent _levelReset = new UnityEvent();

    private void Update() {
        if (Input.anyKeyDown) {
            //Begin the action
            _levelReset.Invoke();
        }
    }


}
