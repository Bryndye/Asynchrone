﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aFollowingUI : MonoBehaviour
{
    public Renderer theRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (theRenderer != null && Camera.main != null)
        {
            var wantedposition = Camera.main.WorldToScreenPoint(theRenderer.transform.position);
            if (theRenderer.IsVisibleFrom(Camera.main)) transform.position = wantedposition;
            else transform.position = new Vector3(2000, 2000, 0);
        }
        else
        {
            transform.position = new Vector3(2000, 2000, 0);
        }
    }
}

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}