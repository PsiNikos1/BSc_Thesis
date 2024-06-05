using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialProperty : MonoBehaviour
{
    public Transform originObject; // The origin object to cast the ray from
    public Transform endObject; // The end object for the ray
    private Renderer rend; // The renderer of the object
    private Material material; // The material of the object
    private float currentDepth = 0.0f; // Current depth value
    private Vector3 currentNormal; // Current normal vector
    public float lerpSpeed = 1.0f; // Speed of interpolation
    public float radiusOfPokingSphere = 0.1f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            material = rend.material;
            material.SetFloat("_Depth", 0.0f);
        }
    }

    void Update()
    {
        CastRay();
    }

    void CastRay()
    {
        Vector3 raycastDirection = endObject.position - originObject.position;
        Ray ray = new Ray(originObject.position, raycastDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDirection.magnitude + radiusOfPokingSphere))
        {
            currentNormal = ray.direction.normalized;
            float targetDepth = Vector3.Distance(originObject.position, endObject.position);
            currentDepth = Mathf.Lerp(currentDepth, targetDepth, lerpSpeed * Time.deltaTime);

            if (rend != null)
            {
                material.SetVector("_Coordinates", hit.point);
                material.SetVector("_Normal", currentNormal);
                material.SetFloat("_Depth", currentDepth);
            }
            else
            {
                Debug.LogWarning("Renderer or target object not assigned.");
            }
        }
        else
        {
            currentNormal = ray.direction.normalized;
            currentDepth = Mathf.Lerp(currentDepth, 0.0f, lerpSpeed * Time.deltaTime);

            if (rend != null)
            {
                material.SetVector("_Coordinates", Vector3.zero);
                material.SetVector("_Normal", currentNormal);
                material.SetFloat("_Depth", currentDepth);
            }
        }
    }
}
