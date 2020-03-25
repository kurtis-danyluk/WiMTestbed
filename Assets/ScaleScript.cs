using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScript : MonoBehaviour
{
    private int viewableTargets = 4;
    private Vector3 startScale;

    private Texture2D cullingTexture;

    public int ViewableTargets {
        get { return viewableTargets; }
        set {
            int oldViewable = viewableTargets;
            viewableTargets += value > 0 ? 1 : -1;
            if (oldViewable != viewableTargets)
            {
                switch (viewableTargets)
                {
                    case 1:
                        viewVolume("circle", size:0.3f, scale:1.0f,xOffSet:0.5f, yOffSet:0.5f);
                        transform.localPosition = new Vector3(-1.5f, 0.0f, 0.0f);
                        transform.localEulerAngles = new Vector3(42.0f, 0.0f, 0.0f);
                        break;
                    case 2:
                        viewVolume("square", size: 0.5f, scale: 1.0f, xOffSet: 0.5f, yOffSet: 0.5f);
                        transform.localPosition = new Vector3(-1.5f, 0.4f, 0.0f);
                        transform.localEulerAngles = new Vector3(22.0f, 0.0f, 0.0f);
                        break;
                    case 3:
                        viewVolume("cutout", size: 1.0f, scale: 1.5f, xOffSet: 0.5f, yOffSet: 0.0f);
                        transform.localPosition = new Vector3(-1.4f, 0.2f, -2.2f);
                        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        break;
                }
            }
        } 
        }
    public int IncViewableTargets => ViewableTargets += 1;
    public int DecViewableTargets => ViewableTargets -= 1;

    // Start is called before the first frame update
    void Start()
    {
        cullingTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
       // GetComponent<Material>().GetTexture("AlphaTexture");
    }
    private void Awake()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Resizes the current object and changes its culling volume
    /// </summary>
    /// <param name="shape">The desired culling shape</param>
    /// <param name="size">0-1: the amount of the total object to render</param>
    /// <param name="scale">A scaling value based on the orignal size</param>
    /// <param name="xOffSet">Where the culling volume should be centered on the x-axis</param>
    /// <param name="yOffSet">Where the culling volume should be centered on the y-axis</param>
    void viewVolume(string shape, float size = 1.0f, float scale = 1.0f, float xOffSet= 0, float yOffSet= 0)
    {
        if (shape == "circle")
        {
            //Create a circular texture of radius size%
            float rad = size *0.5f * cullingTexture.width;
            float xCenter = xOffSet * cullingTexture.width;
            float yCenter = xOffSet * cullingTexture.height;

            for(int i = 0; i< cullingTexture.width; i++)
                for(int j = 0; j < cullingTexture.height; j++)
                {
                    if (Vector2.Distance(new Vector2(i,j), new Vector2(xCenter,yCenter)) < rad)
                        cullingTexture.SetPixel(i, j, Color.black);
                    else
                        cullingTexture.SetPixel(i, j, Color.clear);
                }
        }
        else if(shape == "square")
        {
            float rad = size * 0.5f * cullingTexture.width;
            float xCenter = xOffSet * cullingTexture.width;
            float yCenter = xOffSet * cullingTexture.height;

            for (int i = 0; i < cullingTexture.width; i++)
                for (int j = 0; j < cullingTexture.height; j++)
                {
                    if (Mathf.Abs(xCenter - i) < rad &&  Mathf.Abs(yCenter - j) < rad)
                        cullingTexture.SetPixel(i, j, Color.black);
                    else
                        cullingTexture.SetPixel(i, j, new Color(1,1,1,0));
                }
        }
        else if(shape == "cutout")
        {
            Texture2D albedoTex = (Texture2D)transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].GetTexture("_AlbedoTex");

            float widthRat = albedoTex.width / cullingTexture.width;
            float heightRat = albedoTex.height / cullingTexture.height;

            for(int i = 0; i < cullingTexture.width; i++)
                for(int j = 0; j <cullingTexture.height; j++)
                {
                    cullingTexture.SetPixel(i, j, albedoTex.GetPixel((int)(i*widthRat), (int)(j*heightRat)));
                }

        }
        cullingTexture.Apply();
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("_AlphaTex", cullingTexture);
        transform.localScale = startScale * scale;
    }


}
