using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float aimingFOV;
    [SerializeField] float movingFOV;
    [SerializeField] float idleFOV;
    [SerializeField] float zoomTransitionDuration;
    [SerializeField] float timeTilIdle;
    private float idleTime;

    private GameObject player;
    private Player playerScript;
    private GameObject camObj;

    private Cinemachine.CinemachineVirtualCamera vCam;
    // private Cinemachine.CinemachineFramingTransposer vTransposer;

    private float camDistance;

    private bool zooming;

    void Start() {
        idleTime = 0.0f;
        zooming = false;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        camObj = GameObject.FindWithTag("Camera");
        vCam = camObj.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        // vTransposer = vCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
        camDistance = vCam.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        camDistance = vCam.m_Lens.OrthographicSize;
        if (!zooming) {
            // StopAllCoroutines();
            if (playerScript.aiming()) {
                Debug.Log("Aiming");
                StartCoroutine(ZoomFunction(aimingFOV, zoomTransitionDuration));
            } else {
                // Debug.Log("Moving");
                StartCoroutine(ZoomFunction(movingFOV, zoomTransitionDuration));
            }

            if (!playerScript.isMoving()) {
                // Debug.Log(idleTime);
                if (idleTime >= timeTilIdle) {
                    StartCoroutine(ZoomFunction(idleFOV, zoomTransitionDuration));
                } else {
                    idleTime += Time.deltaTime;
                }
            } else {
                idleTime = 0f;
            }
        }
    }

    IEnumerator ZoomFunction(float endValue, float duration)
    {
        zooming = true;
        float time = 0;
        float startValue = camDistance;
        while (time < duration)
        {
            // Debug.Log(vCam.m_Lens.OrthographicSize);
            vCam.m_Lens.OrthographicSize = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        zooming = false;
    }
}
