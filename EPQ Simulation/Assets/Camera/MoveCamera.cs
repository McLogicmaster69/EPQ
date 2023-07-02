using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Cam
{

    public class MoveCamera : MonoBehaviour
    {
        public float Speed = 10f;
        public float ShiftMultiplier = 2f;
        public int Zoom = 5;
        public int MaxZoom = 20;
        private Camera cam;

        private readonly Vector3 RIGHT_VECTOR = new Vector3(1f, 0f, -1f);
        private readonly Vector3 FOWARD_VECTOR = new Vector3(1f, 0f, 1f);

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!WorldManager.main.ActiveSimulation)
                return;
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            transform.position += (Input.GetKey(KeyCode.LeftShift) ? ShiftMultiplier : 1f) * Speed * Time.deltaTime * Zoom * ((x * RIGHT_VECTOR) + (z * FOWARD_VECTOR));
            if(Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if(Zoom > 1)
                    Zoom--;
            }
            if(Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if(Zoom < MaxZoom)
                    Zoom++;
            }
            cam.orthographicSize = Zoom;
        }

    }

}